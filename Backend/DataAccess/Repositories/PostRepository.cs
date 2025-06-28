using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocialDbcontext _socialDbcontext;

        public PostRepository(SocialDbcontext socialDbcontext)
        {
            _socialDbcontext = socialDbcontext;
        }

        public async Task<PostDto> CreatePost(Post toCreate)
        {
            var post = new Post()
            {
                Id = toCreate.Id,
                Comments = toCreate.Comments,
                Content = toCreate.Content,
                DateCreated = DateTime.Now,
                LastModified = DateTime.Now
            };
            
            await _socialDbcontext.AddAsync(post);  // ✅ Use async version
            await _socialDbcontext.SaveChangesAsync();  // ✅ Await SaveChangesAsync

            return new PostDto(Id: post.Id, Comments: post.Comments, Content: post.Content);
        }

        public async void DeletePost(int postId)
        {
            var post = await _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null) return;
            
            _socialDbcontext.Remove(post);

            _socialDbcontext.SaveChangesAsync();
        }

        public async Task<PostDto?> GetPostById(int postId)
        {
            //var post = await _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            var post = await _socialDbcontext.Posts
                .Where(x => x.Id == postId)
                .Select(x => new PostDto(x.Id, x.Comments, x.Content))
                .FirstOrDefaultAsync();
            
            if (post == null)
            {
                // Log a message or throw an exception if necessary
                Console.WriteLine($"Post with ID {postId} not found.");
            }

            return post;
        }

        public async Task<ICollection<PostDto>> GetPosts()
        {
            return await _socialDbcontext.Posts
                .Select(x => new PostDto(x.Id, x.Comments, x.Content))
                .ToListAsync();
        }

        public async Task<PostDto?> UpdatePost(string updatedContent, int postId)
        {
            var post = await _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
            {
                throw new KeyNotFoundException($"Post with ID {postId} not found.");
            }


            post.LastModified = DateTime.Now;
            post.Content = updatedContent;

            await _socialDbcontext.SaveChangesAsync();
            return new PostDto(Id: post.Id, Comments: post.Comments, Content: post.Content);
        }

    }
}
