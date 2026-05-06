using AltinKasap.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace AltinKasap.Web.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext Db;
    protected readonly DbSet<T> Set;

    public GenericRepository(AppDbContext db)
    {
        Db = db;
        Set = db.Set<T>();
    }

    public Task<T?> GetByIdAsync(int id) => Set.FindAsync(id).AsTask();

    public async Task<IEnumerable<T>> GetAllAsync() => await Set.ToListAsync();

    public IQueryable<T> Query() => Set.AsQueryable();

    public async Task<T> AddAsync(T entity)
    {
        await Set.AddAsync(entity);
        return entity;
    }

    public void Update(T entity) => Set.Update(entity);

    public void Remove(T entity) => Set.Remove(entity);

    public Task<int> SaveChangesAsync() => Db.SaveChangesAsync();
}
