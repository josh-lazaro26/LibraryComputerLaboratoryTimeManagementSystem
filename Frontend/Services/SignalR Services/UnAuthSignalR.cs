using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SignalRDemo
{
    internal class UnauthenticatedSignalRService
    {
        private HubConnection _connection;
        public Action Restart;

        public UnauthenticatedSignalRService()
        {
        }

        public async Task InitializeAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://internet-laboratory-time-management.onrender.com/api/v1/hubs/client-device")
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();
            await _connection.StartAsync();
        }

        public void RegisterHandlers()
        {
            _connection.On("Restart", () =>
            {
                Restart?.Invoke();
            });
        }

        public async Task<bool> SendRestartCommandAsync(string deviceName)
        {
            if (_connection.State != HubConnectionState.Connected)
                return false;
            try
            {
                await _connection.InvokeAsync(deviceName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR] Failed to send restart command: {ex.Message}");
                return false;
            }
        }

        public HubConnection GetHubConnection() => _connection;
    }
}