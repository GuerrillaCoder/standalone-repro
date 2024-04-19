using StandaloneRepo.ServiceModel.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceStack;
using System.Diagnostics;

namespace StandaloneRepo.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Debugger.Launch();
        // PostgreSQL uses the public schema by default - not dbo.
        modelBuilder.HasDefaultSchema("public");
        base.OnModelCreating(modelBuilder);

        //Rename Identity tables to lowercase
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var currentTableName = modelBuilder.Entity(entity.Name).Metadata.GetDefaultTableName();
            var cleanedTableName = currentTableName.Split('<')[0];
            modelBuilder.Entity(entity.Name).ToTable(cleanedTableName.ToLowercaseUnderscore());
        }
    }
}