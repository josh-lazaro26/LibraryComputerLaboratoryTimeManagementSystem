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
                    ""firstName"": ""{studentCreation.FirstName}"",
                    ""middleName"": ""{studentCreation.MiddleName}"",
                    ""lastName"": ""{studentCreation.LastName}"",
                    ""rfidData"": ""{studentCreation.RFID}"",
                    ""studentId"": ""{studentCreation.StudentId}"",
                    ""course"": ""{studentCreation.Course}"",
                    ""yearLevel"": ""{studentCreation.YearLevel}""
                }}";

                    // add bearer like in CreateAdmin
                    Client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                    var content = new StringContent(payload, Encoding.UTF8, "application/json");

                    var response = await Client.PostAsync("api/v1/students", content);

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

        public async Task<string> GetStudents(string query = null, int pageNumber = 1, int pageSize = 10)
        {
            var urlBuilder = new StringBuilder("api/v1/students");

            var hasAny = false;

            try
            {
                void AddParam(string name, string value)
                {
                    if (!hasAny)
                    {
                        urlBuilder.Append("?");
                        hasAny = true;
                    }
                    else
                    {
                        urlBuilder.Append("&");
                    }

                    urlBuilder.Append(Uri.EscapeDataString(name));
                    urlBuilder.Append("=");
                    urlBuilder.Append(Uri.EscapeDataString(value));
                }

                if (!string.IsNullOrWhiteSpace(query))
                    AddParam("query", query);

                AddParam("pageNumber", pageNumber.ToString());
                AddParam("pageSize", pageSize.ToString());

                string url = urlBuilder.ToString();

                Client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.GetAsync(url);

                Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");

                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Body ni kristo:"+ body);

                    var obj = JObject.Parse(body);
                    var role = (string)obj["value"]?["role"]; // adjust path to your actual JSON


                    Console.WriteLine("Role: " + role);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Request failed.");
                        return null;
                    }

                    return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse role: " + ex.Message);
                return null;
            }

        }

    }
}
