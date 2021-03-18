using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;
using System.IO;
using System.Net;
using System.Data.SqlTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace OnlineExam.Helpers
{
    public static class GlobalUtilities
    {
        public static class Settings
        {
            public static string DefaultPurposeofConsultation = "xxxxxxxxxxxxxxx";
        }

        public static class UploadPaths
        {
            public static string ProfilePicture = "~/FileUploads/RMAConsulantProfiles";
        }

        public static class EmailTemplatePaths
        {
            public static string BookingConsultantTemplate = "~/Content/Templates/BookingConsultantTemplate.html";

            public static string BookingUserTemplete = "~/Content/Templates/EmailTemplate.html";
            public static string BookingAdminTemplete = "~/Content/Templates/AdminConsultantTemplate.html";
            public static string UserRegistrationSuccessfullTemplate = "~/Content/Templates/UserRegisterEmailTemplate.html";
            public static string BokingConfirmConsultantTemplate = "~/Content/Templates/ConsultantConfirmTemplate.html";

            public static string verifyEmailTemplate = "~/Content/Templates/VerifyEmail.html";
            public static string ReferralEmailTemplate = "~/Content/Templates/ReferralEmail.html";
            public static string ForgottenPasswordTemplate = "~/Content/Templates/ForgottenPassword.html";

            public static string BookingConsultant_SentForApproval ="~/Content/Templates/BookingConsultantTemplateAfterApproval.html";

            public static string BookingAdmin_SentForApproval = "~/Content/Templates/AdminConsultantTemplateAfterApproval.html";

        }

        public static class EmailSettings
        {
            public static string SMTPServerUrl = "smtp.gmail.com";
            public static int SMTPServerPort = 587;
            public static bool SMTPSecureConnectionRequired = true;
            public static string SMTPServerLoginName = "testinggroovyprojects55@gmail.com";
            public static string SMTPServerPassword = "testing@123";
            public static string NoReplyEmailAddress = "testinggroovyprojects55@gmail.com";
        }

        public static Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }
        public static string RandomCode(int size)
        {
            var random = new Random();
            string input = "abcdefghijklmnopqrstuvwxyz0123456789";
            var chars = Enumerable.Range(0, size)
                                   .Select(x => input[random.Next(0, input.Length)]);
            return new string(chars.ToArray()).ToUpper();
        }
        public static string GetStringRandomNumber(int numberofChar)
        {
            string chars = "1234567890";

            Random randno = new Random();
            StringBuilder TenDigitNo = new StringBuilder();

            for (int i = 0; i <= numberofChar; i++)
            {
                TenDigitNo.Append(chars[randno.Next(0, chars.Length)]);
            }

            string RandomNumber = TenDigitNo.ToString();
            return RandomNumber;
        }
    }
}
