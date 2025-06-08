using Application.Abstractions;
using Domain.Entities;
using Use_Cases.Posts.Queries;

namespace Use_Cases.Posts.QueryHandlers
{
    public class GetAllPostsHandler : IGetAllPostsHandler
    {
        private readonly IPostRepository _postRepository;

        public GetAllPostsHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<ICollection<Post>> Handle(GetAllPosts request)
        {
            return await _postRepository.GetPosts();
        }
    }
}
