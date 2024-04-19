using StandaloneRepo.ServiceModel.Companies;
using ServiceStack.OrmLite;
using System.Data;

namespace StandaloneRepo.Migrations
{
    public class Migration1000 : MigrationBase
    {
        public override void Up()
        {
            Db.CreateTable<Company>();
            Db.CreateTable<CompanyDetails>();
            Db.CreateTable<ApplicationUserCompany>();
            Db.CreateTable<Employee>();
        }

        public override void Down()
        {
            Db.DropTable<Company>();
            Db.DropTable<CompanyDetails>();
            Db.DropTable<ApplicationUserCompany>();
            Db.DropTable<Employee>();
        }
    }
}
