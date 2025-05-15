using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.HabitRepository;

namespace Service.HabitService
{
    public class HabitService : IHabitService
    {
        private readonly IHabitRepository _habitRepository;
        public HabitService(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }

        public async Task<string> AddHabit(HabitModel habit)
        {
            var newHabit = new HabitModel
            {
                Id = habit.Id,
                Name = habit.Name,
                IsDone = habit.IsDone,
                ChallengeId = habit.ChallengeId,
            };
            await _habitRepository.AddHabit(newHabit);
            return "Habit created!";
        }

        public async Task<List<HabitModel>> GetHabitsByChallengeID(int challengeId)
        {
            var habits = await _habitRepository.GetHabitsByChallengeID(challengeId);
            return habits;
        }

        public async Task<string> CheckHabit(bool isChecked, int habitId)
        {
            var res = await _habitRepository.CheckHabit(isChecked, habitId);
            if (!res)
            {
                throw new Exception("Error checking the habit!");
            }
            return "Habit checked!";
        }

        public async Task<string> UpdateHabit(HabitModel habit)
        {
            var isUpdated = await _habitRepository.UpdateHabit(habit);
            if (!isUpdated)
            {
                throw new Exception("Error updating the habit!");
            }
            return "Habit updated!";
        }

        public async Task<string> DeleteHabit(int habitId)
        {
            bool isDeleted = await _habitRepository.DeleteHabit(habitId);
            if (!isDeleted)
            {
                throw new Exception("Error deleting the habit!");
            }
            return "Habit deleted!";
        }
    }
}
