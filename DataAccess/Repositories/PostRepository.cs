using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocialDbcontext _socialDbcontext;
        
        public PostRepository(SocialDbcontext socialDbcontext)
        {
            _socialDbcontext = socialDbcontext; 
        }

         public async Task<Post> CreatePost(Post toCreate)
        { 
            toCreate.DateCreated = DateTime.Now;
            toCreate.LastModified = DateTime.Now;
            _socialDbcontext.Add(toCreate);
            await _socialDbcontext.SaveChangesAsync();
            return toCreate; 
        }

        public async Task DeletePost(int postId)
        {
            var post = await _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null) return;

            _socialDbcontext.Remove(post);

            await _socialDbcontext.SaveChangesAsync();  
        }

        public async Task<Post> GetPostById(int postId)
        {
            return await _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<ICollection<Post>> GetPosts()
        {
           return await _socialDbcontext.Posts.ToListAsync();
        }

        public async Task<Post> UpdatePost(string updatedContent, int postId)
        {
            var post = await _socialDbcontext.Posts.FirstOrDefaultAsync(x => x.Id == postId); 
            post.LastModified = DateTime.Now;
            post.Content = updatedContent;
            await _socialDbcontext.SaveChangesAsync();
            return post;
        }
    }
}
