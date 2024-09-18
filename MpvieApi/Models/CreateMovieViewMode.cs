using MpvieApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace MpvieApi.Models
{
    public class CreateMovieViewMode
    {

        public int Id { get; set; }
        [Required(ErrorMessage ="Movie Title reauired")]
        public string Title { get; set; }
        public string Description { get; set; }

        public List<int> Actors { get; set; }
        public string Language { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
     

    }
}
