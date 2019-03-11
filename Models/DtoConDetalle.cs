using System.Collections.Generic;

namespace rest_api_sigedi.Models
{
    public class DtoConDetalle<TDetalleDto>: DtoBase where TDetalleDto: DtoBase
    {
        public List<TDetalleDto> Detalle {get; set;} = new List<TDetalleDto>();
    }
}