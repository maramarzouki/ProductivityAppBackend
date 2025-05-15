using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.HabitRepository
{
    public interface IHabitRepository
    {
        Task AddHabit(HabitModel habit);
        Task<List<HabitModel>> GetHabitsByChallengeID(int challengeId);
        Task<bool> CheckHabit(bool isChecked, int habitId);
        Task<bool> UpdateHabit(HabitModel habit);
        Task<bool> DeleteHabit(int habitId);
    }
}
