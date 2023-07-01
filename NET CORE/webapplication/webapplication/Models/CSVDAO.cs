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
using System.Dynamic;

namespace webapplication.Models
{
    public class CSVDAO
    {

        public CSVDAO(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public DataTable GetDatosCSV(String p_accion, String p_data, String p_codes)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_GET_CSV";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = p_accion;
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;
                objCmd.Parameters.Add("P_CODES", OracleDbType.Varchar2).Value = p_codes;
                objCmd.Parameters.Add("CURSOR_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    OracleDataReader objReader = objCmd.ExecuteReader();
                    dataTable = new DataTable();
                    dataTable.Load(objReader);


                }
                catch (Exception ex)
                {
                    dataTable = null;
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                }
                objConn.Close();
            }
            return dataTable;
        }

        public DataTable GetDataCSV(String p_accion, String p_data, String p_codes)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = p_accion;
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;
                objCmd.Parameters.Add("P_CODES", OracleDbType.Varchar2).Value = p_codes;
                objCmd.Parameters.Add("CURSOR_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    OracleDataReader objReader = objCmd.ExecuteReader();
                    dataTable = new DataTable();
                    dataTable.Load(objReader);


                }
                catch (Exception ex)
                {
                    dataTable = null;
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                }
                objConn.Close();
            }
            return dataTable;
        }

        public DataTable GetDataCARGA(String p_accion, String p_data, String p_codes)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CARGA";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = p_accion;
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;
                objCmd.Parameters.Add("P_CODES", OracleDbType.Varchar2).Value = p_codes;
                objCmd.Parameters.Add("CURSOR_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    OracleDataReader objReader = objCmd.ExecuteReader();
                    dataTable = new DataTable();
                    dataTable.Load(objReader);


                }
                catch (Exception ex)
                {
                    dataTable = null;
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                }
                objConn.Close();
            }
            return dataTable;
        }



        public List<string> SetCSVLog(string p_accion, string p_data, string p_request, string p_response)
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PRH_SET_CSVLOG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = p_accion;
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;
                objCmd.Parameters.Add("P_REQUEST", OracleDbType.Varchar2).Value = p_request;
                objCmd.Parameters.Add("P_RESPONSE", OracleDbType.Varchar2).Value = p_response;

                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_NEWID", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    msg = objCmd.Parameters["V_MSG"].Value.ToString();
                    result = objCmd.Parameters["V_RESULT"].Value.ToString();
                    newid = objCmd.Parameters["V_NEWID"].Value.ToString();

                    Result.Add(msg);
                    Result.Add(result);
                    Result.Add(newid);

                }
                catch (Exception ex)
                {
                    Result.Add("2");
                    Result.Add(ex.Message.ToString());
                    Result.Add("0");
                }
                objConn.Close();
            }

            return Result;
        }


        public String GetDataInsetCSVLog2(dynamic request, String procedimiento, String newid, String resultado, String mensaje, String file = "")
        {

            var converter = new ExpandoObjectConverter();
            var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(request.ToString(), converter) as dynamic;


            TokenData TokenData = new TokenData();
            Token Token = new Token(Configuration);
            TokenData = Token.GetDataFromToken(jsondata.jwt);




            dynamic jsonObject = new JObject();
            jsonObject.p_accion = jsondata.p_accion;
            jsonObject.p_tabla = jsondata.p_tabla;
            jsonObject.p_procedimiento = procedimiento;
            jsonObject.p_usua_id_reg = TokenData.user_id;
            jsonObject.p_code = newid;
            jsonObject.p_resultado = resultado;
            jsonObject.p_mensaje = mensaje;
            jsonObject.p_file = file;


            string json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);

            return json;
        }

        public String GetDataInsetCSVLog(string request, String token, String procedimiento, String newid, String resultado, String mensaje)
        {

            TokenData TokenData = new TokenData();
            Token Token = new Token(Configuration);
            TokenData = Token.GetDataFromToken(token);

            // var converter = new ExpandoObjectConverter();
            // var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(request.ToString(), converter) as dynamic;


            // dynamic jsonObject = new JObject();
            // //jsonObject.p_accion = jsondata.p_accion;
            //// jsonObject.p_tabla = jsondata.p_tabla;
            // jsonObject.p_procedimiento = procedimiento;
            // jsonObject.p_usua_id_reg = TokenData.user_id;
            // jsonObject.p_code = newid;
            // jsonObject.p_resultado = resultado;
            // jsonObject.p_mensaje = mensaje;


            string json = "";// Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject);

            return json;
        }

        public String GetDataUpdateCSVLog(String newid, String resultado, String mensaje)
        {

            dynamic jsonObject = new JObject();
            jsonObject.p_code = newid;
            jsonObject.p_resultado = resultado;
            jsonObject.p_mensaje = mensaje;

            var obj = JsonConvert.DeserializeObject<dynamic>(jsonObject);

            return obj;
        }

    }
}
