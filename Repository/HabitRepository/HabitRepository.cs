using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;

namespace Repository.HabitRepository
{
    public class HabitRepository : IHabitRepository
    {
        private readonly string _connectionString;
        public HabitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task AddHabit(HabitModel habit)
        {
            string query = "insert into habits_table (name, isDone, challengeId) values(@name, @isDone, @challengeId)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", habit.Name);
                    cmd.Parameters.AddWithValue("@isDone", habit.IsDone);
                    cmd.Parameters.AddWithValue("@challengeId", habit.ChallengeId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<HabitModel>> GetHabitsByChallengeID(int challengeId)
        {
            List<HabitModel> habits = [];
            string query = "SELECT * FROM habits_table WHERE challengeId = @challengeId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@challengeId", challengeId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            habits.Add(new HabitModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                IsDone = reader.GetBoolean(reader.GetOrdinal("IsDone")),
                                ChallengeId = reader.GetInt32(reader.GetOrdinal("ChallengeId")),
                            });
                        }
                    }
                }
            }
            return habits;
        }

        public async Task<bool> CheckHabit(bool isChecked, int habitId)
        {
            string query = "update habits_table set isDone = @isChecked where id = @habitId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@habitId", habitId);
                    cmd.Parameters.AddWithValue("@isChecked", isChecked);
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }

        }

        public async Task<bool> UpdateHabit(HabitModel habit)
        {
            string query = @"UPDATE habits_table 
                     SET name = @name
                     WHERE id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", habit.Name);
                    cmd.Parameters.AddWithValue("@id", habit.Id);

                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;

                }
            }
        }

        public async Task<bool> DeleteHabit(int habitId)
        {
            string query = "delete from habits_table where id=@id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", habitId);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}
