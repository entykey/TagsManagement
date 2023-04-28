using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TagsManagement.DomainModels;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Repositories.Implements.ORM
{
    public abstract class EFRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly EFAppDbContext _dbContext;
        private DbSet<TEntity> entities;    // initialize generic T entity for the entire repository

        public EFRepository(EFAppDbContext dbContext)
        {
            _dbContext = dbContext;
            entities = _dbContext.Set<TEntity>();
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            // or:
            //return await _dbContext.Set<TEntity>().FindAsync(id);
            //return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);
            return await entities.FirstOrDefaultAsync(e => e.Id == id);

        }

        public IQueryable<TEntity> GetAll()     // AsQueryable (synchronous)
        {
            //return _dbContext.Set<TEntity>().AsQueryable();
            return entities.AsQueryable();  // more clean code
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await entities.ToListAsync();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await entities.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await entities.AddAsync(entity);
        }

        public void Update(TEntity entity)    // Update (synchronous)
        {
            entities.Update(entity);
        }

        public void Delete(TEntity entity)    // Delete (synchronous)
        {
            entities.Remove(entity);
        }

        public void DeleteAll()               // DeleteRange (synchronous)
        {
            entities.RemoveRange(_dbContext.Set<TEntity>());
            //await _dbContext.SaveChangesAsync();  // no need here, it's UnitOfWork's job
        }

        /*
         * 
        // remove & commit changes right into database
        public async Task<bool> DeleteAsync(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
        */


        //Note: Entity Framework Core doesn't have asynchronous versions of the 'Remove' and
        //'Update' methods like it does for 'AddAsync'. This is because these methods don't actually
        //perform any database operations until the SaveChanges method is called, which can be asynchronous!!

        //You can still use the synchronous Remove and Update methods in your
        //repository implementation, but you should make sure to call SaveChangesAsync
        //after any changes are made to ensure they are persisted to the database.

        //Leaving the SaveChanges method for the UnitOfWork is a good approach as it allows
        //you to manage all changes across multiple repositories in a single transaction.
        //This way, if any repository fails to save changes, you can rollback all changes
        //in a single operation.
    }

}
