using Domain.Entities;
using MediatR;

namespace Application.Posts.Commands
{
    public class CreatePost: IRequest<Post> 
    {
        public int Id { get; set; }
        public string? PostContent { get; set; }
    }
}
