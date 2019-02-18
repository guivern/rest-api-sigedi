using System.ComponentModel.DataAnnotations;

namespace rest_api_sigedi.Annotations
{
    public class RequeridoAttribute: RequiredAttribute 
    {
        public RequeridoAttribute()
        {
            //ErrorMessage = "{0} es requerido";
            ErrorMessage = "Es requerido";
        }
    }
}