using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Movimiento : SoftDeleteEntityBase
    {
        [ForeignKey("IdEdicion")]
        [JsonIgnore]
        public Edicion Edicion {get; set;}
        public long IdEdicion {get; set;}

        [ForeignKey("IdVendedor")]
        [JsonIgnore]
        public Vendedor Vendedor {get; set;}
        public long IdVendedor {get; set;}
        [Required]
        public long Llevo {get; set;} 
        public long? Devolvio {get; set;} = 0;
        [Required]
        public decimal Monto {get; set;}
        public decimal? Importe {get; set;} = 0;
        [Required]
        public decimal Saldo {get; set;}
        public bool Anulado {get; set;} = false;
        
        [NotMapped]
        public DateTime? FechaEdicion => Edicion?.FechaEdicion;
        [NotMapped]
        public long? NroEdicion => Edicion?.NroEdicion; 
        [NotMapped]
        public String NombreArticulo => Edicion?.Articulo?.Descripcion;
    }
}