using CategoryService.Entities;
namespace CategoryService.Repositories;
public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(string id);
     Task<Category?> GetByNameAsync(string name);
    Task<Category> AddAsync(Category category);
    Task<Category?> UpdateAsync(string id, Category category);
    Task<bool> DeleteAsync(string id);
}
