using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.Controllers
{
    public class EmptyResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public enum ResponseStatuses
    {
        Correcto,
        ErrorApiKey,
        ErrorApiKeyDeshabilitado,
        ErrorParametrosIncorrectos,
        ErrorExcepcion
    }
}
