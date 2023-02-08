using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models.ViewModels;

public class InteractionViewModel
{
    public string Id { get; set; }
    
    public string UserId1 { get; set; }
    public User User1 { get; set; }
    
    public string UserId2 { get; set; }
    public User User2 { get; set; }
    
    public DateTime InteractionDate { get; set; }
    public string InteractionType { get; set; }
}