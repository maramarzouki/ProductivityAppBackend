using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;

namespace Repository.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CreateUser(UserModel user)
        {
            string query = "insert into users_table(firstname, lastname, email, password) values(@firstname, @lastname, @email, @password)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@firstname", user.FirstName);
                    cmd.Parameters.AddWithValue("@lastname", user.LastName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<UserModel> GetUserByEmail(string email)
        {
            string query = "select Id, FirstName, LastName, Email, Password, ChangePasswordCode, ChangePasswordCodeExpiry from users_table where email=@email";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                ChangePasswordCode = reader.IsDBNull(reader.GetOrdinal("ChangePasswordCode")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ChangePasswordCode")),
                                ChangePasswordCodeExpiry = reader.IsDBNull(reader.GetOrdinal("ChangePasswordCodeExpiry")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("ChangePasswordCodeExpiry"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task SetChangePasswordCode(string email, int code, long codeExpiry)
        {
            string query = "update users_table set ChangePasswordCode = @code, ChangePasswordCodeExpiry = @codeExpiry where email = @email";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@codeExpiry", codeExpiry);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdatePassword(string email, string password)
        {
            string query = "update users_table set password = @password where email = @email";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
