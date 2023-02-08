using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProfileMatching.Models;

public class Interactions
{
    [Key]
    public string Id { get; set; }

    [ForeignKey("User1")]
    public string? UserId1 { get; set; }
    public User User1 { get; set; }
    
    [ForeignKey("User2")]
    public string? UserId2 { get; set; }
    public User User2 { get; set; }
    
    public DateTime InteractionDate { get; set; }
    public string InteractionType { get; set; }
}