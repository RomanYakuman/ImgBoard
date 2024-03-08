using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Text.RegularExpressions;
namespace MvcApp.Models;

public partial class User
{
    [Key]
    public int user_id { get; set;}
    public string email {get; set;}
    public string username {get; set;}
    public string password {get; set;}
    public DateTime reg_date { get; set;}

    public bool Registrate(string username, string password, string email)
    {
        this.email = email;
        this.username = username;
        this.password = password;
        this.reg_date = DateTime.Now;
        if(!PasswordCheck() || !UsernameRegCheck())
            return false;
        using (AppContext db = new())
        {
            db.Add(this);
            db.SaveChanges();
        }
        return true;
    }
    public bool Authenticate(string username, string password)
    {
        using (AppContext db = new())
        {
            var user = db.Users.FirstOrDefault(u => u.username == username);
            if(user != null && user.password == password)
                return true;
        }
        return false;
    }
    private bool PasswordCheck()
    {
        var regexItem = validRegex();
        if(password == null || password.Length < 5 || password.Length > 25 
            || !regexItem.IsMatch(password))
        {
            return false;
        }
        return true;
    }
    private bool UsernameRegCheck()
    {
        var regexItem = validRegex();
        
        if(username == null || username.Length < 4 || username.Length > 25 
            || !regexItem.IsMatch(password))
                return false;
        using(AppContext db = new())
        {
            if(db.Users.FirstOrDefault(u => u.username == username) != null)
                return false;
        }
        return true;
    }

    [GeneratedRegex("^[a-zA-Z0-9_]+$")]
    private static partial Regex validRegex();
}