using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Rendicion : EntityBaseConDetalle<RendicionDetalle>
    {
        [ForeignKey("IdVendedor")]
        [JsonIgnore]
        public Vendedor Vendedor {get; set;}
        public long IdVendedor {get; set;}

        [ForeignKey("IdCaja")]
        [JsonIgnore]
        public Caja Caja {get; set;}
        public long IdCaja {get; set;}

        [Required]
        public decimal MontoTotal {get; set;}
        [Required]
        public decimal ImporteTotal {get; set;}
        [Required]
        public decimal SaldoTotal {get; set;}
        public string TipoComprobante {get; set;}
        public string NroComprobante {get; set;}

        public bool Anulable {get; set;} = true;
        public bool Anulado {get; set;} = false;


        [NotMapped]
        public string NombreUsuario => UsuarioCreador?.Username;
        [NotMapped]
        public string NombreVendedor => Vendedor?.Nombre + " " + Vendedor?.Apellido;
    }
}