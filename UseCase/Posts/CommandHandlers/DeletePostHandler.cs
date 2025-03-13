using Application.Abstractions;
using Application.Posts.Commands;

namespace Application.Posts.CommandHandlers
{
    public class DeletePostHandler : IDeletePostHandler 
    {
        private readonly IPostRepository _postRepository;

        public DeletePostHandler(IPostRepository postRepository) 
        {
            _postRepository = postRepository;
        }

        public void Handle(DeletePost request)
        {
             _postRepository.DeletePost(request.PostId);
        }
    }
}
