namespace ProfileMatching.Models.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public string Type { get; set; }
    public string Role { get; set; }
    public int Age { get; set; }
    public double Height { get; set; }
    public string Bio { get; set; }
    public string Gender { get; set; }

    public IFormFile? Image { get; set; }
    public string imagePath { get; set; }
}