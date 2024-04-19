using ServiceStack.DataAnnotations;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Html;

namespace StandaloneRepo.ServiceModel.Companies
{
    [ValidateIsAdmin]
    [AutoApply(Behavior.AuditCreate)]
    public class CreateCompany : ICreateDb<Company>, IReturn<IdResponse>
    {
        public string Name { get; set; }

        [Input(Type = Input.Types.Tag)]
        public List<string> ApplicationUserIds { get; set; }
    }

    [ValidateIsAdmin]
    [AutoApply(Behavior.AuditModify)]
    public class UpdateCompany : IUpdateDb<Company>, IReturn<Company>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CompanyDetails CompanyDetails { get; set; }
    }

    [ValidateIsAdmin]
    [AutoApply(Behavior.AuditDelete)]
    public class DeleteCompany : IDeleteDb<Company>, IReturnVoid
    {
        public int Id { get; set; }
    }

    [ValidateIsAdmin]
    [AutoApply(Behavior.AuditCreate)]
    public class CreateCompanyDetails : ICreateDb<CompanyDetails>, IReturn<IdResponse>
    {
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        [References(typeof(Company))]
        [ValidateNotNull]
        public int CompanyId { get; set; }
    }

    [Authenticate]
    //todo: validate user has access to Company
    [AutoApply(Behavior.AuditModify)]
    public class UpdateCompanyDetails : IUpdateDb<CompanyDetails>, IReturn<CompanyDetails>
    {
        public int CompanyId { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    [ValidateIsAdmin]
    [AutoApply(Behavior.AuditCreate)]
    public class CreateApplicationUserCompany : ICreateDb<ApplicationUserCompany>, IReturn<IdResponse>
    {
        public string ApplicationUserId { get; set; }
        public int CompanyId { get; set; }
    }

}
