using IfakCrane.Server.Services;
using IfakCrane.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft;
using Newtonsoft.Json;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IfakCrane.Server.Services
{
    public class MQTTService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        public MqttClient client { get; private set; }

        public MQTTService(IHubContext<SignalRHub> hubContext)
        {
            client = new MqttClient("test.mosquitto.org");

            string clientId = Guid.NewGuid().ToString();

            client.Connect(clientId);

            client.Subscribe(new string[] { "Position_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            client.MqttMsgPublishReceived += SubscribeToServer;
            _hubContext = hubContext;
        }

        public void PublishToServer(string topic, object[] data)
        {
            client.Publish(topic, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
        }
        public async void SubscribeToServer(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            var jsonObject = JsonConvert.DeserializeObject<TrolleyData>(message);
            Console.WriteLine(jsonObject);
            await _hubContext.Clients.All.SendAsync("PositionData", jsonObject);

        }
    }
}
