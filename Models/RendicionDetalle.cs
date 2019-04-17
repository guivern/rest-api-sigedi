using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using rest_api_sigedi.Annotations;

namespace rest_api_sigedi.Models
{
    public class RendicionDetalle : SoftDeleteEntityBase
    {
        [ForeignKey("IdRendicion")]
        [JsonIgnore]
        public Rendicion Rendicion {get; set;}
        [JsonIgnore]
        [IdPadre]
        public long IdRendicion {get; set;}

        [ForeignKey("IdDistribucionDetalle")]
        [JsonIgnore]
        public DistribucionDetalle DistribucionDetalle {get; set;}
        public long IdDistribucionDetalle {get; set;}

        public long? Devoluciones {get; set;} = 0;
         
        [Required]
        public decimal Monto {get; set;}
        [Required]
        public decimal Importe {get; set;}
        [Required]
        public decimal Saldo {get; set;}

        public bool Anulable {get; set;} = true;
        public bool Anulado {get; set;} = false;

        [NotMapped]
        public DateTime? FechaEdicion => DistribucionDetalle?.Edicion?.FechaEdicion;
        [NotMapped]
        public long? NroEdicion => DistribucionDetalle?.Edicion?.NroEdicion; 
        [NotMapped]
        public long? Cantidad => DistribucionDetalle?.Cantidad;
        [NotMapped]
        public String NombreArticulo => DistribucionDetalle?.Edicion?.Articulo?.Descripcion;
        [NotMapped]
        public Decimal? PrecioVenta => DistribucionDetalle?.Edicion?.Precio?.PrecioVenta; 
        [NotMapped]
        public Decimal? PrecioRendicion => DistribucionDetalle?.Edicion?.Precio?.PrecioRendVendedor; 
    }
}