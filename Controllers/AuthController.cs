using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MvcApp.Models;

namespace MvcApp.Controllers
{
     public class AuthController : Controller
     {
          public IActionResult Login()
          {    
               var request = HttpContext.Request;
               if(request.Method != "POST")
                    return View();
               var username = request.Form["username"];
               var password = request.Form["password"];
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
               var request = HttpContext.Request;
               if(request.Method == "POST")
               {
                    string? username = request.Form["username"];
                    string? password = request.Form["password"];
                    string? email = request.Form["email"];
                    var user = new User();
                    if(user.Registrate(username, password, email))
                         return Redirect("~/auth/login");
                    else
                         return BadRequest();
               }
               return View();
          }
     }
}