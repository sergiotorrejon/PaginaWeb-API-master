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
    public class ReclamosRepository
    {
        internal IDbConnection Connection
        {
            get { return new SqlConnection(AppSettings.ConnectionString_Reclamos); }
        }
        
        public bool GuardarReclamo(Models.Reclamos obj)
        {
            var affectedRows = 0;
            string sql = @"INSERT INTO ReclamosWeb (
            TipoIdentificacion, NumIdentificacion, ExpIdentificacion, NombreCompleto, Celular, 
            CorreoElectronico, Direccion, DescripcionReclamo, CodigoEntidad, FechaReclamo,
            Estado, UsuarioCreacion, FechaCreacion, Ip)
            VALUES (
            @TipoIdentificacion, @NumIdentificacion, @ExpIdentificacion, @NombreCompleto, @Celular, 
            @CorreoElectronico, @Direccion, @DescripcionReclamo, @CodigoEntidad, @FechaReclamo,
            @Estado, @UsuarioCreacion, @FechaCreacion, @Ip);";

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                affectedRows = dbConnection.Execute(sql, obj);
            }

            return !(affectedRows == 0);
        }

        public IEnumerable<Models.ListaReclamos> BuscarReclamo(Models.BuscarReclamo obj)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Models.ListaReclamos>(
                    @"SELECT  t1.IdReclamo, t1.Descripcion as DescripcionReclamo,
                    t4.Descripcion + ' ' + t3.RecNumeroDocumento AS NumIdentificacion,
                    t3.RecNombres + ' ' + t3.RecApellidoPaterno + ' ' + t3.RecApellidoMaterno AS NombreCompleto,
                    t2.Descripcion AS NombreEntidad,
                    t1.FechaReclamo, t1.Estado
                    FROM dbo.Reclamos t1
                    LEFT JOIN dbo.DetalleOperador t2 ON t1.IdOperador = t2.IdDetalleOperador
                    LEFT JOIN dbo.Reclamante t3 ON t1.IdReclamante = t3.IdReclamante
                    LEFT JOIN dbo.DetalleClasificador t4 ON t3.idTipoDocumento = t4.IdDetalleClasificador
                    WHERE t1.IdReclamo = @IdReclamo AND t3.RecNumeroDocumento = @NumIdentificacion",
                    new { IdReclamo = obj.IdReclamo, NumIdentificacion = obj.NumIdentificacion });
                return data;
            }
        }
        
        public Reclamos GetReclamoByIp(string ip)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Reclamos>(
                    @"SELECT TOP 1 t1.IdReclamoWeb, t1.DescripcionReclamo,
                    t1.NumIdentificacion,
                    t1.NombreCompleto,
                    t1.FechaReclamo, t1.FechaCreacion, t1.Estado, t1.Ip
                    FROM dbo.ReclamosWeb t1
                    WHERE t1.ip = @Ip
                    ORDER BY t1.FechaCreacion DESC",
                    new { Ip =ip }).SingleOrDefault();
                return data;
            }
        }
    }
}
