using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Edicion: SoftDeleteEntityBase
    {
        [ForeignKey("IdArticulo")]
        [JsonIgnore]
        public Articulo Articulo {get; set;}
        public long IdArticulo {get; set;}

        [ForeignKey("IdPrecio")]
        [JsonIgnore]
        public Precio Precio {get; set;}
        public long IdPrecio {get; set;}

        //campos propios
        public DateTime? FechaEdicion {get; set;}
        public long NroEdicion {get; set;}
        public long CantidadInicial {get; set;}
        public long CantidadActual {get; set;}
    }
}