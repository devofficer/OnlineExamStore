using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineExam.Models.APIViewModels
{
    public class DashboardViewModel: TransactionalInformation
    {
        public int TotalCBTs { get; set; }
        public int TotalAttemptedCBTs { get; set; }
        public int TotalStdCBTs { get; set; }
        public int TotalAttemptedStdCBTs { get; set; }
        public int TotalCustomCBTs { get; set; }
        public int TotalAttemptedCustomCBTs { get; set; }
    }
}