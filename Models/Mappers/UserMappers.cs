using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DTOs.UserDTOs;

namespace Model.Mappers
{
    public static class UserMappers
    {
        public static UserModel ToUserModel(this LoginDTO dto)
        {
            return new UserModel
            {
                Email = dto.Email,
                Password = dto.Password
            };
        }

        public static UserModel FromVerifyCodeToUserModel(this VerifyCodeDTO dto)
        {
            return new UserModel
            {
                Email = dto.Email,
                ChangePasswordCode = dto.ChangePasswordCode,
            };
        }

        public static UserModel FromResetPasswordToUserModel(this ResetPasswordDTO dto)
        {
            return new UserModel
            {
                Email = dto.Email,
                Password = dto.Password,
            };
        }
    }
}

