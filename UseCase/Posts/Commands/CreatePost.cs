using Domain.Entities;

namespace Application.Posts.Commands
{
    public class CreatePost
    {
        public int Id { get; set; }
        public string? PostContent { get; set; }
        public string? PostComments { get; set; }
    }
}
