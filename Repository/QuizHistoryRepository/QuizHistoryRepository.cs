using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model;

namespace Repository.QuizHistoryRepository
{
    public class QuizHistoryRepository : IQuizHistoryRepository
    {
        private readonly string _connectionString;
        public QuizHistoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private async Task<int> GetNextReportNb(int userId, SqlConnection con, SqlTransaction tx)
        {
            using var cmd = new SqlCommand(
                @"SELECT ISNULL(MAX(reportNb), 0) + 1 
              FROM quiz_history_table 
              WHERE userId = @userId;",
                con, tx);
            cmd.Parameters.AddWithValue("@userId", userId);
            return (int)await cmd.ExecuteScalarAsync()!;
        }

        public async Task<QuizHistoryModel> AddQuizHistory(QuizHistoryModel quizHistory)
        {
            const string insertSql = @"
            INSERT INTO quiz_history_table
              (history, report, reportNb, date, userId)
              OUTPUT INSERTED.Id
            VALUES
              (@history, @report, @reportNb, @date, @userId);";

            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            // start a transaction to avoid races if two inserts happen at once
            await using var tx = con.BeginTransaction();
            try
            {
                // 1) compute next report number for this user
                var nextNb = await GetNextReportNb(quizHistory.UserId, con, tx);

                // 2) perform the insert using that next number
                await using (var cmd = new SqlCommand(insertSql, con, tx))
                {
                    cmd.Parameters.AddWithValue("@history", quizHistory.QuestionsAnswersHistory);
                    cmd.Parameters.AddWithValue("@report", quizHistory.Report);
                    cmd.Parameters.AddWithValue("@reportNb", nextNb);
                    cmd.Parameters.AddWithValue("@date", quizHistory.QuizDate);
                    cmd.Parameters.AddWithValue("@userId", quizHistory.UserId);

                    //await cmd.ExecuteNonQueryAsync();
                    var newIdObj = await cmd.ExecuteScalarAsync();
                    quizHistory.Id = Convert.ToInt32(newIdObj);
                    quizHistory.ReportNb = nextNb;
                }

                await tx.CommitAsync();
                return quizHistory;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        //public async Task AddQuizHistory(QuizHistoryModel quizHistory)
        //{
        //    string query = "insert into quiz_history_table(questionsAnswersHistory, report, reportNb, quizDate, userId) values(@questionsAnswersHistory, @report, @reportNb, @quizDate, @userId)";
        //    using (SqlConnection con = new SqlConnection(_connectionString))
        //    {
        //        await con.OpenAsync();
        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            cmd.Parameters.AddWithValue("@questionsAnswersHistory", quizHistory.QuestionsAnswersHistory);
        //            cmd.Parameters.AddWithValue("@report", quizHistory.Report);
        //            cmd.Parameters.AddWithValue("@reportNb", quizHistory.ReportNb);
        //            cmd.Parameters.AddWithValue("@quizDate", quizHistory.QuizDate);
        //            cmd.Parameters.AddWithValue("@userId", quizHistory.UserId);
        //            await cmd.ExecuteNonQueryAsync();
        //        }
        //    }
        //}

        public async Task<QuizHistoryModel?> GetHistoryQuiz(int userID, int reportNb)
        {
            string query = "select * from quiz_history_table where userId = @userID AND reportNb = @reportNb";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@reportNb", reportNb);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new QuizHistoryModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                QuestionsAnswersHistory = reader.GetString(reader.GetOrdinal("History")),
                                Report = reader.GetString(reader.GetOrdinal("Report")),
                                ReportNb = reader.GetInt32(reader.GetOrdinal("ReportNb")),
                                QuizDate = reader.GetDateTime(reader.GetOrdinal("Date")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<QuizHistoryModel>> GetHistoryQuizByUserID(int userId)
        {
            List<QuizHistoryModel> historyQuizList = [];
            string query = "SELECT * FROM quiz_history_table WHERE userId = @userId";

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
                            historyQuizList.Add(new QuizHistoryModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                QuestionsAnswersHistory = reader.GetString(reader.GetOrdinal("History")),
                                Report = reader.GetString(reader.GetOrdinal("Report")),
                                ReportNb = reader.GetInt32(reader.GetOrdinal("ReportNb")),
                                QuizDate = reader.GetDateTime(reader.GetOrdinal("Date")),
                                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            });
                        }
                    }
                }
            }
            return historyQuizList;
        }

        public async Task<bool> DeleteQuizHistory(int quizHistoryId)
        {
            string query = "delete from quiz_history_table where id=@id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", quizHistoryId);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
    }
}
