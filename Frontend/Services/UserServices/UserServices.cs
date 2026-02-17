using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.UserServices
{
    public class UserServices
    {
        private static HttpClient Client => ApiConfig.Client;

    }
}
