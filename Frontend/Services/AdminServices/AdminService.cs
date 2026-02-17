using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Admin;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Session;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Student;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.API_Client.ApiConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices
{
    public class AdminService
    {
        private static HttpClient Client => ApiConfig.Client;
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            try
            {
                string payload = $@"
                {{
                    ""username"": ""{username}"",
                    ""password"": ""{password}""
                }}";

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("/api/v1/auth/authenticate", content);


                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                return body;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error here:{ex.Message}");
                return null;
            }
            
        }

        public async Task<bool> AuthenticateRfid(string rfid)
        {
            var payloadObj = new { rfid = rfid };
            string jsonPayload = JsonConvert.SerializeObject(payloadObj);


            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("api/v1/auth/authenticate/admin/rfid", content);

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return false;

            var json = JObject.Parse(body);

            // token
            var newToken = (string)json["value"]?["accessToken"];
            if (!string.IsNullOrWhiteSpace(newToken))
                ApiConfig.Token = newToken;

            // student id
            var admintoken = json["value"]?["id"];
            if (admintoken != null && int.TryParse(admintoken.ToString(), out var sid))
                AdminDao.AdminId = sid;

            return true;
        }
      
        public async Task<string> UpdateRfid(int userId, string rfidData)
        {
            string payload = $@"
            {{
                ""userId"": {userId},
                ""rfidData"": ""{rfidData}""
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await Client.PutAsync("/rfids", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> CreateAdmin(AdminCreationDAO adminCreation)
        {
            string payload = $@"
            {{
                ""firstName"": ""{adminCreation.FirstName}"",
                ""middleName"": ""{adminCreation.MiddleName}"",
                ""lastName"": ""{adminCreation.LastName}"",
                ""rfidData"": ""{adminCreation.RFID}"",
                ""personnelId"": ""{adminCreation.PersonnelId}""
            }}";
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/v1/admins", content);

            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"Body: {body}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task UpdateAdmin(AdminUpdateDAO adminUpdate)
        {
            string payload = $@"
            {{
                ""id"": {adminUpdate.Id},
                ""firstName"": ""{adminUpdate.FirstName}"",
                ""middleName"": ""{adminUpdate.MiddleName}"",
                ""lastName"": ""{adminUpdate.LastName}"",
                ""personnelId"": ""{adminUpdate.PersonnelId}""
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await Client.PutAsync("api/v1/admins", content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }

        public async Task DeleteAdmin(AdminDeletionDAO adminDeletion)
        {

            Console.WriteLine($"Admin ID: {adminDeletion.Id}");

            var url = $"api/v1/admins/{adminDeletion.Id}";
            var response = await Client.DeleteAsync(url);

            var body = response.Content == null ? "" : await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // DELETE often returns empty body, so log status too
                Debug.WriteLine($"DELETE SUCCESS: {url} | Status={(int)response.StatusCode} {response.StatusCode} | Body={body}");
                Console.WriteLine($"DELETE SUCCESS: {url} | Status={(int)response.StatusCode} {response.StatusCode} | Body={body}");
            }
            else
            {
                Debug.WriteLine($"DELETE FAILED: {url} | Status={(int)response.StatusCode} {response.StatusCode} | Body={body}");
                Console.WriteLine($"DELETE FAILED: {url} | Status={(int)response.StatusCode} {response.StatusCode} | Body={body}");
            }
        }

        public async Task CreateStudents(StudentCreationDAO studentCreation)
        {
            
            string payload = $@"
            {{
                ""firstName"": ""{studentCreation.FirstName}"",
                ""middleName"": ""{studentCreation.MiddleName}"",
                ""lastName"": ""{studentCreation.LastName}"",
                ""rfidData"": ""{studentCreation.RFID}""
                ""studentId"": ""{studentCreation.StudentId}"",
                ""course"": ""{studentCreation.Course}"",
                ""yearLevel"": ""{studentCreation.YearLevel}"",
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("api/v1/students", content);

            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }

        public async Task UpdateStudent(StudentUpdateDAO studentUpdate)
        {
            string payload = $@"
            {{
                ""id"": {studentUpdate.Id},
                ""firstName"": ""{studentUpdate.FirstName}"",
                ""middleName"": ""{studentUpdate.MiddleName}"",
                ""lastName"": ""{studentUpdate.LastName}"",
                ""rfidData"": ""{studentUpdate.RFID}"",
                ""studentId"": ""{studentUpdate.StudentId}"",
                ""course"": ""{studentUpdate.Course}"",
                ""yearLevel"": ""{studentUpdate.YearLevel}""
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Swagger shows PUT /api/v1/students with the full object in the body (no id in route).
            var response = await Client.PutAsync("api/v1/students", content);

            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }

        public async Task DeleteStudent(StudentDeletionDAO studentDeletion)
        {
            var response = await Client.DeleteAsync($"api/v1/students/{studentDeletion.Id}");

            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }

        public async Task<PagedSessionResponse> GetActiveSessions(string query = null, int pageNumber = 1, int pageSize = 16)
        {
            var urlBuilder = new StringBuilder("api/v1/sessions");
            bool hasAny = false;

            void AddParam(string name, string value)
            {
                urlBuilder.Append(hasAny ? "&" : "?");
                hasAny = true;

                urlBuilder.Append(Uri.EscapeDataString(name));
                urlBuilder.Append("=");
                urlBuilder.Append(Uri.EscapeDataString(value));
            }

            if (!string.IsNullOrWhiteSpace(query))
                AddParam("query", query);

            AddParam("isActive", "true");
            AddParam("pageNumber", pageNumber.ToString());
            AddParam("pageSize", pageSize.ToString());

            // Attach bearer token (if available)
            if (!string.IsNullOrWhiteSpace(ApiConfig.Token))
            {
                Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);
            }

            var response = await Client.GetAsync(urlBuilder.ToString());
            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine(body);
            //Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
            //Console.WriteLine($"Body: {body}");

            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(body))
                return null;

            // This matches: { "isSuccess": true, "value": { "items": [...] ... }, "error": null }
            var result = JsonConvert.DeserializeObject<PagedSessionResponse>(body);

            // Optional: if backend does not send isActive per item, infer it since endpoint requested active anyway
            if (result?.Value?.Items != null)
            {
                foreach (var s in result.Value.Items)
                    s.IsActive = true;
            }

            return result;
        }

        public async Task UpdateSession(int id, string duration)
        {
            string payload = $@"
            {{
                ""id"": {id},
                ""duration"": ""{duration}""
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await Client.PutAsync("api/v1/sessions", content);

            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }
        public async Task UpdateSessionDuration(int id, string duration)
        {
            string payload = $@"
            {{
                ""id"": {id},
                ""duration"": ""{duration}""
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await Client.PutAsync("/api/v1/sessions", content);

            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }

        public async Task<string> GetAdmins(string query = null, int pageNumber = 1, int pageSize = 10)
        {
            var urlBuilder = new StringBuilder("api/v1/admins");
            var hasAny = false;

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

            //Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync();
            //Console.WriteLine("Body:");
            //Console.WriteLine(body);

            // Optional debug parse (remove later)
            try
            {
                var obj = JObject.Parse(body);

                // NOTE: adjust this depending on what /admins returns
                // For paged results you likely want: obj["value"]?["items"]?.Count()
                var itemsCount = obj["value"]?["items"]?.Count() ?? 0;

                //Console.WriteLine("Admin items count: " + itemsCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse admins JSON: " + ex.Message);
            }

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Request failed.");
                return null;
            }

            return body;
        }

    }
}
