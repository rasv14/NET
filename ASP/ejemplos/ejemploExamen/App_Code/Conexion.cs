using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;

namespace nmConexion
{

    /// <summary>
    /// Descripción breve de Conexion
    /// </summary>
    public class Conexion
    {
        public Conexion()
        {
            //
            // TODO: Agregar aquí la lógica del constructor
            //
        }

        public string GetConexion()
        {

            string Miconexion = "Data Source=(Local); Initial Catalog=LoginDB; Integrated Security=True";

            if (object.ReferenceEquals(Miconexion, string.Empty))
            {
                return string.Empty;

            }
            else
            {
                return Miconexion;
            }

        }
    }
}