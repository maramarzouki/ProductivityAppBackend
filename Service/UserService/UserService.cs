using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.DTOs.UserDTOs;
using Model.Mappers;
using Repository.UserRepository;
using Service.EmailSender;

namespace Service.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IEmailSender emailSender, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _configuration = configuration;
        }
        //private string HashPassword(string password)
        //{
        //    using (var sha = SHA256.Create())
        //    {
        //        var bytes = Encoding.UTF8.GetBytes(password);
        //        var hash = sha.ComputeHash(bytes);
        //        return Convert.ToBase64String(hash);
        //    }
        //}
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        private int GenerateSecureRandomCode()
        {
            byte[] bytes = new byte[4];
            RandomNumberGenerator.Fill(bytes);
            int randomInt = BitConverter.ToInt32(bytes, 0);
            randomInt = Math.Abs(randomInt);
            int code = randomInt % 900000 + 100000; // Ensures a 6-digit number between 100000 and 999999
            return code;
        }
        private string GenerateJwtToken(UserModel user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            string secretKey = jwtSettings["Secret"];
            string issuer = jwtSettings["Issuer"];
            string audience = jwtSettings["Audience"];
            int expiryHours = int.Parse(jwtSettings["ExpiryHours"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.UtcNow.AddHours(expiryHours),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<string> LoginUser(LoginDTO loginDTO)
        {
            var userModel = loginDTO.ToUserModel();
            var user = await _userRepository.GetUserByEmail(userModel.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password!");
            }
            if (!VerifyPassword(userModel.Password, user.Password))
            {
                throw new Exception("Password is not correct!");
            }
            string token = GenerateJwtToken(user);
            return token;
        }

        public async Task<string> RegisterUser(UserModel user)
        {
            if (await _userRepository.GetUserByEmail(user.Email) != null)
            {
                throw new Exception("Email already exists!");
            }
            var newUser = new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = HashPassword(user.Password),
                IsFirstTime = user.IsFirstTime
            };
            await _userRepository.CreateUser(newUser);
            return "User registred successfully!";
        }

        public async Task<string> SendCode(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found, make sure you have entered the right email!");
            }
            var code = GenerateSecureRandomCode();
            long codeExpiry = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 5 * 60 * 1000;
            string message = $"This is your code {code} and it will expire in 5 minutes";
            string subject = "Reset password code";
            await _userRepository.SetChangePasswordCode(email, code, codeExpiry);
            await _emailSender.SendEmailAsync(email, subject, message);
            return "Code sent to your email!";
        }

        public async Task<string> VerifyCode(VerifyCodeDTO verifyCodeDTO)
        {
            var userModel = verifyCodeDTO.FromVerifyCodeToUserModel();
            var user = await _userRepository.GetUserByEmail(userModel.Email);
            if (user == null)
            {
                throw new Exception("Wrong email!");
            }
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var name = user.LastName;
            var time = user.ChangePasswordCodeExpiry;
            if (user.ChangePasswordCodeExpiry < currentTime)
            {
                return $"Code has expired! user name : {name} code expiry: {time} current time: {currentTime}";
            }
            if (user.ChangePasswordCode != userModel.ChangePasswordCode)
            {
                return "Code is wrong!";
            }
            return "Code is correct!";
        }

        public async Task<string> UpdatePassword(ResetPasswordDTO resetPasswordDTO)
        {
            var userModel = resetPasswordDTO.FromResetPasswordToUserModel();
            var user = await _userRepository.GetUserByEmail(userModel.Email);
            if (user == null)
            {
                throw new Exception("Wrong email!");
            }
            var newHashedPassword = HashPassword(userModel.Password);
            await _userRepository.UpdatePassword(userModel.Email, newHashedPassword);
            return "Password changed successfully!";
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            return user;
        }

        public async Task<UserModel> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            return user;
        }

        public async Task<string> UpdateUser(UserModel user)
        {
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            var res = await _userRepository.UpdateUser(user);
            if (res)
            {
                return "User updated!";
            }
            return "Error updating the user";
        }

        public async Task<string> UpdateIsFirstTime(int userId)
        {
            var res = await _userRepository.UpdateIsFirstTime(userId);
            if (res)
            {
                return "User IsFirstTime updated!";
            }
            return "Error updating IsFirstTime";
        }

        public async Task<string> FillAreasToDevelop(int userId, string[] areasToDevelop)
        {
            var res = await _userRepository.FillAreasToDevelop(userId, areasToDevelop);
            if (res)
            {
                return "Updated!";
            }
            return "Error updating!";
        }
    }
}
