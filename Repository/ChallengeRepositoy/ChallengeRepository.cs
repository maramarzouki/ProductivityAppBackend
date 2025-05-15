using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;
using static Model.Enums;

namespace Repository.ChallengeRepositoy
{
    public class ChallengeRepository : IChallengeRepository
    {
        private readonly string _connectionString;
        public ChallengeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task CreateChallenge(ChallengeModel challenge)
        {
            const string sql = "INSERT INTO challenges_table(name, description, startDate, totalDays, completedDates, isCanceled, isStarted, isCompleted, userId)VALUES(@name, @description, @startDate, @totalDays, @completedDates, 0, 0, 0, @userId)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@name", challenge.Name);
                    cmd.Parameters.AddWithValue("@description", challenge.Description ?? "");
                    cmd.Parameters.AddWithValue("@startDate", challenge.StartDate);
                    cmd.Parameters.AddWithValue("@totalDays", challenge.TotalDays);
                    cmd.Parameters.AddWithValue("@completedDates", JsonSerializer.Serialize(challenge.CompletedDates));
                    cmd.Parameters.AddWithValue("@userId", challenge.UserId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task<ChallengeModel?> GetChallengeById(int challengeId)
        {
            const string query = "SELECT * FROM challenges_table WHERE id = @id";
            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", challengeId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {

                            int cdOrdinal = reader.GetOrdinal("CompletedDates");
                            string cdJson = reader.IsDBNull(cdOrdinal)
                                ? "[]"
                                : reader.GetString(cdOrdinal);

                            return new ChallengeModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                                   ? null
                                                   : reader.GetString(reader.GetOrdinal("Description")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                TotalDays = reader.GetInt32(reader.GetOrdinal("TotalDays")),
                                CompletedDates = JsonSerializer.Deserialize<List<DateTime>>(cdJson)
                                                 ?? new List<DateTime>(),
                                IsCanceled = reader.GetBoolean(reader.GetOrdinal("IsCanceled")),
                                IsStarted = reader.GetBoolean(reader.GetOrdinal("IsStarted")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        //public async Task<ChallengeModel?> GetChallengeById(int challengeId)
        //{
        //    string query = "select * from challenges_table where id = @id";
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        await con.OpenAsync();
        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.Parameters.AddWithValue("@id", challengeId);
        //            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    int challengeDaysOrdinal = reader.GetOrdinal("ChallengeDays");
        //                    // Default to an empty array in case the field is null or empty
        //                    DateTime[] challengeDays = Array.Empty<DateTime>();
        //                    if (!reader.IsDBNull(challengeDaysOrdinal))
        //                    {
        //                        string jsonData = reader.GetString(challengeDaysOrdinal);
        //                        challengeDays = JsonSerializer.Deserialize<DateTime[]>(jsonData) ?? Array.Empty<DateTime>();
        //                    }
        //                    return new ChallengeModel
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        Name = reader.GetString(reader.GetOrdinal("Name")),
        //                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
        //                        ChallengeTotalDays = reader.GetInt32(reader.GetOrdinal("ChallengeTotalDays")),
        //                        DaysCompletedNb = reader.GetInt32(reader.GetOrdinal("DaysCompletedNb")),
        //                        ChallengeDays = challengeDays,
        //                        IsCanceled = reader.GetBoolean(reader.GetOrdinal("IsCanceled")),
        //                        IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
        //                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),

        //                    };
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}
        public async Task<List<ChallengeModel>> GetUserChallenges(int userId)
        {
            var challenges = new List<ChallengeModel>();
            const string query = "SELECT * FROM challenges_table WHERE userid = @userId";

            using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Safely read CompletedDates JSON (fall back to "[]" if NULL)
                            int cdOrdinal = reader.GetOrdinal("CompletedDates");
                            string cdJson = reader.IsDBNull(cdOrdinal)
                                ? "[]"
                                : reader.GetString(cdOrdinal);

                            challenges.Add(new ChallengeModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                                   ? null
                                                   : reader.GetString(reader.GetOrdinal("Description")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                TotalDays = reader.GetInt32(reader.GetOrdinal("TotalDays")),
                                CompletedDates = JsonSerializer.Deserialize<List<DateTime>>(cdJson)
                                                 ?? new List<DateTime>(),
                                IsCanceled = reader.GetBoolean(reader.GetOrdinal("IsCanceled")),
                                IsStarted = reader.GetBoolean(reader.GetOrdinal("IsStarted")),
                                IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                            });
                        }
                    }
                }
            }

            return challenges;
        }

        public async Task<List<ChallengeModel>> GetAllChallenges()
        {
            var list = new List<ChallengeModel>();
            const string sql = "SELECT * FROM challenges_table";
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SqlCommand(sql, con);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // same mapping you already have in GetUserChallenges...
                var cdJson = reader.IsDBNull(reader.GetOrdinal("CompletedDates"))
                               ? "[]" : reader.GetString(reader.GetOrdinal("CompletedDates"));
                list.Add(new ChallengeModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                  ? null : reader.GetString(reader.GetOrdinal("Description")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    TotalDays = reader.GetInt32(reader.GetOrdinal("TotalDays")),
                    CompletedDates = JsonSerializer.Deserialize<List<DateTime>>(cdJson) ?? new(),
                    IsCanceled = reader.GetBoolean(reader.GetOrdinal("IsCanceled")),
                    IsStarted = reader.GetBoolean(reader.GetOrdinal("IsStarted")),
                    IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                });
            }
            return list;
        }

        public async Task<bool> StartChallenge(int challengeId)
        {
            string query = "update challenges_table set isStarted = 1 where id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", challengeId);
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> RestartChallenge(int challengeId, DateTime newStartDate)
        {
            string query = "update challenges_table set isCanceled = 0, startDate = @startDate, completedDates = @completedDates where id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@startDate", newStartDate);
                    cmd.Parameters.AddWithValue("@completedDates", JsonSerializer.Serialize(Array.Empty<DateTime>()));
                    cmd.Parameters.AddWithValue("@id", challengeId);
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> CancelChallenge(int challengeId)
        {
            string query = "update challenges_table set isCanceled = 1, completedDates = @completedDates where id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@completedDates", JsonSerializer.Serialize(Array.Empty<DateTime>()));
                    cmd.Parameters.AddWithValue("@id", challengeId);
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> UpdateChallenge(ChallengeModel challenge)
        {
            //name, description, challengeTotalDays, daysCompletedNb, challengeDays, isCanceled, isCompleted, userId
            string query = @"
              UPDATE challenges_table
                SET name            = @name,
                    description     = @description,
                    startDate       = @startDate,
                    totalDays       = @totalDays,
                    completedDates  = @completedDates,
                    isCanceled      = @isCanceled,
                    isStarted       = 1,
                    isCompleted     = @isCompleted
              WHERE id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@name", challenge.Name);
                    cmd.Parameters.AddWithValue("@description", challenge.Description ?? "");
                    cmd.Parameters.AddWithValue("@startDate", challenge.StartDate);
                    cmd.Parameters.AddWithValue("@totalDays", challenge.TotalDays);
                    cmd.Parameters.AddWithValue("@completedDates", JsonSerializer.Serialize(challenge.CompletedDates));
                    cmd.Parameters.AddWithValue("@isCanceled", challenge.IsCanceled);
                    cmd.Parameters.AddWithValue("@isCompleted", challenge.IsCompleted);
                    cmd.Parameters.AddWithValue("@id", challenge.Id);

                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
        }

        //public async Task<bool> UpdateChallenge(ChallengeModel challenge)
        //{
        //    //name, description, challengeTotalDays, daysCompletedNb, challengeDays, isCanceled, isCompleted, userId
        //    string query = @"UPDATE challenges_table 
        //             SET name = @name, 
        //                 description = @description, 
        //                 challengeTotalDays = @challengeTotalDays, 
        //                 daysCompletedNb = @daysCompletedNb, 
        //                 challengeDays = @challengeDays, 
        //                 isCanceled = @isCanceled, 
        //                 isCompleted = @isCompleted
        //             WHERE id = @id";
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        await con.OpenAsync();
        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.Parameters.AddWithValue("@name", challenge.Name);
        //            cmd.Parameters.AddWithValue("@description", challenge.Description ?? string.Empty);
        //            cmd.Parameters.AddWithValue("@daysCompletedNb", challenge.DaysCompletedNb);
        //            cmd.Parameters.AddWithValue("@challengeDays", JsonSerializer.Serialize(challenge.ChallengeDays));
        //            cmd.Parameters.AddWithValue("@isCanceled", challenge.IsCanceled);
        //            cmd.Parameters.AddWithValue("@isCompleted", challenge.IsCompleted);
        //            cmd.Parameters.AddWithValue("@id", challenge.Id);

        //            int result = await cmd.ExecuteNonQueryAsync();

        //            return result > 0;
        //        }
        //    }
        //}


        public async Task<bool> DeleteChallenge(int habitId)
        {
            string query = "delete from challenges_table where id = @id";
            using (SqlConnection con = new(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", habitId);
                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
        }
    }
}
