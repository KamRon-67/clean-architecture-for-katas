using Domain.Entities;

namespace Application.Abstractions
{
    public interface IPostRepository
    {
        Task<ICollection<PostDto>> GetPosts();
        Task<PostDto?> GetPostById(int postId);
        Task<PostDto> CreatePost(Post toCreate);
        Task<PostDto?> UpdatePost(string updatedContent, int postId);
        void DeletePost(int postId);
    }
}
