using Use_Cases.Posts.Commands;

namespace Application.Posts.CommandHandlers;

public interface IDeletePostHandler
{
    void Handle(DeletePost command);
}