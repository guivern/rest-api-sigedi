using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Articulo: EntityBaseConDetalle<Precio>
    {
        [ForeignKey("IdCategoria")]
        [JsonIgnore]
        public Categoria Categoria {get; set;}
        public long IdCategoria {get; set;}

        [ForeignKey("IdProveedor")]
        [JsonIgnore]
        public Proveedor Proveedor {get; set;}
        public long IdProveedor {get; set;}

        public long? Codigo {get; set;}

        [NotMapped]
        public string NombreCategoria => Categoria?.Descripcion;
        [NotMapped]
        public string NombreProveedor => Proveedor?.RazonSocial;
    }
}