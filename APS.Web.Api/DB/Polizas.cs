using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Data;
using APS.Web.Api.Models;

namespace APS.Web.Api.DB
{
    public class Polizas
    {
        internal IDbConnection Connection
        {
            get { return new SqlConnection(AppSettings.ConnectionString_Sipof); }
        }

        public string GetFinformacion()
        {       
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var result = dbConnection.ExecuteScalar<string>(@"SELECT dbo.GetRepFecha()");
               /*var data = dbConnection.Query<Poliza>(
                    @"SELECT dbo.GetRepFecha()");*/

                /*var data = dbConnection.Query<Poliza>(
                    @" exec WEB_PolConsulta '65012828'");*/
                return result;
            }

        }

        public IEnumerable<Poliza> GetPoliza(string nro_poliza)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<Poliza>(
                    @" SELECT E.Descripcion [Entidad], Npoliza,       		
             Ntomador, Tdocumento , IDTomador,
			 Lexpedicion, Fafianzado, Nbeneficiario, 
			 CASE Tbeneficiario 
                 WHEN 'E' THEN 'Estatal' 
				 WHEN 'M' THEN 'Mixto'
			     WHEN 'P' THEN 'Privado'
			     ELSE 'N/A'
			 END [Tbeneficiario], 
			 CONVERT(VARCHAR(10),FIVigencia,103)[FIVigencia], CONVERT(VARCHAR(10),FFVigencia,103)[FFVigencia], 
             CASE Nramo 
                WHEN 21 THEN 'Seriedad de Presentación de Propuesta'
                WHEN 22 THEN 'Cumplimiento de Contrato de Obra'
                WHEN 23 THEN 'Buena Ejecución de Obra'
                WHEN 24 THEN 'Cumplimiento de Contrato de Servicios'
                WHEN 25 THEN 'Cumplimiento de Contrato de Suministros'
                WHEN 26 THEN 'Correcta Inversión de Anticipos'
                WHEN 27 THEN 'Fidelidad de empleados'
                WHEN 28 THEN 'Póliza de Seguros de Créditos'
                WHEN 29 THEN 'Garantía de Cumplimiento de Obligaciones Aduaneras'
                WHEN 30 THEN 'Cumplimiento de Obligaciones y/o Derechos Contractuales'
                ELSE 'N/A'
             END [Nramo], 
			 M.Moneda_Sig [Moneda],			
			 LEFT( CONVERT(VARCHAR, CAST(Mcaucionado AS MONEY), 1), LEN(CONVERT(VARCHAR, CAST(Mcaucionado AS MONEY), 1)))  [Mcaucionado]
	    FROM dbo.VpoPolizasFianza P INNER JOIN dbo.VpoEntidades E ON P.CodVen= E.CodVen
	   INNER JOIN dbo.Cla_Monedas M ON M.Moneda=P.CMoneda	
	   WHERE Npoliza= @Nro_poliza",new { Nro_poliza = nro_poliza });
                
                /*var data = dbConnection.Query<Poliza>(
                    @" exec WEB_PolConsulta '65012828'");*/
                return data;
            }

        }

    }
}
