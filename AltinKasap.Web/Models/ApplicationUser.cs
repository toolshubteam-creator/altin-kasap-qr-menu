using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AltinKasap.Web.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    public DateTime? LastLoginAt { get; set; }
}
