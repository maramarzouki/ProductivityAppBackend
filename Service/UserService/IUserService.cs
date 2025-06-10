using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.DTOs.UserDTOs;

namespace Service.UserService
{
    public interface IUserService
    {
        Task<string> RegisterUser(UserModel user);
        Task<string> LoginUser(LoginDTO loginDTO);
        Task<string> SendCode(string email);
        Task<string> VerifyCode(VerifyCodeDTO verifyCodeDTO);
        Task<string> UpdatePassword(ResetPasswordDTO resetPasswordDTO);
        Task<UserModel> GetUserByEmail(string email);
        Task<UserModel> GetUserById(int id);
        Task<string> UpdateUser(UserModel user);
        Task<string> UpdateIsFirstTime(int userId);
        Task<string> FillAreasToDevelop(int userId, string[] areasToDevelop);
    }
}
