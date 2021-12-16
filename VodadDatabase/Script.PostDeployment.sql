/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
EXEC pr_CreateFkTreeDeleteTrigger 'dbo', 0
EXEC pr_PopulateStaticData

--IF('$(populateTest)' = '1')
--	EXEC pr_PopulateTestData 0