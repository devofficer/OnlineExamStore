

/****** Object:  StoredProcedure [dbo].[GetQuestionErrors_SP]    Script Date: 15/12/15 4:49:34 PM ******/
DROP PROCEDURE [dbo].[GetQuestionErrors_SP]
GO

/****** Object:  StoredProcedure [dbo].[GetQuestionErrors_SP]    Script Date: 15/12/15 4:49:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		RAVI K SINGH
-- Create date: 15-DEC-2015
-- Description:	To get all question errors reported by end user
-- =============================================
CREATE PROCEDURE [dbo].[GetQuestionErrors_SP] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 SELECT  CAST(ROW_NUMBER() OVER(ORDER BY QE.ModifiedOn,QE.CreatedOn DESC) AS Int)  AS [Index],
		QE.Id,
		QE.QuestionId,
		QE.UserId,
		QE.ActionTaken,
		QE.CreatedOn,
		UP1.FirstName +SPACE(1)+ UP1.LastName 'CreatedBy',
		QE.ModifiedOn,
		ISNULL(UP.FirstName +SPACE(1)+ UP.LastName,'') as 'ModifiedBy',
		QE.[Description]
		FROM QuestionError QE 
		LEFT JOIN UserProfile UP ON QE.ModifiedBy=UP.ApplicationUser_Id 
		LEFT JOIN UserProfile UP1 ON QE.CreatedBy=UP1.ApplicationUser_Id

ORDER BY QE.ModifiedOn,QE.CreatedOn DESC
END

GO


