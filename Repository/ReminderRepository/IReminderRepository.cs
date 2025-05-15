using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.ReminderRepository
{
    public interface IReminderRepository
    {
        public Task CreateReminder(ReminderModel reminder);
        public Task<List<ReminderModel>> GetRemindersByUserId(int userId);
        public Task<bool> ToggleReminder(int id, int userId, bool IsActive);
        public Task<bool> DeleteReminder(int id);
    }
}
