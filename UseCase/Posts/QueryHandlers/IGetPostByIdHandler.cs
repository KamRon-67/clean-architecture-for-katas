using Application.Posts.Queries;
using Domain.Entities;

namespace Application.Posts.QueryHandlers;

public interface IGetPostByIdHandler
{
    Task<Post?> Handle(GetPostById request, CancellationToken cancellationToken);
}