using FluentValidation;
using MinimalAPIsMovies.DTOs;

namespace MinimalAPIsMovies.Validations
{
    public class CreateActorDTOValidator:AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        {
            //Go to Actor EndPoint to use rules
            RuleFor(x => x.Name).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage).MaximumLength(50)
                .WithMessage(ValidationUtilities.MaxLengthMessage);

            var minimumDate=new DateTime(1900, 1, 1);
            RuleFor(p=>p.DateOfBirth).GreaterThanOrEqualTo(minimumDate)
                //.WithMessage("The field {PropertyName} should be greater than {ComparisonValue}");
                .WithMessage(ValidationUtilities.GreaterThanDate(minimumDate));
                
        }
    }
}
