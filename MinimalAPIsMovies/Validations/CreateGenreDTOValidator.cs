using FluentValidation;
using MinimalAPIsMovies.DTOs;
using MinimalAPIsMovies.Repositories;

namespace MinimalAPIsMovies.Validations
{
    public class CreateGenreDTOValidator:AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenreRepository genreRepository, IHttpContextAccessor httpContextAccessor)
        {
            //Get the id from the route
            var routeValueId= httpContextAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;
            //If routeValueId is not null, try to parse it to int
            if(routeValueId is string routeValueIdString)
            {
                int.TryParse(routeValueIdString, out id);
            }
            //Go to Genre EndPoint to use rules
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage).MaximumLength(50)
                .WithMessage(ValidationUtilities.MaxLengthMessage)
                .Must(ValidationUtilities.FirstLetterIsUppercase).WithMessage(ValidationUtilities.FirstLetterIsUppercaseMessage)
                .MustAsync(async (name, _) =>
                {
                    var exists = await genreRepository.Exists(id, name);
                    return !exists;
                }).WithMessage(g => $"A genre with the name {g.Name} already exists");
        }

       
    }
}
