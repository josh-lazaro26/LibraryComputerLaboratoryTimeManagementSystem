using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class SignalRService
{
    private HubConnection _connection;
    private HubConnection _sycnConnection;
    private readonly Func<Task<string>> _tokenProvider;

    public event Action<Guid, string, TimeSpan> NewSession;
    public event Action<Guid> LoggedOutSession;
    public event Action<TimeSpan> UpdatedSession;
    public event Action Terminate;
    public event Action<string> StudentSyncingProgress;

    public SignalRService(Func<Task<string>> tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected SignalRService() { }

    public async Task InitializeAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://internet-laboratory-time-management.onrender.com/api/v1/hubs/session", options =>
            {
                options.AccessTokenProvider = _tokenProvider;
                options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
            })
            .WithAutomaticReconnect()
            .Build();
        _sycnConnection = new HubConnectionBuilder()
           .WithUrl("https://internet-laboratory-time-management.onrender.com/api/v1/hubs/sync", options =>
           {
               options.AccessTokenProvider = _tokenProvider;
               options.HttpMessageHandlerFactory = _ => new HttpClientHandler
               {
                   ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
               };
           })
           .WithAutomaticReconnect()
           .Build();
        RegisterHandlers();
        await _connection.StartAsync();
        await _sycnConnection.StartAsync();
    }

    public HubConnection GetHubConnection() => _connection;

    public void RegisterHandlers()
    {
        _connection.On("NewSession", (Guid userId, string schoolId, TimeSpan availableDuration) =>
        {
            NewSession.Invoke(userId, schoolId, availableDuration);
        });

        _connection.On("LoggedOutSession", (Guid userId) =>
        {
            LoggedOutSession.Invoke(userId);
        });

        _connection.On("UpdatedSession", (TimeSpan duration) =>
        {
            UpdatedSession.Invoke(duration);
        });

        _connection.On("Terminate", () =>
        {
            Terminate.Invoke();
        });

        _sycnConnection.On("SyncingProgress", (string processedPercentage) =>
        {
            StudentSyncingProgress.Invoke(processedPercentage);
        });

    }
}