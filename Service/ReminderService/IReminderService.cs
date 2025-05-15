using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service.ReminderService
{
    public interface IReminderService
    {
        Task<ReminderModel> CreateReminder(ReminderModel reminder);
        Task<List<ReminderModel>> GetRemindersByUserId(int userId);
        Task<string> ToggleReminder(int id, int userId, bool IsActive);
        Task<string> DeleteReminder(int id);
    }
}
