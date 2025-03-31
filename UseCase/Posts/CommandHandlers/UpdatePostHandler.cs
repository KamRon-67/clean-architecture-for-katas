using Application.Abstractions;
using Application.Posts.Commands;
using Domain.Entities;

namespace Application.Posts.CommandHandlers
{
    public class UpdatePostHandler : IUpdatePostHandler
    {
        private readonly IPostRepository _postRepository;

        public UpdatePostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Post?> Handle(UpdatePost request)
        {
            var post = await _postRepository.UpdatePost(request.UpdatedContent, request.PostId);
            return post;
        }
    }
}
