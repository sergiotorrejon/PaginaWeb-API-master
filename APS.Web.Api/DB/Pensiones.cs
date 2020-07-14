using APS.Web.Api.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.DB
{
    public class PensionesRepository
    {
        //internal IDbConnection Connection
        //{
        //    get { return new SqlConnection(AppSettings.ConnectionString_Seguros); }
        //}

        public IEnumerable<Entidad> ListaEntidades()
        {
            List<Entidad> entidades = new List<Entidad>();
            
            entidades.Add(new Entidad { Nombre = "Futuro de Bolivia S.A. AFP", Sigla = "FUT", TipoEntidad = "AFP", Mercado = "Pensiones" });
            entidades.Add(new Entidad { Nombre = "BBVA Previsión AFP S.A. ", Sigla = "PRE", TipoEntidad = "AFP", Mercado = "Pensiones" });

            return entidades;
        }

        public Entidad GetEntidad(string sigla)
        {
            Entidad obj = null;
            switch (sigla)
            {
                case "FUT":
                    obj = new Entidad { Nombre = "Futuro de Bolivia S.A. AFP", Sigla = "FUT", TipoEntidad = "AFP", Mercado = "Pensiones" };
                    break;
                case "PRE":
                    obj = new Entidad { Nombre = "BBVA Previsión AFP S.A. ", Sigla = "PRE", TipoEntidad = "AFP", Mercado = "Pensiones" };
                    break;
            }
                
            return obj;
        }
    }
    
}
