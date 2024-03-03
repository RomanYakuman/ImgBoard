using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
namespace MvcApp.Models;

public class User
{
    [Key]
    public int user_id { get; set;}
    public string? email {get; set;}
    public string username {get; set;}
    public string password {get; set;}
    public DateTime reg_date { get; set;}

    public bool Registrate(string username, string password, string? email)
    {
        this.email = email;
        this.username = username;
        this.password = password;
        this.reg_date = DateTime.Now;
        if(username == null || password == null)
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
            var user = db.Users.FirstOrDefault(un => un.username == username);
            if(user != null && user.password == password)
                return true;
        }
        return false;
    }
}