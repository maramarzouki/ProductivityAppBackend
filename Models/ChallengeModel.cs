  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.Enums;

namespace Model
{
    public class ChallengeModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public int TotalDays { get; set; } // 21, 40 or 75
        public List<DateTime> CompletedDates { get; set; } = [];
        public bool IsCanceled { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }

        //public int Id { get; set; }
        //public string Name { get; set; } = string.Empty;
        //public string? Description { get; set; } = string.Empty;
        //public int ChallengeTotalDays { get; set; } // 21, 40 or 75 it depends
        //public int DaysCompletedNb { get; set; }
        //public DateTime[] ChallengeDays { get; set; } = Array.Empty<DateTime>();
        //public bool IsCanceled { get; set; }
        //public bool IsCompleted { get; set; }
        //public int UserId { get; set; }
    }
}
