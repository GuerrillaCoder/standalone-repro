using StandaloneRepo.ServiceModel.Users;
using ServiceStack;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneRepo.ServiceModel.Companies
{
    public class Company : AuditBase
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }

        [Reference]
        public CompanyDetails CompanyDetails { get; set; }

        [Reference]
        public List<Employee> Employees { get; set; }

        [Reference]
        public List<ApplicationUserCompany> ApplicationUserCompanies { get; set; }

    }

    public class CompanyDetails : AuditBase
    {
        [AutoIncrement]
        public int Id { get; set; }
        [References(typeof(Company))]
        public int CompanyId { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    [UniqueConstraint(nameof(ApplicationUserId), nameof(CompanyId))]
    public class ApplicationUserCompany
    {
        [AutoIncrement]
        public int Id { get; set; }
        [References(typeof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [References(typeof(Company))]
        public int CompanyId { get; set; }
        public Company Account { get; set; }
    }
}
