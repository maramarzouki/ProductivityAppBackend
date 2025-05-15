using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Admin
{
    public interface IAdminService
    {
        Task<bool> CreateAsync(string username, string password);
        Task<bool> ValidateLoginAsync(string username, string password);
    }
}
