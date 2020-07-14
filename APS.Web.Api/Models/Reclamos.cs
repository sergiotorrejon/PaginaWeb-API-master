using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.Models
{
    public class Reclamos
    {
        public int? IdReclamoWeb { set; get; }
        public DateTime FechaReclamo { set; get; }
        public string TipoIdentificacion { set; get; }
        public string NumIdentificacion { set; get; }
        public string ExpIdentificacion { set; get; }
        public string NombreCompleto { set; get; }
        public string Celular { set; get; }
        public string CorreoElectronico { set; get; }
        public string Direccion { set; get; }
        public string DescripcionReclamo { set; get; }
        public string CodigoEntidad { set; get; }
        public string Estado { set; get; }
        public string Ip { set; get; }
        public int? IdReclamo { set; get; }
        public string UsuarioCreacion { set; get; }
        public DateTime FechaCreacion { set; get; }
        public string UsuarioModificacion { set; get; }
        public DateTime FechaModificacion { set; get; }
    }

    public class BuscarReclamo
    {
        public int IdReclamo { set; get; }
        public string NumIdentificacion { set; get; }
    }

    public class ListaReclamos
    {
        public int? IdReclamoWeb { set; get; }
        public DateTime FechaReclamo { set; get; }
        public string NumIdentificacion { set; get; }
        public string NombreCompleto { set; get; }
        public string Celular { set; get; }
        public string CorreoElectronico { set; get; }
        public string Direccion { set; get; }
        public string DescripcionReclamo { set; get; }
        public string NombreEntidad { set; get; }
        public string Estado { set; get; }
        public int? IdReclamo { set; get; }
    }

    public class RecaptchaResponse
    {
        public bool success { set; get; }
        public string challenge_ts { set; get; }
        public string hostname { set; get; }
    }
}
