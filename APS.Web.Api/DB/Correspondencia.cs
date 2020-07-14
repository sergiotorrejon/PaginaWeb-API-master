using APS.Web.Api.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace APS.Web.Api.DB
{
    public class CorrespondenciaRepository
    {
        internal IDbConnection Connection
        {
            get { return new SqlConnection(AppSettings.ConnectionString_Correspondencia); }
        }

        public IEnumerable<Models.CorrespondenciaRegistro> InsertarCorrespondencia(Models.Correspondencia obj)
        {

            using (IDbConnection dbConnection = Connection)
            {

                dbConnection.Open();
                var data = dbConnection.Query<Models.CorrespondenciaRegistro>(
                    @"                    
                    USE [dbCorrespondencia] 

                    DECLARE	
		                    @IdSp int,
		                    @msgSp nvarchar(100)

                    EXEC	[dbo].[SP_WEBDOCDIGITAL_INS]
		                    @NombresSp = @Nombres,
		                    @ApellidosSp = @Apellidos,
		                    @CorreoElectronicoSp = @CorreoElectronico,
		                    @EntidadSp = @Entidad,
		                    @CiteSp = @Cite,
		                    @ReferenciaSp = @Referencia,
		                    @IpSp = @Ip,
		                    @EstadoSp = @Estado,
		                    @UsuarioCreacionSp = @UsuarioCreacion,
		                    @FechaCreacionSp = @FechaCreacion,
		                    @UsuarioModificacionSp = @UsuarioModificacion,
		                    @FechaModificacionSp = @FechaModificacion,
		                    @IdSp = @IdSp OUTPUT,
		                    @msgSp = @msgSp OUTPUT

                    SELECT	@IdSp as Id,
		                    @msgSp as msg
                    RETURN  ",
                new
                {
                    Nombres = obj.Nombres,
                    Apellidos = obj.Apellidos,
                    CorreoElectronico = obj.CorreoElectronico,
                    Entidad = obj.Entidad,
                    Cite = obj.Cite,
                    Referencia = obj.Referencia,
                    Ip = obj.Ip,
                    Estado = obj.Estado,
                    UsuarioCreacion = obj.UsuarioCreacion,
                    FechaCreacion = obj.FechaCreacion,
                    UsuarioModificacion = obj.UsuarioModificacion,
                    FechaModificacion = obj.FechaModificacion,
                });
                return data;
            }
        }

        public IEnumerable<Models.CorrespondenciaRegistroAdjunto> InsertarCorrespondenciaAdjunto(Models.CorrespondenciaAdjunto obj)
        {

            using (IDbConnection dbConnection = Connection)
            {

                dbConnection.Open();
                var data = dbConnection.Query<Models.CorrespondenciaRegistroAdjunto>(
                    @"                    
                    USE [dbCorrespondencia] 

                    DECLARE	
		                    @IdSpA int,
		                    @msgSpA nvarchar(100)

                    EXEC	[dbo].[SP_WEBDOCDIGITALADJUNTO_INS]
		                    @IdWebDocDigitalSpA = @IdWebDocDigital,
		                    @ArchivoSpA = @Archivo,
		                    @NombreArchivoSpA = @NombreArchivo,
		                    @TamanoSpA = @Tamano,
		                    @ExtensionSpA = @Extension,
		                    @TipoSpA = @Tipo,
		                    @UsuarioCreacionSpA = @UsuarioCreacion,
		                    @FechaCreacionSpA = @FechaCreacion,
		                    @UsuarioModificacionSpA = @UsuarioModificacion,
		                    @FechaModificacionSpA = @FechaModificacion,
		                    @IdSpA = @IdSpA OUTPUT,
		                    @msgSpA = @msgSpA OUTPUT

                    SELECT	@IdSpA as Id,
		                    @msgSpA as msg
                    RETURN  ",
                new
                {
                    IdWebDocDigital = obj.IdWebDocDigital,
                    Archivo = obj.Archivo,
                    NombreArchivo = obj.NombreArchivo,
                    Tamano = obj.Tamano,
                    Extension = obj.Extension,
                    Tipo = obj.Tipo,
                    UsuarioCreacion = obj.UsuarioCreacion,
                    FechaCreacion = obj.FechaCreacion,
                    UsuarioModificacion = obj.UsuarioModificacion,
                    FechaModificacion = obj.FechaModificacion,
                });
                return data;
            }
        }

        public IEnumerable<CorrespondenciaParametros> Parametros()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<CorrespondenciaParametros>(
                     @"SELECT  id,idgrupoparametros,valorstring,valornumber
                    FROM dbo.setparametros
                    ORDER BY id ASC");
                return data;
            }
        }
    }
}
