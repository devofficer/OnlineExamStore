
/****** Object:  StoredProcedure [dbo].[GetQuestionPaperResultSet]    Script Date: 11/09/16 9:39:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		RAVI K SINGH
-- Create date: 5th Feb 2016
-- Description:	TO SHOW QUESTION PAPAR RESULT
-- =============================================
/*
EXEC GetQuestionPaperResultSet 65,45,30,38,'f2e66c59-aa54-490a-a215-118ebc0285eb',15
EXEC GetQuestionPaperResultSet 65,45,30,34,'6638f55f-7601-4655-9e97-299193e4df6a',2 -- CP
*/
ALTER PROCEDURE [dbo].[GetQuestionPaperResultSet] 
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
DECLARE @QuestionResultSet TABLE
 (
   [Index]           INT,
   Id                INT,
   AnswerQuestionId  INT,
   ParentId          INT,
   AnswerOption      VARCHAR(255),
   Answer            VARCHAR(255),
   AnswerStatus      VARCHAR(255),
   Marks             INT,
   Description       VARCHAR(MAX),
   ImagePath         VARCHAR (MAX),
   OptionA           VARCHAR (MAX),
   OptionB           VARCHAR (MAX),
   OptionC           VARCHAR (MAX),
   OptionD           VARCHAR (MAX),
   OptionE           VARCHAR (MAX),
   AnswerDescription VARCHAR(MAX)
 ) 

 DECLARE @TResult TABLE
 (
   [Index] INT,
   Id INT,
   Name VARCHAR(250),
   DurationInMinutes INT,
   TimeTakenInSecond INT,
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
   [Index]           INT,
   Id                INT,
   AnswerQuestionId  INT,
   ParentId          INT,
   AnswerOption      VARCHAR(100),
   Answer            VARCHAR(255),
   AnswerStatus      VARCHAR(255),
   Marks             INT,
   Description       VARCHAR(MAX),
   ImagePath         VARCHAR (MAX),
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

	/* UPDATE TIME TAKEN  */
    UPDATE @TResult SET TimeTakenInSecond= DurationInMinutes - (SELECT TOP 1 TimeTakenInSecond FROM AnswerQuestion
    WHERE QuestionPaperId = @QuestionPaperId AND UserId=@UserId AND UserMaxId=@UserMaxId)

	 /* UPDATE IS-COMPLETLLY ATTEMPTED FLAG  */
	  UPDATE @TResult SET IsCompletelyAttempted =(SELECT CASE WHEN TotalQuestions = TotalAttemptedQuestions THEN 1 ELSE 0 END AS 'IsCompletelyAttempted' FROM @TResult)
	
	/* PREPARE RESULT SET */
	 SELECT QuestionBankId INTO #QuestionBankIds FROM  AnswerQuestion  WHERE QuestionPaperId = @QuestionPaperId AND UserId=@UserId AND UserMaxId=@UserMaxId AND ParentId IS NULL AND FormatType='CP'
	 IF ((SELECT COUNT(*) FROM #QuestionBankIds) > 0)
			BEGIN
			DECLARE @LoopCounter INT = 1
            DECLARE @MaxCount  INT = 0

			 INSERT INTO @QuestionResultSet
			 SELECT ROW_NUMBER() OVER (ORDER BY QB.Id) AS [Index], QB.Id,AQ.Id AS AnswerQuestionId, NULL, QB.AnswerOption,AQ.Answer,CASE WHEN QB.AnswerOption = AQ.Answer THEN 'True' ELSE 'False' END AS 'AnswerStatus',
            		QB.Mark,QB.Decription,QB.ImagePath,QB.OptionA,QB.OptionB,QB.OptionC,QB.OptionD,QB.OptionE,QB.AnswerDescription
             FROM AnswerQuestion AQ INNER JOIN QuestionBank QB ON AQ.QuestionBankId= QB.Id
			 JOIN #QuestionBankIds QBI ON QB.Id = QBI.QuestionBankId AND QB.ParentId IS NULL AND AQ.UserMaxId =@UserMaxId
			  --WHERE QB.Id IN (39736,39856,40107,40113) AND AQ.UserMaxId =@UserMaxId
	 SELECT @MaxCount = COUNT(Id) FROM @QuestionResultSet
	 DECLARE @id INT =0
	 DECLARE @AnswerQuestionId  INT =0
	 WHILE(@LoopCounter <= @MaxCount)
		BEGIN
		   INSERT INTO @TAnswerResult
		   SELECT * FROM @QuestionResultSet WHERE [Index] = @LoopCounter

		  SELECT @id= Id, @AnswerQuestionId=AnswerQuestionId FROM @TAnswerResult WHERE [Index] = @LoopCounter
	   
		   INSERT INTO @TAnswerResult
			SELECT 0 AS [Index],QB.Id,
			             AQ.Id AS AnswerQuestionId, 
					     QB.ParentId,
						 QB.AnswerOption,
						 AQ.Answer,
						 CASE WHEN QB.AnswerOption = AQ.Answer THEN 'True' ELSE 'False' END AS 'AnswerStatus',
						 QB.Mark,
						 QB.Decription,
						 '/images/QuestionImages/Q' + QB.ImagePath + '.png' AS ImagePath,
						 QB.OptionA,
						 QB.OptionB,
						 QB.OptionC,
						 QB.OptionD,
						 QB.OptionE,
						 QB.AnswerDescription
					 FROM AnswerQuestion AQ INNER JOIN QuestionBank QB ON AQ.QuestionBankId= QB.Id
					 AND AQ.ParentId = @AnswerQuestionId
					 WHERE QB.ParentId=@id 

		   SET @LoopCounter  = @LoopCounter  + 1        
		END

	DELETE FROM @QuestionResultSet
	


             --WHERE QB.Id IN (39736, 39742)

					 --JOIN #QuestionBankIds QBI ON AQ.ParentId = QBI.QuestionBankId
			 --WHERE AQ.UserMaxId=0 --AND AQ.ParentId = @QuestionBankId
			END
	ELSE
			BEGIN
			  INSERT INTO @TAnswerResult
			  SELECT 0 AS [Index],QB.Id,
			         AQ.Id AS AnswerQuestionId, 
					 QB.ParentId,
					 QB.AnswerOption,
					 AQ.Answer,
					 CASE WHEN QB.AnswerOption = AQ.Answer THEN 'True' ELSE 'False' END AS 'AnswerStatus',
					 QB.Mark,
					 QB.Decription,
					 '/images/QuestionImages/Q' + QB.ImagePath + '.png' AS ImagePath,
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
	  --UPDATE @TResult SET TotalMarks = (SELECT SUM(Marks) FROM @TAnswerResult)
	  UPDATE @TResult SET TotalMarks = (SELECT SUM(QB.Mark) FROM  QuestionBank QB INNER JOIN
	QuestionPaperMapping QPM ON QB.Id = QPM.QuestionBankId
	WHERE QPM.QuestionPaperId=@QuestionPaperId)
	 

	  /* UPDATE TOTAL OBTAINED MARKS  */
	  UPDATE @TResult SET TotalObtainedMarks = (SELECT SUM(Marks) FROM @TAnswerResult WHERE AnswerStatus='True')

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

	SELECT * FROM @TResult
	SELECT * FROM @TAnswerResult ORDER BY Id

	/************* GET TOP SCORER LIST *****************/
	 SELECT  AU.FirstName + ' ' + AU.LastName AS Name, X.* FROM  UserProfile AU JOIN 
	(SELECT MAX(AQP.TotalObtainedMarks) AS Marks, MAX(AQP.TimeTakenInMinutes) AS TimeTakenInSecond, AQP.UserId
	FROM [dbo].[AttemptedQuestionPapar] AQP 
	WHERE AQP.QuestionPaparId = @QuestionPaperId
	GROUP BY AQP.UserId) X ON AU.ApplicationUser_Id = X.UserId
	ORDER BY X.Marks, X.TimeTakenInSecond DESC
	--SELECT * FROM #QuestionBankIds
END