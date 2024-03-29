
/****** Object:  StoredProcedure [dbo].[GetQuestionsByParameters_SP]    Script Date: 17/10/15 8:54:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
---- Exec GetQuestionsByParameters_SP 'WAEC','Government',null,null,true
ALTER procedure [dbo].[GetQuestionsByParameters_SP](
@ExamName varchar(max)=null,
@Subject varchar(max)=null,
@QuestionFormat varchar(max)=null,
@Marks int=null,
@IsOnline bit=null)
As 
Begin
set nocount on

if(@Marks = 0)
Begin
set @Marks=null;
End
if(@ExamName='')
Begin
set @ExamName=null;
End
if(@Subject = '')
Begin
set @Subject=null;
End
if(@QuestionFormat='')
Begin
set @QuestionFormat=null;
End
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

WHERE 
QB.ExamName=COALESCE(@ExamName, QB.ExamName) and 
QB.[Subject]=COALESCE(@Subject, QB.[Subject]) and
QB.questionFormat=COALESCE(@QuestionFormat, QB.questionFormat) and
QB.Mark=COALESCE(@Marks, QB.Mark) and
QB.isOnline=COALESCE(@IsOnline, QB.isOnline) AND
(QB.ParentId=0 OR ISNULL(QB.ParentId,0)=0 )
End