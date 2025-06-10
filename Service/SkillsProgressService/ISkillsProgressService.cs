using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service.SkillsProgressService
{
    public interface ISkillsProgressService
    {
        Task<SkillsProgressModel> AddSkillsProgress(SkillsProgressModel skillsProgress);
        Task<SkillsProgressModel?> GetSkillsProgress(int userID);
        Task<string> UpdateSkillsProgress(SkillsProgressModel skillsProgress);
        Task<List<SkillsProgressModel>> GetSkillsProgressByUserID(int userId);
        Task<string> DeleteSkillsProgress(int skillsProgressId);
    }
}
