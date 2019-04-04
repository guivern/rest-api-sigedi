using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Ingreso: EntityBaseConDetalle<IngresoDetalle>
    {
        [ForeignKey("IdProveedor")]
        [JsonIgnore]
        public Proveedor Proveedor {get; set;}
        public long IdProveedor {get; set;}
        public string TipoComprobante {get; set;}
        public string NumeroComprobante {get; set;}
        public bool Anulable {get; set;} = true;
        public bool Editable {get; set;} = true;
        public bool Anulado {get; set;} = false;

        [NotMapped]
        public string NombreProveedor => Proveedor?.RazonSocial;
        [NotMapped]
        public string NombreUsuario => UsuarioCreador?.Username;
    }

}