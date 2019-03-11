using System;
using rest_api_sigedi.Models;

namespace rest_api_sigedi.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IdPadre: Attribute
    {}
}