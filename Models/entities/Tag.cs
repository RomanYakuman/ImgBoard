using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MvcApp.Models;

[PrimaryKey(nameof(post_id), nameof(tag))]
public class Tag
{
    public int post_id {get; set;}
    public string tag {get; set;}
}