using Domain.Entities;
using MediatR;

namespace Application.Posts.Commands
{
    public class UpdatePost : IRequest<Post>
    {
        public int PostId { get; set; }
        public string? UpdatedContent { get; set; }
    }
}
