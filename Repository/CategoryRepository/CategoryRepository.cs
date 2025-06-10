using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.CategoryModels;

namespace Repository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<CategoryModel>> GetAllAsync()
        {
            const string sql = "SELECT id, name FROM categories_table";
            var list = new List<CategoryModel>();

            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            await using var cmd = new SqlCommand(sql, con);
            await using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new CategoryModel
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.GetString(1)
                });
            }

            return list;
        }

        public async Task<CategoryModel?> GetByIdAsync(int id)
        {
            const string sql = "SELECT id, name FROM categories_table WHERE id = @id";
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            await using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            await using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;

            return new CategoryModel
            {
                Id = rdr.GetInt32(0),
                Name = rdr.GetString(1)
            };
        }

        public async Task CreateAsync(CategoryModel category)
        {
            const string sql =
                "INSERT INTO categories_table (name) " +
                "VALUES (@name)";
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            await using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@name", category.Name);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(CategoryModel category)
        {
            const string sql =
                "UPDATE categories_table " +
                "SET name = @name " +
                "WHERE id = @id";
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            await using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@name", category.Name);
            cmd.Parameters.AddWithValue("@id", category.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM categories_table WHERE id = @id";
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            await using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
