using System;

namespace APS.Web.Api.Models
{
    public class Documentos
    {
        public string TipoDocumento { set; get; }
        public string Numero { set; get; }
        public string Gestion { set; get; }
        public string Institucion { set; get; }
        public DateTime Fecha { set; get; }
        public string Titulo { set; get; }
        public string TamanioArchivo { set; get; }
        public string UrlArchivo { set; get; }
    }

    public class Notificacion
    {
        public string Ciudad { set; get; }
        public string Fecha { set; get; }
        public string Hora { set; get; }
        public string A { set; get; }
        public string De { set; get; }
    }
}
