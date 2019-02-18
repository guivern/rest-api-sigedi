using System.ComponentModel.DataAnnotations;

namespace rest_api_sigedi.Annotations
{
    public class NoNegativo: RangeAttribute
    {
        public NoNegativo():base(0, int.MaxValue)
        {
            ErrorMessage = "No admite valores negativos";
        }
    }
}