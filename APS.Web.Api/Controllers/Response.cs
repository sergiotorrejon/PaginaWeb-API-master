using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.Controllers
{
    public class Response
    {
        /**/

        public class EntidadesResponse : EmptyResponse
        {
            public object Data { get; set; }
        }

        public class WebResponse : EmptyResponse
        {
            public object Data { get; set; }
        }

        public class DocumentosResponse : EmptyResponse
        {
            public int TotalRows { set; get; }
            public object Data { get; set; }
        }

        public class NotificacionResponse : EmptyResponse
        {
            public object Data { get; set; }
        }

        public class ReclamosResponse : EmptyResponse
        {
            public object Data { get; set; }
        }

        public class PolizasResponse : EmptyResponse
        {
            public string finformacion { get; set; }
            public IEnumerable<Models.Poliza> Data { get; set; }
        }
    }
}
