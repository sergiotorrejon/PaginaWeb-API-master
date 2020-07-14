using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using static APS.Web.Api.Controllers.Response;

namespace APS.Web.Api.Controllers
{
    [Route("[controller]")]
    public class PolizaController : Controller
    {
        private readonly DB.Polizas polizasRepository;

        public PolizaController()
        {
            this.polizasRepository = new DB.Polizas();
        }

        //[Route("getPoliza")]
        [HttpGet]
        public object Get(string nro_poliza)
        {
            try
            {
                var data = this.polizasRepository.GetPoliza(nro_poliza);
                string finf = this.polizasRepository.GetFinformacion();
                return new PolizasResponse { Status = ResponseStatuses.Correcto.ToString(), Data = data, finformacion = finf };
            }
            catch (Exception ex)
            {
                return new PolizasResponse
                {
                    Status = ResponseStatuses.ErrorExcepcion.ToString(),
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
