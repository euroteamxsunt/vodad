CREATE FUNCTION [dbo].[fn_getFullQualifiedTableName] (@sql_table_id INT)
RETURNS NVARCHAR(300)
AS
BEGIN
  DECLARE @schema_id BIGINT
  DECLARE @name      NVARCHAR(300)

  SELECT @schema_id = schema_id FROM sys.tables WHERE object_id = @sql_table_id
  SET @name = '[' + SCHEMA_NAME(@schema_id) + '].[' + OBJECT_NAME(@sql_table_id) + ']' 

  RETURN @name
END 
GO

