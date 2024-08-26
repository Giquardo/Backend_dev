using AutoMapper;
using PhotoAlbumApi.Models;
using PhotoAlbumApi.DTOs;

namespace PhotoAlbumApi.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Album, AlbumDto>().ReverseMap();
        CreateMap<Album, AlbumSummaryDto>().ReverseMap();
        CreateMap<Photo, PhotoDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserDisplayDto>();

    }
}



