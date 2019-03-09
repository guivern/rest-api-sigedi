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
        public string DireccionWeb {get; set;}
        public string Contacto {get; set;}
        public string NumeroContacto {get; set;}
        public string Ciudad {get; set;}
        public string Barrio {get; set;}
    }
}