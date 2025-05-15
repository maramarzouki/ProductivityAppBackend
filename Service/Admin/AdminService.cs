using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.AdminRepository;

namespace Service.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        public AdminService(IAdminRepository repo) => _repo = repo;

        public async Task<bool> CreateAsync(string username, string password)
        {
            if (await _repo.GetByUsernameAsync(username) != null) return false;
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            await _repo.AddAsync(new AdminModel { Username = username, Password = hash });
            return true;
        }

        public async Task<bool> ValidateLoginAsync(string username, string password)
        {
            var admin = await _repo.GetByUsernameAsync(username);
            return admin != null && BCrypt.Net.BCrypt.Verify(password, admin.Password);
        }
    }
}
