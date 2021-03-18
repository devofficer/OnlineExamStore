
IF NOT EXISTS(SELECT 1 FROM [dbo].[Lookup] WHERE ModuleCode='DomainName' AND [Text]='acadastore.com' AND Value ='acadastore.com')
BEGIN
	INSERT INTO [dbo].[Lookup] ([ModuleCode],[Text],[Value],[Description],[Order],[IsActive])
		 VALUES ('DomainName','acadastore.com','acadastore.com','Website domain name',1,1)
END
GO
