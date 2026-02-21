using System;
using System.Net.Http;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig
{
    public static class ApiConfig
    {
        public static string Token { get; set; }
        public static readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        })
        {
            BaseAddress = new Uri("https://library-laboratory-management-system.onrender.com")
        };
    }
}