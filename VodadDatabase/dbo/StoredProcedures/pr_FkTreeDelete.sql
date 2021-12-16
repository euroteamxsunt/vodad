--
-- Delete all records in the table specified that conform to the given 
-- where clause while also deleting children, grandchildren, and so on
-- in a fashion like a recursive delete (Cascading Delete) based on
-- the foreign keys.
--
-- This stored procedure is handy when the tables do not have the cascade
-- delete option.
-- Sometimes CASCATE DELETE option in SQL is not possible due to SQL
-- restrictions:
--    1) There can be only one cascade path from a parent table to a child
--       table. That is, no table can appear more than once in the list of all
--       cascading referential actions.
--    2) Related to above rule, a Self-referencing constraints will not allow
--       CASCADE operations to be defined on them. A classic example is the
--       EMPLOYEE table that has a MANAGER column would exhibit this issue.
--    What SQL Server indicates when the above rules are not match, is 
--    something like CASCATE does must form a tree that contains no circular
--    references.
--
--    http://msdn.microsoft.com/en-us/library/ms186973.aspx
--
-- The idea for this script was taken from Daniel Crowther, who wrote a script
-- to emulate cascading deletes in SQL Server 7. I have very much rewritten 
-- the script from scratch. Just the idea has been taken.
--
-- Example 1
--    EXEC acornpa.pr_fkTreeDelete
--         @parent_table_id = 'shiner.Scenarios',
--         @where_clause = 'Scenarios.Id = 123'
--
--
--
CREATE PROCEDURE [dbo].[pr_FkTreeDelete]
   @parent_table_id NVARCHAR(300),       -- SQL Table_id or SQL TABLE NAME (e.g., shiner.Entities) where rows are to be deleted
   @where_clause    NVARCHAR(MAX),       -- WHERE CLAUSE (Entities.Id = 7) Usually table.pkey = value used to delete records
   @from_clause     NVARCHAR(MAX) = '',  -- ONLY For internal purposes (aka Recursion). Use default
   @cascate_level   INT = 0,             -- ONLY For internal purposes (aka Recursion). Use default
   @is_verbose      BIT = 0
AS
BEGIN
  SET NOCOUNT ON

  DECLARE @do_exec  BIT     SET @do_exec = 1 
  DECLARE @NEW_LINE CHAR(1) SET @NEW_LINE = CHAR(13) 

  DECLARE @cmd             NVARCHAR(MAX)
  DECLARE @cmd_from        NVARCHAR(1000)
  DECLARE @cmd_onjoin      NVARCHAR(MAX)
  DECLARE @fk_const_id     INT
  DECLARE @fk_table_id     INT
  DECLARE @child_level     INT

  IF ISNUMERIC(@parent_table_id) = 0
  BEGIN 
     -- Get the table identifier
     IF CHARINDEX('.', @parent_table_id) = 0
       SET @parent_table_id = OBJECT_ID('[dbo].[' + @parent_table_id + ']')
     ELSE
       SET @parent_table_id = OBJECT_ID(@parent_table_id)
  END

  IF @cascate_level = 0
  BEGIN
     SET @cmd = 'SET NOCOUNT ON' + @NEW_LINE
     SET @from_clause = 'FROM ' + dbo.fn_getFullQualifiedTableName(@parent_table_id)

     IF (@is_verbose = 1) 
     BEGIN
       PRINT '-- **************************************************************'
       PRINT '-- *** Cascade delete for table' + dbo.fn_getFullQualifiedTableName(@parent_table_id)
       PRINT '-- *** with where_clause of ' + @where_clause
       PRINT '-- **************************************************************'
       PRINT @cmd
     END
     IF (@do_exec = 1) EXEC (@cmd) 
  END

  -- Referenced_object_id is the table with the PK 
  -- Parent_object_id is the table with the FK
  DECLARE children_cur CURSOR LOCAL FORWARD_ONLY FOR
   SELECT DISTINCT
          fkNameId = fk.object_id,          -- FK constraint id
          fkTableId = fk.parent_object_id   -- The parent of the constraint, which is the referencing table. The table with the FK (aka child table)
  --      refTableId = referenced_object_id -- The table that we refer to (aka parent table)
     FROM sys.foreign_keys fk 
    WHERE fk.referenced_object_id <> fk.parent_object_id -- WE DO NOT HANDLE self referencing tables!!!
      AND fk.referenced_object_id = @parent_table_id
      AND fk.delete_referential_action <> 1 -- MAKE sure it is not ON DELETE CASCATE in the FK. If it is SQL will do it

  OPEN children_cur 
  FETCH NEXT FROM children_cur INTO @fk_const_id, @fk_table_id
  WHILE @@FETCH_STATUS = 0
  BEGIN
    -- Build a join criteria to delete rows from child table.
    -- The JOINS keep growing with each recursive call
    SET @cmd_onjoin = 
        STUFF((SELECT ' AND' + @NEW_LINE +
                      '[' + OBJECT_NAME(rkeyid) + '].[' + COL_NAME(rkeyid, rkey) + '] = ' +
                      '[' + OBJECT_NAME(fkeyid) + '].[' + COL_NAME(fkeyid, fkey) + ']' 'text()'
                 FROM dbo.sysforeignkeys fk
                WHERE fk.constid = @fk_const_id 
               FOR XML PATH(''), TYPE).value('text()[1]', 'VARCHAR(MAX)')
             , 1, LEN(' AND' + @NEW_LINE), '')
    SELECT @cmd_from = 
           @from_clause + @NEW_LINE +
           'JOIN ' + dbo.fn_getFullQualifiedTableName(@fk_table_id) + @NEW_LINE +
           '  ON ' + @cmd_onjoin

    SET @child_level = @cascate_level + 1
    EXEC dbo.pr_FkTreeDelete
         @parent_table_id = @fk_table_id,
         @where_clause = @where_clause,
         @from_clause = @cmd_from,
         @cascate_level = @child_level,
         @is_verbose = @is_verbose 

    -- NOTE: When a DELETE action to a child/referencing table is the result of 
    --       a CASCADE on a DELTE from parent table, and an INSTEAD of trigger
    --       on DELETE is defined on that child table, the trigger is IGNORED
    --       and the DELETE action is executed. THUS, you would not have 
    --       called the stuff twice.
    --       See Remarks on 
    --           http://msdn.microsoft.com/en-us/library/ms176072.aspx
	 SET @cmd =  
        'DELETE FROM ' + dbo.fn_getFullQualifiedTableName(@fk_table_id) + ' -- Level=' + CAST(@cascate_level AS NVARCHAR) + @NEW_LINE +
        @cmd_from + @NEW_LINE +
        'WHERE ' + @where_clause + @NEW_LINE

    IF (@is_verbose = 1) PRINT @cmd
    IF (@do_exec = 1) EXEC (@cmd) 

	 FETCH NEXT FROM children_cur INTO @fk_const_id, @fk_table_id
  END

  IF @cascate_level = 0
  BEGIN
    -- Delete the rows from parent table
    SET @cmd = 
        'DELETE FROM ' + dbo.fn_getFullQualifiedTableName(@parent_table_id) + ' -- TOP LEVEL PARENT TABLE' + @NEW_LINE +
        ' WHERE ' + @where_clause + @NEW_LINE

    IF (@is_verbose = 1) 
    BEGIN
       PRINT @cmd
    END

    IF (@do_exec = 1) 
    BEGIN
      EXEC (@cmd) 
      PRINT 'Deleted ' + CONVERT(varchar, @@ROWCOUNT) + ' records from table ' + dbo.fn_getFullQualifiedTableName(@parent_table_id)
    END
  END

  CLOSE children_cur 
  DEALLOCATE children_cur 
END


