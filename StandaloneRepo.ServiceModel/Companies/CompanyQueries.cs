using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneRepo.ServiceModel.Companies
{
    [Authenticate]
    public class QueryCompanies : QueryDb<Company, CompanyResponse>
    {
        public int? Id { get; set; }
    }

    public class QueryCompanyDetails : QueryDb<CompanyDetails>
    {
        public int? CompanyId { get; set; }
    }
}
