using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAnalysis.Persistence.Repositories;

public abstract class Repository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    protected IQueryable<T> Query(bool asNoTracking = true)
        => asNoTracking ? DbSet.AsNoTracking() : DbSet.AsQueryable();

    protected async Task AddAsync(T entity)
        => await DbSet.AddAsync(entity);

    protected void Update(T entity)
        => DbSet.Update(entity);

    protected void Delete(T entity)
        => DbSet.Remove(entity);
}