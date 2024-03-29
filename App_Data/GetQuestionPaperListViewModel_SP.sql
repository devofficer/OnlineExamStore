
/****** Object:  StoredProcedure [dbo].[GetQuestionPaperListViewModel_SP]    Script Date: 17/10/15 8:53:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--Exec [GetQuestionPaperListViewModel_SP] null,null,null, '6638f55f-7601-4655-9e97-299193e4df6a', '#unattempted-tab', false
ALTER PROCEDURE [dbo].[GetQuestionPaperListViewModel_SP](
@ClassName VARCHAR(MAX)=NULL,
@ExamName VARCHAR(MAX)=NULL,
@Subject VARCHAR(MAX)=NULL,
@UserId VARCHAR(100)= NULL,
@OpType VARCHAR(50) = NULL,
@IsACDAStoreUser Bit)
As 

Begin
SET NOCOUNT ON
	IF(@ClassName='')
	Begin
	SET @ClassName=null;
	End

	IF(@ExamName='')
	Begin
	set @ExamName=null;
	End
	
	IF(@Subject = '')
	Begin
	set @Subject=null;
	End
	
	IF(@OpType = '#attempted-tab' AND @UserId IS NOT NULL)
		BEGIN 
			SELECT Distinct  CAST(ROW_NUMBER() over(order by aqp.CreatedOn desc) AS Int)  AS [Index],
			CAST(QP.Id AS Int) AS Id,
			QP.Name,
			CAST(RIGHT('0' + CONVERT(VARCHAR(3),  QP.Minute /60), 3) AS Int)  AS [Minute],
			
			ISNULL(aqp.TimeTakenInMinutes,0) AS TimeTakenInMinutes,
			ISNULL(aqp.TotalObtainedMarks,0) AS TotalObtainedMarks,
			QP.IsTrial,
			QP.IsOnline,
			QP.ClassName,
			QP.ExamName,
			QP.Subject,
			(select Count(Id)  from QuestionPaperMapping where QuestionPaperId=QP.Id) as  TotalQuestions,
			(select Sum(Q.Mark) from QuestionPaperMapping QM inner join QuestionBank Q  on QM.QuestionBankId=Q.Id where QM.QuestionPaperId=QP.Id) as  TotalMarks,
			0 AS [Status],			
		   /* 0 MEANS USER IS NON- ADMIN */				
			CASE WHEN dbo.IsAdminUser(QP.CreatedBy) =0 THEN 'Custom' ELSE 'Standard' End as CBTType,
			QP.IsActive,
			aqp.CreatedBy,
			aqp.CreatedOn,
			aqp.ModifiedBy,
			aqp.ModifiedOn
                            
                                    
			FROM QuestionPaper QP 
			INNER JOIN AttemptedQuestionPapar aqp ON QP.Id = aqp.QuestionPaparId
			inner join [Lookup] e on QP.[ExamName] = e.Value and e.ModuleCode='ExamType'  and e.IsActive=1
			inner join [Lookup] s on QP.[Subject] = s.Value and s.ModuleCode='Subject'  and e.IsActive=1
			inner join [Lookup] Qf on QP.ClassName = Qf.Value and Qf.ModuleCode='ClassType'  and e.IsActive=1

			WHERE 
			QP.ExamName=COALESCE(@ExamName, QP.ExamName) AND 
			QP.[Subject]=COALESCE(@Subject, QP.[Subject]) AND
			QP.ClassName=COALESCE(@ClassName, QP.ClassName) AND
			aqp.CreatedBy = @UserId
			ORDER BY aqp.CreatedOn desc--,QP.Name,QP.ClassName,QP.ExamName,QP.Subject
		END
	ELSE
		BEGIN
			IF(@UserId IS NOT NULL AND @IsACDAStoreUser = 'true')
				BEGIN ---For ACDAStoreUser
				   SELECT Distinct  CAST(ROW_NUMBER() over(order by QP.CreatedOn desc) AS Int)  AS [Index],
					CAST(QP.Id AS Int) AS Id,
					QP.Name,
					CAST(RIGHT('0' + CONVERT(VARCHAR(3),  QP.Minute /60), 3) as Int) AS [Minute],
					0 AS TimeTakenInMinutes,
					0 AS TotalObtainedMarks,
					QP.IsTrial,
					QP.IsOnline,
					QP.ClassName,
					QP.ExamName,
					QP.Subject,
					(select COUNT(Id)  from QuestionPaperMapping where QuestionPaperId=QP.Id) as  TotalQuestions,
					ISNULL((select SUM(Q.Mark) from QuestionPaperMapping QM inner join QuestionBank Q  on QM.QuestionBankId=Q.Id where QM.QuestionPaperId=QP.Id),0)  as  TotalMarks,
					0 AS [Status],					
		      	/* 0 MEANS USER IS NON- ADMIN */				
			         CASE WHEN dbo.IsAdminUser(QP.CreatedBy) =0 THEN 'Custom' ELSE 'Standard' End as CBTType,
					QP.IsActive,
					QP.CreatedBy,
					QP.CreatedOn,
					QP.ModifiedBy,
					QP.ModifiedOn
                            
                                    
					FROM QuestionPaper QP 
					inner join [Lookup] e on QP.[ExamName] = e.Value and e.ModuleCode='ExamType'  and e.IsActive=1
					inner join [Lookup] s on QP.[Subject] = s.Value and s.ModuleCode='Subject'  and e.IsActive=1
					inner join [Lookup] Qf on QP.ClassName = Qf.Value and Qf.ModuleCode='ClassType'  and e.IsActive=1

					WHERE 
					QP.ExamName=COALESCE(@ExamName, QP.ExamName) and 
					QP.[Subject]=COALESCE(@Subject, QP.[Subject]) and
					QP.ClassName=COALESCE(@ClassName, QP.ClassName)

					ORDER BY QP.CreatedOn desc --,QP.Name,QP.ClassName,QP.ExamName,QP.Subject DESC
				END
			ELSE
				BEGIN
					SELECT Distinct  CAST(ROW_NUMBER() over(order by QP.CreatedOn desc) AS Int)  AS [Index],
					CAST(QP.Id AS Int) AS Id,
					QP.Name,
					CAST(RIGHT('0' + CONVERT(VARCHAR(3),  QP.Minute /60), 3) AS Int)  AS [Minute],
					0 AS TimeTakenInMinutes,
					0 AS TotalObtainedMarks,
					QP.IsTrial,
					QP.IsOnline,
					QP.ClassName,
					QP.ExamName,
					QP.Subject,
					(select Count(Id)  from QuestionPaperMapping where QuestionPaperId=QP.Id) as  TotalQuestions,
					ISNULL((select SUM(Q.Mark) from QuestionPaperMapping QM inner join QuestionBank Q  on QM.QuestionBankId=Q.Id where QM.QuestionPaperId=QP.Id),0)  as  TotalMarks,
					CASE WHEN ISNULL(AQP.CreatedOn,0) = 0 THEN 1
					     WHEN  datediff(DAY,CAST(AQP.CreatedOn as Date),getdate())>5 and AQP.CreatedBy = @UserId THEN 2
						 WHEN  datediff(DAY,CAST(AQP.CreatedOn as Date),getdate())<5 and AQP.CreatedBy = @UserId THEN 3 ELSE 1
						  
					END  'Status',
			       /* 0 MEANS USER IS NON- ADMIN */				
			         CASE WHEN dbo.IsAdminUser(QP.CreatedBy) =0 THEN 'Custom' ELSE 'Standard' End as CBTType,
					QP.IsActive,
					QP.CreatedBy,
					QP.CreatedOn,
					QP.ModifiedBy,
					QP.ModifiedOn

					FROM QuestionPaper QP 
					LEFT JOIN AttemptedQuestionPapar AQP on QP.Id = AQP.QuestionPaparId  AND AQP.IsArchive=0
					inner join [Lookup] e on QP.[ExamName] = e.Value and e.ModuleCode='ExamType'  and e.IsActive=1
					inner join [Lookup] s on QP.[Subject] = s.Value and s.ModuleCode='Subject'  and e.IsActive=1
					inner join [Lookup] Qf on QP.ClassName = Qf.Value and Qf.ModuleCode='ClassType'  and e.IsActive=1

					WHERE 
					QP.ExamName=COALESCE(@ExamName, QP.ExamName) and 
					QP.[Subject]=COALESCE(@Subject, QP.[Subject]) and
					QP.ClassName=COALESCE(@ClassName, QP.ClassName) and 
					(qp.CreatedBy = @UserId OR qp.CreatedBy in (select Id from AspNetUsers where right(email,14)='acadastore.com'))

					ORDER BY QP.CreatedOn desc --,QP.Name,QP.ClassName,QP.ExamName,QP.Subject DESC					
				END	      
		END
END

