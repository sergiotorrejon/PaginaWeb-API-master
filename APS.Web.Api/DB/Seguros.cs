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

    public class SegurosRepository
    {
        internal IDbConnection Connection
        {
            get { return new SqlConnection(AppSettings.ConnectionString_Seguros); }
        }

        public IEnumerable<Entidad> ListaEntidades()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Entidad>(
                    @"SELECT Sigla = tSigla, 
                                 Nombre = [tNombre], 
                                 TipoEntidad = tTE.[tDescripcion],
                                 'Seguros' as Mercado
                          FROM   [dbo].[rstEmpresas] AS tE 
                                 INNER JOIN [dbo].[rstTipoEntidad] as tTE 
                                    ON tE.cTipoEntidad = tTE.cTipoEntidad 
                          WHERE  [bHabilitado] = 'S'
                                 AND tSigla IS NOT NULL");
                return data;
            }
        }

        public Entidad GetEntidad(string sigla)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Entidad>(
                    @"SELECT Sigla = tSigla, 
                                 Nombre = [tNombre], 
                                 TipoEntidad = tTE.[tDescripcion],
                                 'Seguros' as Mercado
                          FROM   [dbo].[rstEmpresas] AS tE 
                                 INNER JOIN [dbo].[rstTipoEntidad] as tTE 
                                    ON tE.cTipoEntidad = tTE.cTipoEntidad 
                          WHERE  [bHabilitado] = 'S'
                                 AND tSigla = @Sigla ", new { Sigla = sigla }).SingleOrDefault();
                return data;
            }
        }
    }
    
}
