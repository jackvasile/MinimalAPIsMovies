using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.Entities;
using System.Data;

namespace MinimalAPIsMovies.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly string _connectionString;
        private readonly HttpContext _httpContext;

        public CommentsRepository(IConfiguration configuration, IHttpContextAccessor httpContextaccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _httpContext = httpContextaccessor.HttpContext!;
        }
        public async Task<int> Create(Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>("Comments_Create",
                    new { comment.Body, comment.MovieId },
                    commandType: CommandType.StoredProcedure);

                comment.Id = id;
                return id;
            }
        }
        public async Task<List<Comment>> GetAll(int movieId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var comments = await connection.QueryAsync<Comment>("Comments_GetAllByMovieId",
                    new { movieId }, commandType: CommandType.StoredProcedure);
                return comments.ToList();
            }
        }
        public async Task<Comment?> GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var comment = await connection.QueryFirstOrDefaultAsync<Comment>(
                    "Comments_GetById", new { id }, commandType: CommandType.StoredProcedure);
                return comment;
            }
        }
        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>("Comments_Exists",
                    new { id }, commandType: CommandType.StoredProcedure);

                return exists;
            }
        }
        public async Task Update(Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("Comments_Update", new
                {
                    comment.Body,
                    comment.Id,
                    comment.MovieId
                },
                    commandType: CommandType.StoredProcedure);
            }
        }
        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("Comments_Delete", new { id },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
