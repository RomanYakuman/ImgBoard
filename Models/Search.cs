
using Microsoft.EntityFrameworkCore;

namespace MvcApp.Models;

public static class Search
{
    public static List<Post> SearchByTags(string tagString, AppContext db, out int maxPosts)
    {
        string[] tags = tagString.Split(",", StringSplitOptions.TrimEntries);
        var posts = db.Posts.Include(p => p.Tags).Where(p => p.Tags.Select(t => t.TagString).Contains(tags[0])).ToList();
        for(int i = 1; i < tags.Length; i++)
        {
            posts = posts.Where(p => p.Tags.Select(t => t.TagString).Contains(tags[i])).ToList();
        }
        maxPosts = posts.Count;
        return posts.OrderByDescending(p => p.Id).ToList();
    }
}