using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using webapplication.Helpers;
using System.Dynamic;

namespace webapplication.Models
{
    public class ControlesDAO
    {

        public ControlesDAO(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public ResponseDB getNiveles()
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_NIVELES";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("CURSOR_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    OracleDataReader objReader = objCmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(objReader);
                    success_ = (int)ResponseCode.R200;
                    message_ = "OK";
                    objConn.Close();


                    if (dt == null) { return new ResponseDB { success = (int)ResponseCode.R500, message = "No hay datos" }; }
                    if (dt.Rows.Count == 0) { return new ResponseDB { success = (int)ResponseCode.R500, message = "No hay datos" }; }

                    return new ResponseDB { success = success_, message = message_, data = Conversiones.DataTableToJson(dt), datatable = dt };
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                    success_ = (int)ResponseCode.R500;
                    message_ = ex.Message;
                    objConn.Close();
                    return new ResponseDB { success = success_, message = message_ };
                }

            }

        }



    }
}
