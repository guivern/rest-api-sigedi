using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Distribucion : EntityBaseConDetalle<DistribucionDetalle>
    {
        [ForeignKey("IdVendedor")]
        [JsonIgnore]
        public Vendedor Vendedor {get; set;}
        public long IdVendedor {get; set;}

        public bool Anulable {get; set;} = true;
        public bool Editable {get; set;} = true;
        public bool Anulado {get; set;} = false;

        [NotMapped]
        public string NombreUsuario => UsuarioCreador?.Username;
        [NotMapped]
        public string NombreVendedor => Vendedor?.Nombre + " " + Vendedor?.Apellido;
    }
}