using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static APS.Web.Api.Controllers.Response;

namespace APS.Web.Api.Controllers
{
    [Route("[controller]")]
    public class DocumentosController : Controller
    {
        //private readonly DB.ApiKeyRepository apiKeyRepository;
        private readonly DB.DocumentosRepository dbRepository;

        public DocumentosController()
        {
            this.dbRepository = new DB.DocumentosRepository();
        }
        
        // GET documentos
        [HttpGet]
        public object Get([FromQuery] string apiKey, int itemsPerPage, int pagenumber, string gestion, string institucion, string mercado, string tipoDocumento, string categoria, string titulo, string numero)
        {
            try
            {
                if (itemsPerPage > 100)
                {
                    return new DocumentosResponse
                    {
                        Status = ResponseStatuses.ErrorParametrosIncorrectos.ToString(),
                        Message = "El número máximo de items a consultar es 100",
                        Data = null,
                        TotalRows = 0
                    };
                }

                var pGestion = gestion == null ? "" : gestion;
                var pInstitucion = institucion == null ? "" : institucion;
                var pMercado = mercado == null ? "" : mercado;
                var pTipoDocumento = tipoDocumento == null ? "" : tipoDocumento;
                var pCategoria = categoria == null ? "" : categoria;
                var pTitulo = titulo == null ? "" : titulo;
                var pNumero = numero == null ? "" : numero;

                var count = this.dbRepository.ContarDocumentos(pGestion, pInstitucion, pMercado, pTipoDocumento, pCategoria, pTitulo, pNumero);
                var data = this.dbRepository.ListaDocumentos(itemsPerPage, pagenumber, pGestion, pInstitucion, pMercado, pTipoDocumento, pCategoria, pTitulo, pNumero);
                List<Models.Documentos> nData = new List<Models.Documentos>();
                foreach (var obj in data)
                {
                    if (obj.UrlArchivo.Trim() != "")
                    {
                        obj.UrlArchivo = obj.TipoDocumento.Trim() == "RA" ? Models.AppSettings.ResolucionesPath + obj.UrlArchivo : Models.AppSettings.CircularesPath + obj.UrlArchivo;
                    }

                    nData.Add(obj);
                }

                return new DocumentosResponse
                {
                    Status = ResponseStatuses.Correcto.ToString(),
                    Data = nData,
                    TotalRows = count
                };
            }
            catch (Exception ex)
            {
                return new DocumentosResponse
                {
                    Status = ResponseStatuses.ErrorExcepcion.ToString(),
                    Message = ex.Message,
                    Data = null
                };
            }
        }
        
        // GET documentos
        [HttpGet]
        [Route("notificacion")]
        public object GetNotificacion([FromQuery] string apiKey, string institucion, string tipoDocumento, string gestion, string numero)
        {
            try
            {                
                var pInstitucion = institucion == null ? "" : institucion;
                var pTipoDocumento = tipoDocumento == null ? "" : tipoDocumento;
                var pGestion = gestion == null ? "" : gestion;
                var pNumero = numero == null ? "" : numero;

                if (pInstitucion == "" || pTipoDocumento == "" || pGestion == "" || pNumero == "")
                {
                    return new NotificacionResponse
                    {
                        Status = ResponseStatuses.ErrorParametrosIncorrectos.ToString(),
                        Message = "Todos los parámetros son incorrectos",
                        Data = null
                    };
                }

                var data = this.dbRepository.ListaNotificaciones(pInstitucion, pTipoDocumento, pGestion, pNumero);
                
                return new NotificacionResponse
                {
                    Status = ResponseStatuses.Correcto.ToString(),
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new DocumentosResponse
                {
                    Status = ResponseStatuses.ErrorExcepcion.ToString(),
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
