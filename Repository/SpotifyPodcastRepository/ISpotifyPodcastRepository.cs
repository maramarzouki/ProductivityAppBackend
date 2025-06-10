using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;

namespace Repository.SpotifyPodcastRepository
{
    public interface ISpotifyPodcastRepository
    {
        Task AddPodcast(SpotifyPodcastModel podcast);
        Task<List<SpotifyPodcastModel>> GetPodcastsByCategoryId(int categoryId);
        Task<bool> UpdatePodcast(SpotifyPodcastModel podcast);
        Task<bool> DeletePodcast(int podcastId);
    }
}
