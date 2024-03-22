using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers;

public class AuthController : Controller
{
     public IActionResult Login()
     {    
          if(Request.Method == "POST")
          {
            using Models.AppContext db = new();
            var claims = UserManager.Authenticate(Request.Form["username"],
               Request.Form["password"], db);
            if (claims != null)
            {
               HttpContext.SignInAsync("Cookies", claims);
               return Redirect("~/");
            }
            return BadRequest();
          }
          return View();
     }

     public IActionResult SignUp()
     {    
          if(Request.Method == "POST")
          {
            using Models.AppContext db = new();
            if (UserManager.Registrate(Request.Form["username"], Request.Form["password"],
                 Request.Form["Email"], db))
                return Redirect("~/auth/login");
            else
                return BadRequest();
        }
          return View();

     }
}