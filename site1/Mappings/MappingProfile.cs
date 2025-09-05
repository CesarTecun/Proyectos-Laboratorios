using AutoMapper;
using MessageApi.Models;
using MessageApi.Models.DTOs;

namespace MessageApi.Mappings
{
    /// <summary>
    /// Perfil de AutoMapper para configurar los mapeos entre entidades y DTOs.
    /// Esta clase hereda de Profile de AutoMapper y define cómo se mapean los objetos entre las capas de la aplicación.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MappingProfile"/>.
        /// Configura todos los mapeos necesarios para la aplicación.
        /// </summary>
        public MappingProfile()
        {
            // Mapeo de Message a MessageReadDto
            CreateMap<Message, MessageReadDto>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.MessageText));
                
            // Mapeo de Person a PersonReadDto
            CreateMap<Person, PersonReadDto>();
                
            // Mapeo de Order a OrderReadDto
            CreateMap<Order, OrderReadDto>()
                // Se ignora PersonName porque se establecerá manualmente después del mapeo
                .ForMember(dest => dest.PersonName, opt => opt.Ignore())
                // Se ignora Total porque se calculará manualmente después del mapeo
                .ForMember(dest => dest.Total, opt => opt.Ignore());

            // Mapeo de OrderDetail a OrderDetailReadDto
            CreateMap<OrderDetail, OrderDetailReadDto>()
                // Se ignora ItemName porque se establecerá manualmente después del mapeo
                .ForMember(dest => dest.ItemName, opt => opt.Ignore())
                // Se calcula el total multiplicando la cantidad por el precio unitario
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Quantity * src.Price));

            // Mapeo de OrderCreateDto a Order
            // Se ignoran las propiedades que deben ser manejadas por el sistema o el repositorio
            CreateMap<OrderCreateDto, Order>()
                // Se ignoran las propiedades que son generadas por la base de datos
                .ForMember(dest => dest.Id, opt => opt.Ignore())                // Generado por la base de datos
                .ForMember(dest => dest.Number, opt => opt.Ignore())            // Generado por el sistema
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())         // Establecido por el servicio
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())         // Establecido por el servicio
                .ForMember(dest => dest.Person, opt => opt.Ignore())            // Se carga por separado
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());     // Se mapean por separado

            // Mapeo de Product a ProductReadDto
            CreateMap<Product, ProductReadDto>();
            
            // Mapeo de ProductCreateDto a Product
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Mapeo de OrderDetailCreateDto a OrderDetail
            // Se ignoran las propiedades que deben ser manejadas por el sistema o el repositorio
            CreateMap<OrderDetailCreateDto, OrderDetail>()
                // Se ignoran las propiedades que son generadas por la base de datos o el sistema
                .ForMember(dest => dest.Id, opt => opt.Ignore())                // Generado por la base de datos
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())           // Establecido por el servicio
                .ForMember(dest => dest.Item, opt => opt.Ignore())              // Se carga por separado
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())         // Establecido por el servicio
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())         // Establecido por el servicio
                .ForMember(dest => dest.Total, opt => opt.Ignore())             // Se calcula automáticamente
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())         // Establecido por el servicio
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());        // Establecido por el servicio
        }
    }
}
