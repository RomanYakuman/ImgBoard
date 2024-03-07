namespace MvcApp.Models;

public class Post
{
    public int id { get; set;}
    public DateTime time_created { get; set; }
    public string user { get; set; }
    public string path { get; set; }
    public string tags {get; set;}
    public string description {get; set;}
    public Post(IFormFileCollection file, string tags, string user, string description)
    {
        this.time_created = DateTime.Now;
        this.tags = tags;
        this.user = user;
        this.description = description;
        this.path = @$"/server/{Path.GetRandomFileName()}{Path.GetRandomFileName()}.{file[0].ContentType.Split("/")[1]}";
        using (AppContext db = new())
        {
            db.Add(this);
            db.SaveChanges();
        }
        using FileStream fileStream = new("wwwroot/" + this.path, FileMode.Create);
            file[0].CopyTo(fileStream);
    }
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