using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;

namespace Zohan.KafkaDemo
{
    public class KafkaProducer : IKafkaProducer
    {                
        private IProducer<string, string> _producer = null;

        public KafkaProducer(string brokerList, 
            string connectionString, 
            string caCertLocation)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connectionString, 
                SslCaLocation = caCertLocation
            };

            _producer = new ProducerBuilder<string, string>(config).Build();            
        }

        public Task SendMessage(string topicName, string key, string value)
        {
            return  _producer.ProduceAsync(topicName, new Message<string, string>{
                Key = key,
                Value = value
            });
        }
    }

}

