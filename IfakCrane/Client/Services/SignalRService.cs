
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

        
        public event Action<TrolleyData>? OnReceivedPosition;

        public SignalRService(NavigationManager navigationManager)
        {
            this.NavigationManager = navigationManager;

            hubConnection = new HubConnectionBuilder().WithUrl(NavigationManager.ToAbsoluteUri("/SignalRService")).Build();

            hubConnection.On<TrolleyData>("PositionData", data =>
            {

                OnReceivedPosition?.Invoke(data);

            });

        }
        public async Task StartConnection()
        {

            await hubConnection.StartAsync();
        }
        
        public async void PublishToServer(string topic, string data)
        {
            await hubConnection.InvokeAsync("PublishToServer", topic,data);
        }

    }
}
