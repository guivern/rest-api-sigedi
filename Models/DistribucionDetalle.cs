using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using rest_api_sigedi.Annotations;

namespace rest_api_sigedi.Models
{
    public class DistribucionDetalle : SoftDeleteEntityBase
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

        [Required]
        public long Cantidad {get; set;}

        public long? Devoluciones {get; set;} = 0;

        [Required]
        public decimal Monto {get; set;}
        
        public decimal? Importe {get; set;} = 0;
        
        [Required]
        public decimal Saldo {get; set;}
        
        public bool Anulable {get; set;} = true;
        
        public bool Editable {get; set;} = true;

        public bool Anulado {get; set;} = false;

        public bool? YaSeDevolvio {get; set;} = false;

        /* El atributo Activo cuando es:
            True: Indica que aún no se cancelo, es decir, saldo > 0
            False: Indica que ya se cancelo distribucion, saldo == 0
        */

        [NotMapped]
        public DateTime? FechaEdicion => Edicion?.FechaEdicion;
        [NotMapped]
        public long? NroEdicion => Edicion?.NroEdicion; 
        [NotMapped]
        public String NombreArticulo => Edicion?.Articulo?.Descripcion;
        
        public Decimal? PrecioVenta {get; set;}
        public Decimal? PrecioRendicion {get; set;}
    }
}