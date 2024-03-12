using System.ComponentModel.DataAnnotations;

namespace MvcApp.Models;

public class Comment
{
    [Key]
    public int comment_id {get; set;}
    public string user {get; set;}
    public int post_id {get; set;}
    public string comment {get; set;}
    public DateTime time_created {get; set;}
}