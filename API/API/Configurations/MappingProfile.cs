using API.Models;
using API.ViewModels;
using AutoMapper;

namespace API.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mapping
            CreateMap<User, UserVM>();
            CreateMap<User, ProfileVM>();


            // Post mapping
            CreateMap<Post, PostListVM>();
            CreateMap<Post, PostVM>();


            // Product mapping


        }
    }
}
