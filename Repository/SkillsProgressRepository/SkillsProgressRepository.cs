using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;

namespace Repository.SkillsProgressRepository
{
    public class SkillsProgressRepository : ISkillsProgressRepository
    {
        private readonly string _connectionString;
        public SkillsProgressRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<SkillsProgressModel?> GetLatestSkillsProgress(int userId, string categoryName)
        {
            const string query = @"
                SELECT TOP 1 *
                FROM skills_progress_table
                WHERE userId = @userId AND categoryName = @categoryName
                ORDER BY testDate DESC, Id DESC";

            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@categoryName", categoryName);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new SkillsProgressModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TestDate = reader.GetDateTime(reader.GetOrdinal("TestDate")),
                    OldProgress = reader.GetDouble(reader.GetOrdinal("OldProgress")),
                    NewProgress = reader.GetDouble(reader.GetOrdinal("NewProgress")),
                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                };
            }
            return null;
        }

        public async Task<SkillsProgressModel> AddSkillsProgress(SkillsProgressModel skillsProgress)
        {
            var latest = await GetLatestSkillsProgress(skillsProgress.UserId, skillsProgress.CategoryName);
            var oldProgressValue = latest?.NewProgress ?? 0.0;

            string query = "insert into skills_progress_table(testDate, oldProgress, newProgress, categoryName, userId) OUTPUT INSERTED.Id values(@testDate, @oldProgress, @newProgress, @categoryName, @userId)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@testDate", skillsProgress.TestDate);
                    cmd.Parameters.AddWithValue("@oldProgress", oldProgressValue);
                    cmd.Parameters.AddWithValue("@newProgress", skillsProgress.NewProgress);
                    cmd.Parameters.AddWithValue("@categoryName", skillsProgress.CategoryName);
                    cmd.Parameters.AddWithValue("@userId", skillsProgress.UserId);
                    //await cmd.ExecuteNonQueryAsync();
                    var newIdObj = await cmd.ExecuteScalarAsync();
                    skillsProgress.Id = Convert.ToInt32(newIdObj);
                    return skillsProgress;
                }
            }
        }

        public async Task<SkillsProgressModel?> GetSkillsProgress(int userID)
        {
            string query = "select * from skills_progress_table where userId = @userID";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new SkillsProgressModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                TestDate = reader.GetDateTime(reader.GetOrdinal("TestDate")),
                                OldProgress = reader.GetDouble(reader.GetOrdinal("OldProgress")),
                                NewProgress = reader.GetDouble(reader.GetOrdinal("NewProgress")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> UpdateSkillsProgress(SkillsProgressModel skillsProgress)
        {
            string query = @"UPDATE skills_progress_table 
                     SET testDate = @testDate, 
                         oldProgress = @oldProgress,
                         newProgress = @newProgress,
                         categoryName = @categoryName, 
                         userId = @userId
                     WHERE id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@testDate", skillsProgress.TestDate);
                    cmd.Parameters.AddWithValue("@oldProgress", skillsProgress.OldProgress);
                    cmd.Parameters.AddWithValue("@newProgress", skillsProgress.NewProgress);
                    cmd.Parameters.AddWithValue("@categoryName", skillsProgress.CategoryName);
                    cmd.Parameters.AddWithValue("@userId", skillsProgress.UserId);
                    cmd.Parameters.AddWithValue("@id", skillsProgress.Id);
                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
        }

        public async Task<List<SkillsProgressModel>> GetSkillsProgressByUserID(int userId)
        {
            List<SkillsProgressModel> skillsProgressList = [];
            string query = "SELECT * FROM skills_progress_table WHERE userId = @userId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            skillsProgressList.Add(new SkillsProgressModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                TestDate = reader.GetDateTime(reader.GetOrdinal("TestDate")),
                                OldProgress = reader.GetDouble(reader.GetOrdinal("OldProgress")),
                                NewProgress = reader.GetDouble(reader.GetOrdinal("NewProgress")),
                                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            });
                        }
                    }
                }
            }
            return skillsProgressList;
        }

        public async Task<bool> DeleteSkillsProgress(int skillsProgressId)
        {
            string query = "delete from skills_progress_table where id=@id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", skillsProgressId);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}
