using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace rest_api_sigedi.Models
{
    public class EntityBase
    {
        public const int DESCRIPCION_MAX_LENGTH = 256;

        [Key]
        public long Id {get; set;}
        public string Descripcion {get; set;}
    }
}