using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MvcApp.Models;

public partial class User
{
    [Key]
    public int user_id { get; set;}
    public string? email {get; set;}
    public string username {get; set;}
    public string password {get; set;}
    public DateTime reg_date { get; set;}

    public User()
    {
    }
}