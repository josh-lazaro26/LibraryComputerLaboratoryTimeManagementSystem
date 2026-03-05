using Newtonsoft.Json;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Admin
{
    public class AdminDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("personnelId")]
        public string PersonnelId { get; set; }
    }
}
