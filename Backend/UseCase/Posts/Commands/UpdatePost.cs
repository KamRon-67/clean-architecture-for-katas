namespace Use_Cases.Posts.Commands;

public class UpdatePost
{
    public int PostId { get; set; }
    public string? UpdatedContent { get; set; }
}