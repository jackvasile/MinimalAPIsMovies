using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.Entities;
using System.Data;

namespace MinimalAPIsMovies.Repositories
{
   

    public class GenreRepository : IGenreRepository
    {
        private readonly string _configuration;

        public GenreRepository(IConfiguration configuration)
        {
            _configuration = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(_configuration))
            {
                //var query =@"INSERT INTO Genres (Name) VALUES (@Name);
                //            Select Scope_Identity()";                         
                //var id=await connection.QuerySingleAsync<int>(query, genre);
                //genre.Id=id;
                
                var id = await connection.QuerySingleAsync<int>("Genres_Create", new {genre.Name},
                    commandType: CommandType.StoredProcedure);
                genre.Id = id;
                return id;
            }
            
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(_configuration))
            {
                //var query = @"Delete From Genres Where Id=@Id";
                await connection.ExecuteAsync("Genre_Delete", new { id },
                    commandType: CommandType.StoredProcedure);
                
            }
        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(_configuration))
            {
                //var query = @"IF EXISTS (SELECT 1 FROM Genres WHERE Id=@Id) SELECT 1;
                //    ELSE SELECT 0;";
                var exists = await connection.QuerySingleAsync<bool>("Genre_Exists", new {id},
                    commandType: CommandType.StoredProcedure);
                return exists;
            }
        }
        public async Task<bool> Exists(int id, string name)
        {
            using (var connection = new SqlConnection(_configuration))
            {
               
                var exists = await connection.QuerySingleAsync<bool>("Genre_ExistsByIdAndName", new { id, name },
                    commandType: CommandType.StoredProcedure);
                return exists;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(_configuration))
            {
                //var query = @"Select Id, Name From Genres Order by Name";

                //var genres = await connection.QueryAsync<Genre>(query);
                var genres = await connection.QueryAsync<Genre>(@"Genres_GetAll", commandType: CommandType.StoredProcedure);
                
                return genres.ToList();
            }
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(_configuration))
            {
                //var query = @"Select Id, Name From Genres Where Id=@Id";
                //var genre = await connection.QueryFirstOrDefaultAsync<Genre>(query, new {id});
                var genre = await connection.QueryFirstOrDefaultAsync<Genre>("Genres_GetById", new { id },
                    commandType:CommandType.StoredProcedure);
                return genre;
            }
        }

        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(_configuration))
            {
                //var query = @"Update Genres
                //                Set Name=@Name
                //                    Where Id=@Id";
                 //await connection.ExecuteAsync("Genres_Update",genre);
                 //Better to pass parameters in case change genre properties then forget to update query
                await connection.ExecuteAsync("Genres_Update", new {genre.Id, genre.Name}, 
                    commandType: CommandType.StoredProcedure);


            }
        }
        public async Task<List<int>> Exists(List<int> ids)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var genreId in ids)
            {
                dt.Rows.Add(genreId);
            }

            using (var connection = new SqlConnection(_configuration))
            {
                var idsOfGenresThatExists = await connection
                    .QueryAsync<int>("Genres_GetBySeveralIds", new { genresIds = dt },
                    commandType: CommandType.StoredProcedure);

                return idsOfGenresThatExists.ToList();
            }
        }
    }
}
