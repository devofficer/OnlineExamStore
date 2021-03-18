
/****** Object:  UserDefinedFunction [dbo].[IsAdminUser]    Script Date: 12/08/16 5:23:54 PM ******/
DROP FUNCTION [dbo].[IsAdminUser]
GO

/****** Object:  UserDefinedFunction [dbo].[IsAdminUser]    Script Date: 12/08/16 5:23:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		RAVI K SINGH
-- Create date: 12th AUG 2016
-- Description:	To KNOW PASSED USER IS ADMIN USER OR NOT
-- =============================================
CREATE FUNCTION [dbo].[IsAdminUser]
(
 @UserId VARCHAR(100)
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar INT = 0
	DECLARE @AdminDomain VARCHAR(100)
	DECLARE @AdminTab TABLE (
	    AdminDomain VARCHAR(100)
	)

	SELECT @AdminDomain = [Text] FROM Lookup WHERE ModuleCode='DomainName' AND IsActive=1

	IF(LEN(@AdminDomain)=0)
    RETURN @ResultVar

	INSERT INTO @AdminTab
	SELECT SUBSTRING(T.Email,(CHARINDEX('@',T.Email)+1),LEN(T.Email) - (CHARINDEX('@',T.Email))) as DomainName FROM AspNetUsers T
	WHERE T.EmailConfirmed = 1 AND T.[Status] ='Active' AND T.Id = @UserId
	
	
	IF @AdminDomain = (SELECT LOWER(AdminDomain) FROM @AdminTab)
       SET @ResultVar = 1
ELSE 
       SET @ResultVar = 0

	RETURN @ResultVar

END

GO


