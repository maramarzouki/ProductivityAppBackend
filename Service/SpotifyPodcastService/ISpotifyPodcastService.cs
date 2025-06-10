using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;

namespace Service.SpotifyPodcastService
{
    public interface ISpotifyPodcastService
    {
        Task<string> AddPodcast(SpotifyPodcastModel podcast);
        Task<List<SpotifyPodcastModel>> GetPodcastsByCategoryId(int categoryId);
        Task<string> UpdatePodcast(SpotifyPodcastModel podcast);
        Task<string> DeletePodcast(int podcastId);
    }
}
