using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using OnlineExam.Helpers;
using System.Net;
using System.Configuration;
using OnlineExam;

/// <summary>
/// Summary description for MailGun
/// </summary>
public static class EmailService
{

    public static void ContactUs(string toAddress, string body, string subject)
    {
#if DEBUG
            var username = System.Configuration.ConfigurationManager.AppSettings["FromMailAddress"];//GlobalUtilities.EmailSettings.SMTPServerLoginName;
            string password = System.Configuration.ConfigurationManager.AppSettings["MailingPassword"];//GlobalUtilities.EmailSettings.SMTPServerPassword;

            MailMessage msg = new MailMessage();
            msg.Subject = subject;
            msg.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromMailAddress"]);
            msg.CC.Add(new MailAddress(toAddress));

            msg.IsBodyHtml = true;
            msg.Body = body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = System.Configuration.ConfigurationManager.AppSettings["MailingHost"];//GlobalUtilities.EmailSettings.SMTPServerUrl;
            smtpClient.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);//GlobalUtilities.EmailSettings.SMTPServerPort;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
            smtpClient.Send(msg);

#else
        //Create the msg object to be sent
        MailMessage msg = new MailMessage();
        //Add your email address to the recipients

        //Configure the address we are sending the mail from
        //MailAddress address = new MailAddress("no-reply@acadastore.com");
        MailAddress address = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Prod_ContactMailAddress"]);

        msg.From = new MailAddress(toAddress);
        msg.To.Add(address);

        msg.Subject = subject;
        msg.Body = body;
        msg.IsBodyHtml = true;

        SmtpClient client = new SmtpClient();
        client.Host = System.Configuration.ConfigurationManager.AppSettings["Prod_MailingHost"];
        client.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Prod_Port"]);
        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Prod_ContactMailAddress"], ConfigurationManager.AppSettings["Prod_MailingPassword"]);
        //Send the msg
        client.Send(msg);

#endif
    }
    public static void SendMail(string toAddress, string body, string subject)
    {
#if DEBUG
            var username = System.Configuration.ConfigurationManager.AppSettings["FromMailAddress"];//GlobalUtilities.EmailSettings.SMTPServerLoginName;
            string password = System.Configuration.ConfigurationManager.AppSettings["MailingPassword"];//GlobalUtilities.EmailSettings.SMTPServerPassword;

            MailMessage msg = new MailMessage();
            msg.Subject = subject;
            msg.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["FromMailAddress"]);
            msg.CC.Add(new MailAddress(toAddress));

            msg.IsBodyHtml = true;
            msg.Body = body;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = System.Configuration.ConfigurationManager.AppSettings["MailingHost"];//GlobalUtilities.EmailSettings.SMTPServerUrl;
            smtpClient.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);//GlobalUtilities.EmailSettings.SMTPServerPort;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
            smtpClient.Send(msg);

#else
        //Create the msg object to be sent
        MailMessage msg = new MailMessage();
        //Add your email address to the recipients
        msg.To.Add(toAddress);

        // TO STOP EMAIL ENTER INTO SPAM FOLDER
        msg.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
        System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString
(System.Text.RegularExpressions.Regex.Replace(body, @"<(.|\n)*?>", string.Empty), null, "text/plain");
        System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(body, null, "text/html");

        msg.AlternateViews.Add(plainView);
        msg.AlternateViews.Add(htmlView);


        //Configure the address we are sending the mail from
        //MailAddress address = new MailAddress("no-reply@acadastore.com");
        MailAddress address = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Prod_NoReplyMailAddress"]);
        msg.From = address;
        msg.Subject = subject;
        msg.Body = body;
        msg.IsBodyHtml = true;

        SmtpClient client = new SmtpClient();
        client.Host = System.Configuration.ConfigurationManager.AppSettings["Prod_MailingHost"];
        client.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Prod_Port"]);
        client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Prod_NoReplyMailAddress"], ConfigurationManager.AppSettings["Prod_MailingPassword"]);
        //Send the msg
        client.Send(msg);
#endif

    }
}