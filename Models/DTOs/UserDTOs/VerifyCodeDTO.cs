using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTOs.UserDTOs
{
    public class VerifyCodeDTO
    {
        public string Email { get; set; } = string.Empty;
        public int ChangePasswordCode { get; set; }
    }
}
