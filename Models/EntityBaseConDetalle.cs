using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace rest_api_sigedi.Models
{
    public class EntityBaseConDetalle<TEntityDetalle>: AuditEntityBase
    {
        [NotMapped]
        public ICollection<TEntityDetalle> Detalle {get; set;}
    }
}