using TagsManagement.DTOs;

public interface ITagService
{
    Task<TagViewModel> GetTagByIdAsync(string id);
    Task<IEnumerable<TagViewModel>> GetAllTagsAsync();
    Task AddTagAsync(TagViewModel tagvm);
    Task UpdateTagAsync(string id, TagViewModel tagvm);
    Task DeleteTagAsync(string id);
}