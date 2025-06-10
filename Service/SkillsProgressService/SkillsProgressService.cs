using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.SkillsProgressRepository;
using Repository.UserRepository;
using Service.UserService;

namespace Service.SkillsProgressService
{
    public class SkillsProgressService : ISkillsProgressService
    {
        private readonly ISkillsProgressRepository _skillsProgressRepository;
        private readonly IUserRepository _userRepository;
        public SkillsProgressService(ISkillsProgressRepository skillsProgressRepository, IUserRepository userRepository)
        {
            _skillsProgressRepository = skillsProgressRepository;
            _userRepository = userRepository;
        }

        public async Task<SkillsProgressModel> AddSkillsProgress(SkillsProgressModel skillsProgress)
        {
            var user = _userRepository.GetUserById(skillsProgress.UserId);
            if(user == null)
            {
                throw new Exception("User not found!");
            }
            //var newSkillsProgress = new SkillsProgressModel
            //{
            //    Id = skillsProgress.Id,
            //    TestDate = skillsProgress.TestDate,
            //    OldProgress = skillsProgress.OldProgress,
            //    NewProgress = skillsProgress.NewProgress,
            //    CategoryName = skillsProgress.CategoryName,
            //    UserId = skillsProgress.UserId,
            //};
            var newSkillsProgress = new SkillsProgressModel
            {
                TestDate = skillsProgress.TestDate,
                OldProgress = skillsProgress.OldProgress,
                NewProgress = skillsProgress.NewProgress,
                CategoryName = skillsProgress.CategoryName,
                UserId = skillsProgress.UserId,
            };
            var res = await _skillsProgressRepository.AddSkillsProgress(newSkillsProgress);
            return res;
            //return "Skills Progress created successfully!";
        }

        public async Task<SkillsProgressModel?> GetSkillsProgress(int userID)
        {
            var skillsProgress = await _skillsProgressRepository.GetSkillsProgress(userID);
            if (skillsProgress == null)
            {
                throw new KeyNotFoundException($"Skills progress with userId {userID} not found! Seems like this user hasn't passed a test yet!");
            }
            return skillsProgress;
        }

        public async Task<List<SkillsProgressModel>> GetSkillsProgressByUserID(int userId)
        {
            var skillsProgressList = await _skillsProgressRepository.GetSkillsProgressByUserID(userId);
            return skillsProgressList;
        }

        public async Task<string> UpdateSkillsProgress(SkillsProgressModel skillsProgress)
        {
            var result = await _skillsProgressRepository.UpdateSkillsProgress(skillsProgress);
            if (!result)
            {
                throw new Exception("Error updating the progress!");
            }
            return "User skills progress updated!";
        }

        public async Task<string> DeleteSkillsProgress(int skillsProgressId)
        {
            bool deleted = await _skillsProgressRepository.DeleteSkillsProgress(skillsProgressId);
            if (!deleted)
            {
                return "Skills Progress not found!";
            }
            return "Skills Progress deleted!";
        }
    }
}
