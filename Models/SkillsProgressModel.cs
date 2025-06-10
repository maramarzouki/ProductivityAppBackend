using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SkillsProgressModel
    {
        public int Id { get; set; }
        public DateTime TestDate { get; set; }
        public double OldProgress { get; set; }
        public double NewProgress { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
