namespace MvcApp.Models;

public class Post
{
    public int id { get; set;}
    public DateTime time_created { get; set; }
    public string user { get; set; }
    public string path { get; set; }
    public bool UploadToServer(IFormFileCollection file)
    {
        this.user = "Temp";
        this.time_created = DateTime.Now;
        this.path = @$"/server/{Path.GetRandomFileName()}{Path.GetRandomFileName()}.{file[0].ContentType.Split("/")[1]}";
        using (AppContext db = new())
        {
            db.Add(this);
            db.SaveChanges();
        }
        using FileStream fileStream = new("wwwroot/" + this.path, FileMode.Create);
            file[0].CopyTo(fileStream);
        return true;
    }
    public Post GetPostById(int postId)
    {
        using(AppContext db = new())
        {
            return db.Posts.FirstOrDefault(p => p.id == postId);
        }
    }
}