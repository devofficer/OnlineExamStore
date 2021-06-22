using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.ViewModels
{
    public class LessonsViewModel
    {
        public int LessonId { get; set; }
        public Guid LessonGuId { get; set; }
        public int ProfileId { get; set; }
        public string TeacherName { get; set; }
        public string ClassType { get; set; }
        public string SubjectCategory { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PaymentType { get; set; }
        public decimal? Amount { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsActive { get; set; }
        public string IsEnroll { get; set; }
    }
    public class LessonsList
    {
        public int LessonId { get; set; }
        public Guid LessonGuId { get; set; }
        public int ProfileId { get; set; }
        public string ClassType { get; set; }
        public string SubjectCategory { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PaymentType { get; set; }
        public decimal? Amount { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsActive { get; set; }
    }
    public class LessonItemsViewModel
    {
        public long Id { get; set; }
        public int LessonId { get; set; }
        public string ItemTitle { get; set; }
        public string ItemDescription { get; set; }
        public string FolderName { get; set; }
        public string FilePath { get; set; }
        public string FileNames { get; set; }
        public string DownloadLinks { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsActive { get; set; }
    }

    public class LessonDiscussionsViewModel
    {
        public long Id { get; set; }
        public int LessonId { get; set; }
        public int ProfileId { get; set; }
        public string UserName { get; set; }
        public string UserAvater { get; set; }
        public string Note { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public string IsActive { get; set; }
    }

    public class TeacherFollowViewModel
    {
        public long Id { get; set; }
        public int TeacherId { get; set; }
        public int FollowUserId { get; set; }
        public string TeacherName { get; set; }
        public string FollowUserName { get; set; }
        public string StartDate { get; set; }
        public string UserType { get; set; }
        public string ClassType { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }

    }

    public class GroupsViewModel
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class TeachersViewModel
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string ClassType { get; set; }
        public string SubjectType { get; set; }
        public string IsFollowed { get; set; }
    }

    public class EnrollQuestionPaperViewModel
    {
        public int AttenId { get; set; }
        public string AttendUserId { get; set; }
        public int QuestionPaperId { get; set; }
        public int TeacherProfileId { get; set; }
        public int StudentProfileId { get; set; }
        public string AttendStudentName { get; set; }
        public string QuestionPaperTitle { get; set; }
        public int TimeTaken { get; set; }
        public int TotalQuestions { get; set; }
        public int TotalMarks { get; set; }
        public int TotalObtainedMarks { get; set; }
        public int TotalCorrectedAnswered { get; set; }
        public int TotalInCorrectedAnswered { get; set; }
        public string IsCompleted { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string TeacherName { get; set; }
        public string ClassType { get; set; }
        public string CategoryType { get; set; }
        public string SubjectType { get; set; }
    }

    public class QuestionPaperSuggestionViewModel
    {
        public int QuestionPaperId { get; set; }
        public int TeacherProfileId { get; set; }
        public string AttendStudentName { get; set; }
        public string QuestionPaperTitle { get; set; }
        public int TotalQuestions { get; set; }
        public int? TotalTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string TeacherName { get; set; }
        public string ClassType { get; set; }
        public string CategoryType { get; set; }
        public string SubjectType { get; set; }
        public int TotalAttend { get; set; }
    }


    public class PendinQuestionViewModel
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int QuestionId { get; set; }
        public string TeacherName { get; set; }
        public string Description { get; set; }
        public string CategoryType { get; set; }
        public string SubjectType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}