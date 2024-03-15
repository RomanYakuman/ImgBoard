using AppContext = MvcApp.Models.AppContext;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using Microsoft.AspNetCore.Authentication;

namespace MvcApp.Controllers;

public class UserController : Controller
{
    public IActionResult Profile(string username)
    {
        using (AppContext db = new())
        {
            var user = db.Users.FirstOrDefault(u => u.username == username);
            ViewBag.User = user;
            ViewData["uploads"] = db.Posts.
                Where(p => p.user_id == user.user_id).Count();
            ViewData["comments"] = db.Comments.
                Where(c => c.user_id == user.user_id).Count();
        }
        return View();
    }
    public IActionResult Comments(int user_id)
    {
        using (AppContext db = new())
        ViewBag.comments = db.Comments.Where(c => c.user_id == user_id)
            .OrderByDescending(c => c.time_created).ToList();
        return View();
    }
    public IActionResult Posts(int user_id, int id = 1)
    {
        using AppContext db = new();
        ViewBag.Posts = db.Posts.Where(p => p.user_id == user_id)
            .OrderByDescending(x =>  x.id).ToList();
        return View();
    }
    public IActionResult ChangePassword()
    {
        if(Request.Method == "POST")
        {
            using AppContext db = new();
            var user = db.Users.FirstOrDefault(u => u.username == User.Identity.Name);
            var password = Request.Form["password"];
            if(UserManager.PasswordCheck(password))
            {
                user.password = password;
                db.Users.Update(user);
                db.SaveChanges();
            }
            else
                return BadRequest();
        }
        return View();
    }
    public IActionResult ChangeUsername()
    {
        if(Request.Method == "POST")
        {
            using AppContext db = new();
            var user = db.Users.FirstOrDefault(u => u.username == User.Identity.Name);
            var username = Request.Form["username"];
            if(UserManager.UsernameRegCheck(username, db))
            {
                user.username = username;
                db.Users.Update(user);
                db.SaveChanges();
                Response.Cookies.Delete("auth");
                var claims = UserManager.Authenticate(user.username,
                    user.password, db);
                HttpContext.SignInAsync("Cookies", claims);
                    return Redirect("~/");
            }
            else
                return BadRequest();
        }
        return View();
    }
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth");
        return Redirect("~/");
    }
}
