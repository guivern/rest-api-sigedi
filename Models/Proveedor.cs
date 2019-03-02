using System.ComponentModel.DataAnnotations;

namespace rest_api_sigedi.Models
{
    public class Proveedor: SoftDeleteEntityBase
    {
        [Required]
        public string RazonSocial {get; set;}
        [Required]
        public string NumeroDocumento {get; set;}
        public string Direccion {get; set;}
        public string Telefono {get; set;}
        public string Email {get; set;}
    }
}