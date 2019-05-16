using System.Threading.Tasks;

namespace Zohan.KafkaDemo
{
    public interface IKafkaProducer 
    {
        Task SendEvent(string topicName, string key, string value);
    }
}