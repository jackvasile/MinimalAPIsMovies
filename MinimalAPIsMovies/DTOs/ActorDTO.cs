namespace MinimalAPIsMovies.DTOs
{
    public class ActorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        //returns url for the picture
        public string? Picture { get; set; }

    }
}
