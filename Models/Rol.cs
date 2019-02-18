
using System.ComponentModel.DataAnnotations;

namespace rest_api_sigedi.Models
{
    public class Rol: SoftDeleteEntityBase
    {
        [Required]
        public string Nombre{get; set;}
    }
}