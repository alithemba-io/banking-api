using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class{
    protected readonly BankDbContext _dbContext;
    public Repository(BankDbContext dbContext){
        _dbContext = dbContext;       
    }
    public async Task<IEnumerable<T>> GetAllAsync(){
        return await _dbContext.Set<T>().ToListAsync();
    }
    public async Task<T?> GetByIdAsync(int id){
        return await _dbContext.Set<T>().FindAsync(id);
    }
    public async Task<T> AddAsync(T entity){
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }
    public async Task UpdateAsync(T entity){
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(T entity){
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();    
    }
}