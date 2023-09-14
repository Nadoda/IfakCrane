

using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Text;

namespace IfakCrane.Server.Services
{
    public class SignalRHub:Hub
    {
        private readonly MQTTService _Mqtt;

        public SignalRHub(MQTTService Mqtt)
        {
           _Mqtt=Mqtt;
        }

        public async void PositionData(string data)
        {
            await Clients.All.SendAsync("PositionData", data);
        }
        public void PublishToServer(string topic, string data)
        {
            _Mqtt.client.Publish(topic, Encoding.UTF8.GetBytes(data));
        }



    }
}
