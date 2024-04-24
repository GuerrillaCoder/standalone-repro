using StandaloneRepo.ServiceModel.Companies;
using Microsoft.AspNetCore.Identity;
using ServiceStack.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StandaloneRepo.ServiceModel.Users;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? ProfileUrl { get; set; }
    public string? FacebookUserId { get; set; }
    public string? GoogleUserId { get; set; }
    public string? GoogleProfilePageUrl { get; set; }
    public string? MicrosoftUserId { get; set; }
    public List<string> Permissions { get; set; } = new List<string>();
    //[Reference]
    //[NotMapped]
    //public List<ApplicationUserCompany> ApplicationUserCompanies { get; set; } = new();
}

//below are provided solely for access purposes
//[Alias("identity_role_claim")]
//public class IdentityRoleClaimAccess
//{
//    public int Id { get; set; }
//    public string RoleId { get; set; }
//    public string ClaimType { get; set; }
//    public string ClaimValue { get; set; }
//}
