using System;
using System.ComponentModel.DataAnnotations;

namespace rest_api_sigedi.Models
{
    public class PersonaEntityBase: EntityBase
    {
        public static readonly string[] DOCUMENTOS = { "CEDULA", "RUC"};

        [Required]
        public string Nombre {get; set;}
        [Required]
        public string Apellido {get; set;}
        public string TipoDocumento {get; set;}
        public string NumeroDocumento {get; set;}
        public DateTime? FechaNacimiento {get; set;}
        public string Direccion {get; set;}
        public string Telefono {get; set;}
        public string Email {get; set;}
    }
}