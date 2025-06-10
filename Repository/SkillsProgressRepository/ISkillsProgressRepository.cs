using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.SkillsProgressRepository
{
    public interface ISkillsProgressRepository
    {
        Task<SkillsProgressModel> AddSkillsProgress(SkillsProgressModel skillsProgress);
        Task<SkillsProgressModel?> GetSkillsProgress(int userID);
        Task<bool> UpdateSkillsProgress(SkillsProgressModel skillsProgress);
        Task<List<SkillsProgressModel>> GetSkillsProgressByUserID(int userID);
        public Task<bool> DeleteSkillsProgress(int skillsProgressId);
    }
}
