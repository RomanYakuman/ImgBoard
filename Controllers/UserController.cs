﻿using AppContext = MvcApp.Models.AppContext;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace MvcApp.Controllers;

public class UserController : Controller
{
    public IActionResult Profile(string Username)
    {
        using (AppContext db = new())
        {
            var user = db.Users.First(u => u.Username == Username);
            ViewBag.User = user;
            ViewData["uploads"] = db.Posts.
                Where(p => p.UserId == user.Id).Count();
            ViewData["comments"] = db.Comments.
                Where(c => c.UserId == user.Id).Count();
        }
        return View();
    }
    public IActionResult Comments(int Id)
    {
        using (AppContext db = new())
        ViewBag.comments = db.Comments.Where(c => c.UserId == Id)
            .OrderByDescending(c => c.TimeCreated).ToList();
        return View();
    }
    public IActionResult Posts(int Id)
    {
        using AppContext db = new();
        ViewBag.Posts = db.Posts.Where(p => p.UserId == Id)
            .OrderByDescending(x =>  x.Id).ToList();
        ViewData["Username"] = db.Users.First(u => u.Id == Id).Username;
        return View();
    }
    [Authorize]
    public IActionResult ChangePassword()
    {
        if(Request.Method == "POST")
        {
            using AppContext db = new();
            var user = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            var password = Request.Form["password"];
            if(UserManager.PasswordCheck(password))
            {
                user.Password = password;
                db.Users.Update(user);
                db.SaveChanges();
            }
            else
                return BadRequest();
        }
        return View();
    }
    [Authorize]
    public IActionResult ChangeUsername()
    {
        if(Request.Method == "POST")
        {
            using AppContext db = new();
            var user = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            var username = Request.Form["username"];
            if(UserManager.UsernameRegCheck(username, db))
            {
                user.Username = username;
                db.Users.Update(user);
                db.SaveChanges();
                Response.Cookies.Delete("auth");
                var claims = UserManager.Authenticate(user.Username,
                    user.Password, db);
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
