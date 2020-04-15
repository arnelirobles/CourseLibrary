using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(
                    d => d.Name,
                    o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(
                    d => d.Age,
                    o => o.MapFrom(s => s.DateOfBirth.GetCurrentAge()))
                ;

            CreateMap<AuthorCreationDto, Author>();
        }
    }
}