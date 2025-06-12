using Application.Abstractions;
using Domain.Entities;
using Use_Cases.Posts.Queries;

namespace Use_Cases.Posts.QueryHandlers
{
    public class GetPostByIdHandler : IGetPostByIdHandler
    {
        private readonly IPostRepository _postRepository;

        public GetPostByIdHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Post?> Handle(GetPostById request)
        {
            return await _postRepository.GetPostById(request.PostId);
        }
    }
}
