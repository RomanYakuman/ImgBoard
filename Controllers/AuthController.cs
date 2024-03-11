using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers;

public class AuthController : Controller
{
     public IActionResult Login()
     {    
          if(Request.Method != "POST")
               return View();
          var username = Request.Form["username"];
          var password = Request.Form["password"];
          var user = new User();
          if(user.Authenticate(username, password))
          {
               var claims = new List<Claim>{new Claim(ClaimTypes.Name, username)};
               ClaimsIdentity claimsIdentity = new(claims, "Cookies");
               HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));
               return Redirect("~/");
          }
          return BadRequest();
     }

     public IActionResult SignUp()
     {    
          if(Request.Method == "POST")
          {
               string? username = Request.Form["username"];
               string? password = Request.Form["password"];
               string? email = Request.Form["email"];
               var user = new User();
               if(user.Registrate(username, password, email))
                    return Redirect("~/auth/login");
               else
                    return BadRequest();
          }
          return View();
     }
}