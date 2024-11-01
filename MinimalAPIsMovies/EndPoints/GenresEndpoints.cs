using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Entities;
using MinimalAPIsMovies.Filters;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.EndPoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {

            group.MapGet("/", GetGenres)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"))
                .RequireAuthorization();
            //Added filter to Filters folder
            //group.MapGet("/{id:int}", GetById).AddEndpointFilter(async (efiContext, next) =>
            //{
            //    //This is the code that will execute before the endpoint
            //    var result=await next(efiContext);

            //    //This is the code that will execute after the endpoint
            //    return result;
            //});
            //group.MapGet("/{id:int}", GetById).AddEndpointFilter<TestFilter>();
            //filter was just a test to see how it works by merely passing parameters; no filters for a Get request
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateGenreDTO>>();
            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateGenreDTO>>();
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenreRepository genreRepository, IMapper mapper)
        {
            var genres = await genreRepository.GetAll();
            //Using AutoMapper instead of manual mapping
            //var genresDTO = genres
            //    .Select(g => new GenreDTO { Id = g.Id, Name = g.Name }).ToList();
            var genresDTO = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genresDTO);
        }
        //Add 2 result types
        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenreRepository genreRepository, IMapper mapper)
        {
            var genre = await genreRepository.GetById(id);
            if (genre is null)
            {
                return TypedResults.NotFound();
            }
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Ok(genreDTO);
        }
        static async Task<Created<GenreDTO>> Create(CreateGenreDTO createGenreDTO, IGenreRepository genreRepository,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genre = mapper.Map<Genre>(createGenreDTO);
            var id = await genreRepository.Create(genre);
            //remove cache
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Created($"/genres/{id}", genreDTO);
        }
        //Add 2 result types as well-Order does not matter
        static async Task<Results<NotFound, NoContent>> Update(int id, CreateGenreDTO createGenreDTO, IGenreRepository genreRepository,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
           
            var exists = await genreRepository.Exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }
            var genre = mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;
            //Manual map Id because not in DTO

            await genreRepository.Update(genre);
            //remove cache
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
        static async Task<Results<NotFound, NoContent>> Delete(int id, IGenreRepository genreRepository,
            IOutputCacheStore outputCacheStore)
        {

            var exists = await genreRepository.Exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }
            await genreRepository.Delete(id);
            //remove cache
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
    }
}
