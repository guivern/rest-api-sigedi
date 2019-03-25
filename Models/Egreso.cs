using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class Egreso : AuditEntityBase
    {
        [ForeignKey("IdEdicion")]
        [JsonIgnore]
        public Edicion Edicion {get; set;}
        public long IdEdicion {get; set;}

        public long Cantidad {get; set;}
        public DateTime? Fecha {get; set;}

        [NotMapped]
        public long? NroEdicion => Edicion?.NroEdicion; 
        [NotMapped]
        public string NombreUsuario => UsuarioCreador?.Username;

        [NotMapped]
        public String NombreArticulo => Edicion?.Articulo?.Descripcion;

        [NotMapped]
        public Decimal? PrecioVenta => Edicion?.Precio?.PrecioVenta; 
        [NotMapped]
        public Decimal? PrecioRendicion => Edicion?.Precio?.PrecioRendVendedor;

    }
}