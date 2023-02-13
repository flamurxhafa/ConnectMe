using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;
using ProfileMatching.Data;
using ProfileMatching.Models;
using ProfileMatching.Models.ViewModels;
using ProfileMatching.SD;

namespace ProfileMatching.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, AppDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        List<UserViewModel> userViewModels = new List<UserViewModel>();

        var interactions = _context.Interactions.ToList();




        for (int i = 0; i < users.Count; i++)
        {
            var interaction = interactions.Find(x => (x.UserId1 == userId && x.UserId2 == users[i].Id));

            if (users[i].Id != userId && interaction== null)
            {
                userViewModels.Add(new UserViewModel()
                {
                    Id = users[i].Id,
                    Name = users[i].Name,
                    Lastname = users[i].Lastname,
                    Height = users[i].Height,
                    Gender = users[i].Gender,
                    Age = users[i].Age,
                    Bio = users[i].Bio,
                    imagePath= users[i].imagePath
                });
            }
        }

        return View(userViewModels);
    }


    public IActionResult Swipes()
    {
        var matches = _context.Matches.ToList();
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        List<UserViewModel> userViewModels = new List<UserViewModel>();
        for (int i = 0; i < matches.Count; i++)
        {
            if (userId == matches[i].Userid1 )
            {
                var user = _userManager.Users.FirstOrDefault(x => x.Id == matches[i].Userid2);
                if (user != null)
                {
                    userViewModels.Add(new UserViewModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Lastname = user.Lastname,
                        Height = user.Height,
                        Gender = user.Gender,
                        Age = user.Age,
                        Bio = user.Bio,
                        imagePath = user.imagePath
                    });
                }

            }else if (userId == matches[i].Userid2) {
                var user = _userManager.Users.FirstOrDefault(x => x.Id == matches[i].Userid1);
                if (user != null)
                {
                    userViewModels.Add(new UserViewModel()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Lastname = user.Lastname,
                        Height = user.Height,
                        Gender = user.Gender,
                        Age = user.Age,
                        Bio = user.Bio,
                        imagePath = user.imagePath

                    });
                }
            }
        }

        return View(userViewModels);
    }

    [Authorize(Roles = SDClass.Admin)]
    public IActionResult Dashboard()
    {
        if (User.IsInRole(SDClass.Admin))
        {
            var users = _userManager.Users.ToList();
            List<UserViewModel> userViewModels = new List<UserViewModel>();
            for (int i = 0; i < users.Count; i++)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (users[i].Id != userId)
                {
                    userViewModels.Add(new UserViewModel()
                    {
                        Id = users[i].Id,
                        Name = users[i].Name,
                        Lastname = users[i].Lastname,
                        Type = users[i].Type,
                        Role = users[i].Role,
                        Height = users[i].Height,
                        Gender = users[i].Gender,
                        Age = users[i].Age,
                        Bio = users[i].Bio,
                    });
                }
            }

            return View(userViewModels);
        }
        else
        {
            string returnUrl = HttpContext.Request.Query["returnUrl"];
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Account()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);

        if (user != null)
        {
            UserViewModel model = new UserViewModel()
            {
                Id = user.Id,
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                Type = user.Type,
                Role = user.Role,
                Height = user.Height,
                Gender = user.Gender,
                Age = user.Age,
                Bio = user.Bio,
                imagePath = user.imagePath
            };

            return View(model);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Account(UserViewModel model)
    {
        
            
            if (model.Email != null && model.Fullname != null && model.Type != null && model.Age != 0 &&
                model.Height != 0 && model.Bio != null && model.Bio.Length <= 170 && model.Bio.Length >= 30)
            {
           
           
               var user = await _userManager.FindByEmailAsync(model.Email);
               var path = user.imagePath;

               if (model.Image != null)
                    {
                        var fullPath = Directory.GetCurrentDirectory() + @"/wwwroot/images/" + model.Image.FileName;
                         path = @"/images/" + model.Image.FileName;
                        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(stream);
                            stream.Close();
                        }
                    }
               

                user.Name = model.Fullname.Split(' ')[0];
                user.Lastname = model.Fullname.Split(' ')[1];
                user.UserName = model.Email;
                user.Email = model.Email;
                user.Type = model.Type;
                user.Height = model.Height;
                user.Age = model.Age;
                user.Bio = model.Bio;
                user.imagePath = path;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
               
        }
        else
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            model.imagePath = user.imagePath;
            model.Name = model.Fullname.Split(' ')[0];
            model.Lastname = model.Fullname.Split(' ')[1];
            model.Gender = user.Gender;
            if (model.Age == 0)
            {
                ModelState.AddModelError("Age", "This cant be 0");

            }
            if (model.Height == 0)
            {
                ModelState.AddModelError("Height", "This cant be 0");

            }
            if (model.Bio.Length >= 170 || model.Bio.Length <= 30)
            {
                ModelState.AddModelError("Bio", "This cant be less than 30 and more than 170 characters");
            }
            ModelState.AddModelError("", "None of these values can be null!");
            return View("Account",model);
        }
        

        return RedirectToAction("Account", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}