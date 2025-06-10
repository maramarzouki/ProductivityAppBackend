using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.CategoryModels;
using static Repository.SpotifyPodcastRepository.SpotifyPodcastRepository;

namespace Repository.SpotifyPodcastRepository
{
    public class SpotifyPodcastRepository : ISpotifyPodcastRepository
    {
        
            private readonly string _connectionString;
            public SpotifyPodcastRepository(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            }

            public async Task AddPodcast(SpotifyPodcastModel podcast)
            {
                const string query = @"
                INSERT INTO spotify_podcasts_table (SpotifyEpisodeId, Title, Description, StreamUrl, ThumbnailUrl, PublishedAt, CategoryId)
                VALUES (@spId, @title, @desc, @stream, @thumb, @pub, @catId)";
                using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();
                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@spId", podcast.SpotifyEpisodeId);
                cmd.Parameters.AddWithValue("@title", podcast.Title);
                cmd.Parameters.AddWithValue("@desc", podcast.Description);
                cmd.Parameters.AddWithValue("@stream", "https://open.spotify.com/embed/"+podcast.SpotifyEpisodeId+"?utm_source=generator&theme=0");
                cmd.Parameters.AddWithValue("@thumb", podcast.ThumbnailUrl);
                cmd.Parameters.AddWithValue("@pub", podcast.PublishedAt);
                cmd.Parameters.AddWithValue("@catId", podcast.CategoryId);
                await cmd.ExecuteNonQueryAsync();
            }

            public async Task<List<SpotifyPodcastModel>> GetPodcastsByCategoryId(int categoryId)
            {
                var list = new List<SpotifyPodcastModel>();
                const string query = "SELECT * FROM spotify_podcasts_table WHERE CategoryId = @catId";
                using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();
                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@catId", categoryId);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new SpotifyPodcastModel
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        SpotifyEpisodeId = reader.GetString(reader.GetOrdinal("SpotifyEpisodeId")),
                        Title = reader.GetString(reader.GetOrdinal("Title")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        StreamUrl = reader.GetString(reader.GetOrdinal("StreamUrl")),
                        ThumbnailUrl = reader.GetString(reader.GetOrdinal("ThumbnailUrl")),
                        PublishedAt = reader.GetDateTime(reader.GetOrdinal("PublishedAt")),
                        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                    });
                }
                return list;
            }

            public async Task<bool> UpdatePodcast(SpotifyPodcastModel podcast)
            {
                const string query = @"
                UPDATE spotify_podcasts_table
                SET Title = @title, publishedAt = @pub
                WHERE Id = @id";
                using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();
                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@title", podcast.Title);
            cmd.Parameters.AddWithValue("@pub", podcast.PublishedAt);
            cmd.Parameters.AddWithValue("@id", podcast.Id);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }

            public async Task<bool> DeletePodcast(int podcastId)
            {
                const string query = "DELETE FROM spotify_podcasts_table WHERE Id = @id";
                using var con = new SqlConnection(_connectionString);
                await con.OpenAsync();
                using var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", podcastId);
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
}
