using Application.Posts.Queries;
using Domain.Entities;

namespace Application.Posts.QueryHandlers;

public interface IGetAllPostsHandler
{
    Task<ICollection<Post>> Handle(GetAllPosts request);
}