using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GeminiModels
{
    public class GeminiRequest
    {
        public List<HistoryItem> History { get; set; } = [];
        public int NextQuestionNumber { get; set; }
        public List<string> Categories { get; set; } = [];
    }
}
