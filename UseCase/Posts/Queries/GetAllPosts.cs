using Domain.Entities;
using MediatR;

namespace Application.Posts.Queries
{
    public class GetAllPosts : IRequest<ICollection<Post>>
    {
    }
}
