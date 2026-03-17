using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.UserServices
{
    public class UserDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("school_id")]
        public string SchoolId { get; set; }

        [JsonProperty("course_code")]
        public string CourseCode { get; set; }

        [JsonProperty("school_year")]
        public string SchoolYear { get; set; }

        [JsonProperty("enrollment_status")]
        public string EnrollmentStatus { get; set; }
    }

    public class SessionHistoryDto
    {
        [JsonProperty("school_id")]
        public string SchoolId { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("consumed_time")]
        public string ConsumedTime { get; set; }

        public string FormattedConsumedTime
        {
            get
            {
                if (!TimeSpan.TryParse(ConsumedTime, out var ts))
                    return "00:00:00";
                ts = ts.Duration();
                if (ts.TotalDays >= 1)
                    return $"{(int)ts.TotalDays}d {ts.Hours:D2}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
                else
                    return $"{ts.Hours:D2}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
            }
        }
    }

    public static class SuperAdminState
    {
        public static bool isSuperAdmin { get; set; }
    }

}
