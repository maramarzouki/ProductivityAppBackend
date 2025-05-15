using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.AdminRepository
{
    public interface IAdminRepository
    {
        Task<AdminModel> GetByUsernameAsync(string username);
        Task AddAsync(AdminModel admin);
    }
}
