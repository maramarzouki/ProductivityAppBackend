using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.UserRepository
{
    public interface IUserRepository
    {
        public Task<UserModel> GetUserByEmail(string email);
        public Task<UserModel> GetUserById(int id);
        public Task CreateUser(UserModel user);
        public Task SetChangePasswordCode(string email, int code, long codeExpiry);
        public Task UpdatePassword(string email, string password);
        Task<bool> UpdateUser(UserModel user);
        Task<bool> UpdateIsFirstTime(int userId);
        Task<bool> FillAreasToDevelop(int userId, string[] areasToDevelop);
    }
}
