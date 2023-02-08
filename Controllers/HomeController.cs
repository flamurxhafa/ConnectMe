using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProfileMatching.Data;
using ProfileMatching.Models;
using ProfileMatching.Models.ViewModels;

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

        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].Id != userId)
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
        for (int i = 0; i<matches.Count; i++)
        {
            if (userId == matches[i].Userid1)
            {
                var user = _userManager.Users.FirstOrDefault(x=>x.Id == matches[i].Userid2);
                userViewModels.Add(new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Lastname = user.Lastname,
                    Height = user.Height,
                    Gender = user.Gender,
                    Age = user.Age,
                    Bio = user.Bio,
                });
            }
        }
        return View(userViewModels);
    }

    public IActionResult Dashboard()
    {
        var users = _userManager.Users.ToList();
        List<UserViewModel> userViewModels = new List<UserViewModel>();
        for (int i = 0; i < users.Count; i++)
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

        return View(userViewModels);
    }

    [HttpPost]
    public async Task Delete()
    {
        return;
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
            };

            return View(model);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        if (model.Email != null && model.Fullname != null && model.Type != null && model.Age != 0 &&
            model.Height != 0 && model.Bio != null)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            user.Name = model.Fullname.Split(' ')[0];
            user.Lastname = model.Fullname.Split(' ')[1];
            user.UserName = model.Email;
            user.Email = model.Email;
            user.Type = model.Type;
            user.Height = model.Height;
            user.Age = model.Age;
            user.Bio = model.Bio;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "None of these values can be null!");
            }
        }

        return RedirectToAction("Account", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}