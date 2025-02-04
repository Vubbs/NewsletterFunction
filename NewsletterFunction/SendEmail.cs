using System;
using System.Linq;
using Azure.Storage.Queues.Models;
using MailKit.Net.Smtp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using NewsletterFunction.Models;
using Newtonsoft.Json;

namespace NewsletterFunction
{
    public class SendEmail
    {
        private readonly ILogger<SendEmail> _logger;
        private readonly IConfiguration _configuration;
        private readonly FuncDBContext _db;

        public SendEmail(ILogger<SendEmail> logger,
            IConfiguration configuration,
            FuncDBContext db)
        {
            _logger = logger;
            _configuration = configuration;
            _db = db;
        }

        [Function(nameof(SendEmail))]
        public void Run([QueueTrigger("newsletterqueue", Connection = "AzureWebJobsStorage")] string qItem)
        {
            //var queueItem = JsonConvert.DeserializeObject<User>(qItem);
            var response = "";
            var user = _db.Users.Where(c => c.Id == qItem).FirstOrDefault();
            var articles = _db.Articles.OrderByDescending(x => x.DateStamp).Take(10).ToList();
            _logger.LogInformation($"C# Queue trigger function processed: {qItem}");
            var message = new MimeMessage();
            message.Sender = MailboxAddress.Parse("news.fyrasidor@gmail.com");
            message.Sender.Name = "Fyra Sidor News";
            message.To.Add(MailboxAddress.Parse(user!.Email));
            message.From.Add(message.Sender);
            message.Subject = "Fyra Sidor Weekly Newsletter";
            // We will say we are sending HTML. But there are options for plaintext etc.
            message.Body = new TextPart(TextFormat.Html)
            {
                Text =
                $"Here are 10 new articles for you to check out!" +
                $"<br><br>" +
                $"1. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[0].Id}' title='{articles[0].ContentSummary}'>{articles[0].Headline}</a><br>" +
                $"2. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[1].Id}' title='{articles[1].ContentSummary}'>{articles[1].Headline}</a><br>" +
                $"3. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[2].Id}' title='{articles[2].ContentSummary}'>{articles[2].Headline}</a><br>" +
                $"4. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[3].Id}' title='{articles[3].ContentSummary}'>{articles[3].Headline}</a><br>" +
                $"5. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[4].Id}' title='{articles[4].ContentSummary}'>{articles[4].Headline}</a><br>" +
                $"6. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[5].Id}' title='{articles[5].ContentSummary}'>{articles[5].Headline}</a><br>" +
                $"7. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[6].Id}' title='{articles[6].ContentSummary}'>{articles[6].Headline}</a><br>" +
                $"8. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[7].Id}' title='{articles[7].ContentSummary}'>{articles[7].Headline}</a><br>" +
                $"9. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[8].Id}' title='{articles[8].ContentSummary}'>{articles[8].Headline}</a><br>" +
                $"10. <a href='https://fyrasidornews.azurewebsites.net/Article/DisplayArticle/{articles[9].Id}' title='{articles[9].ContentSummary}'>{articles[9].Headline}</a><br><br>" +
                "Thank you for subscribing to our Newsletter!<br>//Fyra Sidor News"
            };

            using (var emailClient = new SmtpClient())
            {
                try
                {
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect("smtp.gmail.com", 465, true);
                }
                catch (SmtpCommandException ex)
                {
                    response = "Error trying to connect: " + ex.Message + "Status Code: " + ex.StatusCode;
                    Console.WriteLine(response);
                }
                catch (SmtpProtocolException ex)
                {
                    response = "Protocol error while trying to connect:" + ex.Message;
                    Console.WriteLine(response);
                }
                //Remove any OAuth functionality as we won't be using it.
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
                emailClient.Authenticate("news.fyrasidor@gmail.com", "puty xcoo ekbb fwpy");

                try
                {
                    emailClient.Send(message);
                }
                catch (SmtpCommandException ex)
                {
                    response = "Error sending message: " + ex.Message + "Status Code: " + ex.StatusCode;
                    Console.WriteLine(response);
                    switch (ex.ErrorCode)
                    {
                        case SmtpErrorCode.RecipientNotAccepted:
                            response += " Recipient not accepted: " + ex.Mailbox;
                            Console.WriteLine(response);
                            break;
                        case SmtpErrorCode.SenderNotAccepted:
                            response += " Sender not accepted: " + ex.Mailbox;
                            Console.WriteLine(response);
                            break;
                        case SmtpErrorCode.MessageNotAccepted:
                            response += " Message not accepted.";
                            Console.WriteLine(response);
                            break;
                    }
                }
                emailClient.Disconnect(true);
            }
            // Be careful that the SmtpClient class is the one from Mailkit not the framework!
        }
    }
}
