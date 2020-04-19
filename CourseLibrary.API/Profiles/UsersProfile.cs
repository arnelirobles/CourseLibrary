using AutoMapper;
using CourseLibrary.API.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserCreateDto, UserDto>();               
            CreateMap<UserDto, UserCreateDto>();

            CreateMap<UserCreateDto, IdentityUser>()
                .ForMember(
                    d => d.UserName,
                    o => o.MapFrom(s => s.Email)
                );
            CreateMap<IdentityUser, UserDto>();

        }
    }
}
