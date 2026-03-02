using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Session;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Student;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.StudentServices
{
    public  class StudentServices
    {
        private static HttpClient Client => ApiConfig.Client;
        public async Task<bool> CreateStudents(StudentCreationDAO studentCreation)
        {
            try
            {
                string payload = $@"
                {{
                    ""rfid"": ""{studentCreation.RFID}"",
                    ""school_id"": ""{studentCreation.StudentId}""
                }}";

                // add bearer like in CreateAdmin
                Client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                    var content = new StringContent(payload, Encoding.UTF8, "application/json");

                    var response = await Client.PostAsync("api/v1/users/students", content);

                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
                    Console.WriteLine($"Body: {body}");

                    return response.IsSuccessStatusCode;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Create Student error {ex.Message}");
                return false;
            }
        }
        public async Task<string> GetStudents(int pageNumber = 1, int pageSize = 10)
        {
            var urlBuilder = new StringBuilder("api/v1/accounts");
            var hasAny = false;

            try
            {
                void AddParam(string name, string value)
                {
                    urlBuilder.Append(hasAny ? "&" : "?");
                    hasAny = true;
                    urlBuilder.Append(Uri.EscapeDataString(name));
                    urlBuilder.Append("=");
                    urlBuilder.Append(Uri.EscapeDataString(value));
                }

                AddParam("pageNumber", pageNumber.ToString());
                AddParam("pageSize", pageSize.ToString());
        
                Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.GetAsync(urlBuilder.ToString());
                var body = await response.Content.ReadAsStringAsync();
                

                Console.WriteLine("Body ni kristo part 2: "+body);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Request failed: {(int)response.StatusCode}");
                    return null;
                }

                return body; // ← just return the body, let the caller parse it
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetStudents failed: " + ex.Message);
                return null;
            }
        }
        public async Task<string> GetSchoolIdByUserId(string userId)
        {
            try
            {
                Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.GetAsync($"api/v1/accounts/{userId}");
                if (!response.IsSuccessStatusCode) return null;

                var body = await response.Content.ReadAsStringAsync();
                var obj = Newtonsoft.Json.Linq.JObject.Parse(body);

                // Adjust the JSON path to match your actual API response
                return obj["school_id"]?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
