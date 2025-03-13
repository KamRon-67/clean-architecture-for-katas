using Application.Abstractions;
using Application.Posts.Queries;
using Domain.Entities;

namespace Application.Posts.QueryHandlers
{
    public class GetAllPostsHandler
    { 
        private readonly IPostRepository _postRepository;

        public GetAllPostsHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<ICollection<Post>> Handle(GetAllPosts request, CancellationToken cancellationToken)
        {
            return await _postRepository.GetPosts();
        }
    }
}
