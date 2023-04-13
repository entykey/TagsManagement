namespace TagsManagement.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable  // in order to call Dispose method
    {
        ITagRepository TagRepository { get; }
        ITagRepository Tags { get; }
        IPostRepository Posts { get; }
        Task<Object> SaveEntitiesAsync();
        void Rollback();
        Task<bool> RollbackAsync(); // with status as return
    }
}
