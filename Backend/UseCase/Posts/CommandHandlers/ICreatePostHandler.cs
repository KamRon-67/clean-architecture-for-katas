using Domain.Entities;
using Use_Cases.Posts.Commands;

namespace Application.Posts.CommandHandlers
{
    public interface ICreatePostHandler
    {
        Task<Post> Handle(CreatePost command);  // ✅ Return Task<Post> to support async
    }
}