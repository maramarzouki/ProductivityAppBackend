using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ReminderModel
    {
        public int Id {  get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public String ReminderTime { get; set; } = string.Empty;
        //public long ReminderTime { get; set; }
        public int UserId { get; set; }
    }
}
