using Application.Posts.Commands;
using Domain.Entities;

namespace Application.Posts.CommandHandlers;

public interface IUpdatePostHandler
{
    Task<Post> Handle(UpdatePost command);
}