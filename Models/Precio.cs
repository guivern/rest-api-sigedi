using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using rest_api_sigedi.Annotations;

namespace rest_api_sigedi.Models
{
    public class Precio: SoftDeleteEntityBase
    {
        [ForeignKey("IdArticulo")]
        [JsonIgnore]
        public Articulo Articulo {get; set;}
        [JsonIgnore]
        [IdPadre]
        public long IdArticulo {get; set;}

        public decimal PrecioVenta {get; set;}
        public decimal PrecioRendVendedor {get; set;}
        public decimal? PrecioRendAgencia {get; set;}
    }
}