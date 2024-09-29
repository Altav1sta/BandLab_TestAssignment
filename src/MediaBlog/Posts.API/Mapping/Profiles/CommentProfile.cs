using AutoMapper;

namespace Posts.API.Mapping.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Data.Entities.Comment, SDK.Models.Comment>();
        }
    }
}
