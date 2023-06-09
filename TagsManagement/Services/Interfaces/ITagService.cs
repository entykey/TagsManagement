﻿using TagsManagement.DTOs;

public interface ITagService
{
    Task<TagViewModel> GetTagByIdAsync(string id);
    Task<IEnumerable<TagViewModel>> GetAllTagsAsQueryable();
    Task<IEnumerable<TagViewModel>> GetAllTagsAsList();
    Task AddTagAsync(TagViewModel tagvm);
    Task UpdateTagAsync(string id, TagViewModel tagvm);
    Task DeleteTagAsync(string id);
}