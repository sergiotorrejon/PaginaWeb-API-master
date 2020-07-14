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

    public class DocumentosRepository
    {
        internal IDbConnection Connection
        {
            get { return new SqlConnection(AppSettings.ConnectionString_Docs); }
        }

        public int ContarDocumentos(string gestion, string institucion, string mercado, string tipoDocumento, string categoria, string titulo, string numero)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                int count = dbConnection.Query<int>(
                    @"SELECT count(*) FROM dbo.Cartas_Resoluciones
                    LEFT JOIN dbo.crTabla t2 ON rc_subtipo = t2.cod_ele
                    WHERE ('' = @Gestion or rc_year = @Gestion)
                    AND ('' = @Institucion OR rc_inten = @Institucion)
                    AND ('' = @Numero OR rc_numero = RIGHT('0000'+ CAST(@Numero AS VARCHAR(4)),4))
                    AND rc_titulo LIKE @Titulo
                    AND ('' = @TipoDocumento OR rc_tipo = @TipoDocumento)
                    AND ('' = @Mercado OR rc_mercado = @Mercado)
                    AND ('' = @Categoria OR t2.cod_superior = @Categoria)",
                    new { Gestion = gestion, Institucion = institucion, Mercado = mercado, Numero = numero, Titulo = "%" + titulo + "%", TipoDocumento = tipoDocumento, Categoria = categoria }).FirstOrDefault();
                return count;
            }
        }

        public IEnumerable<Documentos> ListaDocumentos(int itemsPerPage, int pagenumber, string gestion, string institucion, string mercado, string tipoDocumento, string categoria, string titulo, string numero)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Documentos>(
                @"DECLARE @FirstRow INT, @LastRow INT, @PageNumber INT, @PageSize INT
                    SET @PageNumber= @pPageNumber
                    SET @PageSize = @pPageSize

                    SELECT  @FirstRow   = ((@PageNumber - 1) * @PageSize) + 1,
                            @LastRow    = ((@PageNumber - 1) * @PageSize) + @PageSize
                    ;
                    WITH CTE AS
                    (
                        SELECT rc_tipo AS TipoDocumento, rc_numero AS Numero, rc_year AS Gestion, 'APS' AS Institucion,
                               rc_fecha AS Fecha, rc_titulo AS Titulo, rc_filesize AS TamanioArchivo, rc_fileName AS UrlArchivo, 
                               ROW_NUMBER() OVER (ORDER BY rc_fecha DESC) as RowNumber 
                        FROM dbo.Cartas_Resoluciones
                        LEFT JOIN dbo.crTabla t2 ON  rc_subtipo = t2.cod_ele
                        WHERE ('' = @Gestion or rc_year = @Gestion)
                            AND ('' = @Institucion OR rc_inten = @Institucion)
                            AND ('' = @Numero OR rc_numero = RIGHT('0000'+ CAST(@Numero AS VARCHAR(4)),4))
                            AND rc_titulo LIKE @Titulo
                            AND ('' = @TipoDocumento OR rc_tipo = @TipoDocumento)
                            AND ('' = @Mercado OR rc_mercado = @Mercado)
                            AND ('' = @Categoria OR t2.cod_superior = @Categoria)
                    )
                    SELECT *, (SELECT COUNT(*) FROM CTE) AS TotalRecords
                    FROM CTE
                    WHERE RowNumber BETWEEN @FirstRow AND @LastRow
                    ORDER BY RowNumber ASC",
                new { @pPageNumber = pagenumber, @pPageSize = itemsPerPage, Gestion = gestion, Institucion = institucion, Mercado = mercado, Numero = numero, Titulo = "%" + titulo + "%", TipoDocumento = tipoDocumento, Categoria = categoria });
                return data;
            }
        }
        
        /*
        public int ContarDocumentosHistorico(string gestion, string institucion, string tipoDocumento, string titulo, string numero)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                int count = dbConnection.Query<int>(
                @"SELECT count(*)
                    FROM dbo.Cartas_Resoluciones
                    WHERE ('' = @Gestion or rc_year = @Gestion)
                            AND ('' = @Institucion OR rc_inten = @Institucion)
                            AND ('' = @Numero OR rc_numero = RIGHT('0000'+ CAST(@Numero AS VARCHAR(4)),4))
                            AND rc_titulo LIKE @Titulo
                            AND ('' = @TipoDocumento OR rc_tipo = @TipoDocumento)",
                new { Gestion = gestion, Institucion = institucion, Numero = numero, Titulo = "%" + titulo + "%", TipoDocumento = tipoDocumento }).FirstOrDefault();
                return count;
            }
        }

        public IEnumerable<Documentos> ListaDocumentosHistorico(int itemsPerPage, int pagenumber, string gestion, string institucion, string tipoDocumento, string titulo, string numero)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Documentos>(
                @"DECLARE @FirstRow INT, @LastRow INT, @PageNumber INT, @PageSize INT
                    SET @PageNumber= @pPageNumber
                    SET @PageSize = @pPageSize

                    SELECT  @FirstRow   = ((@PageNumber - 1) * @PageSize) + 1,
                            @LastRow    = ((@PageNumber - 1) * @PageSize) + @PageSize
                    ;
                    WITH CTE AS
                    (
                        SELECT  rc_tipo AS TipoDocumento, rc_numero AS Numero, rc_year AS Gestion, rc_inten AS Institucion,
                                rc_fecha AS Fecha, rc_titulo AS Titulo, rc_filesize AS TamanioArchivo, rc_fileName AS UrlArchivo
                               , ROW_NUMBER() OVER (ORDER BY rc_fecha DESC) as RowNumber 
                        FROM dbo.Cartas_Resoluciones
                        WHERE ('' = @Gestion or rc_year = @Gestion)
                            AND ('' = @Institucion OR rc_inten = @Institucion)
                            AND ('' = @Numero OR rc_numero = RIGHT('0000'+ CAST(@Numero AS VARCHAR(4)),4))
                            AND rc_titulo LIKE @Titulo
                            AND ('' = @TipoDocumento OR rc_tipo = @TipoDocumento)
                    )
                    SELECT *, (SELECT COUNT(*) FROM CTE) AS TotalRecords
                    FROM CTE
                    WHERE RowNumber BETWEEN @FirstRow AND @LastRow
                    ORDER BY RowNumber ASC",
                new { @pPageNumber = pagenumber, @pPageSize = itemsPerPage, Gestion = gestion, Institucion = institucion, Numero = numero, Titulo = "%" + titulo + "%", TipoDocumento = tipoDocumento });
                return data;
            }
        }
        /**/

        public IEnumerable<Notificacion> ListaNotificaciones(string institucion, string tipoDocumento, string gestion, string numero)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Notificacion>(
                @"SELECT t_ciudad as Ciudad, convert(varchar(10), t_fecha, 103) as Fecha,
                t_hora as Hora, t_aquien as A, t_atraves as De
                FROM dbo.Notificacion
                WHERE t_inten = @Institucion
                and t_year = @Gestion
                and t_tipo = @TipoDocumento
                and t_numero = RIGHT('0000'+ CAST(@Numero AS VARCHAR(4)),4)",
                new { Institucion = institucion, Gestion = gestion, TipoDocumento = tipoDocumento,  Numero = numero });
                return data;
            }
        }

    }
}
