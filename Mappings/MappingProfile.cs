using AutoMapper;
using rest_api_sigedi.Controllers;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Mappings
{
    public class MappingProfile: Profile
    {
        
        public MappingProfile()
        {
            /* Ejemplos:
                CreateMap<CategoriaDto, Categoria>();

                Mapper.CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

                CreateMap<CategoriaDto, Categoria>()
                .ForMember(d => d.FechaCreacion, opt => opt.Ignore());
             */
            CreateMap<VendedorDto, Vendedor>();
            CreateMap<CategoriaDto, Categoria>();
            CreateMap<ProveedorDto, Proveedor>();
            CreateMap<ArticuloDto, Articulo>()
            .ForMember(a => a.Detalle, opt => opt.Ignore());
            CreateMap<PrecioDto, Precio>();
            CreateMap<IngresoDto, Ingreso>()
            .ForMember(i => i.Detalle, opt => opt.Ignore());
            CreateMap<IngresoDetalleDto, IngresoDetalle>();
            CreateMap<IngresoDetalleDto, Edicion>()
            .ForMember(d => d.CantidadInicial, opt => opt.MapFrom(src => src.Cantidad))
            .ForMember(d => d.CantidadActual, opt => opt.MapFrom(src => src.Cantidad));
            //CreateMap<EdicionDto, Edicion>();  
        }
    }
}