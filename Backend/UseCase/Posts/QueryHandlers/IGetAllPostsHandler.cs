using Domain.Entities;
using Use_Cases.Posts.Queries;

namespace Use_Cases.Posts.QueryHandlers;

public interface IGetAllPostsHandler
{
    Task<ICollection<PostDto>> Handle(GetAllPosts request);
}