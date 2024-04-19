using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneRepo.ServiceModel.Companies
{
    public class Employee
    {
        public Employee(string firstName, string phoneNumber)
        {
            FirstName = firstName;
            PhoneNumber = phoneNumber;
        }

        [AutoIncrement]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Instructons { get; set; }
        public string? Status { get; set; }

        [References(typeof(Company))]
        public int CompanyId { get; set; }
    }
}
