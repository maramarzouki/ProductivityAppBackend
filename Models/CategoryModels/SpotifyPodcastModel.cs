using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.CategoryModels
{
    public class SpotifyPodcastModel
    {
        public int Id { get; set; }
        public string SpotifyEpisodeId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StreamUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public int CategoryId { get; set; }
    }
}
