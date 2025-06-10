using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.CategoryModels;

namespace Repository.YoutubeVideoRepository
{
    public class YoutubeVideoRepository : IYoutubeVideoRepository
    {

        private readonly string _connectionString;
        public YoutubeVideoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task AddVideo(YoutubeVideoModel video)
        {
            const string query = @"
                INSERT INTO youtube_videos_table (YouTubeVideoId, Title, Description, EmbedUrl, ThumbnailUrl, PublishedAt, CategoryId)
                VALUES (@ytId, @title, @desc, @embed, @thumb, @pub, @catId)";
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@ytId", video.YouTubeVideoId);
            cmd.Parameters.AddWithValue("@title", video.Title);
            cmd.Parameters.AddWithValue("@desc", video.Description);
            cmd.Parameters.AddWithValue("@embed", "https://www.youtube.com/embed/"+@video.YouTubeVideoId);
            cmd.Parameters.AddWithValue("@thumb", "https://img.youtube.com/vi/"+video.YouTubeVideoId+"/hqdefault.jpg");
            cmd.Parameters.AddWithValue("@pub", video.PublishedAt);
            cmd.Parameters.AddWithValue("@catId", video.CategoryId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<YoutubeVideoModel>> GetVideosByCategoryId(int categoryId)
        {
            var list = new List<YoutubeVideoModel>();
            const string query = "SELECT * FROM youtube_videos_table WHERE CategoryId = @catId";
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@catId", categoryId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new YoutubeVideoModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    YouTubeVideoId = reader.GetString(reader.GetOrdinal("YouTubeVideoId")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    EmbedUrl = reader.GetString(reader.GetOrdinal("EmbedUrl")),
                    ThumbnailUrl = reader.GetString(reader.GetOrdinal("ThumbnailUrl")),
                    PublishedAt = reader.GetDateTime(reader.GetOrdinal("PublishedAt")),
                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                });
            }
            return list;
        }

        public async Task<bool> UpdateVideo(YoutubeVideoModel video)
        {
            const string query = @"
                UPDATE youtube_videos_table
                SET Title = @title, publishedAt = @pub
                WHERE Id = @id";
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@title", video.Title);
            cmd.Parameters.AddWithValue("@pub", video.PublishedAt);
            cmd.Parameters.AddWithValue("@id", video.Id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteVideo(int videoId)
        {
            const string query = "DELETE FROM youtube_videos_table WHERE Id = @id";
            using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();
            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", videoId);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}

