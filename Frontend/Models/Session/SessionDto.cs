using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Session
{
    public class SessionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("lastLoggedIn")]
        public DateTime LastLoggedIn { get; set; }

   
        [JsonProperty("name")]
        public string Name { get; set; }

        public bool IsActive { get; set; } 
    }

    public class PagedSessionValue
    {
        [JsonProperty("items")]
        public List<SessionDto> Items { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }
    }

    public class PagedSessionResponse
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("value")]
        public PagedSessionValue Value { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }
}
