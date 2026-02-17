using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Services.UserServices
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public string Course { get; set; }
        public string YearLevel { get; set; }
    }

    public class PagedUserResponse
    {
        public bool IsSuccess { get; set; }
        public UserValue Value { get; set; }
    }

    public class UserValue
    {
        public List<UserDto> Items { get; set; }
    }

    public static class SuperAdminState
    {
        public static bool isSuperAdmin { get; set; }
    }

}
