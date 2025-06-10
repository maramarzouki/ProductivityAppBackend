using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;
using Repository.CategoryRepository;

namespace Service.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
            => _repo = repo;

        public Task<IEnumerable<CategoryModel>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<CategoryModel?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task CreateAsync(CategoryModel category)
            => _repo.CreateAsync(category);

        public Task UpdateAsync(CategoryModel category)
            => _repo.UpdateAsync(category);

        public Task DeleteAsync(int id)
            => _repo.DeleteAsync(id);
    }
}
