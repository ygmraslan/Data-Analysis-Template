namespace DataAnalysis.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}