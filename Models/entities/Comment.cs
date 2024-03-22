using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models;

public class Comment
{
    [Key]
    public int CommentId {get; set;}
    public int UserId {get; set;}
    public int PostId {get; set;}
    public required string CommentString {get; set;}
    public DateTime TimeCreated {get; set;}
}

public class CommentWithUser
{
    public int CommentId { get; set; }
    public required string Username { get; set; }
    public int PostId { get; set; }
    public required string CommentString { get; set; }
    public DateTime TimeCreated { get; set; }
}