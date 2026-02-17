using Newtonsoft.Json;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Admin
{
    public class AdminDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("personnelId")]
        public string PersonnelId { get; set; }

    }
}
