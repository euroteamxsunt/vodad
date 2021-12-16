/*
   -- List of Triggers
   SELECT 'DROP TRIGGER [' + SCHEMA_NAME(t.schema_id) + '].[' + tr.name + ']'
     FROM sys.triggers tr
     JOIN sys.objects t on t.object_id = tr.parent_id
*/

--
--  EXEC [acornpa].[pr_FkCreateTreeDeleteTrigger] 1
--
-- Somewhat like a FK Cascade Delete but it is put
-- on the referenced table (aka Parent table) instead of the
-- table with the FK.
CREATE PROCEDURE  [dbo].[pr_CreateFkTreeDeleteTrigger]
       @schema_name NVARCHAR(128) = NULL, -- Null means all Schemas
       @is_verbose BIT = 0
AS
BEGIN
   SET NOCOUNT ON

   DECLARE @cmd             NVARCHAR(MAX)
   DECLARE @table_id        INT
   DECLARE @table_schema    NVARCHAR(128)
   DECLARE @table_name      NVARCHAR(128)
   DECLARE @column_name     NVARCHAR(128)
   DECLARE @column_ordinal  INT

   -- Referenced tables along with their PKs
   DECLARE reftable_cur CURSOR FOR
     SELECT DISTINCT t.object_id, SCHEMA_NAME(t.schema_id) AS table_schema, t.name
       FROM sys.tables t
       JOIN (SELECT DISTINCT fk.referenced_object_id FROM sys.foreign_key_columns fk) ref
         ON t.object_id = ref.referenced_object_id
       JOIN sys.key_constraints kc
         ON kc.parent_object_id = t.object_id
            AND kc.type = 'PK'      
       LEFT JOIN sys.triggers tr
         ON tr.parent_id = t.object_id
            AND tr.is_instead_of_trigger = 1
       LEFT JOIN sys.trigger_events te
         ON te.object_id = tr.object_id
            AND te.type = 3 -- DELETE event
      WHERE t.type = 'U' -- User Defnied Table
        AND te.object_id IS NULL -- We do not have DELETE Trigger on table
		AND (@schema_name IS NULL OR @schema_name = SCHEMA_NAME(t.schema_id)) -- Only in schemas that we were asked
   ORDER BY t.name

   OPEN reftable_cur
   FETCH NEXT FROM reftable_cur INTO @table_id, @table_schema, @table_name
   WHILE @@FETCH_STATUS = 0
   BEGIN
      DECLARE @triggerName      NVARCHAR(128)
      DECLARE @tmpTableName     NVARCHAR(128) 
      DECLARE @list_column_into NVARCHAR(MAX)
      DECLARE @cmd_where_clause NVARCHAR(MAX)

      SET @triggerName = 'TR_TreeDelete_' + @table_name
      SET @tmpTableName = dbo.fn_quotename_brackets('#TMP_DELETED_' + @table_name)
      SET @list_column_into = NULL
      SET @cmd_where_clause = NULL

      DECLARE refcol_cur CURSOR FOR
        SELECT COL_NAME(@table_id, idx_col.column_id) AS column_name, idx_col.key_ordinal
          FROM sys.key_constraints kc
          JOIN sys.indexes idx
            ON kc.unique_index_id = idx.index_id
               AND kc.parent_object_id = idx.object_id
          JOIN sys.index_columns idx_col
            ON idx.object_id = idx_col.object_id
               AND idx.index_id = idx_col.index_id
         WHERE idx_col.is_included_column = 0
           AND kc.parent_object_id = @table_id
           AND kc.type = 'PK'
        ORDER BY idx_col.key_ordinal;

      OPEN refcol_cur
      FETCH NEXT FROM refcol_cur INTO @column_name, @column_ordinal
      WHILE @@FETCH_STATUS = 0
      BEGIN
        IF @list_column_into IS NULL
        BEGIN
           SET @list_column_into = dbo.fn_quotename_brackets(@column_name)
           SET @cmd_where_clause = dbo.fn_quotename_brackets(@table_name)  + '.' + dbo.fn_quotename_brackets(@column_name) + ' IN (SELECT ' + dbo.fn_quotename_brackets(@column_name) + ' FROM ' + @tmpTableName + ')'
        END
        ELSE 
        BEGIN
           SET @list_column_into = @list_column_into + ', ' + dbo.fn_quotename_brackets(@column_name)
           SET @cmd_where_clause = @cmd_where_clause + CHAR(13) + 
               ' AND ' + @table_name  + '.' + dbo.fn_quotename_brackets(@column_name) + ' IN (SELECT ' + dbo.fn_quotename_brackets(@column_name) + ' FROM ' + @tmpTableName + ')'
        END
        FETCH NEXT FROM refcol_cur INTO @column_name, @column_ordinal
      END       
      CLOSE refcol_cur
      DEALLOCATE refcol_cur

      SET @cmd = 
          'CREATE TRIGGER ' + @triggerName + CHAR(13) +
          '    ON ' + dbo.fn_getFullQualifiedTableName(@table_id) + CHAR(13) +
          'INSTEAD OF DELETE AS' + CHAR(13) +
          'BEGIN' + CHAR(13) +
          '  IF @@rowcount = 0 RETURN;' + CHAR(13) +
          '  -- PRINT ''Trigger:' + @triggerName + ' has been called'';' + CHAR(13) +
          '  -- Put DELETED into a temporary table, so it can be seen by stored procedure' + CHAR(13) +
          '  SELECT ' + @list_column_into + ' INTO ' + @tmpTableName + ' FROM DELETED;' + CHAR(13) +
          '  --' + CHAR(13) +
          '  -- Call recursive delete stored procedure' + CHAR(13) +
          '  -- We need to disable the trigger to avoid recursion on an INSTEAD TRIGGER.' + CHAR(13) +
          '  -- This may not be good, if another transaction deletes stuff from the table.' + CHAR(13) +
          '  DISABLE TRIGGER ' +  @triggerName + ' ON ' + dbo.fn_getFullQualifiedTableName(@table_id) + ';' + CHAR(13) +
          '  EXEC dbo.pr_FkTreeDelete' + CHAR(13) +
          '       @parent_table_id = ''' + dbo.fn_getFullQualifiedTableName(@table_id) + ''',' +  CHAR(13) +
          '       @where_clause = ''' + @cmd_where_clause + '''' + CHAR(13) +
          '  ;' + CHAR(13) +
          '  -- Enable it back' + CHAR(13) +
          '  ENABLE TRIGGER ' +  @triggerName + ' ON ' + dbo.fn_getFullQualifiedTableName(@table_id) + ';' + CHAR(13) +
          '  DROP TABLE ' + @tmpTableName + ';' + CHAR(13) +
          'END' + CHAR(13)

      IF (@is_verbose = 1) PRINT @cmd

      EXEC(@cmd)
      FETCH NEXT FROM reftable_cur INTO @table_id, @table_schema, @table_name
   END       
   CLOSE reftable_cur
   DEALLOCATE reftable_cur
END


