using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProfileMatching.Data;
using ProfileMatching.Models;
using ProfileMatching.Models.ViewModels;

namespace ProfileMatching.Controllers;

public class InteractionsController : Controller
{
    private readonly UserManager<User> _userManager;
    private AppDbContext _db;

    public InteractionsController(UserManager<User> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    [HttpPost]
    public void Add(string id, string interactionType)
    {
        var loggedUser = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var person = _userManager.Users.FirstOrDefault(x => x.Id == id);

        var interaction = new Interactions()
        {
            Id = Guid.NewGuid().ToString(),
            UserId1 = loggedUser,
            UserId2 = id,
            InteractionDate = DateTime.Now,
            InteractionType = interactionType
        };

         _db.Interactions.Add(interaction);
         _db.SaveChanges();

        var result = _db.Interactions.FromSqlRaw("Exec GetInteractions @id1, @id2",
            new SqlParameter("@id1", id),
            new SqlParameter("@id2", loggedUser)).ToList();

        if (result.Count >= 2)
        {
            Match match = new Match()
            {
                Id = Guid.NewGuid().ToString(),
                Userid1 = loggedUser,
                Userid2 = id,
                match = true,
                matchDate = DateTime.Now
            };

            _db.Matches.Add(match);
            _db.SaveChanges();
        }
    }

    [HttpPost]
    public IActionResult Delete(string id)
    {
        if(id != null)
        {
            var user = _db.Users.FirstOrDefault(o => o.Id == id);
            var match = _db.Matches.ToList();
            for (var i = 0; i < match.Count; i++)
            {
                if (match[i].Userid1 == id || match[i].Userid2 == id)
                {
                    _db.Matches.Remove(match[i]);

                }
            }
            



            _db.Users.Remove(user);
            _db.SaveChanges();

            return RedirectToAction("Dashboard", "Home");

        }
        return RedirectToAction("Dashboard", "Home");
    }
}