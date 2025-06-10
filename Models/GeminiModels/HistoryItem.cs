using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GeminiModels
{
    public class HistoryItem
    {
        public string Question { get; set; } = string.Empty;
        public Dictionary<string, string> Options { get; set; } = [];
        public string UserChoice { get; set; } = string.Empty;
    }
}
