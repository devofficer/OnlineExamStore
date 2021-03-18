
/****** Object:  StoredProcedure [dbo].[GetUserDashboardInfo]    Script Date: 27/08/16 3:00:12 PM ******/
DROP PROCEDURE [dbo].[GetUserDashboardInfo]
GO

/****** Object:  StoredProcedure [dbo].[GetUserDashboardInfo]    Script Date: 27/08/16 3:00:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		RAVI K SINGH
-- Create date: 10th AUG 2016
-- Description:	TO SHOW ALL TYPES OF CBTs ON DASHBOARD
-- =============================================
/*
EXEC GetUserDashboardInfo 'f2e66c59-aa54-490a-a215-118ebc0285eb','JSS 1- JSS 3'
*/
CREATE PROCEDURE [dbo].[GetUserDashboardInfo] 
 @UserId     VARCHAR(100)  = NULL,
 @ClassName  VARCHAR(MAX)=NULL
	
AS
BEGIN
	SET NOCOUNT ON;
			DECLARE @CBTResultSet TABLE
			(
			   [Index]           INT,
			   IsTrial           INT,
			   IsOnline          INT,
			   ClassName         VARCHAR(255),
			   [Subject]         VARCHAR(255),
			   CBTType           VARCHAR(255),
			   IsActive          INT,
			   CreatedBy         VARCHAR(255),
			   CreatedOn         DATETIME,
			   ModifiedBy        VARCHAR (255),
			   ModifiedOn        DATETIME
			 )

			DECLARE @DashboardReport TABLE
			(
			   TotalCBTs                    INT,
			   TotalAttemptedCBTs           INT,
			   TotalStdCBTs                 INT,
			   TotalAttemptedStdCBTs        INT,
			   TotalCustomCBTs              INT,
			   TotalAttemptedCustomCBTs     INT
			 )

			INSERT INTO @CBTResultSet
			SELECT Distinct  CAST(ROW_NUMBER() over(order by aqp.CreatedOn desc) AS Int)  AS [Index],
			QP.IsTrial,
			QP.IsOnline,
			QP.ClassName,
			QP.Subject,
			CASE WHEN QP.CreatedBy = @UserId THEN 'Custom' ELSE 'Standard' End as CBTType,
			QP.IsActive,
			aqp.CreatedBy,
			aqp.CreatedOn,
			aqp.ModifiedBy,
			aqp.ModifiedOn
                                    
			FROM QuestionPaper QP 
			INNER JOIN AttemptedQuestionPapar aqp ON QP.Id = aqp.QuestionPaparId
			INNER JOIN [Lookup] Qf on QP.ClassName = Qf.Value AND Qf.ModuleCode='ClassType'  

			WHERE 
			QP.ClassName=COALESCE(@ClassName, QP.ClassName) AND	aqp.CreatedBy = @UserId
			
			INSERT INTO @DashboardReport (TotalAttemptedCBTs) SELECT COUNT(*) FROM @CBTResultSet

			UPDATE @DashboardReport SET TotalAttemptedStdCBTs= (SELECT COUNT(*) FROM @CBTResultSet WHERE CBTType ='Standard')
			UPDATE @DashboardReport SET TotalAttemptedCustomCBTs= (SELECT COUNT(*) FROM @CBTResultSet WHERE CBTType ='Custom')

		
			DELETE FROM @CBTResultSet

			INSERT INTO @CBTResultSet
			 SELECT Distinct  CAST(ROW_NUMBER() over(order by QP.CreatedOn desc) AS Int)  AS [Index],
					QP.IsTrial,
					QP.IsOnline,
					QP.ClassName,
					QP.Subject,
					CASE WHEN QP.CreatedBy = @UserId THEN 'Custom' ELSE 'Standard' End as CBTType,
					QP.IsActive,
					QP.CreatedBy,
					QP.CreatedOn,
					QP.ModifiedBy,
					QP.ModifiedOn
                                    
					FROM QuestionPaper QP 
					INNER JOIN [Lookup] s on QP.[Subject] = s.Value and s.ModuleCode='Subject'  and s.IsActive=1
					INNER JOIN [Lookup] Qf on QP.ClassName = Qf.Value and Qf.ModuleCode='ClassType'  and Qf.IsActive=1
					WHERE QP.ClassName=COALESCE(@ClassName, QP.ClassName)

			  	UPDATE @DashboardReport SET TotalCBTs=  (SELECT COUNT(*) FROM @CBTResultSet)
			    UPDATE @DashboardReport SET TotalStdCBTs= (SELECT COUNT(*) FROM @CBTResultSet WHERE CBTType ='Standard')
			    UPDATE @DashboardReport SET TotalCustomCBTs= (SELECT COUNT(*) FROM @CBTResultSet WHERE CBTType ='Custom')

			 	SELECT * FROM @DashboardReport
		
END

GO


