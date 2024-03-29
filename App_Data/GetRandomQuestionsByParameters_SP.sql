
/****** Object:  StoredProcedure [dbo].[GetRandomQuestionsByParameters_SP]    Script Date: 17/10/15 8:55:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

---- Exec GetRandomQuestionsByParameters_SP 'WAEC','Government',null,null,true
ALTER PROCEDURE [dbo].[GetRandomQuestionsByParameters_SP](
@ExamName       VARCHAR(MAX)=NULL,
@Subject        VARCHAR(MAX)=NULL,
@QuestionFormat VARCHAR(MAX)=NULL,
@Marks          INT=NULL,
@IsOnline       BIT=NULL,
@NoOfRecords    INT = 10)
AS 
BEGIN
SET NOCOUNT ON

IF(@Marks = 0)
BEGIN
SET @Marks=null;
END
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

print @NoOfRecords

SELECT DISTINCT TOP (@NoOfRecords) QB.Id AS QuestionId, QB.DurationInSecond, NEWID() AS [NewId]   
FROM QuestionBank QB 
inner join [Lookup] e on QB.[ExamName] = e.Value and e.ModuleCode='ExamType'  and e.IsActive=1
inner join [Lookup] s on QB.[Subject] = s.Value and s.ModuleCode='Subject'  and e.IsActive=1
inner join [Lookup] Qf on QB.QuestionFormat = Qf.Value and Qf.ModuleCode='QuestionFormatType'  and e.IsActive=1

WHERE QB.ExamName=COALESCE(@ExamName, QB.ExamName) and 
QB.[Subject]=COALESCE(@Subject, QB.[Subject]) and
QB.questionFormat=COALESCE(@QuestionFormat, QB.questionFormat) and
QB.Mark=COALESCE(@Marks, QB.Mark) and
QB.isOnline=COALESCE(@IsOnline, QB.isOnline) AND
(QB.ParentId=0 OR ISNULL(QB.ParentId,0)=0 )
 ORDER BY NEWID()
END