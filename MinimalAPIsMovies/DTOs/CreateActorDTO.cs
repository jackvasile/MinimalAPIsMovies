namespace MinimalAPIsMovies.DTOs
{
    public class CreateActorDTO
    {
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        //returns the actual picture
        public IFormFile? Picture { get; set; }

    }
}
