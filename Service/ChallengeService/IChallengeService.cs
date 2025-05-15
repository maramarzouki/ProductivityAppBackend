using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Service.ChallengeService
{
    public interface IChallengeService
    {
        Task<string> CreateChallenge(ChallengeModel challenge);
        Task<ChallengeModel?> GetChallengeById(int challengeId);
        Task<List<ChallengeModel>> GetUserChallenges(int userId);
        Task<string> StartChallenge(int challengeId);
        Task<string> RestartChallenge(int challengeId, DateTime statDate);
        Task<string> CancelChallenge(int challengeId);
        Task<string> CheckChallengeStatus(int challengeId);
        Task<string> ConfirmToday(int challengeId);
        Task<string> UpdateChallenge(ChallengeModel challenge);
        Task<string> DeleteChallenge(int challengeId);
    }
}
