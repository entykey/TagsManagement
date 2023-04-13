using TagsManagement.DomainModels;

namespace TagsManagement.Repositories.Filters.Interfaces
{
    public interface IEntityFilter<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> FilterObjects(IQueryable<TEntity> query);
    }
}
