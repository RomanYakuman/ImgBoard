using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MvcApp.Models;

public static partial class UserManager
{
    public static bool Registrate(string username, string password, string email, AppContext db)
    {
        if (PasswordCheck(password) && UsernameRegCheck(password, db))
        {
            User user = new User
            {
               username = username,
               password = password,
               email = email
            };
            db.Add(user);
            db.SaveChanges();
            return true;
        }
        else
            return false;
    }
    public static ClaimsPrincipal Authenticate(string username, string password, AppContext db)
    {
        var user = db.Users.FirstOrDefault(u => u.username == username);
        if(user != null && user.password == password)
        {
            var claims = new List<Claim>{new Claim(ClaimTypes.Name, user.username)};
            ClaimsIdentity claimsIdentity = new(claims, "Cookies");
            return new ClaimsPrincipal(claimsIdentity);
        }
        return null;
    }
    private static bool PasswordCheck(string password)
    {
        var regexItem = validRegex();
        if(password == null || password.Length < 5 || password.Length > 25 
            || !regexItem.IsMatch(password))
        {
            return false;
        }
        return true;
    }
    private static bool UsernameRegCheck(string username, AppContext db)
    {
        var regexItem = validRegex();
        if(username == null || username.Length < 4 || username.Length > 25 
            || db.Users.FirstOrDefault(u => u.username == username) != null)
                return false;
        return true;
    }
    [GeneratedRegex("^[a-zA-Z0-9_]+$")]
    private static partial Regex validRegex();
}