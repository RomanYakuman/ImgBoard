using AppContext = MvcApp.Models.AppContext;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers;

public class UserController : Controller
{
    public IActionResult Profile(string username)
    {
        if(HttpContext.Request.Method == "POST")
        {
            Response.Cookies.Delete("auth");
            return Redirect("~/");
        }
        using (AppContext db = new())
        {
            var user = db.Users.FirstOrDefault(u => u.username == username);
            ViewBag.User = user;
            ViewData["uploads"] = db.Posts.
                Where(p => p.user == user.username).Count();
            ViewData["comments"] = db.Comments.
                Where(c => c.user == user.username).Count();
        }
        return View();
    }
    public IActionResult Comments(string username)
    {
        using (AppContext db = new())
        {
            var comments = db.Comments.Where(c => c.user == username)
                .OrderByDescending(c => c.time_created).ToList();
            ViewBag.comments = comments;
        }
        return View();
    }
    public IActionResult Posts(string username, int id = 1)
    {
        using (AppContext db = new())
        {
            ViewBag.Posts = db.Posts.Where(p => p.user == username)
                .OrderByDescending(x =>  x.id).ToList();
        }
        return View();
    }
}
