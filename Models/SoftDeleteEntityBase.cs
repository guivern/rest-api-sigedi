namespace rest_api_sigedi.Models
{
    public class SoftDeleteEntityBase: EntityBase
    {
        public bool Activo {get; set;} = true;
    }
}