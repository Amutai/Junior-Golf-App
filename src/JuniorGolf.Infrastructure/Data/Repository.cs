using JuniorGolf.Core.Entities;
using JuniorGolf.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JuniorGolf.Infrastructure.Data;

/// <summary>
/// Generic repository implementation using EF Core.
///
/// Dependency map:
///   IRepository<T> (Core) ← Repository<T> (Infrastructure) → AppDbContext → PostgreSQL
///
/// Data flow:
///   INPUT: Method call (e.g. GetByIdAsync(guid))
///   PROCESS: EF Core translates to SQL, executes via Npgsql
///   OUTPUT: Entity or collection of entities (null if not found)
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is not null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
