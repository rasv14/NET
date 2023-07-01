
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
    public class SecurityDAO
    {

        public SecurityDAO(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public ResponseDB verificaAccesoForma(string p_id, string p_forma)
        {
            int success_;

            string message_,forma_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PYG_ACCESO_FORMA";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ID", OracleDbType.Varchar2).Value = p_id;//SILABO_MIGRO
                objCmd.Parameters.Add("P_FORMA", OracleDbType.Varchar2).Value = p_forma;
                objCmd.Parameters.Add("R_FORMA", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("R_ACCESO", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("R_MENSAJE", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    
                    forma_ = objCmd.Parameters["R_FORMA"].Value.ToString();
                    message_ = objCmd.Parameters["R_MENSAJE"].Value.ToString();

                    if (forma_ != "SC")
                    {
                        success_ = (int)ResponseCode.R200;
                    }
                    else {
                        success_ = (int)ResponseCode.R500;
                    }

                   
                    objConn.Close();

                    return new ResponseDB { success = success_, message = message_ };
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
