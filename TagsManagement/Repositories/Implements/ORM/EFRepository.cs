using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TagsManagement.DomainModels;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Repositories.Implements.ORM
{
    public abstract class EFRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly EFAppDbContext _dbContext;

        public EFRepository(EFAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            // or:
            //return await _dbContext.Set<TEntity>().FindAsync(id);
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);

        }

        public IQueryable<TEntity> GetAll()     // AsQueryable (synchronous)
        {
            return _dbContext.Set<TEntity>().AsQueryable();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Update(TEntity entity)    // Update (synchronous)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)    // Delete (synchronous)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void DeleteAll()               // DeleteRange (synchronous)
        {
            _dbContext.Set<TEntity>().RemoveRange(_dbContext.Set<TEntity>());
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
