using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service.QuizHistoryService
{
    public interface IQuizHistoryService
    {
        Task<QuizHistoryModel> AddQuizHistory(QuizHistoryModel quizHistory);
        Task<QuizHistoryModel> GetHistoryQuiz(int userID, int reportNb);
        Task<string> CompareReports(string prompt);
        Task<List<QuizHistoryModel>> GetQuizHistoryByUserID(int userId);
        Task<string> DeleteQuizHistory(int quizHistoryId);
    }
}
