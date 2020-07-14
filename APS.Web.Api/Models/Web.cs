using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.Models
{
    public class formTransparencia
    {
        public string id { set; get; }

        public string nombre_completo { set; get; }
        public string ci { set; get; }
        public string fono { set; get; }
        public string direccion { set; get; }
        public string email { set; get; }
        public string otro { set; get; }
        public string descripcion1 { set; get; }
        public string descripcion2 { set; get; }
        public string descripcion3 { set; get; }
        public string descripcion4 { set; get; }

    }

    public class denunciaTransparencia
    {
        public string id { set; get; }
        public string fecha_creacion { set; get; }
        public string descripcion { set; get; }
        public int created_by { set; get; }
        public int state { set; get; }
        public int ordering { set; get; }
    }
}
