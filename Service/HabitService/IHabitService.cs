using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.HabitRepository;

namespace Service.HabitService
{
    public interface IHabitService
    {
        Task<string> AddHabit(HabitModel habit);
        Task<List<HabitModel>> GetHabitsByChallengeID(int challengeId);
        Task<string> CheckHabit(bool isChecked, int habitId);
        Task<string> UpdateHabit(HabitModel habit);
        Task<string> DeleteHabit(int habitId);
    }
}
