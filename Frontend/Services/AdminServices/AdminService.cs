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
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.AdminServices
{
    public class AdminService
    {
        private static HttpClient Client => ApiConfig.Client;

        public async Task<bool> AuthenticateRfid(string rfid)
        {
            try
            {
                var payloadObj = new { rfid = rfid };
                string jsonPayload = JsonConvert.SerializeObject(payloadObj);


                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("api/v1/accounts/authenticate", content);

                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return false;

                var json = JObject.Parse(body);

                // token
                var newToken = (string)json["access_token"];
                if (!string.IsNullOrWhiteSpace(newToken))
                    ApiConfig.Token = newToken;

                // student id
                var admintoken = json["value"]?["id"];
                if (admintoken != null && int.TryParse(admintoken.ToString(), out var sid))
                    AdminDao.AdminId = sid;

                return true;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Exeption: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> CreateAdmin(AdminCreationDAO adminCreation)
        {
            string payload = $@"
            {{
                ""rfid"": ""{adminCreation.RFID}"",
                ""school_id"": ""{adminCreation.PersonnelId}""
            }}";
            Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/v1/users/admins", content);

            var body = await response.Content.ReadAsStringAsync();

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

        public async Task UpdateStudent(StudentUpdateDAO studentUpdate)
        {
            string payload = $@"
            {{
                ""id"": {studentUpdate.Id},
                ""rfidData"": ""{studentUpdate.RFID}"",
                ""studentId"": ""{studentUpdate.StudentId}"",
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

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

        public async Task<PagedSessionResponse> GetActiveSessions(int pageNumber = 1, int pageSize = 16)
        {
            var urlBuilder = new StringBuilder("api/v1/accounts"); 
            bool hasAny = false;
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

                AddParam("page_number", pageNumber.ToString());
                AddParam("page_size", pageSize.ToString());
                AddParam("active", "true");
                if (!string.IsNullOrWhiteSpace(ApiConfig.Token))
                {
                    Client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiConfig.Token);
                }

                var response = await Client.GetAsync(urlBuilder.ToString());
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("GetActiveSessions URL: " + urlBuilder.ToString());
                Console.WriteLine("GetActiveSessions RESPONSE: " + body);
                Console.WriteLine("GetActiveSessions STATUS: " + response.StatusCode);

                if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(body))
                    return null;

                var result = JsonConvert.DeserializeObject<PagedSessionResponse>(body);

                if (result?.Items != null)
                {
                    var json = JObject.Parse(body);
                    var items = json["items"];

                    for (int i = 0; i < result.Items.Count && i < items.Count(); i++)
                    {
                        result.Items[i].Active = items[i]["active"]?.Value<bool>() ?? false;
                    }
                }

                return result;
            }
            catch(Exception ex) 
            {
                MessageBox.Show("Error getting sesssions: " + ex.Message);
                return null;
            }
        }

        public async Task UpdateSession(string userId, string duration)
        {
            string payload = $@"
            {{
                ""user_id"": ""{userId}"",
                ""new_duration"": ""{duration}""
            }}";
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync("api/v1/accounts/time", content);
            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }
        public async Task UpdateSessionDuration(string userId, string duration)
        {
            string payload = $@"
            {{
                ""user_id"": ""{userId}"",
                ""new_duration"": ""{duration}""
            }}";

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await Client.PutAsync("api/v1/accounts/time", content);

            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync();
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed: {response.StatusCode} | {body}");
            }
        }

        public async Task<string> GetAdmins(string query = null, int pageNumber = 1, int pageSize = 10)
        {
            var urlBuilder = new StringBuilder("api/v1/admins");
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

                var body = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(body))  // ← ADD THIS CHECK
                {
                    Console.WriteLine("GetAdmins returned empty body.");
                    return null;
                }

                var obj = JObject.Parse(body);

                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse admins JSON: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> CreateEvaluation(string question)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(new { question = question });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.PostAsync("api/v1/evaluations", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEvaluation(string id, string question)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(new { question = question });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.PutAsync($"api/v1/evaluations/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }
        public async Task<string> GetLatestEvaluation()
        {
            try
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.GetAsync("api/v1/evaluations/new");
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed: {response.StatusCode}");
                    return null;
                }

                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> Logout()
        {
            try
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.PostAsync("api/v1/accounts/logout", null);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Logout failed: {response.StatusCode}");
                    return false;
                }

                ApiConfig.Token = null;
                AdminDao.AdminId = 0;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during logout: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateSetting(bool isSyncing)
        {
            try
            {
                ApiConfig.Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await ApiConfig.Client.PostAsync(
                    "api/v1/sync-requests/enrolled-students",
                    null
                );

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"SyncEnrolledStudents failed: {response.StatusCode} | {body}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during SyncEnrolledStudents: {ex.Message}");
                return false;
            }
        }
        public async Task<string> GetClientDevices(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var urlBuilder = new StringBuilder("api/v1/client-devices");
                urlBuilder.Append($"?page_number={pageNumber}&page_size={pageSize}");

                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.GetAsync(urlBuilder.ToString());
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GetClientDevices failed: {response.StatusCode} | {body}");
                    return null;
                }
                
                Console.WriteLine("Device Body:"+body);
                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during GetClientDevices: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RestartClientDevice(string deviceName)
        {
            try
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.PostAsync(
                    $"api/v1/client-devices/restart?device_name={Uri.EscapeDataString(deviceName)}",
                    null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RestartClientDevice failed: {ex.Message}");
                return false;
            }
        }
        public async Task<string> GetStudents(string query = null, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var urlBuilder = new StringBuilder("api/v1/users/students");
                var hasAny = false;

                void AddParam(string name, string value)
                {
                    urlBuilder.Append(hasAny ? "&" : "?");
                    hasAny = true;
                    urlBuilder.Append(Uri.EscapeDataString(name));
                    urlBuilder.Append("=");
                    urlBuilder.Append(Uri.EscapeDataString(value));
                }

                if (!string.IsNullOrWhiteSpace(query))
                    AddParam("search_query", query);

                AddParam("page_number", pageNumber.ToString());
                AddParam("page_size", pageSize.ToString());

                Console.WriteLine("GetStudents URL: " + urlBuilder.ToString());

                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.GetAsync(urlBuilder.ToString());
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine("GetStudents RESPONSE: " + body);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GetStudents failed: {response.StatusCode} | {body}");
                    return null;
                }

                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during GetStudents: {ex.Message}");
                return null;
            }
        }
        public async Task<string> GetSessionHistories(int pageNumber = 1, int pageSize = 10, string query = null)
        {
            try
            {
                var urlBuilder = new StringBuilder("api/v1/accounts/session-histories");
                urlBuilder.Append($"?page_number={pageNumber}&page_size={pageSize}");

                if (!string.IsNullOrWhiteSpace(query))
                    urlBuilder.Append($"&search_query={Uri.EscapeDataString(query)}");

                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", ApiConfig.Token);

                var response = await Client.GetAsync(urlBuilder.ToString());
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("SessionHistories RESPONSE: " + body); // ← THIS
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GetSessionHistories failed: {response.StatusCode} | {body}");
                    return null;
                }

                Console.WriteLine("SessionHistories RESPONSE: " + body);
                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during GetSessionHistories: {ex.Message}");
                return null;
            }
        }
    }
}
