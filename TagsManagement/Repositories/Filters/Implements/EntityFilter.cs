using TagsManagement.DomainModels;
using TagsManagement.Repositories.Filters.Interfaces;

namespace TagsManagement.Repositories.Filters.Implements
{
    public class EntityFilter<T> : IEntityFilter<T> where T : BaseEntity
    {
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public IQueryable<T> FilterObjects(IQueryable<T> query)
        {
            if (!string.IsNullOrEmpty(Description))
                query = query.Where(x => x.Description.Contains(Description));

            if (CreatedDate.HasValue)
                query = query.Where(x => x.CreatedDate == CreatedDate.Value);

            if (LastModifiedDate.HasValue)
                query = query.Where(x => x.LastModifiedDate == LastModifiedDate.Value);

            if (IsDeleted.HasValue)
                query = query.Where(x => x.IsDeleted == IsDeleted.Value);

            return query;
        }
    }

    #region old code
    //public class EntityFilter<T> where T : BaseEntity, IEntityFilter<T>
    //{
    //    public string? Description { get; set; }
    //    public DateTime? CreatedDate { get; set; }
    //    public DateTime? LastModifiedDate { get; set; }
    //    public bool? IsDeleted { get; set; }

    //    public IQueryable<T> FilterObjects(IQueryable<T> query)
    //    {
    //        if (!string.IsNullOrEmpty(Description))
    //            query = query.Where(x => x.Description.Contains(Description));

    //        if (CreatedDate.HasValue)
    //            query = query.Where(x => x.CreatedDate == CreatedDate.Value);

    //        if (LastModifiedDate.HasValue)
    //            query = query.Where(x => x.LastModifiedDate == LastModifiedDate.Value);

    //        if (IsDeleted.HasValue)
    //            query = query.Where(x => x.IsDeleted == IsDeleted.Value);

    //        return query;
    //    }
    //}
    #endregion
}
