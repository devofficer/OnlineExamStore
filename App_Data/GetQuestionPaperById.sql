
/****** Object:  StoredProcedure [dbo].[GetQuestionPaperById]    Script Date: 07/08/16 12:28:38 PM ******/
DROP PROCEDURE [dbo].[GetQuestionPaperById]
GO

/****** Object:  StoredProcedure [dbo].[GetQuestionPaperById]    Script Date: 07/08/16 12:28:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		RAVI K SINGH
-- Create date: 05th MAY 2016
-- Description:	TO GET QUESTION PAPER & QUESTIONS BY QUESTION PAPER ID
-- =============================================
/*
EXEC GetQuestionPaperById 4
*/
CREATE PROCEDURE [dbo].[GetQuestionPaperById] 
	@QuestionPaperId INT 
AS
BEGIN
DECLARE @QuestionsTab TABLE
 (
   [Index]           INT, 
   QuestionPaperId   INT,
   Name              VARCHAR(MAX),
   Id                INT, --QuestionBankId
   QuestionId                INT, --QuestionId
   Title             VARCHAR(MAX),
   OptionA           VARCHAR(MAX),
   OptionB           VARCHAR(MAX),
   OptionC           VARCHAR(MAX),
   OptionD           VARCHAR(MAX),
   OptionE           VARCHAR(MAX),
   FormatType        VARCHAR(MAX),
   ImagePath         VARCHAR(MAX),
   Mark              INT,
   Duration          INT,
   QuestionCount     INT,
   Subject           VARCHAR(200)  
 )

 DECLARE @ChildQuestionsTab TABLE
 (
   Id                INT,
   QuestionId    INT,
   Title             VARCHAR(MAX),
   OptionA           VARCHAR(MAX),
   OptionB           VARCHAR(MAX),
   OptionC           VARCHAR(MAX),
   OptionD           VARCHAR(MAX),
   OptionE           VARCHAR(MAX),
   FormatType        VARCHAR(MAX),
   ImagePath         VARCHAR(MAX)
 )
    INSERT @QuestionsTab
	SELECT ROW_NUMBER() OVER (ORDER BY QuestionBankId) AS [Index],
	    QuestionPaperId, 
	    Name,
		QB.Id AS QuestionBankId,  
		QB.QuestionId,
		QB.Decription AS Title, 
		QB.OptionA, QB.OptionB, QB.OptionC, QB.OptionD, QB.OptionE, 
		QB.QuestionFormat AS FormatType,
		CASE WHEN LEN(QB.ImagePath) > 0 THEN '/images/QuestionImages/Q'+ + '.png' ELSE NULL END AS ImagePath,
		QB.Mark,
		QB.DurationInSecond,
		0, --QuestionCount
		QB.Subject 
		FROM 
		(
			SELECT QPM.QuestionBankId, QP.Id AS QuestionPaperId, QP.Name  FROM QuestionPaper QP 
			 JOIN QuestionPaperMapping QPM 
			ON QP.Id = QPM.QuestionPaperId
		) Q JOIN QuestionBank QB ON Q.QuestionBankId = QB.Id
		WHERE Q.QuestionPaperId=@QuestionPaperId
    
	SELECT QuestionPaperId, Name, SUM(Mark) AS TotalMarks, SUM(Duration) AS Duration, COUNT (Id) AS QuestionCount, Subject 
	FROM @QuestionsTab GROUP BY QuestionPaperId, Name, Duration, QuestionCount, Subject

	/* GET CHILD QUESTIONS */
	 DECLARE @LoopCounter INT = 1
	 DECLARE @MaxCount  INT = 0
	 DECLARE @QuestionBankId INT =0
	 SELECT @MaxCount = COUNT(QuestionPaperId) FROM @QuestionsTab
	 DECLARE @id INT =0
	 DECLARE @AnswerQuestionId  INT =0
	 WHILE(@LoopCounter <= @MaxCount)
		BEGIN
		  SELECT @QuestionBankId=Id FROM @QuestionsTab WHERE [Index] = @LoopCounter
	      
		  INSERT INTO @ChildQuestionsTab
		  SELECT Id, QuestionId,Decription,OptionA,OptionB,OptionC,OptionD,OptionE,QuestionFormat,
		  CASE WHEN LEN(ImagePath) > 0 THEN '/images/QuestionImages/Q'+ + '.png' ELSE NULL END AS ImagePath
		  FROM QuestionBank WHERE LEN(ParentId) > 0 AND ParentId=@QuestionBankId AND QuestionFormat='CP'
		  
		  SET @LoopCounter  = @LoopCounter  + 1        
		END

	 SELECT * FROM @QuestionsTab
     SELECT * FROM @ChildQuestionsTab
END

GO


