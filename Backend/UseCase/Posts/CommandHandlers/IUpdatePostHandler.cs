using Domain.Entities;
using Use_Cases.Posts.Commands;

namespace Application.Posts.CommandHandlers;

public interface IUpdatePostHandler
{
    Task<PostDto?> Handle(UpdatePost command);
}