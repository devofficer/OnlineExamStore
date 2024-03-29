/****** Object:  StoredProcedure [dbo].[GetQuestionListViewByParameters_SP]    Script Date: 10/09/16 5:36:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-----Exec GetQuestionListViewByParameters_SP 'JAMB','ENGLISH','SQMA',null,null
---- Exec GetQuestionListViewByParameters_SP 'JAMB','ENGLISH','CP',null,null
ALTER procedure [dbo].[GetQuestionListViewByParameters_SP](
@ExamName       VARCHAR(MAX)=NULL,
@Subject        VARCHAR(MAX)=NULL,
@QuestionFormat VARCHAR(MAX)=NULL,
@Marks          INT=NULL,
@IsOnline       BIT=NULL)
AS 
BEGIN
SET NOCOUNT ON

IF(@IsOnline = 0)
BEGIN
SET @IsOnline=NULL;
END
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
SELECT Distinct QB.Id,QB.QuestionId,Decription,QB.DurationInSecond, qf.Text as QuestionFormat
        
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
(QB.ParentId=0 OR ISNULL(QB.ParentId,0)=0) 
End