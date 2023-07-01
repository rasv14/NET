using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

            string MiConexion = "Data Source=(Local); Initial Catalog=LoginDB; Integrated Security=True";

            if (object.ReferenceEquals(MiConexion, string.Empty))
            {

                return string.Empty;
            }
            else
            {
                return MiConexion;
            }

        }
    }

}