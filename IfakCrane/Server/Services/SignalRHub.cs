

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
        //public async void MqttConnectionStatus(string data)
        //{
        //    await Clients.All.SendAsync("MqttConnectionStatus", data);
        //}

        // For Crane 1 //
        //public async void PositionDataC1(string data)
        //{
        //    await Clients.All.SendAsync("PositionDataC1", data);
        //}

        //// For Crane 2 //
        //public async void PositionDataC2(string data)
        //{
        //    await Clients.All.SendAsync("PositionDataC2", data);
        //}

        //public async void ManualControlData(string data)
        //{
        //    await Clients.All.SendAsync("ManualControlData", data);
        //}
        //public async void AutoModeData(string data)
        //{
        //    await Clients.All.SendAsync("AutoModeData", data);
        //}

        //    //      //         //
        public void PublishToServer(string CraneName, string topic, string data)
        {
            _Mqtt.PublishToServer(CraneName,topic, data);
        }
        public void ConnectToMQTT(string CraneName, string IP_Address)
        {
            _Mqtt.ConnectToMQTTServer(CraneName, IP_Address);
        }


    }
}
