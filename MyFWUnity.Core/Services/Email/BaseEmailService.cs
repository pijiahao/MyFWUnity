using MyFWUnity.Common.Config;
using MyFWUnity.Common.Module;
using MyFWUnity.Core.Infrastructure.Email;
using MyFWUnity.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Core.Services.Email
{
    public class BaseEmailService : IEmailService
    {
        public EmailServerSettingSection EmailServerConfig { get; set; }

        public bool SendEmail(EmailMessage msg)
        {
            bool result = false;
            if (msg == null)
            {
                LogModule.Error("Missing email message");
                return result;
            }

            if (EmailServerConfig == null)
            {
                LogModule.Error("Email Server is not configured correctly");
                return result;
            }

            string fromAddress = string.IsNullOrEmpty(msg.From) ? EmailServerConfig.DefaultFromAddress : msg.From;

            if (string.IsNullOrEmpty(EmailServerConfig.SMTPHost) || string.IsNullOrEmpty(fromAddress))
            {
                LogModule.Error("Email Server is not configured correctly");
                return result;
            }

            if (string.IsNullOrEmpty(msg.SendTo))
            {
                LogModule.Error("Missing receiver address");
                return result;
            }

            if (string.IsNullOrEmpty(msg.Subject))
            {
                LogModule.Warn("Missing email subject");
                return result;
            }

            if (string.IsNullOrEmpty(msg.Body))
            {
                LogModule.Warn("Missing email body");
                //return;
            }

            MailMessage mail = null;
            try
            {
                SmtpClient emailClient = new SmtpClient();
                emailClient.Host = EmailServerConfig.SMTPHost;
                emailClient.Port = EmailServerConfig.Port;
                emailClient.Timeout = EmailServerConfig.Timeout;
                emailClient.EnableSsl = EmailServerConfig.EnableSSL;
                if (!string.IsNullOrEmpty(EmailServerConfig.UserName))
                {
                    System.Net.NetworkCredential netCredential = new System.Net.NetworkCredential();
                    emailClient.Credentials = netCredential;
                    netCredential.UserName = EmailServerConfig.UserName;
                    netCredential.Password = EmailServerConfig.Password;
                }
                mail = GetMail(msg);

                LogModule.Debug(string.Format("Start to send mail to {0}", msg.SendTo));

                EmailUserState userState = new EmailUserState() { From = mail.From.Address, SendTo = msg.SendTo, Subject = msg.Subject, Attachments = msg.Attachments };
                emailClient.SendCompleted += new SendCompletedEventHandler(EmailClientSendCompleted);
                //emailClient.SendAsync(mail, userState);

                LogModule.Debug(string.Format("{0} sends email to {1}. Subject {2}", mail.From, mail.To, mail.Subject));

                emailClient.Send(mail);
                result = true;
            }
            catch (Exception ex)
            {
                LogModule.Error("Failed to send email", ex);
                if (mail != null)
                {
                    LogModule.Error(string.Format("{0} failed to send email to {1}. Subject {2}", mail.From, mail.To, mail.Subject));
                }
            }
            return result;
        }

        private MailMessage GetMail(EmailMessage emailMessage)
        {
            MailMessage mail = new MailMessage();
            mail.Subject = emailMessage.Subject;
            string fromAddress = string.IsNullOrEmpty(emailMessage.From) ? EmailServerConfig.DefaultFromAddress : emailMessage.From;
            mail.From = new MailAddress(fromAddress);
            mail.To.Add(emailMessage.SendTo);
            if (!string.IsNullOrEmpty(emailMessage.CC))
                mail.CC.Add(emailMessage.CC);

            mail.IsBodyHtml = emailMessage.IsBodyHTML;
            mail.Body = emailMessage.Body;
            if (emailMessage.Attachments != null)
            {
                foreach (var attachment in emailMessage.Attachments)
                {
                    try
                    {
                        if (System.IO.File.Exists(attachment.FilePath))
                        {
                            mail.Attachments.Add(new Attachment(attachment.FilePath, "application/octet-stream"));
                        }
                    }
                    catch (Exception ex)
                    {
                        LogModule.Error(string.Format("Failed to add attachment {0} to email.", attachment), ex);
                    }
                }
            }

            return mail;
        }

        private void EmailClientSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            SmtpClient emailClient = (SmtpClient)sender;
            EmailUserState userState = (EmailUserState)e.UserState;
            try
            {
                emailClient.SendCompleted -= new SendCompletedEventHandler(EmailClientSendCompleted);

                if (e.Error != null)
                {
                    LogModule.Error("Failed to send email", e.Error);
                }
                else
                {
                    LogModule.Debug(string.Format("Send mail {0} to {1}", userState.Subject, userState.SendTo));
                }

                // Clear file attachment
                //if (userState.Attachments != null)
                //{
                //    foreach (var attachment in userState.Attachments)
                //    {
                //        if (System.IO.File.Exists(attachment.FilePath))
                //        {
                //            System.IO.File.Delete(attachment.FilePath);
                //        }
                //    }
                //}

                emailClient.Dispose();
            }
            catch (Exception ex)
            {
                LogModule.Error("Failed to run complete sending email", ex);
                LogModule.Error(string.Format("{0} failed to send email to {1}. Subject {2}", userState.From, userState.SendTo, userState.Subject));
            }
            finally
            {
                if (emailClient != null)
                    emailClient.Dispose();
            }
        }
    }

    internal class EmailUserState
    {
        public string From { get; set; }
        public string SendTo { get; set; }
        public string Subject { get; set; }
        public List<AttachmentsDataInfo> Attachments { get; set; }
    }
}
