using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.CategoryModels
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<YoutubeVideoModel> YoutubeVideoModel { get; set; } = new List<YoutubeVideoModel>();

        public List<SpotifyPodcastModel> SpotifyPodcastModel { get; set; } = new List<SpotifyPodcastModel>();
    }
}
