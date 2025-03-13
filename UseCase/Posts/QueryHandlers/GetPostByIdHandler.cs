using Application.Abstractions;
using Application.Posts.Queries;
using Domain.Entities;

namespace Application.Posts.QueryHandlers
{
    public class GetPostByIdHandler 
    {
        private readonly IPostRepository _postRepository;
        public Post Handle(GetPostById request, CancellationToken cancellationToken)
        {
            return _postRepository.GetPostById(request.PostId);
        }
    }
}
