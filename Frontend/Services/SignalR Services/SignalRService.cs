using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

internal class SignalRService
{
    private HubConnection _connection;
    private readonly Func<Task<string>> _tokenProvider;

    public event Action DisconnectUser;

    public event Action<Object> NewStudentOpenedSession;
    public event Action<int> LoggedOutSession; 
    public SignalRService(Func<Task<string>> tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected SignalRService() { }
    public async Task InitializeAsync()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://library-laboratory-management-system.onrender.com/api/v1/hubs/session", options =>
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
    public HubConnection GetHubConnection()
    {
        return _connection;
    }
    public void RegisterHandlers()
    {
        _connection.On("NewStudentOpenedSession", (Object data) =>
        {
            NewStudentOpenedSession.Invoke(data);
        });
        _connection.On("LoggedOutSession", (int sessionId) =>
        {
            LoggedOutSession.Invoke(sessionId);
        });
    }
}
