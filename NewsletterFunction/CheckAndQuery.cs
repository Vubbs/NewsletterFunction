using System;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NewsletterFunction.Services;
using System.Text;

namespace NewsletterFunction
{
    public class CheckAndQuery
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public CheckAndQuery(ILoggerFactory loggerFactory, IUserService userService, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<CheckAndQuery>();
            _userService = userService;
            _configuration = configuration;
        }

        [Function("CheckAndQuery")]
        public void Run([TimerTrigger("0 35 10 * * 2",RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var userList = _userService.CheckSubscribers();
            foreach ( var subscriber in userList )
            {
                QueueClient queueClient = new(_configuration["AzureWebJobsStorage"], "newsletterqueue", options: new()
                { MessageEncoding = QueueMessageEncoding.Base64 });
                string message = Convert.ToBase64String(Encoding.UTF8.GetBytes(subscriber.Id));
                queueClient.CreateIfNotExists();
                queueClient.SendMessage(message);
            }
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
