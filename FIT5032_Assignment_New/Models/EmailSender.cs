using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace FIT5032_Assignment_New.Utils
{
    public class EmailSender
    {
        private const String API_KEY = "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";

        public void Send(String toEmail, String subject, String contents, String link)
        {
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("noreply@tinv.tinv", "Tinv");
            var to = new EmailAddress(toEmail, "");
            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>" + "<Br />" + "<a href=\"" + link + "\">" + link + "</a>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }
    }
}