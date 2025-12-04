using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Application.Common;
public class QuoteProfile : Profile
{
    public QuoteProfile()
    {
        // Quote → QuoteResponseDto
        CreateMap<Quote, QuoteResponseDto>()
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.Name : string.Empty))
            .ForMember(dest => dest.QuoteItems,
                opt => opt.MapFrom(src => src.QuoteItems));

        // CreateQuoteDto → Quote
        CreateMap<CreateQuoteDto, Quote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
            .ForMember(dest => dest.QuoteItems, opt => opt.MapFrom(src => src.Items)); // Calculado pelo serviço

        // UpdateQuoteDto → Quote
        CreateMap<UpdateQuoteDto, Quote>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // QuoteItem → QuoteItemResponseDto
        CreateMap<QuoteItem, QuoteItemResponseDto>();

        // CreateQuoteItemDto → QuoteItem
        CreateMap<QuoteItemDto, QuoteItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuoteId, opt => opt.Ignore()); // Calculado pelo serviço
    }
}
