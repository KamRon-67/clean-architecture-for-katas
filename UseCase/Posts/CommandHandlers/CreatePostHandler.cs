using Application.Abstractions;
using Application.Posts.Commands;
using Domain.Entities;

namespace Application.Posts.CommandHandlers
{
    public class CreatePostHandler : ICreatePostHandler 
    {
        private readonly IPostRepository _postRepository;

        public CreatePostHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Post> Handle(CreatePost request)
        {
            var newPost = new Post
            {
                Comments = request.PostComments, // Use the request data instead of hardcoded values
                Content = request.PostContent, 
                DateCreated = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            try
            {
                await _postRepository.CreatePost(newPost);  // ✅ Await the async repository method
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error creating post: {ex.Message}");
                throw;  // Re-throw exception to ensure proper error handling
            }
            
            return newPost;
        }
    }
}