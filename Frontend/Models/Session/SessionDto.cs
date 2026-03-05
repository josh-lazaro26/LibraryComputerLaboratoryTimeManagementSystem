using Newtonsoft.Json;
using System.Collections.Generic;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Session
{
    public class SessionDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        public string Name { get; set; }
        [JsonProperty("school_id")]

        public string SchoolId { get; set; }
    }

    public class PagedSessionResponse
    {
        [JsonProperty("items")]
        public List<SessionDto> Items { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("page_number")]
        public int PageNumber { get; set; }

        [JsonProperty("page_size")]
        public int PageSize { get; set; }

        public bool IsSuccess => true;

        public PagedSessionResponse Value => this;
    }
}