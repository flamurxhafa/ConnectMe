using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace ProfileMatching.Models;

public class User : IdentityUser
{
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Type { get; set; }
    public string Role { get; set; }
    public int Age { get; set; }
    public double Height { get; set; }
    public string Bio { get; set; }
    public string Gender { get; set; }
    [AllowNull]
    public string Location { get; set; }
}