using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.QuizHistoryRepository
{
    public interface IQuizHistoryRepository
    {
        Task<QuizHistoryModel> AddQuizHistory(QuizHistoryModel quizHistory);
        Task<QuizHistoryModel?> GetHistoryQuiz(int userID, int reportNb);
        Task<List<QuizHistoryModel>> GetHistoryQuizByUserID(int userID);
        public Task<bool> DeleteQuizHistory(int quizHistory);
    }
}
