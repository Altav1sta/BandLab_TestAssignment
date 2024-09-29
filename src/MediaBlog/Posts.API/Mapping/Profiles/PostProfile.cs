using AutoMapper;

namespace Posts.API.Mapping.Profiles
{
    internal class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Data.Entities.Post, SDK.Models.Post>();
        }
    }
}
