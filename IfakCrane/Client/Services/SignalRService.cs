
using IfakCrane.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace IfakCrane.Client.Services
{
    public class SignalRService
    {
        private static HubConnection? hubConnection;
        private readonly NavigationManager NavigationManager;
        public bool IsConnected { get { return hubConnection?.State == HubConnectionState.Connected; } }

        // For Crane 1
        public event Action<dynamic>? OnReceivedPositionC1;
        // For Crane 2
        public event Action<dynamic>? OnReceivedPositionC2;

        public event Action<dynamic>? OnManualControlStatus;
        public event Action<dynamic>? OnAutoModeStatus;
        public event Action<dynamic>? OnMqttConnectionStatus;
        public event Action<dynamic>? OnGoToStatus;
        public SignalRService(NavigationManager navigationManager)
        {
            this.NavigationManager = navigationManager;

            hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/SignalRService")).Build();

            // For Crane 1
            hubConnection.On<dynamic>("PositionDataC1", data =>
            {

                OnReceivedPositionC1?.Invoke(data);

            });
  
            // For Crane 2
            hubConnection.On<dynamic>("PositionDataC2", data =>
            {

                OnReceivedPositionC2?.Invoke(data);

            });

            hubConnection.On<dynamic>("ManualControlData", data =>
            {

                OnManualControlStatus?.Invoke(data);

            });
            hubConnection.On<dynamic>("AutoModeData", data =>
            {

                OnAutoModeStatus?.Invoke(data);

            });
            hubConnection.On<dynamic>("MqttConnectionStatus", data =>
            {

                OnMqttConnectionStatus?.Invoke(data);

            });
            hubConnection.On<dynamic>("GoToStatus", data =>
            {

                OnGoToStatus?.Invoke(data);

            });

        }
        public async Task StartConnection()
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
        }
        
        public async void PublishToServer(string CraneName, string topic, string data)
        {
            await hubConnection.InvokeAsync("PublishToServer",CraneName, topic,data);
        }
        public async void ConnectToMQTT(string CraneName, string IP_Address)
        {
            await hubConnection.InvokeAsync("ConnectToMQTT", CraneName, IP_Address);
        }

    }
}
