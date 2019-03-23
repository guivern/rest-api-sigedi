using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api_sigedi.Models
{
    public class Vendedor: PersonaEntityBase
    {
        public string ZonaVenta {get; set;}
        public string Nacionalidad {get; set;}
        public string TelefonoMovil {get; set;}
        public DateTime? FechaIngreso {get; set;}

        [NotMapped]
        public string NombreCompleto => Nombre + " " + Apellido;
    }
}