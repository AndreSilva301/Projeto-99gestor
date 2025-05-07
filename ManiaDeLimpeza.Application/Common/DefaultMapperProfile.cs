using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Common
{
    public class DefaultMapperProfile : Profile
    {
        public DefaultMapperProfile() 
        {
            CreateMap<RegisterUserDto, User>();
            CreateMap<User, AuthResponseDto>();
        }
    }
}
