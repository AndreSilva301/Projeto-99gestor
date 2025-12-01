using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Application.Common
{
    public class DefaultMapperProfile : Profile
    {
        public DefaultMapperProfile()
        {
            CreateMap<RegisterUserRequestDto, User>();
            CreateMap<User, AuthResponseDto>()
                .ForMember(dest => dest.BearerToken, opts => opts.Ignore());

            CreateMap<CreateQuoteDto, Quote>()
                .ForMember(dest => dest.QuoteItems, opt => opt.MapFrom(src => src.Items));

            CreateMap<CreateQuoteDto, QuoteItem>();

            CreateMap<QuoteDto, Quote>()
                .ForMember(dest => dest.CashDiscount, opt => opt.MapFrom(src => src.CashDiscount))
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.QuoteItems, opt => opt.MapFrom(src => src.QuoteItems));

            CreateMap<Quote, QuoteDto>()
                 .ForMember(dest => dest.QuoteItems, opt => opt.MapFrom(src => src.QuoteItems));


            CreateMap<QuoteItem, QuoteItemDto>();
            CreateMap<QuoteItemDto, QuoteItem>();

            CreateMap<User, UserLightDto>();

            // Customer mappings
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CustomerCreateDto, Customer>();
            CreateMap<CustomerUpdateDto, Customer>();
            CreateMap<Customer, CustomerListItemDto>();

            // Related value object mappings
            CreateMap<AddressDto, Address>().ReverseMap();
            CreateMap<PhoneDto, Phone>().ReverseMap();

            // Customer relationship mappings
            CreateMap<CustomerRelationshipCreateDto, CustomerRelationship>();
            CreateMap<CustomerRelationshipDto, CustomerRelationship>();
            CreateMap<CustomerRelationship, CustomerRelationshipDto>();
        }
    }
}
