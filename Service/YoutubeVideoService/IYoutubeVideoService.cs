using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.CategoryModels;

namespace Service.YoutubeVideoService
{
    public interface IYoutubeVideoService
    {
        Task<string> AddVideo(YoutubeVideoModel video);
        Task<List<YoutubeVideoModel>> GetVideosByCategoryId(int categoryId);
        Task<string> UpdateVideo(YoutubeVideoModel video);
        Task<string> DeleteVideo(int videoId);
    }
}
