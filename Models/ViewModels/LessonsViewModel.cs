﻿using System;
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

    }
}