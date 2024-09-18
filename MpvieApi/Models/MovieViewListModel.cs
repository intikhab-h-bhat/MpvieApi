﻿namespace MpvieApi.Models
{
    public class MovieViewListModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
       
        public List<ActorViewModel> Actors { get; set; }
        public string Language { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
    }
}
