using Microsoft.EntityFrameworkCore;
using System.Data;
using TagsManagement.DomainModels;
using TagsManagement.DomainModels.Contents;
using TagsManagement.DTOs;
using TagsManagement.Repositories.Implements.ORM;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Repositories.Implements
{
    public class PostRepository : EFRepository<Post>, IPostRepository
    {
        private readonly EFAppDbContext _dbContext;
        private readonly ITagRepository _tagRepository;
        private readonly IConfiguration _configuration;  // net 7.0 (get environment variables)

        public PostRepository(EFAppDbContext dbContext,
            ITagRepository tagRepository,
            IConfiguration configuration
            ) : base(dbContext)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tagRepository = tagRepository;
        }


        // additional methods to generic Repository:

        public async Task<List<Post>> GetAllPostsWithTagsAsync()
        {
            // multiple tables (complex query):
            return await _dbContext.Posts
                .Include(p => p.PostTags)   // refrence to PostTags entity
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();
        }

        public async Task AddPostWithTagsAsync(PostAddModel postAddModel)
        {
            var post = new Post
            {
                Id = Guid.NewGuid().ToString(),
                Title = postAddModel.Title,
                Content = postAddModel.Content,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            //Tag tag = new Tag();

            foreach (var tagName in postAddModel.Tags)
            {
                // Faulty code: not suitable for SQL case-sensitive search (eg. 'ASP' & 'asp')
                //Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tagName.Normalize());

                #region ADO.NET way, using allocation, issue: unitofwork detect new enities and try save it causing Primary key duplicated
                //// preference: https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/asynchronous-programming

                //string sqlConString = _configuration["ConnectionStrings:IdentityConnection"]; // specify to match your appsettings.json


                //using (SqlConnection conn = new SqlConnection(sqlConString))
                //using (SqlCommand comm = new SqlCommand("SELECT * FROM dbo.Tags WHERE Name = @Name COLLATE SQL_Latin1_General_CP1_CS_AS", conn))
                //{
                //    comm.Parameters.AddWithValue("@Name", SqlDbType.NVarChar).Value = tagName;

                //    await conn.OpenAsync();

                //    using (var r = comm.ExecuteReader())
                //    {
                //        if (r.Read()) // Don't assume we have any rows.
                //        {
                //            // hand written assign code:
                //            tag = new Tag
                //            {
                //                Id = r.GetString(0),
                //                Name = r.GetString(1),
                //                CreatedDate = r.GetDateTime(2),
                //                IsDeleted = r.GetBoolean(3),
                //                LastModifiedDate = r.GetDateTime(4),
                //                Description = r[5] as string, // nullable
                //                Level = r.GetInt32(6)

                //            };

                //        }
                //        await r.CloseAsync();   // seems to no need async (same performance)
                //    }

                //    // close to save resources:
                //    await conn.CloseAsync();    // seems to no need async (same performance)
                //}
                #endregion


                #region attemp 1:
                //Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t =>
                //    string.Equals(t.Name, tagName, StringComparison.OrdinalIgnoreCase));
                #endregion

                #region attempt 2:
                //Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t =>
                //    t.Name.Equals(tagName.Normalize(), StringComparison.OrdinalIgnoreCase));
                #endregion

                #region attempt 3 (successfully fixed the collocation!!), reference: https://stackoverflow.com/questions/3843060/linq-to-entities-case-sensitive-comparison
                Tag? tag = _dbContext.Tags.Where(t => t.Name == tagName)
                    .AsEnumerable()
                    .First(t => t.Name == tagName);
                #endregion

                #region test attempt (still failed search case-sensitive)
                //Tag? tag = _dbContext.Tags
                //    .AsEnumerable()
                //    .FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
                #endregion


                // If tag with that name doesn't exist in database, create it
                if (tag == null)
                {
                    tag = new Tag { Name = tagName, Id = Guid.NewGuid().ToString() };
                    await _dbContext.Tags.AddAsync(tag);
                    //await _tagRepository.AddAsync(tag);
                }

                // Create a PostTag object to link/join the post with the tag
                PostTag postTag = new PostTag { Post = post, Tag = tag };

                
                await _dbContext.PostTags.AddAsync(postTag);
                
            }

            await base.AddAsync(post);

            // Detach the ADO.NET's tag entity so that it won't be tracked for saving changes
            //_dbContext.Entry(tag).State = EntityState.Detached;
        }
    }
}
