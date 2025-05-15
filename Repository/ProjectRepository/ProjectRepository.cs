using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;

namespace Repository.ProjectRepository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;
        public ProjectRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task CreatePrject(ProjectModel project)
        {
            string query = "insert into projects_table(title, description, deadline, isCompleted, userId) values(@title, @description, @deadline, @isCompleted, @userId)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@title", project.Title);
                    cmd.Parameters.AddWithValue("@description", project.Description);
                    cmd.Parameters.AddWithValue("@deadline", project.Deadline);
                    cmd.Parameters.AddWithValue("@isCompleted", project.IsCompleted);
                    cmd.Parameters.AddWithValue("@userId", project.UserId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<ProjectModel?> GetProjectById(int projectId)
        {
            string query = "select * from projects_table where id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", projectId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ProjectModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),

                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<ProjectModel>> GetProjectsByUserId(int userId)
        {
            List<ProjectModel> projects = [];
            string query = "SELECT * FROM projects_table WHERE userid = @userId";

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
                            projects.Add(new ProjectModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            });
                        }
                    }
                }
            }
            return projects;
        }

        public async Task CompleteProject(bool isCompleted, int projectId)
        {
            string query = "update projects_table set isCompleted = @isCompleted where id = @projectId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("projectId", projectId);
                    cmd.Parameters.AddWithValue("isCompleted", isCompleted);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> DeleteProject(int projectId)
        {
            string query = "delete from projects_table where id=@id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", projectId);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> UpdateProject(ProjectModel project)
        {
            string query = @"UPDATE projects_table 
                     SET title = @title, 
                         description = @description,
                         deadline = @deadline,
                         isCompleted = @isCompleted, 
                         userId = @userId
                     WHERE id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@title", project.Title);
                    cmd.Parameters.AddWithValue("@description", project.Description);
                    cmd.Parameters.AddWithValue("@deadline", project.Deadline);
                    cmd.Parameters.AddWithValue("@isCompleted", project.IsCompleted);
                    cmd.Parameters.AddWithValue("@userId", project.UserId);
                    cmd.Parameters.AddWithValue("@id", project.Id);
                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
        }
    }
}
