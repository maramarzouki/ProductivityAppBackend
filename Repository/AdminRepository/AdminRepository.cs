using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;

namespace Repository.AdminRepository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectionString;
        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<AdminModel> GetByUsernameAsync(string username)
        {
            const string query = @"select id, username, password from admins_table where username = @username";

            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new AdminModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("username")),
                                Password = reader.GetString(reader.GetOrdinal("password"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task AddAsync(AdminModel admin)
        {
            const string query = @"INSERT INTO admins_table (Username, Password)VALUES (@Username, @Hash)";

            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", admin.Username);
                    cmd.Parameters.AddWithValue("@Hash", admin.Password);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}