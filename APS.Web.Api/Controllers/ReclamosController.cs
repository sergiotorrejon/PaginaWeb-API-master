using APS.Web.Api.Models;
using MailKit.Net.Smtp;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static APS.Web.Api.Controllers.Response;
using MailKit.Net.Pop3;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace APS.Web.Api.Controllers
{
    [Route("[controller]")]
    public class ReclamosController : Controller
    {
        private readonly DB.ApiKeyRepository apiKeyRepository;
        private readonly DB.ReclamosRepository dbRepository;
        private readonly DB.SegurosRepository segurosRepository;

        public ReclamosController()
        {
            this.dbRepository = new DB.ReclamosRepository();
            this.segurosRepository = new DB.SegurosRepository();
        }
        
        // GET documentos
        [HttpPost]
        public object Post([FromQuery] string apiKey, [FromQuery] string recaptcha, [FromBody] Models.Reclamos reclamo)
        {
            try
            {
                //if (apiKey == null)
                //{
                //    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKey.ToString() };
                //}

                //var itemKey = this.apiKeyRepository.FinByKey(apiKey);

                //if (itemKey == null)
                //{
                //    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKey.ToString() };
                //}

                //if (!itemKey.Habilitado)
                //{
                //    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKeyDeshabilitado.ToString() };
                //}


                // hacer algunas validaciones
                // validar el recaptcha
                if(recaptcha == null)
                {
                    return new EmptyResponse { Status = ResponseStatuses.ErrorParametrosIncorrectos.ToString() };
                }
                                
                Models.RecaptchaResponse recaptchaResponse = Utils.GoogleRecaptcha.VerifyRecaptcha(recaptcha);
                
                if (recaptchaResponse == null || !recaptchaResponse.success)
                {
                    return new EmptyResponse {
                        Status = ResponseStatuses.ErrorParametrosIncorrectos.ToString(),
                        Message = "Ocurrió un error en la verificación del código captcha"
                    };
                }

                // Se verifica que no se haya realizado un registro desde el mismo IP
                //var ultimoReclamo = dbRepository.GetReclamoByIp(Utils.Http.GetIPAddressAsync().GetAwaiter().GetResult());
                //if (ultimoReclamo != null)
                //{
                //    TimeSpan ts = DateTime.Now - ultimoReclamo.FechaCreacion;
                //    if (ts.Minutes <= 60)
                //    {
                //        return new ReclamosResponse
                //        {
                //            Status = ResponseStatuses.ErrorParametrosIncorrectos.ToString(),
                //            Message = "Ya ha realizado un reclamo recientemente. Debe esperar al menos una hora para realizar un nuevo reclamo",
                //            Data = null
                //        };
                //    }
                //}

                var pensionesRepository = new DB.PensionesRepository();
                var data = pensionesRepository.GetEntidad(reclamo.CodigoEntidad);
                if (data == null)
                {
                    var segurosRepository = new DB.SegurosRepository();
                    data = segurosRepository.GetEntidad(reclamo.CodigoEntidad);
                }

                if(data == null)
                {
                    return new ReclamosResponse
                    {
                        Status = ResponseStatuses.ErrorParametrosIncorrectos.ToString(),
                        Message = "El código de la entidad es inválido.",
                        Data = null
                    };
                }

                // si todo esta bien, guardar
                reclamo.FechaReclamo = DateTime.Now;
                reclamo.Estado = "P";
                reclamo.UsuarioCreacion = "web";
                reclamo.FechaCreacion = DateTime.Now;
                reclamo.Ip = this.Request.HttpContext.Connection.RemoteIpAddress.ToString();


                var result = this.dbRepository.GuardarReclamo(reclamo);

                //this.apiKeyRepository.AddLog(
                //     itemKey.Id,
                //     this.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                //     Utils.Utilities.GetIPAddressAsync().GetAwaiter().GetResult(),
                //     this.Request.Method,
                //     this.Request.Path,
                //     this.Request.QueryString.ToString(),
                //     this.GetType().FullName);

                if (result)
                {
                    var builder = new ConfigurationBuilder();
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json");
                    var ReclamoEmailConfig = builder.Build();
                    var EmailFrom = ReclamoEmailConfig["Reclamo:EmailFrom"];
                    var EmailFromUser = ReclamoEmailConfig["Reclamo:EmailFromUser"];
                    var EmailTo = ReclamoEmailConfig["Reclamo:EmailTo"];
                    var EmailToUser = ReclamoEmailConfig["Reclamo:EmailToUser"];
                    var MailServer = ReclamoEmailConfig["Reclamo:MailServer"];
                    int MailPort = Convert.ToInt16(ReclamoEmailConfig["Reclamo:MailPort"]);
                    bool MailUseSsl = Convert.ToBoolean(ReclamoEmailConfig["Reclamo:MailUseSsl"]);
                    var MailUser = ReclamoEmailConfig["Reclamo:MailUser"];
                    var MailUserPassword = ReclamoEmailConfig["Reclamo:MailUserPassword"];
                    var MailDefaultSubject = ReclamoEmailConfig["Reclamo:MailDefaultSubject"];

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(EmailFromUser, EmailFrom));

                    message.To.Add(new MailboxAddress(EmailToUser, EmailTo));
                    message.Subject = MailDefaultSubject;

                    message.Body = new TextPart("plain")
                    {
                        Text = @"RECLAMANTE: " + reclamo.NombreCompleto +
                        "\n" + reclamo.TipoIdentificacion + ": " + reclamo.NumIdentificacion + " " + reclamo.ExpIdentificacion +
                        "\nCELULAR: " + reclamo.Celular +
                        "\nCORREO: " + reclamo.CorreoElectronico +
                        "\nDIRECCIÓN: " + reclamo.Direccion +
                        "\nENTIDAD: " + reclamo.CodigoEntidad +
                        "\nDESCRIPCIÓN DEL RECLAMO: " + reclamo.DescripcionReclamo +
                        "\nFECHA DEL RECLAMO: " + reclamo.FechaReclamo
                    };

                    using (var client = new SmtpClient())
                    {
                        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        client.Connect(MailServer, MailPort, MailUseSsl);

                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove("XOAUTH2");

                        // Note: only needed if the SMTP server requires authentication
                        client.Authenticate(MailUser, MailUserPassword);

                        client.Send(message);
                        client.Disconnect(true);
                    }

                    return new ReclamosResponse
                    {
                        Status = ResponseStatuses.Correcto.ToString(),
                        Message = "Se ha guardado el registro.",
                        Data = null
                    };
                }
                else
                {
                    return new ReclamosResponse
                    {
                        Status = ResponseStatuses.ErrorParametrosIncorrectos.ToString(),
                        Message = "Ocurrió un error, no se ha guardado el registro.",
                        Data = result
                    };
                }
            }
            catch (Exception ex)
            {
                return new ReclamosResponse
                {
                    Status = ResponseStatuses.ErrorExcepcion.ToString(),
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        [HttpGet]
        public object Get([FromQuery] string apiKey, Models.BuscarReclamo reclamo)
        {
            try
            {
                //if (apiKey == null)
                //{
                //    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKey.ToString() };
                //}

                //var itemKey = this.apiKeyRepository.FinByKey(apiKey);

                //if (itemKey == null)
                //{
                //    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKey.ToString() };
                //}

                //if (!itemKey.Habilitado)
                //{
                //    return new EmptyResponse { Status = ResponseStatuses.ErrorApiKeyDeshabilitado.ToString() };
                //}


                // hacer algunas validaciones
                
                var lReclamos = this.dbRepository.BuscarReclamo(reclamo);

                if (lReclamos.Count() == 0)
                {
                    return new ReclamosResponse
                    {
                        Status = ResponseStatuses.Correcto.ToString(),
                        Message = "No se encontraron resultados",
                        Data = lReclamos
                    };
                }

                /*List<Entidad> lEntidades = new List<Entidad>();
                var pensionesRepository = new DB.PensionesRepository();
                var lEntidadesP = pensionesRepository.ListaEntidades();
                var lEntidadesS = segurosRepository.ListaEntidades();

                foreach (var obj in lEntidadesS)
                {
                    lEntidades.Add(obj);
                }                
                foreach (var obj in lEntidadesP)
                {
                    lEntidades.Add(obj);
                }*/
                
                var listaReclamos = new List<ListaReclamos>();

                foreach (var objReclamo in lReclamos)
                {
                    //Estado del reclamo 'C': Cerrado o Completo; 'A'; En proceso en la APS; 'F': En proceso fuera de la APS, 'E': eliminado
                    var estado = "PENDIENTE";
                    if(objReclamo.Estado == "A")
                        estado = "En atención";
                    else if(objReclamo.Estado == "C")
                        estado = "Concluido";
                    else if (objReclamo.Estado == "F")
                        estado = "En proceso fuera de la APS";

                    listaReclamos.Add(new ListaReclamos {
                        NombreCompleto = objReclamo.NombreCompleto,
                        NumIdentificacion = objReclamo.NumIdentificacion,
                        DescripcionReclamo = objReclamo.DescripcionReclamo,
                        FechaReclamo = objReclamo.FechaReclamo,
                        Estado = estado,
                        NombreEntidad = objReclamo.NombreEntidad});
                }
                
                //this.apiKeyRepository.AddLog(
                //     itemKey.Id,
                //     this.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                //     Utils.Utilities.GetIPAddressAsync().GetAwaiter().GetResult(),
                //     this.Request.Method,
                //     this.Request.Path,
                //     this.Request.QueryString.ToString(),
                //     this.GetType().FullName);

                if (listaReclamos.Count() > 0)
                {
                    return new ReclamosResponse
                    {
                        Status = ResponseStatuses.Correcto.ToString(),
                        Message = "",
                        Data = listaReclamos
                    };
                }
                else
                {
                    return new ReclamosResponse
                    {
                        Status = ResponseStatuses.Correcto.ToString(),
                        Message = "No se encontraron resultados",
                        Data = listaReclamos
                    };
                }
            }
            catch (Exception ex)
            {
                return new ReclamosResponse
                {
                    Status = ResponseStatuses.ErrorExcepcion.ToString(),
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
