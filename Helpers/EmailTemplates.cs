using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace OnlineExam.Helpers
{
    public class EmailTemplates
    {

        /// <summary>
        /// Sent for to Consultant after booking
        /// </summary>
        /// <param name="ToName"></param>
        /// <param name="fromName"></param>
        /// <param name="bookingID"></param>
        /// <returns></returns>
        public static string GetEmailConsultant_BookingForApproval(string ToName, string fromName, int bookingID)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.BookingConsultant_SentForApproval));
            TemplateContent = TemplateContent.Replace("##RMAConsultant##", ToName);
            TemplateContent = TemplateContent.Replace("##USER##", fromName);
            TemplateContent = TemplateContent.Replace("##AbsoluteUrl##", currentContext.Request.Url.Authority);
            TemplateContent = TemplateContent.Replace("##RequestID##", bookingID.ToString());
            
            return TemplateContent;

        }
        /// <summary>
        /// Sent to admin before approval
        /// </summary>
        /// <param name="consultName"></param>
        /// <param name="fromName"></param>
        /// <param name="bookingID"></param>
        /// <returns></returns>
        public static string GetEmailAdmin_BookingApproval(string consultName, string fromName, int bookingID)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.BookingAdmin_SentForApproval));
            TemplateContent = TemplateContent.Replace("##Consultant##", consultName);
            TemplateContent = TemplateContent.Replace("##USER##", fromName);
            TemplateContent = TemplateContent.Replace("##AbsoluteUrl##", currentContext.Request.Url.Authority);
            TemplateContent = TemplateContent.Replace("##RequestID##", bookingID.ToString());
            return TemplateContent;
        }

        /// <summary>
        /// Sent to user for booking confirmation.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="consultentName"></param>
        /// <returns></returns>
        public static string GetEmailUser_BookingVerification(string firstName, string lastName, string consultentName, string timeslot)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.BookingUserTemplete));
            TemplateContent = TemplateContent.Replace("##FirstName##", firstName);
            TemplateContent = TemplateContent.Replace("##LastName##", lastName);
            TemplateContent = TemplateContent.Replace("##TimeSlot##", timeslot);
            TemplateContent = TemplateContent.Replace("##ConsaltName##", consultentName);
            return TemplateContent;

        }
        /// <summary>
        /// Sent when Admin adds contastant then username and passwords are emailed.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="userName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string GetEmailUser_BookingConsultant(string firstName, string userName, string Password )
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.BokingConfirmConsultantTemplate));
            TemplateContent = TemplateContent.Replace("##Name##", firstName);
            TemplateContent = TemplateContent.Replace("##Name##", firstName);
            TemplateContent = TemplateContent.Replace("##UserName##",userName);
            TemplateContent = TemplateContent.Replace(" ##Password##", Password);
            return TemplateContent;

        }

        /// <summary>
        /// When user is registered then user click on the verification link then this email is sent back to the user.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetEmailUser_UserRegistration(string username)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.UserRegistrationSuccessfullTemplate));
            TemplateContent = TemplateContent.Replace("##Username##", username);            
            TemplateContent = TemplateContent.Replace("##AbsoluteUrl##", currentContext.Request.Url.Authority);
            return TemplateContent;
        }
        /// <summary>
        /// Verify the email account email by the user.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static string GetEmailUser_VerifyAccount(string name, string userid)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.verifyEmailTemplate));
            TemplateContent = TemplateContent.Replace("##Username##", name);
            TemplateContent = TemplateContent.Replace("##AbsoluteUrl##", currentContext.Request.Url.Authority);
            TemplateContent = TemplateContent.Replace("##UserID##", userid);
            return TemplateContent;
        }
        public static string GetEmailUser_VerifyEmail(string name, string callbackUrl)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.verifyEmailTemplate));
            TemplateContent = TemplateContent.Replace("##Username##", name);
            TemplateContent = TemplateContent.Replace("##AbsoluteUrl##", callbackUrl);
            //TemplateContent = TemplateContent.Replace("##UserID##", userid);
            return TemplateContent;
        }
        public static string GetEmailUser_ReferralEmail(string name, string callbackUrl)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.ReferralEmailTemplate));
            TemplateContent = TemplateContent.Replace("##Username##", name);
            TemplateContent = TemplateContent.Replace("##AbsoluteUrl##", callbackUrl);
            //TemplateContent = TemplateContent.Replace("##UserID##", userid);
            return TemplateContent;
        }
        public static string GetEmailUser_ForgottenPassword(string name, string callbackUrl)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.ForgottenPasswordTemplate));
            TemplateContent = TemplateContent.Replace("##Username##", name);
            TemplateContent = TemplateContent.Replace("##AbsoluteUrl##", callbackUrl);
            //TemplateContent = TemplateContent.Replace("##UserID##", userid);
            return TemplateContent;
        }

        /// <summary>
        /// Sent email to Consultant for approval  any booking 7 days
        /// </summary>
        /// <param name="name"></param>
        /// <param name="user"></param>
        /// <param name="timesloat"></param>
        /// <returns></returns>
        public static string GetBookingEmailConsultant_Approved(string consultantName, string user, string timesloat )
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.BookingConsultantTemplate));
            TemplateContent = TemplateContent.Replace("##RMAConsultant##", consultantName);
            TemplateContent = TemplateContent.Replace("##USER##", user);
            TemplateContent = TemplateContent.Replace("##TimeSlot##", timesloat);
            return TemplateContent;
        }
        /// <summary>
        ///  Sent email to Admin for approval  any booking 7 days
        /// </summary>
        /// <param name="name"></param>
        /// <param name="user"></param>
        /// <param name="timesloat"></param>
        /// <returns></returns>
        public static string GetBookingEmailAdmin_Approved(string user, string timesloat)
        {
            var currentContext = HttpContext.Current;
            var TemplateContent = File.ReadAllText(currentContext.Server.MapPath(GlobalUtilities.EmailTemplatePaths.BookingAdminTemplete));
            TemplateContent = TemplateContent.Replace("##USER##", user);
            TemplateContent = TemplateContent.Replace("##TimeSlot##", timesloat);
            return TemplateContent;
        }


    }
}