using System.ComponentModel.DataAnnotations;

namespace rest_api_sigedi.Annotations
{
    public class RangoAttribute: RangeAttribute
    {
        RangoAttribute(int min, int max):base(min, max)
        {
            ErrorMessage = "Solo admite valores entre {0} y {1}";
        }

        RangoAttribute(double min, double max):base(min, max)
        {
            ErrorMessage = "Solo admite valores entre {0} y {1}";
        }
    }
}