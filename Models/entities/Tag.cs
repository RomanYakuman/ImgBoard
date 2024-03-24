using Microsoft.EntityFrameworkCore;

namespace MvcApp.Models;

[PrimaryKey(nameof(PostId), nameof(TagString))]
public class Tag
{
    public int PostId {get; set;}
    public required string TagString {get; set;}
    public Post? Post {get; set;}

}