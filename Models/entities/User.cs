using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models;

public partial class User
{
    [Key]
    public int Id { get; set;}
    public string? Email {get; set;}
    public required string Username {get; set;}
    public required string Password {get; set;}
    public DateTime RegDate { get; set;}
}