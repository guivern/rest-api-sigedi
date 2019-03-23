using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using rest_api_sigedi.Annotations;

namespace rest_api_sigedi.Models
{
    public class IngresoDetalle: EntityBase
    {
        [ForeignKey("IdIngreso")]
        [JsonIgnore]
        public Ingreso Ingreso {get; set;}
        [JsonIgnore]
        [IdPadre]
        public long IdIngreso {get; set;}

        [ForeignKey("IdArticulo")]
        [JsonIgnore]
        public Articulo Articulo {get; set;}
        public long IdArticulo {get; set;}

        [ForeignKey("IdEdicion")]
        [JsonIgnore]
        public Edicion Edicion {get; set;}
        public long IdEdicion {get; set;}

        [ForeignKey("IdPrecio")]
        [JsonIgnore]
        public Precio Precio {get; set;}
        public long IdPrecio {get; set;}

        public long Cantidad {get; set;}
        public DateTime? FechaEdicion {get; set;}
        public long NroEdicion {get; set;}

        [NotMapped]
        public String NombreArticulo => Articulo?.Descripcion;

        [NotMapped]
        public Decimal? PrecioVenta => Precio?.PrecioVenta; 
        [NotMapped]
        public Decimal? PrecioRendicion => Precio?.PrecioRendVendedor; 
    }
}
