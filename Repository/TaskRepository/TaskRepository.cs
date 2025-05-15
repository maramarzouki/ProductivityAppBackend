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

namespace Repository.TaskRepository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;
        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task CreateTask(TaskModel task)
        {
            string query = @"
                INSERT INTO tasks_table
                    (label, description, StartTime, endtime, date, complexity, priority, taskcolor,
                     interval, duration, durationunit, isChecked, isRepetitive, repetitionDates, weekdays, userId, project_Id)
                VALUES
                    (@label, @description, @StartTime, @endtime, @date, @complexity, @priority, @taskcolor,
                     @interval, @duration, @durationunit, @isChecked, @isRepetitive, @repetitionDates, @weekdays, @userId, @projectId)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@label", task.Label);
                    cmd.Parameters.AddWithValue("@description", task.Description);
                    cmd.Parameters.AddWithValue("@StartTime", task.StartTime);
                    cmd.Parameters.AddWithValue("@endtime", task.EndTime);
                    cmd.Parameters.AddWithValue("@date", task.Date);
                    cmd.Parameters.AddWithValue("@complexity", task.Complexity);
                    cmd.Parameters.AddWithValue("@priority", task.Priority);
                    cmd.Parameters.AddWithValue("@taskcolor", task.TaskColor);

                    cmd.Parameters.AddWithValue("@interval", task.Interval.HasValue ? (object)task.Interval.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@duration", task.Duration.HasValue ? (object)task.Duration.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@durationunit", task.DurationUnit.HasValue ? (object)(int)task.DurationUnit.Value : DBNull.Value);
                    //cmd.Parameters.AddWithValue("@interval", task.Interval);
                    //cmd.Parameters.AddWithValue("@intervalunit", task.IntervalUnit);
                    //cmd.Parameters.AddWithValue("@duration", task.Duration);
                    //cmd.Parameters.AddWithValue("@durationunit", task.DurationUnit);
                    cmd.Parameters.AddWithValue("@isChecked", task.IsChecked);

                    cmd.Parameters.AddWithValue("@isRepetitive", task.IsRepetitive);
                    // Serialize the Dates array to JSON.
                    cmd.Parameters.AddWithValue("@repetitionDates", task.RepetitionDates != null ? JsonSerializer.Serialize(task.RepetitionDates) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@weekdays", task.Weekdays != null ? JsonSerializer.Serialize(task.Weekdays) : (object)DBNull.Value);

                    cmd.Parameters.AddWithValue("@userId", task.UserId);
                    cmd.Parameters.AddWithValue("@projectId", (object?)task.ProjectId ?? DBNull.Value);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<TaskModel?> GetTaskById(int id)
        {
            string query = "select * from tasks_table where id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Deserialize the dates JSON string to a DateTime array.
                            string? repetitionDatesJson = reader.IsDBNull(reader.GetOrdinal("repetitionDates"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("repetitionDates"));
                            DateTime[] datesArray = string.IsNullOrEmpty(repetitionDatesJson)
                                ? Array.Empty<DateTime>()
                                : JsonSerializer.Deserialize<DateTime[]>(repetitionDatesJson)!;

                            string? weekdaysJson = reader.IsDBNull(reader.GetOrdinal("weekdays"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("weekdays"));
                            string[] weekdaysArray = !string.IsNullOrEmpty(weekdaysJson)
                                ? JsonSerializer.Deserialize<string[]>(weekdaysJson)!
                                : Array.Empty<string>();

                            return new TaskModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                StartTime = reader.GetInt64(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetInt64(reader.GetOrdinal("EndTime")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Complexity = (Complexity)reader.GetInt32(reader.GetOrdinal("Complexity")),
                                Priority = (Priority)reader.GetInt32(reader.GetOrdinal("Priority")),
                                TaskColor = reader.GetString(reader.GetOrdinal("TaskColor")),
                                Interval = reader.IsDBNull(reader.GetOrdinal("Interval")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Interval")),
                                //IntervalUnit = reader.IsDBNull(reader.GetOrdinal("IntervalUnit")) ? (TimeUnit?)null : (TimeUnit)reader.GetInt32(reader.GetOrdinal("IntervalUnit")),
                                Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Duration")),
                                IsChecked = reader.GetBoolean(reader.GetOrdinal("IsChecked")),
                                DurationUnit = reader.IsDBNull(reader.GetOrdinal("DurationUnit")) ? (TimeUnit?)null : (TimeUnit)reader.GetInt32(reader.GetOrdinal("DurationUnit")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                IsRepetitive = reader.GetBoolean(reader.GetOrdinal("isRepetitive")),
                                RepetitionDates = datesArray,
                                Weekdays = weekdaysArray,
                                ProjectId = reader.IsDBNull(reader.GetOrdinal("project_Id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("project_Id")),
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<TaskModel>> GetEachDayTasksByUserId(int userId, DateTime selectedDate)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            string query = @"
                SELECT * FROM tasks_table 
                WHERE userId = @userId
                  AND (
                      date = @selectedDate
                      OR (
                          isRepetitive = 1
                          AND (
                              -- interval-based repetition stored in repetitionDates JSON
                              EXISTS (
                                  SELECT 1
                                  FROM OPENJSON(repetitionDates) WITH (dateValue datetime '$')
                                  WHERE dateValue = @selectedDate
                              )
                              -- weekday-based repetition stored in weekdays JSON
                              OR EXISTS (
                                  SELECT 1
                                  FROM OPENJSON(weekdays) WITH (weekday nvarchar(20) '$')
                                  WHERE weekday = @weekDayName
                              )
                          )
                      )
                  )";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@selectedDate", selectedDate);
                    cmd.Parameters.AddWithValue("@weekDayName", selectedDate.DayOfWeek.ToString());

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Deserialize repetitionDates
                            string? repetitionDatesJson = reader.IsDBNull(reader.GetOrdinal("repetitionDates"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("repetitionDates"));
                            DateTime[] repetitionDatesArray = !string.IsNullOrEmpty(repetitionDatesJson)
                                ? JsonSerializer.Deserialize<DateTime[]>(repetitionDatesJson)!
                                : Array.Empty<DateTime>();

                            // Deserialize weekdays
                            string? wdJson = reader.IsDBNull(reader.GetOrdinal("weekdays"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("weekdays"));
                            string[] wds = !string.IsNullOrEmpty(wdJson)
                                ? JsonSerializer.Deserialize<string[]>(wdJson)!
                                : Array.Empty<string>();

                            tasks.Add(new TaskModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                StartTime = reader.GetInt64(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetInt64(reader.GetOrdinal("EndTime")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Complexity = (Complexity)reader.GetInt32(reader.GetOrdinal("Complexity")),
                                Priority = (Priority)reader.GetInt32(reader.GetOrdinal("Priority")),
                                TaskColor = reader.GetString(reader.GetOrdinal("TaskColor")),
                                Interval = reader.IsDBNull(reader.GetOrdinal("Interval")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Interval")),
                                Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Duration")),
                                DurationUnit = reader.IsDBNull(reader.GetOrdinal("DurationUnit")) ? (TimeUnit?)null : (TimeUnit)reader.GetInt32(reader.GetOrdinal("DurationUnit")),
                                IsChecked = reader.GetBoolean(reader.GetOrdinal("IsChecked")),
                                IsRepetitive = reader.GetBoolean(reader.GetOrdinal("isRepetitive")),
                                RepetitionDates = repetitionDatesArray,
                                Weekdays = wds,
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                ProjectId = reader.IsDBNull(reader.GetOrdinal("project_Id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("project_Id")),
                            });
                        }
                    }
                }
            }
            return tasks;
        }

        public async Task<List<TaskModel>> GetUserTasks(int userId)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            string query = @"SELECT * FROM tasks_table WHERE userid = @userId ";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    //cmd.Parameters.AddWithValue("@selectedDate", selectedDate);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string? repetitionDatesJson = reader.IsDBNull(reader.GetOrdinal("repetitionDates"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("repetitionDates"));
                            DateTime[] repetitionDatesArray = string.IsNullOrEmpty(repetitionDatesJson)
                                ? Array.Empty<DateTime>()
                                : JsonSerializer.Deserialize<DateTime[]>(repetitionDatesJson)!;


                            string? wdJson = reader.IsDBNull(reader.GetOrdinal("weekdays")) ? null : reader.GetString(reader.GetOrdinal("weekdays"));
                            string[] wds = !string.IsNullOrEmpty(wdJson) ? JsonSerializer.Deserialize<string[]>(wdJson)! : Array.Empty<string>();
                            tasks.Add(new TaskModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                StartTime = reader.GetInt64(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetInt64(reader.GetOrdinal("EndTime")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Complexity = (Complexity)reader.GetInt32(reader.GetOrdinal("Complexity")),
                                Priority = (Priority)reader.GetInt32(reader.GetOrdinal("Priority")),
                                TaskColor = reader.GetString(reader.GetOrdinal("TaskColor")),
                                Interval = reader.IsDBNull(reader.GetOrdinal("Interval")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Interval")),
                                Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Duration")),
                                IsChecked = reader.GetBoolean(reader.GetOrdinal("IsChecked")),
                                DurationUnit = reader.IsDBNull(reader.GetOrdinal("DurationUnit")) ? (TimeUnit?)null : (TimeUnit)reader.GetInt32(reader.GetOrdinal("DurationUnit")),
                                IsRepetitive = reader.GetBoolean(reader.GetOrdinal("isRepetitive")),
                                RepetitionDates = repetitionDatesArray,
                                Weekdays = wds,
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                ProjectId = reader.IsDBNull(reader.GetOrdinal("project_Id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("project_Id")),
                            });
                        }
                    }
                }
            }
            return tasks;
        }


        //public async Task<List<TaskModel>> GetEachDayTasksByUserId(int userId, DateTime selectedDate)
        //{
        //    List<TaskModel> tasks = new List<TaskModel>();
        //    string query = @"
        //        SELECT * FROM tasks_table 
        //        WHERE userid = @userId 
        //        AND (
        //            date = @selectedDate 
        //            OR (
        //                isRepetitive = 1 
        //                AND EXISTS (
        //                    SELECT 1 
        //                    FROM OPENJSON(repetitionDates) 
        //                    WITH (dateValue datetime '$') 
        //                    WHERE dateValue = @selectedDate
        //                )
        //            )
        //        )";

        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        await con.OpenAsync();
        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.Parameters.AddWithValue("@userId", userId);
        //            cmd.Parameters.AddWithValue("@selectedDate", selectedDate);
        //            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    string? repetitionDatesJson = reader.IsDBNull(reader.GetOrdinal("repetitionDates"))
        //                        ? null
        //                        : reader.GetString(reader.GetOrdinal("repetitionDates"));
        //                    DateTime[] repetitionDatesArray = string.IsNullOrEmpty(repetitionDatesJson)
        //                        ? Array.Empty<DateTime>()
        //                        : JsonSerializer.Deserialize<DateTime[]>(repetitionDatesJson)!;


        //                    string? wdJson = reader.IsDBNull(reader.GetOrdinal("weekdays")) ? null : reader.GetString(reader.GetOrdinal("weekdays"));
        //                    string[] wds = !string.IsNullOrEmpty(wdJson) ? JsonSerializer.Deserialize<string[]>(wdJson)! : Array.Empty<string>();
        //                    tasks.Add(new TaskModel
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        Label = reader.GetString(reader.GetOrdinal("Label")),
        //                        Description = reader.GetString(reader.GetOrdinal("Description")),
        //                        StartTime = reader.GetInt64(reader.GetOrdinal("StartTime")),
        //                        EndTime = reader.GetInt64(reader.GetOrdinal("EndTime")),
        //                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
        //                        Complexity = (Complexity)reader.GetInt32(reader.GetOrdinal("Complexity")),
        //                        Priority = (Priority)reader.GetInt32(reader.GetOrdinal("Priority")),
        //                        TaskColor = reader.GetString(reader.GetOrdinal("TaskColor")),
        //                        Interval = reader.IsDBNull(reader.GetOrdinal("Interval")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Interval")),
        //                        Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Duration")),
        //                        IsChecked = reader.GetBoolean(reader.GetOrdinal("IsChecked")),
        //                        DurationUnit = reader.IsDBNull(reader.GetOrdinal("DurationUnit")) ? (TimeUnit?)null : (TimeUnit)reader.GetInt32(reader.GetOrdinal("DurationUnit")),
        //                        IsRepetitive = reader.GetBoolean(reader.GetOrdinal("isRepetitive")),
        //                        RepetitionDates = repetitionDatesArray,
        //                        Weekdays = wds,
        //                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
        //                        ProjectId = reader.IsDBNull(reader.GetOrdinal("project_Id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("project_Id")),
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return tasks;
        //}

        public async Task<List<TaskModel>> GetTasksByProjectId(int projectId)
        {
            List<TaskModel> tasks = new List<TaskModel>();
            string query = "SELECT * FROM tasks_table WHERE project_id = @projectId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string? repetitionDatesJson = reader.IsDBNull(reader.GetOrdinal("repetitionDates")) ? null : reader.GetString(reader.GetOrdinal("repetitionDates"));
                            DateTime[] repetitionDatesArray = string.IsNullOrEmpty(repetitionDatesJson) ? Array.Empty<DateTime>() : JsonSerializer.Deserialize<DateTime[]>(repetitionDatesJson)!;

                            string? wdJson = reader.IsDBNull(reader.GetOrdinal("weekdays")) ? null : reader.GetString(reader.GetOrdinal("weekdays"));
                            string[] wds = !string.IsNullOrEmpty(wdJson) ? JsonSerializer.Deserialize<string[]>(wdJson)! : Array.Empty<string>();

                            tasks.Add(new TaskModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Label = reader.GetString(reader.GetOrdinal("Label")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                StartTime = reader.GetInt64(reader.GetOrdinal("StartTime")),
                                EndTime = reader.GetInt64(reader.GetOrdinal("EndTime")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Complexity = (Complexity)reader.GetInt32(reader.GetOrdinal("Complexity")),
                                Priority = (Priority)reader.GetInt32(reader.GetOrdinal("Priority")),
                                TaskColor = reader.GetString(reader.GetOrdinal("TaskColor")),
                                Interval = reader.IsDBNull(reader.GetOrdinal("Interval")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Interval")),
                                Duration = reader.IsDBNull(reader.GetOrdinal("Duration")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Duration")),
                                IsChecked = reader.GetBoolean(reader.GetOrdinal("IsChecked")),
                                DurationUnit = reader.IsDBNull(reader.GetOrdinal("DurationUnit")) ? (TimeUnit?)null : (TimeUnit)reader.GetInt32(reader.GetOrdinal("DurationUnit")),
                                IsRepetitive = reader.GetBoolean(reader.GetOrdinal("isRepetitive")),
                                RepetitionDates = repetitionDatesArray,
                                Weekdays = wds,
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                ProjectId = reader.IsDBNull(reader.GetOrdinal("project_Id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("project_Id")),
                            });
                        }
                    }
                }
            }
            return tasks;
        }

        public async Task CheckTask(bool isChecked, int taskId)
        {
            string query = "update tasks_table set ischecked = @isChecked where id = @taskId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("taskId", taskId);
                    cmd.Parameters.AddWithValue("isChecked", isChecked);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> DeleteTask(int id)
        {
            string query = "delete from tasks_table where id=@id";
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

        public async Task<bool> UpdateTask(TaskModel task)
        {
            string query = @"UPDATE tasks_table 
                     SET label = @label, 
                         description = @description, 
                         StartTime = @startTime, 
                         EndTime = @endtime, 
                         date = @date, 
                         complexity = @complexity, 
                         priority = @priority, 
                         taskcolor = @taskcolor, 
                         interval = @interval, 
                         duration = @duration, 
                         durationunit = @durationunit, 
                         isChecked = @isChecked, 
                         userId = @userId,
                         isRepetitive = @isRepetitive,
                         repetitionDates = @repetitionDates,
                         weekdays = @weekdays,
                         project_Id = @projectId
                     WHERE id = @id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@label", task.Label);
                    cmd.Parameters.AddWithValue("@description", task.Description);
                    cmd.Parameters.AddWithValue("@startTime", task.StartTime);
                    cmd.Parameters.AddWithValue("@endtime", task.EndTime);
                    cmd.Parameters.AddWithValue("@date", task.Date);
                    cmd.Parameters.AddWithValue("@complexity", task.Complexity);
                    cmd.Parameters.AddWithValue("@priority", task.Priority);
                    cmd.Parameters.AddWithValue("@taskcolor", task.TaskColor);

                    //cmd.Parameters.AddWithValue("@interval", task.Interval);
                    ////cmd.Parameters.AddWithValue("@intervalunit", task.IntervalUnit);
                    //cmd.Parameters.AddWithValue("@duration", task.Duration);
                    //cmd.Parameters.AddWithValue("@durationunit", task.DurationUnit);

                    cmd.Parameters.AddWithValue("@interval", task.Interval.HasValue ? (object)task.Interval.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@duration", task.Duration.HasValue ? (object)task.Duration.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@durationunit", task.DurationUnit.HasValue ? (object)(int)task.DurationUnit.Value : DBNull.Value);

                    cmd.Parameters.AddWithValue("@isChecked", task.IsChecked);
                    cmd.Parameters.AddWithValue("@userId", task.UserId);
                    cmd.Parameters.AddWithValue("@id", task.Id);

                    cmd.Parameters.AddWithValue("@isRepetitive", task.IsRepetitive);
                    cmd.Parameters.AddWithValue("@repetitionDates", task.RepetitionDates != null ? JsonSerializer.Serialize(task.RepetitionDates) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@weekdays", task.Weekdays != null ? JsonSerializer.Serialize(task.Weekdays) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@projectId", (object?)task.ProjectId ?? DBNull.Value);

                    int result = await cmd.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
        }

    }
}
