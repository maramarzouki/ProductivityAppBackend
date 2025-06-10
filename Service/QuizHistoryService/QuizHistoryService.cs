using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Model;
using Repository.ProjectRepository;
using Repository.QuizHistoryRepository;
using Repository.UserRepository;

namespace Service.QuizHistoryService
{
    public class QuizHistoryService : IQuizHistoryService
    {
        private readonly IQuizHistoryRepository _quizHistoryRepository;
        private readonly string _apiKey;
        private readonly IUserRepository _userRepository;
        public QuizHistoryService(IQuizHistoryRepository quizHistoryRepository, IUserRepository userRepository, IConfiguration configuration)
        {
            _quizHistoryRepository = quizHistoryRepository;
            _apiKey = configuration.GetSection("Gemini")["ApiKey"]!;
            _userRepository = userRepository;
        }
        public async Task<QuizHistoryModel> AddQuizHistory(QuizHistoryModel quizHistory)
        {
            var user = _userRepository.GetUserById(quizHistory.UserId);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            //var newQuizHistory = new QuizHistoryModel
            //{
            //    Id = quizHistory.Id,
            //    QuestionsAnswersHistory = quizHistory.QuestionsAnswersHistory,
            //    Report = quizHistory.Report,
            //    ReportNb = quizHistory.ReportNb,
            //    QuizDate = quizHistory.QuizDate,
            //    UserId = quizHistory.UserId,
            //};
            var newQuizHistory = new QuizHistoryModel
            {
                QuestionsAnswersHistory = quizHistory.QuestionsAnswersHistory,
                Report = quizHistory.Report,
                ReportNb = quizHistory.ReportNb,
                QuizDate = quizHistory.QuizDate,
                UserId = quizHistory.UserId,
            };
            var res = await _quizHistoryRepository.AddQuizHistory(newQuizHistory);
            return res;
        }

        public async Task<QuizHistoryModel> GetHistoryQuiz(int userID, int reportNb)
        {
            var quizHistory = await _quizHistoryRepository.GetHistoryQuiz(userID, reportNb);
            if (quizHistory == null)
            {
                throw new KeyNotFoundException($"Quiz history with userId {userID} not found! Seems like this user hasn't passed a test yet!");
            }
            return quizHistory;
        }

        public async Task<string> CompareReports(string prompt)
        {
            using var http = new HttpClient();
            // Build the request URL with your key as a query parameter
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


            using var response = await http.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errBody = await response.Content.ReadAsStringAsync();
                return $"Error {(int)response.StatusCode}: {errBody}";
            }

            // 3. Return raw JSON response (you can parse out .candidates[0].output if you like)
            var resultJson = await response.Content.ReadAsStringAsync();

            // 2. Parse it and drill down to candidates[0].content.parts[0].text
            using var doc = JsonDocument.Parse(resultJson);
            var root = doc.RootElement;

            // (Adjust these indices/properties if your structure is slightly different)
            var text = root
              .GetProperty("candidates")[0]
              .GetProperty("content")
              .GetProperty("parts")[0]
              .GetProperty("text")
              .GetString();

            return text ?? "error";
            //return resultJson;
        }

        public async Task<List<QuizHistoryModel>> GetQuizHistoryByUserID(int userId)
        {
            var quizHistoryList = await _quizHistoryRepository.GetHistoryQuizByUserID(userId);
            return quizHistoryList;
        }

        public async Task<string> DeleteQuizHistory(int quizHistoryId)
        {
            bool deleted = await _quizHistoryRepository.DeleteQuizHistory(quizHistoryId);
            if (!deleted)
            {
                return "Quiz History not found!";
            }
            return "Quiz History deleted!";
        }
    }
}
