using Application.Posts.Commands;
using Domain.Entities;

namespace Application.Posts.CommandHandlers
{
    public interface ICreatePostHandler
    {
        Task<Post> Handle(CreatePost command);  // ✅ Return Task<Post> to support async
    }
}