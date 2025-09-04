using AutoMapper;
using HelloApi.Models;
using HelloApi.Models.DTOs;

namespace HelloApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de Order a OrderReadDto
            CreateMap<Order, OrderReadDto>()
                .ForMember(dest => dest.PersonName, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore());

            // Mapeo de OrderDetail a OrderDetailReadDto
            CreateMap<OrderDetail, OrderDetailReadDto>()
                .ForMember(dest => dest.ItemName, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Quantity * src.Price));

            // Mapeo de OrderCreateDto a Order (ignorar propiedades que no se deben mapear)
            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Number, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Person, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());

            // Mapeo de OrderDetailCreateDto a OrderDetail
            CreateMap<OrderDetailCreateDto, OrderDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Item, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
        }
    }
}
