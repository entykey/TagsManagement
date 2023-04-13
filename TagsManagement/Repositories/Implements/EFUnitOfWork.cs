using Microsoft.EntityFrameworkCore;
using TagsManagement.DomainModels;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Repositories.Implements
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly EFAppDbContext _dbContext;

        private IRepository<Tag> _genericTags;
        private ITagRepository _tags;
        private IPostRepository _posts;
        private readonly IConfiguration _configuration;  // net 7.0 (get environment variables)

        public EfUnitOfWork(EFAppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));   // This is a defensive programming technique to ensure that the dbContext parameter is not null, and to provide a clear and descriptive error message if it is. If dbContext is not null, it is assigned to the _dbContext field.
            _configuration = configuration;
        }

        #region Tags repo:
        // both 2 Tags repo work:
        public ITagRepository TagRepository => _tags ??= new TagRepository(_dbContext);

        public ITagRepository Tags
        {
            get
            {
                return _tags ?? (_tags= new TagRepository(_dbContext));
            }
        }
        #endregion
        #region Generic Tag repo:
        //public IRepository<Tag> genericTags 
        //{
        //    get
        //    {
        //        // also pass the TagRepository into PostRepository
        //        //return _posts ?? (_posts = new PostRepository(_dbContext, _tags));
        //        return _genericTags ?? (_genericTags = new IRepository<Tag>(_dbContext));
        //    }
        //}
        #endregion
        #region Post repo:
        public IPostRepository Posts
        {
            get
            {
                // also pass the TagRepository into PostRepository
                //return _posts ?? (_posts = new PostRepository(_dbContext, _tags));
                return _posts ?? (_posts = new PostRepository(_dbContext, _tags, _configuration));
            }
        }
        #endregion

        // Commit all changes to database:
        public async Task<Object> SaveEntitiesAsync()
        {
            try
            {
                int rowsAffected = await _dbContext.SaveChangesAsync(); // returns the number of rows (entities) affected by the database operation
                if (rowsAffected > 0)
                {
                    // Success: Rows were affected in the database
                    return new
                    {
                        success = true,
                        rowsAffected = rowsAffected
                    };
                }
                else
                {
                    // Failure: No rows were affected in the database
                    return new
                    {
                        success = false,
                        rowsAffected = rowsAffected
                    };
                }
            }
            catch (Exception ex)
            {
                // Exception occurred while saving changes
                // Handle the exception appropriately
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Rollback()
        {
            // Rollback anything, if necessary
            //return Task.CompletedTask;

            try
            {
                // Get all modified or added entities in the current transaction
                var modifiedEntities = _dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added)
                    .ToList();

                // Reset the state of each entity to its original values
                foreach (var entity in modifiedEntities)
                {
                    entity.State = EntityState.Unchanged;
                }

                // Discard any entities that were marked for deletion
                var deletedEntities = _dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Deleted)
                    .ToList();

                foreach (var entity in deletedEntities)
                {
                    entity.State = EntityState.Unchanged;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        //  handle exceptions internally and return a success or failure status,
        //  we can change the return type to Task<bool> or Task<Object>
        //  as we did in the SaveEntitiesAsync method above
        public async Task<bool> RollbackAsync()
        {
            try
            {
                // Get all modified or added entities in the current transaction
                var modifiedEntities = _dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added)
                    .ToList();

                // Reset the state of each entity to its original values
                foreach (var entity in modifiedEntities)
                {
                    entity.State = EntityState.Unchanged;
                }

                // Discard any entities that were marked for deletion
                var deletedEntities = _dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Deleted)
                    .ToList();

                foreach (var entity in deletedEntities)
                {
                    entity.State = EntityState.Unchanged;
                }

                // Save the changes to the database
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}
