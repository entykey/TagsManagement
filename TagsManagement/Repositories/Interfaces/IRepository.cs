using System.Linq.Expressions;
using TagsManagement.DomainModels;

namespace TagsManagement.Repositories.Interfaces
{
    // IGenericRepository
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(string id);
        IQueryable<TEntity> GetAll();  // AsQueryable (synchronous but fast: 0ms)(high speed)
        Task<List<TEntity>> GetAllAsync();  // AsEnumerable (async but a bit slower: 1ms)(high i/o)
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        //Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);    // (entityframwork doesn't have async version of Update())
        void Delete(TEntity entity);
        void DeleteAll();
    }
}
