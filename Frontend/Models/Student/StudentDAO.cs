using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Models.Student
{
    public class StudentCreationDAO
    {             
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public string Course { get; set; }
        public string YearLevel { get; set; }
        public string RFID { get; set; }
    }
    public class StudentUpdateDAO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string RFID { get; set; }
        public string StudentId { get; set; }
        public string Course { get; set; }
        public string YearLevel { get; set; }
    }
    public class StudentDeletionDAO
    {
        public string Id { get; set; }
    }
}
