using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>();
        CreateMap<Client, ClientDto>();
        CreateMap<Job, JobDto>()
            .ForMember(d => d.ClientName, opt => opt.MapFrom(s => s.Client.Name));
        CreateMap<Job, JobSummaryDto>()
            .ForMember(d => d.ClientName, opt => opt.MapFrom(s => s.Client.Name));
        CreateMap<Quote, QuoteDto>();
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<LineItem, LineItemDto>();
    }
}