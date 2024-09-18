using AutoMapper;
using MpvieApi.Entities;
using MpvieApi.Models;

namespace MpvieApi
{
    public class MappinProfile: Profile
    {

        public MappinProfile()
        {
            CreateMap<Movie, MovieViewListModel>();
            CreateMap<Movie, MovieDetailsViewModel>();
            CreateMap<MovieViewListModel, Movie>();
            CreateMap<CreateMovieViewMode, Movie>().ForMember(x=>x.Actors,y=> y.Ignore());

            CreateMap<Person, ActorViewModel>();
            CreateMap<Person, ActorDetailsViewModel>();
            CreateMap<ActorViewModel, Person>(); 



        }
    }
}
