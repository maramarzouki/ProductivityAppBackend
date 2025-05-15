using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;

namespace Repository.ReminderRepository
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly string _connectionString;
        public ReminderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task CreateReminder(ReminderModel reminder)
        {
            //string query = "insert into reminders_table(title, isActive, reminderDate, reminderTime, userId) values(@title, @isActive, @reminderDate, @reminderTime, @userId)";
            string query = "insert into reminders_table(title, isActive, reminderTime, userId) values(@title, @isActive, @reminderTime, @userId)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@title", reminder.Title);
                    cmd.Parameters.AddWithValue("@isActive", reminder.IsActive);
                    //cmd.Parameters.AddWithValue("@reminderDate", reminder.ReminderDate);
                    cmd.Parameters.AddWithValue("@reminderTime", reminder.ReminderTime);
                    cmd.Parameters.AddWithValue("@userId", reminder.UserId);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> DeleteReminder(int id)
        {
            string query = "delete from reminders_table where id=@id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<List<ReminderModel>> GetRemindersByUserId(int userId)
        {
            List<ReminderModel> reminders = new List<ReminderModel>();
            string query = "SELECT * FROM reminders_table WHERE userid = @userId";

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
                            reminders.Add(new ReminderModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                //ReminderDate = reader.GetDateTime(reader.GetOrdinal("ReminderDate")),
                                ReminderTime = reader.GetString(reader.GetOrdinal("ReminderTime")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                            });
                        }
                    }
                }
            }
            return reminders;
        }

        public async Task<bool> ToggleReminder(int id, int userId, bool isActive)
        {
            string query = "update reminders_table set isActive = @isActive where id = @id and userId = @userId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@isActive", isActive);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
        }
    }
}
