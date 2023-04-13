using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Text.Json;
using TagsManagement.DomainModels;
using TagsManagement.DomainModels.Contents;
using TagsManagement.DTOs;
using TagsManagement.Repositories;
using TagsManagement.Repositories.Filters.Implements;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PostController : Controller
    {
        private readonly EFAppDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(EFAppDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        // GET: PostController
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var posts = await _dbContext.Posts.ToListAsync();
        //    return Ok(posts);
        //}


        [HttpGet]
        public async Task<IActionResult> GetAllPostsWithTags()
        {
            // get all posts with their associated tags, we can use Entity Framework's Include method
            // to eager load the related PostTags and Tag entities. We can then map the resulting data to schema
            var posts = await _dbContext.Posts.Include(p => p.PostTags).ThenInclude(pt => pt.Tag).ToListAsync();

            var result = posts.Select(p => new
            {
                title = p.Title,
                content = p.Content,
                createdDate = p.CreatedDate,
                lastModifiedDate = p.LastModifiedDate,
                postTags = p.PostTags.Select(pt => pt.Tag.Name),
                id = p.Id
            });

            return Ok(result);
        }

        [HttpGet("GetAllUsingUwO")]
        public async Task<IActionResult> GetAllPostsWithTagsUsingUwO()
        {
            // get all posts with their associated tags, we can use Entity Framework's Include method
            // to eager load the related PostTags and Tag entities. We can then map the resulting data to schema
            var posts = await _unitOfWork.Posts.GetAllPostsWithTagsAsync();

            var result = posts.Select(p => new
            {
                title = p.Title,
                content = p.Content,
                createdDate = p.CreatedDate,
                lastModifiedDate = p.LastModifiedDate,
                postTags = p.PostTags.Select(pt => pt.Tag.Name),
                id = p.Id
            });

            return Ok(result);
        }


        // GET: PostController/GetPostByLambdaExpression
        [HttpGet("GetPostByLambdaExpression")]
        public async Task<List<Post>> GetPostByLambdaExpression(string query)
        {
            // Define your search condition using a lambda expression
            Expression<Func<Post, bool>> searchCondition1 = entity => entity.Content.Contains(query); //compile to 'Like' operator

            // Call the Find method with the search condition
            List<Post> results = await _unitOfWork.Posts.FindAsync(searchCondition1);

            return results;
        }

        // POST: PostController/addPost
        [HttpPost("addPost")]
        public async Task<IActionResult> Create([FromBody] PostAddModel postvm)
        {
            Post post = new Post
            {
                Title = postvm.Title,
                Content = postvm.Content,
                Id = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
            };

            try
            {
                // Loop through all tags string in postvm to check if there is an existing tag with the name
                foreach (var tagName in postvm.Tags)
                {
                    Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

                    // If tag with that name doesn't exist in database, create it
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName, Id = Guid.NewGuid().ToString() };
                        await _dbContext.Tags.AddAsync(tag);
                    }

                    // Create a PostTag object to link the post with the tag
                    PostTag postTag = new PostTag { Post = post, Tag = tag };
                    await _dbContext.PostTags.AddAsync(postTag);
                }

                // Add the post to the database and save changes
                await _dbContext.Posts.AddAsync(post);
                var created = await _dbContext.SaveChangesAsync();
                return Ok(created);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            #region example input:
            //            {
            //                "title": "Post 1",
            //  "content": "some content...",
            //  "tags": [
            //    "Generic Repository",
            //    "new tag"
            //  ]
            //}
            #endregion
        }

        // Call the repository method in the controller
        [HttpPost("addPostUsingUwO")]
        public async Task<Object> AddPostUsingUwO([FromBody] PostAddModel postvm)
        {
            try
            {
                // Perform some business logic here
                // ...
                // Create the entity in the database
                await _unitOfWork.Posts.AddPostWithTagsAsync(postvm);
                var rsCreated = await _unitOfWork.SaveEntitiesAsync();
                return rsCreated;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                // An error occurred while saving the entity
                // Rollback any changes made to the database
                await _unitOfWork.RollbackAsync();
                return StatusCode(400, $"An error occurred while saving the entity: {ex.Message}");
            }
            #region example input model
            //{
            //    "title": "Post 2",
            //  "content": "some content...",
            //  "tags": [
            //    "asp.net"
            //  ]
            //}
            #endregion
        }

        // DELETE: PostController/DeleteAllUsingUwO
        [HttpDelete("DeleteAllUsingUwO")]
        public async Task<Object> DeleteAllUsingUwO()
        {
            try
            {
                _unitOfWork.Posts.DeleteAll();
                var rsCreated = await _unitOfWork.SaveEntitiesAsync();
                return rsCreated;
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpGet("FilterExample")]
        public async Task<IActionResult> FilterExample()
        {
            #region filter entity:
            var filter = new EntityFilter<Post>();
            filter.IsDeleted = false;
            //filter.Description = "some description";
            //filter.CreatedDate = DateTime.Now.AddDays(-7);
            #endregion

            #region query:
            //var posts = _dbContext.Posts.Include(p => p.PostTags).Where(p => !p.IsDeleted); // old code
            var posts = await _unitOfWork.Posts.GetAllPostsWithTagsAsync();
            var convertedPosts = posts.AsQueryable();
            var filteredPosts = filter.FilterObjects(convertedPosts); // apply the filtering on the IQueryable

            var result = filteredPosts.Select(p => new
            {
                title = p.Title,
                content = p.Content,
                createdDate = p.CreatedDate,
                lastModifiedDate = p.LastModifiedDate,
                postTags = p.PostTags.Select(pt => pt.Tag.Name),
                id = p.Id
            });
            #endregion

            // Serialize the posts and configure the JSON serializer to ignore circular references
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var json = JsonSerializer.Serialize(result, options);
            return Content(json, "application/json");
        }
        [HttpGet("FilterExample2")]
        public async Task<IActionResult> FilterExample2()
        {
            var filter = new EntityFilter<Post>();
            //filter.Description = "some description";
            //filter.CreatedDate = DateTime.Now.AddDays(-7);
            var query = _dbContext.Posts.Include(p => p.PostTags).Where(p => !p.IsDeleted);
            //var query = await _unitOfWork.Posts.GetAllPostsWithTagsAsync();
            query = filter.FilterObjects(query);
            var posts = await query.ToListAsync();


            // Serialize the posts and configure the JSON serializer to ignore circular references
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var json = JsonSerializer.Serialize(posts, options);
            return Content(json, "application/json");

            //return Ok(posts);
        }

        // POST: PostController/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // both 2 lines work!
                //var post = await _dbContext.Posts.FindAsync(id);
                var post = await _dbContext.Posts.FirstOrDefaultAsync(i => i.Id == id);


                if(post == null)
                {
                    return BadRequest("post with that id not found!");
                }

                _dbContext.Posts.Remove(post);

                await _dbContext.SaveChangesAsync();    // commit delete changes
                return Ok("deleted successfully!");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
