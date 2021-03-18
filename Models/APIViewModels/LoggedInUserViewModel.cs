using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class LoggedInUserViewModel
    {
        public bool IsACDAStoreUser { get; set; }
        public string UserFullName { get; set; }
        public string UserType { get; set; }
        public string ClassTypes { get; set; }
        public string MembershipPlan { get; set; }
        public string MembershipPlanCode { get; set; }
        public string Avatar { get; set; }
        public string CurrentRole { get; set; }
        public string CurrentUserEmail { get; set; }
        public string UserId { get; set; }
    }
}