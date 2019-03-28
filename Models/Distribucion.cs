using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Distribucion : AuditEntityBase
    {
        [ForeignKey("IdVendedor")]
        [JsonIgnore]
        public Vendedor Vendedor {get; set;}
        public long IdVendedor {get; set;}

        [NotMapped]
        public string NombreUsuario => UsuarioCreador?.Username;
        [NotMapped]
        public string NombreVendedor => Vendedor?.Nombre + " " + Vendedor?.Apellido;
        [NotMapped]
        public ICollection<DistribucionDetalle> Detalle {get; set;}
        
    }
}