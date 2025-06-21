using Domain.Entities;
using Use_Cases.Posts.Queries;

namespace Use_Cases.Posts.QueryHandlers;

public interface IGetPostByIdHandler
{
    Task<PostDto?> Handle(GetPostById request);
}