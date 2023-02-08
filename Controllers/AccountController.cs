using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProfileMatching.Data;
using ProfileMatching.Models;
using ProfileMatching.Models.ViewModels;
using ProfileMatching.SD;

namespace ProfileMatching.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = new User()
                    {
                        Name = model.Fullname.Split(' ')[0],
                        Lastname = model.Fullname.Split(' ')[1],
                        Gender = model.Gender,
                        Email = model.Email,
                        Role = SDClass.Simple,
                        EmailConfirmed = true,
                        NormalizedEmail = model.Email.ToUpper(),
                        LockoutEnabled = false,
                        UserName = model.Fullname.Split(' ')[0] + model.Fullname.Split(' ')[1],
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                }

                return View(model);
            }
            catch (Exception e)
            {
                return Json(new {message = $"{e.Message}{e.InnerException?.Message}"});
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _context.Users.FirstOrDefault(x => x.Email == model.Email);
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false,
                        lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                return View();
            }
            catch (Exception e)
            {
                return Json(new {message = $"{e.Message}{e.InnerException?.Message}"});
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }
    }
}