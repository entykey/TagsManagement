using TagsManagement.DomainModels;
using TagsManagement.Repositories.Implements.ORM;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Repositories
{
    public class TagRepository : EFRepository<Tag>, ITagRepository
    {
        public TagRepository(EFAppDbContext dbContext) : base(dbContext)
        {
        }
        // extra specific methods here...
    }

    //public class TagRepository : ITagRepository
    //{
    //    private readonly AppDbContext _dbContext;

    //    public TagRepository(AppDbContext dbContext)
    //    {
    //        _dbContext = dbContext;
    //    }

    //    public async Task<Tag> GetTagByIdAsync(string id)
    //    {
    //        return await _dbContext.Tags.FindAsync(id);
    //    }

    //    public async Task<Tag> GetTagByNameAsync(string name)
    //    {
    //        return await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name);
    //    }

    //    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    //    {
    //        return await _dbContext.Tags.ToListAsync();
    //    }

    //    public async Task AddTagAsync(TagViewModel tagvm)
    //    {
    //        Tag tagtoBeAdded = new Tag()
    //        {
    //            // mapping: (direct assign) hand written code
    //            Name = tagvm.Name,
    //            Id = Guid.NewGuid().ToString()
    //        };


    //        await _dbContext.Tags.AddAsync(tagtoBeAdded);
    //        await _dbContext.SaveChangesAsync();
    //    }

    //    public async Task<Tag> UpdateTagAsync(Tag tag)
    //    {
    //        _dbContext.Entry(tag).State = EntityState.Modified;
    //        await _dbContext.SaveChangesAsync();
    //        return tag;
    //    }

    //    public async Task DeleteTagAsync(string id)
    //    {
    //        var tag = await GetTagByIdAsync(id);
    //        if (tag != null)
    //        {
    //            // Check if there are any PostTags that reference this Tag
    //            var postTags = await _dbContext.PostTags
    //                .Where(pt => pt.TagId == id)
    //                .ToListAsync();

    //            // Delete the PostTags that reference this Tag first
    //            _dbContext.PostTags.RemoveRange(postTags);

    //            // Delete the Tag
    //            _dbContext.Tags.Remove(tag);
    //            await _dbContext.SaveChangesAsync();
    //        }
    //    }
    //}
}
