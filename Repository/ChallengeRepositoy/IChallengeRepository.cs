using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository.ChallengeRepositoy
{
    public interface IChallengeRepository
    {
        public Task CreateChallenge(ChallengeModel challenge);
        public Task<ChallengeModel?> GetChallengeById(int challengeId);
        public Task<List<ChallengeModel>> GetUserChallenges(int userId);
        Task<List<ChallengeModel>> GetAllChallenges();
        public Task<bool> StartChallenge(int challengeId);
        public Task<bool> RestartChallenge(int challengeId, DateTime newStartDate);
        public Task<bool> CancelChallenge(int challengeId);
        public Task<bool> UpdateChallenge(ChallengeModel challenge);
        public Task<bool> DeleteChallenge(int challengeId);
    }
}
