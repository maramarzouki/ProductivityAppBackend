using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.ChallengeRepositoy;

namespace Service.ChallengeService
{
    public class ChallengeService : IChallengeService
    {
        private readonly IChallengeRepository _challengeRepository;
        public ChallengeService(IChallengeRepository challengeRepository)
        {
            _challengeRepository = challengeRepository;
        }

        public async Task<string> CreateChallenge(ChallengeModel challenge)
        {
            var newChallenge = new ChallengeModel
            {
                Id = challenge.Id,
                Name = challenge.Name,
                Description = challenge.Description,
                StartDate = challenge.StartDate,
                CompletedDates = challenge.CompletedDates,
                TotalDays = challenge.TotalDays,
                IsCanceled = challenge.IsCanceled,
                IsStarted = challenge.IsStarted,
                IsCompleted = challenge.IsCompleted,
                UserId = challenge.UserId,
            };
            await _challengeRepository.CreateChallenge(newChallenge);
            return "Challenge created!";
        }

        public async Task<ChallengeModel?> GetChallengeById(int challengeId)
        {
            var challenge = await _challengeRepository.GetChallengeById(challengeId);
            if (challenge == null)
            {
                throw new KeyNotFoundException($"Challenge with ID {challengeId} not found.");
            }
            return challenge;
        }

        public async Task<List<ChallengeModel>> GetUserChallenges(int userId)
        {
            var userChallenges = await _challengeRepository.GetUserChallenges(userId);
            return userChallenges;
        }

        public async Task<string> StartChallenge(int challengeId)
        {
            var res = await _challengeRepository.StartChallenge(challengeId);
            if (!res)
            {
                throw new Exception("Error Starting the challenge!");
            }
            return "Challenge Started!";
        }

        public async Task<string> RestartChallenge(int challengeId, DateTime startTime)
        {
            var res = await _challengeRepository.RestartChallenge(challengeId, startTime);
            if (!res)
            {
                throw new Exception("Error restarting the challenge!");
            }
            return "Challenge restarted!";
        }

        public async Task<string> CancelChallenge(int challengeId)
        {
            var res = await _challengeRepository.CancelChallenge(challengeId);
            if (!res)
            {
                throw new Exception("Error cancelling the challenge!");
            }
            return "Challenge cancelled!";
        }

        public async Task<string> CheckChallengeStatus(int challengeId)
        {
            var challenge = await _challengeRepository.GetChallengeById(challengeId)
                  ?? throw new Exception("Challenge not found!");
            var today = DateTime.Today;
            var elapsed = (today - challenge.StartDate).Days + 1;
            var expected = Math.Min(elapsed, challenge.TotalDays);

            var doneCount = challenge.CompletedDates
                                  .Where(d => d.Date <= today)
                                  .Select(d => d.Date)
                                  .Distinct()
                                  .Count();

            if (doneCount < expected)
            {
                // MISS → restart from today
                await _challengeRepository.RestartChallenge(challengeId, today);
            }
            else if (doneCount >= challenge.TotalDays)
            {
                challenge.IsCompleted = true;
                await _challengeRepository.UpdateChallenge(challenge);
            }

            return "checked!";
        }

        public async Task<string> ConfirmToday(int challengeId)
        {
            var challenge = await _challengeRepository.GetChallengeById(challengeId) ?? throw new Exception("Challenge not found!");
            var today = DateTime.Today;
            if (!challenge.CompletedDates.Any(d => d.Date == today))
            {
                challenge.CompletedDates.Add(today);
                await _challengeRepository.UpdateChallenge(challenge);
                return "Confirmed!";
            }
            return "Something is wrong!";
        }

        public async Task<string> UpdateChallenge(ChallengeModel challenge)
        {
            var isUpdated = await _challengeRepository.UpdateChallenge(challenge);
            if (!isUpdated)
            {
                throw new Exception("Error updating the challenge!");
            }
            return "Challenge updated!";
        }

        public async Task<string> DeleteChallenge(int challengeId)
        {
            bool isDeleted = await _challengeRepository.DeleteChallenge(challengeId);
            if (!isDeleted)
            {
                throw new Exception("Error deleting the challenge!");
            }
            return "Challenge deleted!";
        }
    }
}
