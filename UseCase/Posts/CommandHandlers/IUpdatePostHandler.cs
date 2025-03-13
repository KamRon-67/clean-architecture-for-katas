using Application.Posts.Commands;
using Domain.Entities;

namespace Application.Posts.CommandHandlers;

public interface IUpdatePostHandler
{
    Post Handle(UpdatePost command);
}