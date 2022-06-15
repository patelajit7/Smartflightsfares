using Common;
using Configration;
using Infrastructure;
using Infrastructure.HelpingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class EmailHelper
    {

        public static bool SendMails(EmailTransaction transaction)
        {
            bool isMailSent = false;
            try
            {
                string url = string.Empty;
                EmailTemplate emailTemplate = null;
                Emails emails = null;
                emails = Utility.PortalSettings.Emails.Where<Emails>(o => o.Portal == transaction.PortalId).FirstOrDefault<Emails>();
                if (emails != null)
                {
                    emailTemplate = null;

                    switch (transaction.EmailType)
                    {
                        case EmailType.BookingReceipt:
                            emailTemplate = new EmailTemplate()
                            {
                                AdminMailBcC = emails.BookingReceipt.AdminMailBcC,
                                EmailPass = emails.BookingReceipt.EmailPass,
                                EmailUserId = emails.BookingReceipt.EmailUserId,
                                IsHtml = emails.BookingReceipt.IsHtml,
                                Subject = emails.BookingReceipt.Subject,
                                MailRecipient = transaction.MailRecipient,
                                Body = transaction.MailBody
                            };
                            emailTemplate.Subject = emailTemplate.Subject.Replace("####", transaction.TransactionId.ToString());
                            isMailSent = IsMailSent(emailTemplate);
                            break;
                        case EmailType.ItineryDetails:
                            emailTemplate = new EmailTemplate()
                            {
                                AdminMailBcC = emails.BookingReceipt.AdminMailBcC,
                                EmailPass = emails.BookingReceipt.EmailPass,
                                EmailUserId = emails.BookingReceipt.EmailUserId,
                                IsHtml = emails.BookingReceipt.IsHtml,
                                Subject = string.Format("{0}: Requested Itinerary",Utility.PortalSettings.PortalDetails.BrandName),
                                MailRecipient = transaction.MailRecipient,
                                Body = transaction.MailBody
                            };
                            emailTemplate.Subject = emailTemplate.Subject;
                            isMailSent = IsMailSent(emailTemplate);
                            break;

                    }
                    return isMailSent;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Business.EmailHelper|PORATAL:{0}|EMAILTYPE:{1}|ID:{2}|Exception :{3}", transaction.PortalId, transaction.EmailType.ToString(), transaction.TransactionId, ex.ToString()));
            }

            return isMailSent;
        }

        public static bool IsMailSent(EmailTemplate template)
        {
            bool sucess = false;
            SmtpClient smtp = null;
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(template.EmailUserId);
                mailMessage.To.Add(template.MailRecipient);
                try
                {
                    if (!string.IsNullOrEmpty(template.AdminMailBcC))
                    {
                        mailMessage.Bcc.Add(template.AdminMailBcC);
                    }
                }
                catch { }

                mailMessage.Subject = template.Subject;
                mailMessage.Body = template.Body;
                mailMessage.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(template.Attachment)) {
                    Attachment attachment;
                    attachment = new Attachment(template.Attachment);
                    attachment.ContentDisposition.FileName = System.IO.Path.GetFileName(template.Attachment);
                    mailMessage.Attachments.Add(attachment);
                }
               
                smtp = new SmtpClient(Utility.PortalSettings.EmailServerInfo.Server);
                smtp.EnableSsl = Convert.ToBoolean(Utility.PortalSettings.EmailServerInfo.IsEnableSsl);
                smtp.Port = Convert.ToInt32(Utility.PortalSettings.EmailServerInfo.Port);
                smtp.Credentials = new System.Net.NetworkCredential(Utility.PortalSettings.EmailServerInfo.User, Utility.PortalSettings.EmailServerInfo.Password);

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                smtp.Send(mailMessage);
                sucess = true;
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Framework.Business.IsMailSent" + ex.ToString());
            }
            finally
            {
                if (smtp != null)
                    smtp.Dispose();
            }
            return sucess;
        }


    }
}
