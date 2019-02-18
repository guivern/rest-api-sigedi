using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class AuditEntityBase: SoftDeleteEntityBase
    {
        [ForeignKey("IdUsuarioCreador")]
        [JsonIgnore]
        public Usuario UsuarioCreador {get; set;}
        [Required]
        public long IdUsuarioCreador {get; set;}

        [ForeignKey("IdUsuarioModificador")]
        [JsonIgnore]
        public Usuario UsuarioModificador {get; set;}
        public long? IdUsuarioModificador {get; set;}

        public DateTime FechaCreacion {get; set;} = DateTime.Now;
        
        public DateTime? FechaUltimaModificacion {get; set;}

        [NotMapped]
        public string NombreUsuarioCreador => UsuarioCreador?.Username;
    }
}