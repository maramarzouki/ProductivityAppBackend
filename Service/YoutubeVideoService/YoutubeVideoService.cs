using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;
using Repository.YoutubeVideoRepository;

namespace Service.YoutubeVideoService
{
    public class YoutubeVideoService : IYoutubeVideoService
    {
        private readonly IYoutubeVideoRepository _repo;
        public YoutubeVideoService(IYoutubeVideoRepository repo)
        {
            _repo = repo;
        }

        public async Task<string> AddVideo(YoutubeVideoModel video)
        {
            await _repo.AddVideo(video);
            return "Video added!";
        }

        public async Task<List<YoutubeVideoModel>> GetVideosByCategoryId(int categoryId)
        {
            return await _repo.GetVideosByCategoryId(categoryId);
        }

        public async Task<string> UpdateVideo(YoutubeVideoModel video)
        {
            var ok = await _repo.UpdateVideo(video);
            if (!ok) throw new Exception("Error updating video!");
            return "Video updated!";
        }

        public async Task<string> DeleteVideo(int videoId)
        {
            var ok = await _repo.DeleteVideo(videoId);
            if (!ok) throw new Exception("Error deleting video!");
            return "Video deleted!";
        }
    }
}
