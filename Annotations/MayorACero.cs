using System.ComponentModel.DataAnnotations;

namespace rest_api_sigedi.Annotations
{
    public class MayorACero: RangeAttribute
    {
        public MayorACero(): base(1, int.MaxValue)
        {
            ErrorMessage = "Debe ser mayor a 0";
        }
    }
}