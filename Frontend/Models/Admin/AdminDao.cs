using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Admin
{
    public class AdminCreationDAO
    {
        //firstName, middleName, lastName, personnelId, rfid
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string RFID { get; set; }

        public string PersonnelId { get; set; }
    }
    public class AdminUpdateDAO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PersonnelId { get; set; }
        public string RFID { get; set; }
    }
    public class AdminDeletionDAO
    {
        public int Id { get; set; }
    }
    public class AdminDao
    {
        public static int AdminId { get; set; }
    }
    public class PagedAdminResponse
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("value")]
        public PagedAdminValue Value { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }

    public class PagedAdminValue
    {
        [JsonProperty("items")]
        public List<AdminDto> Items { get; set; }

        [JsonProperty("pageNumber")]
        public int PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        // Optional (only if your API sends it; safe to keep)
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
}
