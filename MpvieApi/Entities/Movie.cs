namespace MpvieApi.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<Person> Actors { get; set; }
        public string Language { get; set;}
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public DateTime Createddate { get; set; }=DateTime.Now;
        public DateTime? ModifiedDate {  get; set; }


    }
}
