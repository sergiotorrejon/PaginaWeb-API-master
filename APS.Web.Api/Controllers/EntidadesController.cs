using APS.Web.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static APS.Web.Api.Controllers.Response;

namespace APS.Web.Api.Controllers
{
    [Route("[controller]")]
    public class EntidadesController : Controller
    {

        private readonly DB.SegurosRepository segurosRepository;
        
        public EntidadesController()
        {
            this.segurosRepository = new DB.SegurosRepository();
        }

        // GET Entidades
        [HttpGet]
        public object GetEntidades([FromQuery] string apiKey, [FromQuery] string mercado = "")
        {
            try
            {
                /*
                if (apiKey == null)
                {
                    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKey.ToString() };
                }

                var itemKey = this.apiKeyRepository.FinByKey(apiKey);

                if (itemKey == null)
                {
                    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKey.ToString() };
                }

                if (!itemKey.Habilitado)
                {
                    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKeyDeshabilitado.ToString() };
                }

                this.apiKeyRepository.AddLog(
                    itemKey.Id,
                    this.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Utils.Http.GetIPAddressAsync().GetAwaiter().GetResult(),
                    this.Request.Method,
                    this.Request.Path,
                    this.Request.QueryString.ToString(),
                    this.GetType().FullName);
                /**/
                string mercado_ = mercado;
                List<Entidad> entidades = new List<Entidad>();

                if (mercado.ToLower() == "seguros" || mercado == "")
                {
                    var data = segurosRepository.ListaEntidades();
                    foreach (var obj in data)
                    {
                        entidades.Add(obj);
                    }
                }

                if (mercado.ToLower() == "pensiones" || mercado == "")
                {
                    var pensionesRepository = new DB.PensionesRepository();
                    var data = pensionesRepository.ListaEntidades();
                    foreach (var obj in data)
                    {
                        entidades.Add(obj);
                    }
                }

                return new EntidadesResponse { Status = ResponseStatuses.Correcto.ToString(), Data = entidades };

            }
            catch (Exception ex)
            {
                return new EntidadesResponse
                {
                    Status = ResponseStatuses.ErrorExcepcion.ToString(),
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
