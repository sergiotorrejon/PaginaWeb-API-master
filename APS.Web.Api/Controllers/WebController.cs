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
using WebResponse = APS.Web.Api.Controllers.Response.WebResponse;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;

namespace APS.Web.Api.Controllers
{
    [Route("[controller]")]
    public class WebController : Controller
    {
        private readonly DB.WebRepository webRepository;
        private readonly DB.CorrespondenciaRepository CorrespondenciaRepository;


        public WebController()
        {
            this.webRepository = new DB.WebRepository();
            this.CorrespondenciaRepository = new DB.CorrespondenciaRepository();
        }




        [HttpPost("correspondencia")]
        [DisableRequestSizeLimit]
        public async Task<OkObjectResult> PostFiles([FromQuery] string apiKey, [FromQuery] string recaptcha, Models.Correspondencia Correspondencia, ICollection<IFormFile> files)

        {
            try
            {
            //VERIFICA EL CAPTCHA
                if (recaptcha == null)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        message = "El CAPTCHA no puede ser nulo "
                    });
                }

                //Models.RecaptchaResponse recaptchaResponse = Utils.GoogleRecaptcha.VerifyRecaptcha(recaptcha);

                //if (recaptchaResponse == null || !recaptchaResponse.success)
                //{
                //    return new OkObjectResult(new
                //    {
                //        status = "error",
                //        message = "Ocurrió un erroral validar el CAPTCHA " 
                //    });
                //}

            // VERIFICA SI EXITE ARCHIVOS QUE REGISTRAR
                if (files.Count == 0)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        message = "No existen Archivos para Registrar Correspondencia"
                    });
                }

                //VERIFICA LA EXTENSION DE ARCHIVOS PERMITIDOS
                foreach (var file in files)
                {
                    var verifica = false;
                    var VerificaExtension = (file.FileName.Substring(file.FileName.LastIndexOf(".")));
                    var VerificaTamano = file.Length;
                    var extensiones = CorrespondenciaRepository.Parametros();
                    foreach (var ext in extensiones)
                    {
                        if (ext.ValorString.Contains(VerificaExtension))
                        {
                            if (ext.ValorNumber < VerificaTamano)
                            {
                                return new OkObjectResult(new
                                {
                                    status = "error",
                                    message = "El Archivo " + file.FileName + " es mayor al permitido " + (ext.ValorNumber / 1024) + "Kb"
                                });
                            }
                            verifica = true;
                            break;
                        }
                            
                    }
                    if (verifica == false)
                    {
                        return new OkObjectResult(new
                        {
                            status = "error",
                            message = "El Archivo " + file.FileName + " NO se encuentra dentro de los archivos permitidos "
                        });
                    }

                }
                //VERIFICA LA CAPACIDA MAXIMA DE CARGA
                long capacidadMax = 0;
                long capacidadRecibida = 0;
                foreach (var file in files)
                {
                    var VerificaTamano = file.Length;
                    capacidadRecibida = capacidadRecibida + VerificaTamano;
                }
                var extensionTotal = CorrespondenciaRepository.Parametros();
                foreach (var ext in extensionTotal)
                {
                    if (ext.ValorString == "MAX")
                    {
                        capacidadMax = ext.ValorNumber;
                        break;
                    }
                }
                if (capacidadMax < capacidadRecibida)
                {
                    return new OkObjectResult(new
                    {
                        status = "error",
                        message = "La capcidad maxima es de " + (capacidadMax / 1048576) + "Mb"
                    });
                }
            
            //SETA PARAMETROS DE WEBDOCDIGITAL
                Correspondencia.Estado = "P";
                Correspondencia.UsuarioCreacion = "Admin";
                string fechaC = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Correspondencia.FechaCreacion = fechaC;
                Correspondencia.Ip = this.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            //REALIZA LA INSERCION A LA TABLA WEBDOCDIGITAL
                var RegCorrespondencia = CorrespondenciaRepository.InsertarCorrespondencia(Correspondencia);
                int idReg = 0;
                foreach (var Regc in RegCorrespondencia)
                {

                    idReg = Regc.Id;
                    if (idReg == 0)
                    {
                        return new OkObjectResult(new
                        {
                            status = "error",
                            message = "Error:"+ Regc.msg
                        });
                    }

                }
                int idRegistro = 0;
                idRegistro = idReg;
                string NombresArchivosMail = "";
            //INTRODUCE A LA TABLA WEBDOCDIGITALADJUINTOS
                foreach (var file in files)
                {
                    if (file.Length >= 0)
                    {
                        var IdWebDocDigitalFile = idRegistro;
                        var nombrerchivo = file.FileName;
                        var Tamanio = (file.Length.ToString());
                        var Ext = (nombrerchivo.Substring(nombrerchivo.LastIndexOf(".")));
                        var Tipofile = file.ContentType;
                        string fechaCA = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        var FechaCreacionA = fechaCA;
                        MemoryStream ms = new MemoryStream();
                        file.CopyTo(ms);
                        var ArchivoData = ms.ToArray();
                        NombresArchivosMail = NombresArchivosMail + "," + nombrerchivo;
                        var RegCorrespondenciaAdjunto = CorrespondenciaRepository.InsertarCorrespondenciaAdjunto(new CorrespondenciaAdjunto
                        {
                            IdWebDocDigital = IdWebDocDigitalFile,
                            Archivo = ArchivoData,
                            NombreArchivo = nombrerchivo,
                            Tamano = Tamanio,
                            Extension = Ext,
                            Tipo = Tipofile,
                            UsuarioCreacion = "Admin",
                            FechaCreacion = fechaCA,
                            UsuarioModificacion = "Admin",
                            FechaModificacion = fechaCA,
                        });
                        if (RegCorrespondenciaAdjunto.Count() == 0)
                        {
                            return new OkObjectResult(new
                            {
                                status = "error",
                                message = "Error en la Carga de Archivos " 
                            });
                        }
                        ms.Close();
                        ms.Dispose();

                    }
                }
            
            //REALIZA EL ENVIO DEL CORREO A LOS DESTINATARIOS
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("appsettings.json");
                string destinatario = "";
                destinatario = Correspondencia.CorreoElectronico.ToString();
                var CorrespondenciaEmailConfig = builder.Build();
                var EmailFrom = CorrespondenciaEmailConfig["Correspondencia:EmailFrom"];
                var EmailFromUser = CorrespondenciaEmailConfig["Correspondencia:EmailFromUser"];
                var EmailTo = CorrespondenciaEmailConfig["Correspondencia:EmailTo"];
                var EmailToCC = destinatario;
                var EmailToUser = CorrespondenciaEmailConfig["Correspondencia:EmailToUser"];
                var MailServer = CorrespondenciaEmailConfig["Correspondencia:MailServer"];
                int MailPort = Convert.ToInt16(CorrespondenciaEmailConfig["Correspondencia:MailPort"]);
                bool MailUseSsl = Convert.ToBoolean(CorrespondenciaEmailConfig["Correspondencia:MailUseSsl"]);
                var MailUser = CorrespondenciaEmailConfig["Correspondencia:MailUser"];
                var MailUserPassword = CorrespondenciaEmailConfig["Correspondencia:MailUserPassword"];
                var MailDefaultSubject = CorrespondenciaEmailConfig["Correspondencia:MailDefaultSubject"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(EmailFromUser, EmailFrom));

                message.To.Add(new MailboxAddress(EmailTo));
                message.To.Add(new MailboxAddress(EmailToCC));
                message.Subject = MailDefaultSubject;

                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                        Text = @"<b> id: </b>" + idRegistro + "<hr>" +
                   "<b> Nombres: </b> " + Correspondencia.Nombres + "<br>" +
                   "<b> Apellidos: </b> " + Correspondencia.Apellidos + "<br>" +
                   "<b> Entidad: </b> " + Correspondencia.Entidad + "<br>" +
                   "<b> Cite: </b> " + Correspondencia.Cite + "<br>" +
                   "<b> Referencia: </b> " + Correspondencia.Referencia + "<hr>" +
                   "<b> Archivos: </b> " + NombresArchivosMail + "<hr>"
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
            //RESPUESTA POR PROCESO EXITOSO
            return new OkObjectResult(new
                {
                    status = "Correcto",
                    message = "Registrado correctamente"
                });
            }
            //RESPUESTA POR EXCEPCIONES Y PROCESO CON ERRORES
            catch (Exception ex)
            {
                return new OkObjectResult(new
                {
                    status = "error",
                    message = "Ocurrió un error: " + ex.Message
                });
            }
        }

        // GET Entidades
        [HttpGet("parametros")]
        public object GetParametros()
        {
            List<CorrespondenciaParametros> getParametros = new List<CorrespondenciaParametros>();
            var CorrespondenciaRepository = new DB.CorrespondenciaRepository();
            var Data = CorrespondenciaRepository.Parametros();
            foreach (var obj in Data)
            {
                getParametros.Add(obj);
            }
            return new WebResponse { Status = ResponseStatuses.Correcto.ToString(), Data = getParametros };
        }

        // GET Entidades
        [HttpGet("simple")]
        public object GetFormulario()
        {
            List<formTransparencia> formotro = new List<formTransparencia>();
            var WebRepository = new DB.WebRepository();
            var Data = WebRepository.FormularioTransparencia();
            foreach (var obj in Data)
            {
                formotro.Add(obj);
            }
            return new WebResponse { Status = ResponseStatuses.Correcto.ToString(), Data = formotro };
        }

        //TRANSPARENCIA

        [HttpGet("db")]
        public object GetFormulariodb()
        {
            List<formTransparencia> formotro = new List<formTransparencia>();
            var WebRepository = new DB.WebRepository();
            var Data = WebRepository.FormuTransparencia();
            foreach (var obj in Data)
            {
                formotro.Add(obj);
            }
            return new WebResponse { Status = ResponseStatuses.Correcto.ToString(), Data = formotro };
        }

        [HttpPost("formulario")]
        public object Post([FromBody] Models.formTransparencia _formTransparencia)
        {
            var result = this.webRepository.GuardarFormulario(_formTransparencia);

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var formTransparenciaEmailConfig = builder.Build();
            var EmailFrom = formTransparenciaEmailConfig["formTransparencia:EmailFrom"];
            var EmailFromUser = formTransparenciaEmailConfig["formTransparencia:EmailFromUser"];
            var EmailTo = formTransparenciaEmailConfig["formTransparencia:EmailTo"];
            var EmailToUser = formTransparenciaEmailConfig["formTransparencia:EmailToUser"];
            var MailServer = formTransparenciaEmailConfig["formTransparencia:MailServer"];
            int MailPort = Convert.ToInt16(formTransparenciaEmailConfig["formTransparencia:MailPort"]);
            bool MailUseSsl = Convert.ToBoolean(formTransparenciaEmailConfig["formTransparencia:MailUseSsl"]);
            var MailUser = formTransparenciaEmailConfig["formTransparencia:MailUser"];
            var MailUserPassword = formTransparenciaEmailConfig["formTransparencia:MailUserPassword"];
            var MailDefaultSubject = formTransparenciaEmailConfig["formTransparencia:MailDefaultSubject"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(EmailFromUser, EmailFrom));

            message.To.Add(new MailboxAddress(EmailToUser, EmailTo));
            message.Subject = MailDefaultSubject;

            message.Body = new TextPart("plain")
            {
                Text = @"id: " + _formTransparencia.id +
                "\nnombre_completo: " + _formTransparencia.nombre_completo +
                "\nci: " + _formTransparencia.ci +
                "\nfono: " + _formTransparencia.fono +
                "\ndireccion: " + _formTransparencia.direccion +
                "\nemail: " + _formTransparencia.email +
                "\notro: " + _formTransparencia.otro +
                "\ndescripcion1: " + _formTransparencia.descripcion1 +
                "\ndescripcion2: " + _formTransparencia.descripcion2 +
                "\ndescripcion3: " + _formTransparencia.descripcion3 +
                "\ndescripcion4: " + _formTransparencia.descripcion4
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

            return new WebResponse
            {
                Status = ResponseStatuses.Correcto.ToString(),
                Message = "Se ha guardado el registro.",
                Data = null
            };



        }

        [HttpPost("denuncia")]
        public object Postdenuncia([FromBody] Models.denunciaTransparencia _denunciaTransparencia)
        {

            var result = this.webRepository.GuardarDenuncia(_denunciaTransparencia);

            var descripcion = _denunciaTransparencia.descripcion;
            string[] separadas;

            separadas = descripcion.Split(';');

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var formTransparenciaEmailConfig = builder.Build();
            var EmailFrom = formTransparenciaEmailConfig["formTransparencia:EmailFrom"];
            var EmailFromUser = formTransparenciaEmailConfig["formTransparencia:EmailFromUser"];
            var EmailTo = formTransparenciaEmailConfig["formTransparencia:EmailTo"];
            var EmailToUser = formTransparenciaEmailConfig["formTransparencia:EmailToUser"];
            var MailServer = formTransparenciaEmailConfig["formTransparencia:MailServer"];
            int MailPort = Convert.ToInt16(formTransparenciaEmailConfig["formTransparencia:MailPort"]);
            bool MailUseSsl = Convert.ToBoolean(formTransparenciaEmailConfig["formTransparencia:MailUseSsl"]);
            var MailUser = formTransparenciaEmailConfig["formTransparencia:MailUser"];
            var MailUserPassword = formTransparenciaEmailConfig["formTransparencia:MailUserPassword"];
            var MailDefaultSubject = formTransparenciaEmailConfig["formTransparencia:MailDefaultSubject"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(EmailFromUser, EmailFrom));

            message.To.Add(new MailboxAddress(EmailToUser, EmailTo));
            message.Subject = MailDefaultSubject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<b> id: </b>" + _denunciaTransparencia.id + "<hr>" +
                "<b> Nombre Completo: </b> " + separadas[0] + "<br>" +
                "<b> Cedula de Identidad: </b> " + separadas[1] + "<br>" +
                "<b> Telefono o Celular: </b> " + separadas[2] + "<br>" +
                "<b> Direccion: </b> " + separadas[3] + "<br>" +
                "<b> Correo electronico: </b> " + separadas[4] + "<br>" +
                "<b> Otro: </b> " + separadas[5] + "<hr>" +
                "<b> Descripcion: </b> " + separadas[6] + "<br>" +
                "<b> Descripcion: </b> " + separadas[7] + "<br>" +
                "<b> Descripcion: </b> " + separadas[8] + "<br>" +
                "<b> Descripcion: </b> " + separadas[9] + "<hr>" +
                "<b> Fecha de Denuncia: </b>" + _denunciaTransparencia.fecha_creacion + "<hr>"


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

            return new WebResponse
            {
                Status = ResponseStatuses.Correcto.ToString(),
                Message = "Se ha guardado el registro.",
                Data = null
            };



        }

    }
}
