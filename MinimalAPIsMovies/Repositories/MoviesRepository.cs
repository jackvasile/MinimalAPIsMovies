﻿using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using System.Data;

namespace MinimalAPIsMovies.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly string _connectionString;
        private readonly HttpContext _httpContext;

        public MoviesRepository(IConfiguration configuration, IHttpContextAccessor httpContextaccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _httpContext = httpContextaccessor.HttpContext!;
        }




        public async Task<int> Create(Movie movie)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>("Movies_Create",
                    new { movie.Title, movie.Poster, movie.ReleaseDate, movie.InTheaters },
                    commandType: CommandType.StoredProcedure);
                movie.Id = id;
                return id;
            }
        }
        public async Task<List<Movie>> GetAll(PaginationDTO paginationDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var movies = await connection.QueryAsync<Movie>("Movies_GetAll",
                    new { paginationDTO.Page, paginationDTO.RecordsPerPage },
                    commandType: CommandType.StoredProcedure);

                var moviesCount = await connection.QuerySingleAsync<int>("Movies_Count",
                commandType: CommandType.StoredProcedure);

                _httpContext.Response.Headers.Append("totalAmountOfRecords",
                    moviesCount.ToString());

                return movies.ToList();
            }
        }
        public async Task<Movie?> GetById(int id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi=await connection.QueryMultipleAsync("Movies_GetById",
                    new { id }))
                {
                    var movie =await multi.ReadFirstAsync<Movie>();
                    var comments =await multi.ReadAsync<Comment>();
                    var genres=await multi.ReadAsync<Genre>();  
                    var actors=await multi.ReadAsync<ActorMovieDTO>();

                    movie.Comments = comments.ToList();

                    foreach (var genre in genres)
                    {
                        movie.GenresMovies.Add(new GenreMovie
                        {
                            GenreId = genre.Id,
                            Genre = genre
                        });
                    }

                    foreach (var actor in actors)
                    {
                        movie.ActorsMovies.Add(new ActorMovie
                        {
                            ActorId = actor.Id,
                            Character = actor.Character,
                            Actor = new Actor { Name = actor.Name }
                        });
                    }
                    return movie;
                }
               
            }
        }
        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>("Movies_Exists",
                    new { id }, commandType: CommandType.StoredProcedure);
                return exists;
            }
        }

        public async Task Update(Movie movie)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("Movies_Update", new
                {
                    movie.Id,
                    movie.Title,
                    movie.InTheaters,
                    movie.Poster,
                    movie.ReleaseDate
                },
                 commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("Movies_Delete", new { id });
            }
        }

        public async Task Assign(int id, List<int> genresIds)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var genreId in genresIds)
            {
                dt.Rows.Add(genreId);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("Movies_AssignGenres",
                    new { movieId = id, genresIds = dt });
            }
        }
        public async Task Assign(int id, List<ActorMovie> actors)
        {
            for (int i = 1; i <= actors.Count; i++)
            {
                actors[i - 1].Order = i;
            }

            var dt = new DataTable();
            dt.Columns.Add("ActorId", typeof(int));
            dt.Columns.Add("Character", typeof(string));
            dt.Columns.Add("Order", typeof(int));

            foreach (var actorMovie in actors)
            {
                dt.Rows.Add(actorMovie.ActorId, actorMovie.Character, actorMovie.Order);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync("Movies_AssignActors",
                    new { movieId = id, actors = dt });
            }
        }
    }
}
