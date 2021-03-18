
/****** Object:  StoredProcedure [dbo].[GetQuestionsById_SP]    Script Date: 04/11/15 5:38:06 PM ******/
DROP PROCEDURE [dbo].[GetQuestionsById_SP]
GO

/****** Object:  StoredProcedure [dbo].[GetQuestionsById_SP]    Script Date: 04/11/15 5:38:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

---- Exec GetQuestionsById_SP 15897
CREATE procedure [dbo].[GetQuestionsById_SP](
  @QuestionId INT=null
)
AS 
Begin
SET NOCOUNT ON
IF(@QuestionId = 0)
BEGIN
SET @QuestionId=null;
END
SELECT
Distinct
	    QB.Id,QB.QuestionId, Decription,ImagePath,OptionA,
        OptionB,OptionC,OptionD,OptionE, 
        AnswerDescription,qb.IsActive,CreatedOn,CreatedBy, 
        ModifiedOn,ModifiedBy,AnswerOption, e.Text as ExamName,
        s.Text as Subject,qf.Text as QuestionFormat
        
FROM QuestionBank QB 
inner join [Lookup] e on QB.[ExamName] = e.Value and e.ModuleCode='ExamType'  and e.IsActive=1
inner join [Lookup] s on QB.[Subject] = s.Value and s.ModuleCode='Subject'  and e.IsActive=1
inner join [Lookup] Qf on QB.QuestionFormat = Qf.Value and Qf.ModuleCode='QuestionFormatType'  and e.IsActive=1

WHERE QB.QuestionId=COALESCE(@QuestionId, QB.QuestionId) AND (QB.ParentId=0 OR ISNULL(QB.ParentId,0)=0 )
End
GO


