CREATE FUNCTION [dbo].[fn_quotename_brackets] (@str_dbname NVARCHAR(258))
RETURNS NVARCHAR(258)
AS
BEGIN
   IF (@str_dbname IS NULL OR LEN(@str_dbname) > 128)
   BEGIN
      DECLARE @msg VARCHAR(128)
      SET @msg = '@str_dbname cannot be NULL nor greater than 128'

--      EXEC('SELECT 1 FROM' + @msg)
--      RAISERROR (@msg,15,10)
      RETURN NULL
   END

   -- If we have [ and ] in the middle of string...remove all of them
   IF (CHARINDEX('[', @str_dbname, 2) > 1 AND
       CHARINDEX(']', @str_dbname, 2) > 1)
   BEGIN
      -- Remove all of them and we will be adding the brackets next
      SET @str_dbname = REPLACE(REPLACE(@str_dbname,'[',''), ']', '')
   END

	-- Return the result of the function
   IF (CHARINDEX('[', @str_dbname) <> 1 OR
       CHARINDEX(']', @str_dbname) <> LEN(@str_dbname))
   BEGIN
      SET @str_dbname = QUOTENAME(@str_dbname)
   END
	RETURN @str_dbname
END


