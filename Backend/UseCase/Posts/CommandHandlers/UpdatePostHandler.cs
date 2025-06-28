using Application.Abstractions;
using Domain.Entities;
using Use_Cases.Posts.Commands;

namespace Application.Posts.CommandHandlers
{
    public class UpdatePostHandler : IUpdatePostHandler
    {
        private readonly IPostRepository _postRepository;

        public UpdatePostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<PostDto?> Handle(UpdatePost request)
        {
            var post = await _postRepository.UpdatePost(request.UpdatedContent, request.PostId);
            return post;
        }
    }
}
