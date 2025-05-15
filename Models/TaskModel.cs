using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.Enums;

namespace Model
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public DateTime Date { get; set; }
        public Complexity Complexity { get; set; }
        public Priority Priority { get; set; }
        public string TaskColor { get; set; } = string.Empty;
        public int? Interval { get; set; }
        public String[]? Weekdays { get; set; } = [];
        //public TimeUnit? IntervalUnit { get; set; }
        public int? Duration { get; set; }
        public TimeUnit? DurationUnit { get; set; }
        public bool IsChecked { get; set; }
        public bool IsRepetitive { get; set; }
        public DateTime[]? RepetitionDates { get; set; } = Array.Empty<DateTime>();
        public int? ProjectId { get; set; }
        public int UserId { get; set; }
    }
}
