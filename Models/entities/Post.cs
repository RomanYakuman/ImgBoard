namespace MvcApp.Models;

public class Post
{
    public int id { get; set;}
    public DateTime time_created { get; set; }
    public int user_id { get; set; }
    public string path { get; set; }
    public string description {get; set;}
    public Post()
    {

    }
    public Post GetPostById(int postId)
    {
        using(AppContext db = new())
        {
            return db.Posts.FirstOrDefault(p => p.id == postId);
        }
    }
}