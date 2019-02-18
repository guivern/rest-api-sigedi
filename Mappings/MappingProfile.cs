using AutoMapper;

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
        }
    }
}