using System.Threading.Tasks;

namespace Zohan.KafkaDemo
{
    public interface IKafkaProducer 
    {
        Task SendMessage(string topicName, string key, string value);
    }
}