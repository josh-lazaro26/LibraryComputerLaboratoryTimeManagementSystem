using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

internal class SignalRService
{
    private HubConnection _connection;
    private readonly Func<Task<string>> _tokenProvider;

    // Admin listeners
    public event Action<Guid, string, TimeSpan> NewStudentOpenedSession;
    public event Action<Guid> LoggedOutSession;

    // Student listeners
    public event Action<TimeSpan> UpdatedSession;
    public event Action Terminate;

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

        RegisterHandlers();
        await _connection.StartAsync();
    }

    public HubConnection GetHubConnection() => _connection;

    public void RegisterHandlers()
    {
        _connection.On<Guid, string, TimeSpan>("NewSession", (userId, schoolId, availableDuration) =>
        {
            NewStudentOpenedSession?.Invoke(userId, schoolId, availableDuration);
        });

        _connection.On<Guid>("LoggedOutSession", (userId) =>
        {
            LoggedOutSession?.Invoke(userId);
        });

        _connection.On<TimeSpan>("UpdatedSession", (duration) =>
        {
            UpdatedSession?.Invoke(duration);
        });

        _connection.On("Terminate", () =>
        {
            Terminate?.Invoke();
        });
    }
}