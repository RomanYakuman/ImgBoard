namespace MvcApp.Models;

public class Post
{
    public int Id { get; set; }
    public DateTime TimeCreated { get; set; }
    public int UserId { get; set; }
    public required string Path { get; set; }
    public string? Description { get; set; }
    public ICollection<Tag>? Tags { get; } = new List<Tag>();
}
