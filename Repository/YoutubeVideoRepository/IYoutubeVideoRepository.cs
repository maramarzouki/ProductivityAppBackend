using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;

namespace Repository.YoutubeVideoRepository
{
    public interface IYoutubeVideoRepository
    {
        Task AddVideo(YoutubeVideoModel video);
        Task<List<YoutubeVideoModel>> GetVideosByCategoryId(int categoryId);
        Task<bool> UpdateVideo(YoutubeVideoModel video);
        Task<bool> DeleteVideo(int videoId);
    }
}
