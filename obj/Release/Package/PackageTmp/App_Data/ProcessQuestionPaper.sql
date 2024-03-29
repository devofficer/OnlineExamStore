
/****** Object:  StoredProcedure [dbo].[ProcessQuestionPaper]    Script Date: 11/09/16 9:39:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		RAVI K SINGH
-- Create date: 11th Feb 2016
-- Description:	TO PROCESS QUESTION PAPER RESULT
-- =============================================
/*
EXEC ProcessQuestionPaper 65,45,30,3,'4bb6fe4b-ab31-45f0-b0b5-8ee3b19fc87c',1
*/
ALTER PROCEDURE [dbo].[ProcessQuestionPaper] 
	-- Add the parameters for the stored procedure here
	@ExcellentPercentage   INT           = 65,
    @Average               INT           = 45,
    @BelowAverage          INT           = 30,
    @QuestionPaperId       INT           = 0,
    @UserId                VARCHAR(100)  = NULL,
    @UserMaxId             INT           = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--TO SHOW QUESTION PAPAR RESULT


 DECLARE @TResult TABLE
 (
   [Index] INT  IDENTITY(1,1),
   Id INT,
   Name VARCHAR(250),
   DurationInMinutes INT,
   TimeTakenInMinutes INT,
   TotalQuestions INT,
   TotalAttemptedQuestions INT,
   TotalCorrectedAnswers INT,
   TotalInCorrectedAnswers INT,
   TotalMarks INT,
   TotalObtainedMarks INT,
   IsCompletelyAttempted BIT,
   FinalStatus VARCHAR(100),
   UserId VARCHAR(100),
   CreatedOn DATETIME
 ) 

 DECLARE @TAnswerResult TABLE
 (
   [Index]           INT  IDENTITY(1,1),
   Id                INT,
   AnswerOption      VARCHAR(1),
   Answer            VARCHAR(1),
   AnswerStatus      VARCHAR(10),
   Marks             INT,
   Description       VARCHAR(MAX),
   ImagePath         VARCHAR (500),
   OptionA           VARCHAR (MAX),
   OptionB           VARCHAR (MAX),
   OptionC           VARCHAR (MAX),
   OptionD           VARCHAR (MAX),
   OptionE           VARCHAR (MAX),
   AnswerDescription VARCHAR(MAX)
 ) 
 --DECLARE @ExcellentPercentage INT SET @ExcellentPercentage= 65
 --DECLARE @Average INT SET @Average = 45
 --DECLARE @BelowAverage INT SET @BelowAverage = 30
 --DECLARE @QuestionPaperId INT SET @QuestionPaperId=3
 --DECLARE @UserId VARCHAR(100) SET @UserId='ravi-ku@hcl.com'
 --DECLARE @UserMaxId INT=0 SET @UserMaxId= 9

 INSERT INTO @TResult (Id, Name,DurationInMinutes, UserId,CreatedOn)
  SELECT QP.Id, QP.Name, QP.Minute, @UserId,GETDATE()
    FROM QuestionPaper QP WHERE QP.Id =@QuestionPaperId
	
	/* UPDATE TOTAL QUESTIONS  */
	 UPDATE @TResult SET TotalQuestions =(SELECT COUNT(QB.Id) FROM  QuestionBank QB INNER JOIN
	QuestionPaperMapping QPM ON QB.Id = QPM.QuestionBankId
	WHERE QPM.QuestionPaperId=@QuestionPaperId)

	/* UPDATE TOTAL ATTEMPTED QUESTIONS  */
    UPDATE @TResult SET TotalAttemptedQuestions= (SELECT COUNT(Id) FROM AnswerQuestion
    WHERE QuestionPaperId = @QuestionPaperId AND UserId=@UserId AND UserMaxId=@UserMaxId)

	

	 /* UPDATE IS-COMPLETLLY ATTEMPTED FLAG  */
	  UPDATE @TResult SET IsCompletelyAttempted =(SELECT CASE WHEN TotalQuestions = TotalAttemptedQuestions THEN 1 ELSE 0 END AS 'IsCompletelyAttempted' FROM @TResult)
	
	/* PREPARE RESULT SET */
	 SELECT Id INTO #QuestionBankIds FROM  AnswerQuestion  WHERE QuestionPaperId = @QuestionPaperId AND UserId=@UserId AND UserMaxId=@UserMaxId AND ParentId IS NULL AND FormatType='CP'
	 IF ((SELECT COUNT(*) FROM #QuestionBankIds) > 0) /* COMPREHENSIVE TYPE QUESTIONS */
			BEGIN
			 INSERT INTO @TAnswerResult (Id,AnswerOption,Answer,AnswerStatus, Marks,[Description]
      ,[ImagePath]
      ,[OptionA]
      ,[OptionB]
      ,[OptionC]
      ,[OptionD]
      ,[OptionE]
      ,[AnswerDescription])
			  SELECT QB.Id,
					 QB.AnswerOption,
					 AQ.Answer,
					 CASE WHEN QB.AnswerOption = AQ.Answer THEN 'True' ELSE 'False' END AS 'AnswerStatus',
					 QB.Mark,
					 QB.Decription,
					 QB.ImagePath,
					 QB.OptionA,
					 QB.OptionB,
					 QB.OptionC,
					 QB.OptionD,
					 QB.OptionE,
					 QB.AnswerDescription
					 FROM AnswerQuestion AQ
					 INNER JOIN #QuestionBankIds QBI ON AQ.ParentId = QBI.Id
					 INNER JOIN QuestionBank QB ON AQ.QuestionBankId= QB.Id
			END
	ELSE
			BEGIN
			  INSERT INTO @TAnswerResult (Id,AnswerOption,Answer,AnswerStatus, Marks,[Description]
      ,[ImagePath]
      ,[OptionA]
      ,[OptionB]
      ,[OptionC]
      ,[OptionD]
      ,[OptionE]
      ,[AnswerDescription])
			  SELECT QB.Id,
					 QB.AnswerOption,
					 AQ.Answer,
					 CASE WHEN QB.AnswerOption = AQ.Answer THEN 'True' ELSE 'False' END AS 'AnswerStatus',
					 QB.Mark,
					 QB.Decription,
					 QB.ImagePath,
					 QB.OptionA,
					 QB.OptionB,
					 QB.OptionC,
					 QB.OptionD,
					 QB.OptionE,
					 QB.AnswerDescription
					 FROM AnswerQuestion AQ INNER JOIN QuestionBank QB ON AQ.QuestionBankId= QB.Id
			 WHERE QuestionPaperId = @QuestionPaperId AND UserId=@UserId AND UserMaxId=@UserMaxId
			END   
	
    
	 /* UPDATE TOTAL CORRECT & IN-CORRECT ANSWERS  */
	 UPDATE @TResult SET TotalCorrectedAnswers= True,
	 TotalInCorrectedAnswers= False FROM (SELECT AnswerStatus FROM @TAnswerResult) up
	 PIVOT (COUNT(AnswerStatus) FOR AnswerStatus IN (True, False)) AS pvt

	  /* UPDATE TOTAL MARKS  */
	  UPDATE @TResult SET TotalMarks = (SELECT SUM(Marks) FROM @TAnswerResult)

	  /* UPDATE TOTAL OBTAINED MARKS  */
	  UPDATE @TResult SET TotalObtainedMarks = (SELECT SUM(Marks) FROM @TAnswerResult WHERE AnswerStatus='True')

	   /* UPDATE TIME TAKEN IN MINUTES  */
	  UPDATE @TResult SET TimeTakenInMinutes= DurationInMinutes - (SELECT TOP 1 TimeTakenInSecond FROM AnswerQuestion
    WHERE QuestionPaperId = @QuestionPaperId AND UserId=@UserId AND UserMaxId=@UserMaxId)

	   /* UPDATE FINAL STATUS  */
	  DECLARE @Percentage FLOAT=(SELECT ROUND((TotalObtainedMarks * 100 / TotalMarks), 2) FROM @TResult)


	   IF(@Percentage >= @ExcellentPercentage)
		BEGIN
		 UPDATE @TResult SET FinalStatus ='Excellent'
		END
		ELSE IF(@Percentage>= @Average AND @Percentage < @ExcellentPercentage)
		BEGIN
		 UPDATE @TResult SET FinalStatus ='Average'
		END
		ELSE IF(@Percentage>= @BelowAverage AND @Percentage < @Average)
		BEGIN
		 UPDATE @TResult SET FinalStatus ='Below Average'
		END
		ELSE 
		 UPDATE @TResult SET FinalStatus ='Below Average'

		 /* UPDATE 'IsArchive' IF RECORD IS ALREADY EXISTS FOR SAME USER */
		  IF EXISTS(SELECT COUNT(*) FROM AttemptedQuestionPapar WHERE UserId= @UserId AND QuestionPaparId= @QuestionPaperId AND IsArchive=0)
		    BEGIN
			    UPDATE AttemptedQuestionPapar SET IsArchive= 1 WHERE UserId= @UserId AND QuestionPaparId= @QuestionPaperId AND IsArchive=0
			END

     /* INSERT PROCESSED QUESTION PAPAR RESULT SET */
		 INSERT INTO [dbo].[AttemptedQuestionPapar]
           ([UserId]
           ,[QuestionPaparId]
           ,[QuestionPaparName]
           ,[TimeTakenInMinutes]
           ,[TotalQuestions]
           ,[TotalMarks]
           ,[TotalObtainedMarks]
           ,[IsCompletelyAttempted]
           ,[TotalCorrectedAnswered]
           ,[TotalInCorrectedAnswered]
           ,[FormatType]
           ,[Status]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
		   ,[IsArchive]
           )
	SELECT UserId, 
	       Id AS QuestionPaperId,
		   Name AS QuestionPaparName,
		   --DurationInMinutes AS '',
		   ISNULL(TimeTakenInMinutes,0),
		   ISNULL(TotalQuestions,0),
		   ISNULL(TotalMarks,0),
		   ISNULL(TotalObtainedMarks,0),
		   IsCompletelyAttempted,
		   ISNULL(TotalCorrectedAnswers,0) AS TotalCorrectedAnswered,
		   ISNULL(TotalInCorrectedAnswers,0) AS TotalInCorrectedAnswered,
		   NULL AS 'FormatType',
		   FinalStatus AS [Status],
		   CreatedOn,
		   UserId AS CreatedBy,
		   NULL AS 'ModifiedOn',
		   NULL AS 'ModifiedBy',
		   0 AS 'IsArchive'
		   FROM @TResult

		   --DROP TABLE #QuestionBankIds
		   --DELETE @TAnswerResult
		   --DELETE @TResult
    --SELECT * FROM #QuestionBankIds
	--SELECT *  FROM @TResult
	--SELECT * FROM @TAnswerResult
	--SELECT *  FROM AttemptedQuestionPapars
    
END
