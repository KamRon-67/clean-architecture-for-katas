using Application.Abstractions;
using Application.Posts.Queries;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Posts.QueryHandlers
{
    public class GetPostByIdHandler : IRequestHandler<GetPostById, Post>
    {
        private readonly IPostRepository _postRepository;
        public async Task<Post> Handle(GetPostById request, CancellationToken cancellationToken)
        {
            return await _postRepository.GetPostById(request.PostId);
        }
    }
}
