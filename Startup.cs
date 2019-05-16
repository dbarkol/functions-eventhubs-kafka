using System;
using System.IO;
using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Zohan.KafkaDemo.Startup))]

namespace Zohan.KafkaDemo
{
    public class Startup : FunctionsStartup
    {            
        public override void Configure(IFunctionsHostBuilder builder)
        {                    
            // Retrieve the Event Hubs connection string that will allow us
            // to send events.           
            var eventHubConnectionString = Environment.GetEnvironmentVariable("EventHubConnectionString"); 
            
            // Retrieve the Event Hubs fully qualified domain name along
            // with the port number. 
            // Example: {event-hub-namespace}.servicebus.windows.net:9093
            var eventHubFqdn = Environment.GetEnvironmentVariable("EventHubFqdn");             

            // Retrieve the certificate file location. This is required 
            // for client-broker encryption with Event Hubs.
            var caCertFileLocation = GetCaCertFileLocation();

            // Add a singleton instance of the kafka producer class.
            builder.Services.AddSingleton<IKafkaProducer>(new KafkaProducer(eventHubFqdn, eventHubConnectionString, caCertFileLocation));
        }

        private string GetCaCertFileLocation()
        {
            // For local testing
            if (Environment.GetEnvironmentVariable("IsLocal") == "1")
            {
                return "cacert.pem";
            }
            else
            {
                // When the certificate file is copied to the output directory
                // of the Azure Function, it will reside in the site/wwwroot
                // folder. 
                string home = Environment.GetEnvironmentVariable("HOME");
                string path = Path.Combine(home, "site", "wwwroot");
                return Path.Combine(path, "cacert.pem");
            }
        }
    }
}