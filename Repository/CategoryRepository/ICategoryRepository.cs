using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;

namespace Repository.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryModel>> GetAllAsync();
        Task<CategoryModel?> GetByIdAsync(int id);
        Task CreateAsync(CategoryModel category);
        Task UpdateAsync(CategoryModel category);
        Task DeleteAsync(int id);
    }
}
