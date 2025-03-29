using Domain.Entities;

namespace Application.Posts.CommandHandlers;

public interface IGetPostByIdHandler
{
    Task<Post> Handle(int id);  // ✅ Return Task<Post> to support async
}