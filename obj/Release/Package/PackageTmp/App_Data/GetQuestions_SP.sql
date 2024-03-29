USE [OnlineExam_1]
GO
/****** Object:  StoredProcedure [dbo].[GetQuestions_SP]    Script Date: 02/08/15 9:58:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
---- Exec GetQuestions_SP
ALTER procedure [dbo].[GetQuestions_SP]
As 
Begin
set nocount on

SELECT
	   distinct QB.Id,Decription,ImagePath,OptionA,
        OptionB,OptionC,OptionD,OptionE, 
        AnswerDescription,qb.IsActive,CreatedOn,CreatedBy, 
        ModifiedOn,ModifiedBy,AnswerOption, e.Text as ExamName,
        s.Text as Subject,qf.Text as QuestionFormat
        
FROM QuestionBank QB 
inner join [Lookup] e on QB.[ExamName] = e.Value and e.ModuleCode='ExamType' and e.IsActive=1
inner join [Lookup] s on QB.[Subject] = s.Value and s.ModuleCode='Subject' and s.IsActive=1
inner join [Lookup] Qf on QB.QuestionFormat = Qf.Value and Qf.ModuleCode='QuestionFormatType' and Qf.IsActive=1

WHERE 
QB.ParentId=0 OR ISNULL(QB.ParentId,0)=0 
End