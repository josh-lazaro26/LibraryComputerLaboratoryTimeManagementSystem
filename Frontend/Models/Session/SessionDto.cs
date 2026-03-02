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

    // Matches the ACTUAL flat API response
    public class PagedSessionValue
    {
        [JsonProperty("items")]
        public List<SessionDto> Items { get; set; }

        [JsonProperty("total_count")]      // ← was "totalCount"
        public int TotalCount { get; set; }

        [JsonProperty("page_number")]      // ← was "pageNumber"
        public int PageNumber { get; set; }

        [JsonProperty("page_size")]        // ← was "pageSize"
        public int PageSize { get; set; }

        [JsonProperty("total_pages")]      // ← was "totalPages"
        public int TotalPages { get; set; }

        [JsonProperty("has_previous_page")] // ← was "hasPreviousPage"
        public bool HasPreviousPage { get; set; }

        [JsonProperty("has_next_page")]    // ← was "hasNextPage"
        public bool HasNextPage { get; set; }
    }

    // API returns flat structure, no "isSuccess"/"value" envelope
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

        // Always true since 200 OK
        public bool IsSuccess => true;

        // So existing code using resp.Value.Items still works
        public PagedSessionResponse Value => this;
    }
}