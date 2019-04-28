using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api_sigedi.Models
{
    public class Caja: AuditEntityBase
    {
        public DateTime? FechaCierre {get; set;}
        public Decimal? MontoInicial {get; set;}
        public Decimal? Monto {get; set;}
        [NotMapped]
        public string Cajero => UsuarioCreador?.NombreCompleto;
        /*
            FechaApertura = FechaCreacion
            Cuando Activo es:
            True = Caja Abierta,
            False = Caja Cerrada
         */   
    }
}