using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProfileMatching.Models;

public class Match
{
    [Key]
    public string? Id { get; set; }
    
    [ForeignKey("User1")]
    public string? Userid1 { get; set; }
    public User User1 { get; set; }
    
    [ForeignKey("User2")]
    public string? Userid2 { get; set; }
    public User User2 { get; set; }
    
    public bool match { get; set; }
}