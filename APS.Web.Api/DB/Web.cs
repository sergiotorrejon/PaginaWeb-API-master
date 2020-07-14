using APS.Web.Api.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace APS.Web.Api.DB
{
    public class WebRepository
    {
        internal IDbConnection Connection
        {
            get { return new MySqlConnection(AppSettings.ConnectionString_Web); }
        }

        public IEnumerable<formTransparencia> FormularioTransparencia()
        {
            List<formTransparencia> formulario = new List<formTransparencia>();

            formulario.Add(new formTransparencia { id = "4", nombre_completo = "FUT"});
           

            return formulario;
        }

        public IEnumerable<formTransparencia> FormuTransparencia()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var data = dbConnection.Query<formTransparencia>(
                     @"SELECT  id,nombre_completo
                    FROM aps_formulario_transparencia");
                return data;
            }
        }

        public bool GuardarFormulario(formTransparencia obj)
        {
            var affectedRows = 0;
            string sql = @"INSERT INTO aps_formulario_transparencia (
            id,nombre_completo,ci,fono,direccion,email,otro,descripcion1,descripcion2,descripcion3,descripcion4)
            VALUES (@id,@nombre_completo,@ci,@fono,@direccion,@email,@otro,@descripcion1,@descripcion2,@descripcion3,@descripcion4);";

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                affectedRows = dbConnection.Execute(sql, obj);
            }

            return !(affectedRows == 0);
        }

        public bool GuardarDenuncia(denunciaTransparencia obj)
        {
            var affectedRows = 0;
            string sql = @"INSERT INTO aps_transparencia_denuncias (
            id, descripcion,fecha_creacion,created_by,state,ordering)
            VALUES (@id,hex(aes_encrypt(@descripcion,'sergio')),@fecha_creacion,@created_by,@state,@ordering);";

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                affectedRows = dbConnection.Execute(sql, obj);
            }

            return !(affectedRows == 0);
        }

    }
}
