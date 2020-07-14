using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.DB
{

    public class ApiKey
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Entidad { get; set; }
        public string Descripcion { get; set; }
        public bool Habilitado { get; set; }
    }

    public class ApiKeyRepository
    {
        internal IDbConnection Connection
        {
            get { return new SqlConnection(Models.AppSettings.ConnectionString_Api); }
        }

        public ApiKey FinByKey(string key)
        {
            using (IDbConnection dbConnection = Connection)
            {
                try
                {
                    dbConnection.Open();
                    var data = dbConnection.QuerySingle<ApiKey>("SELECT * FROM ApiKeys WHERE [Key] = @Key", new { Key = key });
                    return data;
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool AddLog(int apiKeyId, string remoteIp, string clientIP, string method, string path, string queryString, string resource)
        {
            using (IDbConnection dbConnection = Connection)
            {
                try
                {
                    dbConnection.Open();
                    dbConnection.Execute("INSERT INTO ApiLogs (ApiKeyId, Fecha, RemoteIP, ClientIP, Method, Path, QueryString, Resource) VALUES (@ApiKeyId, @Fecha, @RemoteIP, @ClientIP, @Method, @Path, @QueryString, @Resource)", new { ApiKeyId = apiKeyId, Fecha = DateTime.Now, RemoteIP = remoteIp, ClientIP = clientIP, Method = method, Path = path, QueryString = queryString, Resource = resource });

                    return true;
                }
                catch (Exception ex)
                {
                    var fff = ex.Message;
                    return false;
                }
            }
        }

    }
}
