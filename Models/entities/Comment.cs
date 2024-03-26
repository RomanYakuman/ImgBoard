using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models;

public class Comment
{
    [Key]
    public int Id {get; set;}
    public int UserId {get; set;}
    public int PostId {get; set;}
    public required string CommentString {get; set;}
    public DateTime TimeCreated {get; set;}
    public User? User {get; set;}
}