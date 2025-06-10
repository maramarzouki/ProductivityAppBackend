using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.ReminderRepository;
using Repository.UserRepository;

namespace Service.ReminderService
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IUserRepository _userRepository;
        public ReminderService(IReminderRepository reminderRepository, IUserRepository userRepository)
        {
            _reminderRepository = reminderRepository;
            _userRepository = userRepository;
        }
        public async Task<ReminderModel> CreateReminder(ReminderModel reminder)
        {
            var user = _userRepository.GetUserById(reminder.UserId);
            if (user == null)
            {
                throw new Exception("User not found!");
            }
            var newReminder = new ReminderModel
            {
                Id = reminder.Id,
                Title = reminder.Title,
                IsActive = reminder.IsActive,
                //ReminderDate = reminder.ReminderDate,
                ReminderTime = reminder.ReminderTime,
                UserId = reminder.UserId
            };
            await _reminderRepository.CreateReminder(newReminder);
            return newReminder;
        }

        public async Task<string> DeleteReminder(int id)
        {
            bool deleted = await _reminderRepository.DeleteReminder(id);
            if (!deleted)
            {
                return "Reminder not found!";
            }
            return "Reminder deleted!";
        }

        public async Task<List<ReminderModel>> GetRemindersByUserId(int userId)
        {
            var reminders = await _reminderRepository.GetRemindersByUserId(userId);
            return reminders;
        }

        public async Task<string> ToggleReminder(int id, int userId, bool IsActive)
        {
            var res = await _reminderRepository.ToggleReminder(id, userId, IsActive);
            if(res){
                return "Reminder toggled";
            }
            else
            {
                throw new Exception("toggle error!");
            }
        }
    }
}
