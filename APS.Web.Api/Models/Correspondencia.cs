using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.Models
{
        public class Correspondencia
        {
            public int Id { set; get; }
            public string Nombres { set; get; }
            public string Apellidos { set; get; }
            public string CorreoElectronico { set; get; }
            public string Entidad { set; get; }
            public string Cite { set; get; }
            public string Referencia { set; get; }
            public string Ip { set; get; }
            public string Estado { set; get; }
            public string UsuarioCreacion { set; get; }
            public string FechaCreacion { set; get; }
            public string UsuarioModificacion { set; get; }
            public string FechaModificacion { set; get; }
            public string msg { set; get; }
        }

    public class CorrespondenciaRegistro
        {
           public int Id { set; get; }
           public string msg { set; get; }
        }
    public class CorrespondenciaAdjunto
    {
        public int Id { set; get; }
        public int IdWebDocDigital { set; get; }
        public byte[] Archivo { set; get; }
        public string NombreArchivo { set; get; }
        public string Tamano { set; get; }
        public string Extension { set; get; }
        public string Tipo { set; get; }
        public string UsuarioCreacion { set; get; }
        public string FechaCreacion { set; get; }
        public string UsuarioModificacion { set; get; }
        public string FechaModificacion { set; get; }
        public string msg { set; get; }
    }

    public class CorrespondenciaRegistroAdjunto
    {
        public int Id { set; get; }
        public string msg { set; get; }
    }
    public class RecaptchaResponseCorrespondencia
    {
        public bool success { set; get; }
        public string challenge_ts { set; get; }
        public string hostname { set; get; }
    }
    public class CorrespondenciaParametros
    {
        public int Id { set; get; }
        public int IdGrupoParametros { set; get; }
        public string ValorString { set; get; }
        public int ValorNumber { set; get; }

    }


}
