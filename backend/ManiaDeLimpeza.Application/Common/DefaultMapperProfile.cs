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
            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.BearerToken, opts => opts.Ignore());

            CreateMap<QuoteDto, Quote>()
                .ForMember(dest => dest.CashDiscount, opt => opt.MapFrom(src => src.CashDiscount))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                //.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.QuoteItems, opt => opt.MapFrom(src => src.LineItems));

            CreateMap<Quote, QuoteDto>();

            CreateMap<LineItemDto, QuoteItem>();
            CreateMap<QuoteItem, LineItemDto>();
        }
    }
}
