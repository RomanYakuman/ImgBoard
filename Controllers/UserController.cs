﻿using AppContext = MvcApp.Models.AppContext;
using Microsoft.AspNetCore.Mvc;

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
        using (AppContext db = new())
        ViewBag.Posts = db.Posts.Where(p => p.user_id == user_id)
            .OrderByDescending(x =>  x.id).ToList();
        return View();
    }
}
