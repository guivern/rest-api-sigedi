using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using rest_api_sigedi.Annotations;

namespace rest_api_sigedi.Models
{
    public class DistribucionDetalle : EntityBase
    {
        [ForeignKey("IdDistribucion")]
        [JsonIgnore]
        public Distribucion Distribucion {get; set;}
        [JsonIgnore]
        [IdPadre]
        public long IdDistribucion {get; set;}

        [ForeignKey("IdEdicion")]
        [JsonIgnore]
        public Edicion Edicion {get; set;}
        public long IdEdicion {get; set;}

        [ForeignKey("IdMovimiento")]
        [JsonIgnore]
        public Movimiento Movimiento {get; set;}
        public long IdMovimiento {get; set;}

        [Required]
        public long Cantidad {get; set;}

        [NotMapped]
        public DateTime? FechaEdicion => Edicion?.FechaEdicion;
        [NotMapped]
        public long? NroEdicion => Edicion?.NroEdicion; 
        [NotMapped]
        public String NombreArticulo => Edicion?.Articulo?.Descripcion;
        [NotMapped]
        public Decimal? PrecioVenta => Edicion?.Precio?.PrecioVenta; 
        [NotMapped]
        public Decimal? PrecioRendicion => Edicion?.Precio?.PrecioRendVendedor; 
    }
}