using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;
using Repository.SpotifyPodcastRepository;

namespace Service.SpotifyPodcastService
{
    public class SpotifyPodcastService : ISpotifyPodcastService
    {
        private readonly ISpotifyPodcastRepository _repo;
        public SpotifyPodcastService(ISpotifyPodcastRepository repo)
        {
            _repo = repo;
        }

        public async Task<string> AddPodcast(SpotifyPodcastModel podcast)
        {
            await _repo.AddPodcast(podcast);
            return "Podcast added!";
        }

        public async Task<List<SpotifyPodcastModel>> GetPodcastsByCategoryId(int categoryId)
        {
            return await _repo.GetPodcastsByCategoryId(categoryId);
        }

        public async Task<string> UpdatePodcast(SpotifyPodcastModel podcast)
        {
            var ok = await _repo.UpdatePodcast(podcast);
            if (!ok) throw new Exception("Error updating podcast!");
            return "Podcast updated!";
        }

        public async Task<string> DeletePodcast(int podcastId)
        {
            var ok = await _repo.DeletePodcast(podcastId);
            if (!ok) throw new Exception("Error deleting podcast!");
            return "Podcast deleted!";
        }
    }
}
