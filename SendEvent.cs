using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Confluent.Kafka;

namespace Zohan.KafkaDemo
{
    public class SendEvent
    {
        private readonly IKafkaProducer _producer;        

        public SendEvent(IKafkaProducer producer)
        {
            // Save an instance of the Kafka producer
            _producer = producer;
        }

        [FunctionName("sendevent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Send event function triggered");

            // Retrieve the topic name
            var topicName = Environment.GetEnvironmentVariable("EventHubName");

            // Send the event to the topic
            await _producer.SendEvent(topicName, null, DateTime.UtcNow.ToLongTimeString());

            return (ActionResult)new OkObjectResult($"Ok");  
        }
    }
}
