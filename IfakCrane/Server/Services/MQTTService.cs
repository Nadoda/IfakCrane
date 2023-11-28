using IfakCrane.Server.Services;
using IfakCrane.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace IfakCrane.Server.Services
{
    public class MQTTService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        public MqttClient? client1 { get; private set; }
        public MqttClient? client2 { get; private set; }

        public MQTTService(IHubContext<SignalRHub> hubContext)
        {

            InitializedConnection();
            _hubContext = hubContext;
        }

        public void InitializedConnection() 
        {
            client1 = new MqttClient("172.16.53.11");  //172.16.53.11 
            string clientId1 = Guid.NewGuid().ToString();
            client1.Connect(clientId1);
            client1.Subscribe(new string[] { "position_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client1.Subscribe(new string[] { "manual_control_status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client1.Subscribe(new string[] { "auto_mode_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client1.Subscribe(new string[] { "status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            client1.MqttMsgPublishReceived += SubscribeToServer1;
          
            // client2 configuration //

            client2 = new MqttClient("172.16.53.12");  //172.16.53.12
            string clientId2 = Guid.NewGuid().ToString();
            client2.Connect(clientId2);
            client2.Subscribe(new string[] { "position_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client2.Subscribe(new string[] { "manual_control_status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client2.Subscribe(new string[] { "auto_mode_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client2.Subscribe(new string[] { "status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
          
            client2.MqttMsgPublishReceived += SubscribeToServer2;
        }
        public async void ConnectToMQTTServer(string CraneName,string IP_Address)
        {
            if (CraneName == "crane1")
            {
               
                try
                {
                    client1 = new MqttClient(IP_Address);  //172.16.53.11 
                    string clientId1 = Guid.NewGuid().ToString();
                    client1.Connect(clientId1);
                    await _hubContext.Clients.All.SendAsync("MqttConnectionStatus", new
                    {
                        Crane_Name = CraneName,
                        ConnectionStatus = client1.IsConnected,
                        ConnectedClientId = client1.ClientId,
                        ClientIPAddress = IP_Address
                    });

                    client1.Subscribe(new string[] { "position_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client1.Subscribe(new string[] { "manual_control_status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client1.Subscribe(new string[] { "auto_mode_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client1.Subscribe(new string[] { "status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client1.MqttMsgPublishReceived += SubscribeToServer1;
                }
                catch
                {
                    Console.WriteLine($"Connection Error! with {IP_Address} server !Try Again!");
                } 
                
            }
            else if (CraneName == "crane2")
            {
               
                try
                {
                    client2 = new MqttClient(IP_Address);  //172.16.53.12
                    string clientId2 = Guid.NewGuid().ToString();
                    client2.Connect(clientId2);
                    await _hubContext.Clients.All.SendAsync("MqttConnectionStatus", new
                    {
                        Crane_Name = CraneName,
                        ConnectionStatus = client2.IsConnected,
                        ConnectedClientId = client2.ClientId,
                        ClientIPAddress = IP_Address
                    });
                    client2.Subscribe(new string[] { "position_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client2.Subscribe(new string[] { "manual_control_status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client2.Subscribe(new string[] { "auto_mode_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client2.Subscribe(new string[] { "status_topic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    client2.MqttMsgPublishReceived += SubscribeToServer2;
                }
                catch
                {
                    Console.WriteLine($"Connection Error! with {IP_Address} server !Try Again!");
                }
            }
        }

        public void PublishToServer(string CraneName, string topic, string data)
        {
            if (CraneName == "crane1")
            {
                client1.Publish(topic, Encoding.UTF8.GetBytes(data));
            }
            else if (CraneName == "crane2")
            {
                client2.Publish(topic, Encoding.UTF8.GetBytes(data)); 
            }           
        }
        public async void SubscribeToServer1(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            //var hii = "trolley1, {\"x\":51684,\"y\":18431,\"z\":11400}";
            if (e.Topic == "position_topic")
            {
                await _hubContext.Clients.All.SendAsync("PositionDataC1", message);
            }
            else if (e.Topic == "manual_control_status_topic")
            {
                await _hubContext.Clients.All.SendAsync("ManualControlData", message);
            }
            else if(e.Topic == "auto_mode_topic")
            {
                await _hubContext.Clients.All.SendAsync("AutoModeData", message);
            }
            else if (e.Topic == "status_topic")
            {
                await _hubContext.Clients.All.SendAsync("GoToStatus", message);
            }
        }
        public async void SubscribeToServer2(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            if (e.Topic == "position_topic")
            {
                await _hubContext.Clients.All.SendAsync("PositionDataC2", message);
            }
            else if (e.Topic == "manual_control_status_topic")
            {
                await _hubContext.Clients.All.SendAsync("ManualControlData", message);
            }
            else if (e.Topic == "auto_mode_topic")
            {
                await _hubContext.Clients.All.SendAsync("AutoModeData", message);
            }
            else if (e.Topic == "status_topic")
            {
                await _hubContext.Clients.All.SendAsync("GoToStatus", message);
            }


        }
    }
}
