using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace Yusr.Email
{
    public class EmailService
    {
        public async Task SendAsync(EmailMessage emailDTO)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailDTO.SenderName, emailDTO.SenderEmail));
            message.To.Add(new MailboxAddress(emailDTO.SenderName, emailDTO.SenderEmail));

            FillReceivers(emailDTO.ReceiversEmailsList, ref message);

            message.Subject = emailDTO.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = $"<h2>{emailDTO.Body}</h2>";
            builder.HtmlBody += "<h3>المرفقات</h3>";

            int fileCounter = 1;
            foreach (var fileBytes in emailDTO.FilesBytes)
            {
                string fileName = $"Ticket_{fileCounter}.pdf";
                builder.Attachments.Add(fileName, fileBytes, ContentType.Parse("application/pdf"));
                fileCounter++;
            }

            message.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailDTO.SenderEmail, emailDTO.SenderAppKey);
            var response = await smtp.SendAsync(message);
            Console.WriteLine(response);
            await smtp.DisconnectAsync(true);
        }
        private void FillReceivers(string[] receiversEmailsList, ref MimeMessage message)
        {
            foreach (var receiver in receiversEmailsList)
            {
                message.Bcc.Add(new MailboxAddress(name: "", address: receiver));
            }
        }
    }
}
