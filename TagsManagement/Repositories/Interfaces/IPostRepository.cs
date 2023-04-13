using TagsManagement.DomainModels.Contents;
using TagsManagement.DTOs;

namespace TagsManagement.Repositories.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        // additional methods: maybe put these in Service layer

        Task<List<Post>> GetAllPostsWithTagsAsync();
        Task AddPostWithTagsAsync(PostAddModel postAddModel);
    }
}
