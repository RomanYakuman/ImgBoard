using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models;

public class Comment
{
    [Key]
    public int comment_id {get; set;}
    public int user_id {get; set;}
    public int post_id {get; set;}
    public string comment {get; set;}
    public DateTime time_created {get; set;}
}

public class CommentWithUser
{
    public int comment_id { get; set; }
    public string username { get; set; }
    public int post_id { get; set; }
    public string comment { get; set; }
    public DateTime time_created { get; set; }
}