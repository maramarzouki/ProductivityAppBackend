using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class QuizHistoryModel
    {
        public int Id { get; set; }
        public string QuestionsAnswersHistory {  get; set; } = string.Empty;
        public string Report {  get; set; } = string.Empty;
        public int ReportNb { get; set; }
        public DateTime QuizDate { get; set; }
        public int UserId { get; set; }

    }
}
