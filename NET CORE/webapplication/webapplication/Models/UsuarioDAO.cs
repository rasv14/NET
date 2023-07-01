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
using webapplication.Helpers;

namespace webapplication.Models
{
    public class UsuarioDAO
    {

        public UsuarioDAO(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }



        public DataTable GetSeccionesCargados(String p_periodo, String p_carrera, String p_ind_migrado = "", String p_sis = "")
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_SEC_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_CARRERA", OracleDbType.Varchar2).Value = p_carrera;
                objCmd.Parameters.Add("P_IND_MIGRADO", OracleDbType.Varchar2).Value = p_ind_migrado;
                objCmd.Parameters.Add("P_SIS", OracleDbType.Varchar2).Value = p_sis;
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
        public DataTable GetUsuariosCargados(String p_id="", String p_tipo="", String p_ind_migrado = "")
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();
             
                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_USUARIOS_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ID", OracleDbType.Varchar2).Value = p_id;
                objCmd.Parameters.Add("P_TIPO", OracleDbType.Varchar2).Value = p_tipo;
                objCmd.Parameters.Add("P_IND_MIGRADO", OracleDbType.Varchar2).Value = p_ind_migrado;             
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

        public DataTable GetUsuariosNoCargados(String p_periodo, String p_tipo)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_USU_PROG_MASIVO";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_TIPO", OracleDbType.Varchar2).Value = p_tipo;
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


        public List<string> SincronizarBannerUsuariosMasivo(String p_periodo, String p_tipo, String p_entorno, String p_usuario )
        {
            List<string> Result = new List<string>();
            String msg, result, newid;

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();
         
                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_USU_SINC_BANNER_MASIVO";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_TIPO", OracleDbType.Varchar2).Value = p_tipo;
                objCmd.Parameters.Add("P_ENTORNO", OracleDbType.Varchar2).Value = p_entorno;
                objCmd.Parameters.Add("P_USUARIO", OracleDbType.Varchar2).Value = p_usuario;
                objCmd.Parameters.Add("CURSOR_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    OracleDataReader objReader = objCmd.ExecuteReader();
                    dataTable = new DataTable();
                    dataTable.Load(objReader);


                    Result.Add("200");
                    Result.Add("Se sincronizó con éxito.");

                }
                catch (Exception ex)
                {
                    Result.Add("500");
                    Result.Add(ex.Message.ToString());
                }
                objConn.Close();
            }
            return Result;
        }

        public List<string> SincronizarBannerUsuariosIndividual(String p_data)
        {
            List<string> Result = new List<string>();
            String msg, result, newid;

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_USU_SINC_BANNER_INDIV";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;

                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    msg = objCmd.Parameters["V_MSG"].Value.ToString();
                    result = objCmd.Parameters["V_RESULT"].Value.ToString();

                    Result.Add(result);
                    Result.Add(msg);
                    

                }
                catch (Exception ex)
                {
                    Result.Add("500");
                    Result.Add(ex.Message.ToString());
                    
                }
                objConn.Close();
            }
            return Result;
        }

        public ResponseDB getUsuarioIndividual(string p_id, string p_tipo)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_USU_INDVIDUAL";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ID", OracleDbType.Varchar2).Value = p_id;
                objCmd.Parameters.Add("P_TIPO", OracleDbType.Varchar2).Value = p_tipo;
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

                    if (dt == null)
                    {
                        return new ResponseDB { success = (int)ResponseCode.R300, message = "No se encontro Usuario o ya ha sido Cargado" };
                    }

                    if (dt.Rows.Count == 0)
                    {
                        return new ResponseDB { success = (int)ResponseCode.R300, message = "No se encontro Usuario o ya fue Cargado" };
                    }

                    return new ResponseDB { success = success_, message = message_, data = dt };
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                    success_ = (int)ResponseCode.R500;
                    message_ = "Error de BD";
                    objConn.Close();
                    return new ResponseDB { success = success_,  message = ex.Message.ToString() };
                }

            }

        }


        public DataTable GetUsuarioCargado(String p_code)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_USUARIO_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_CODE", OracleDbType.Varchar2).Value = p_code;
                
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


        public List<string> ActualizarUsuarioCargado(string p_accion, string p_data, string p_codes)
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_ACTUALIZAR_USUARIO_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = p_accion;
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;
                objCmd.Parameters.Add("P_CODES", OracleDbType.Varchar2).Value = p_codes;
                

                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    msg = objCmd.Parameters["V_MSG"].Value.ToString();
                    result = objCmd.Parameters["V_RESULT"].Value.ToString();
                    

                    Result.Add(msg);
                    Result.Add(result);
                    

                }
                catch (Exception ex)
                {
                    Result.Add("500");
                    Result.Add(ex.Message.ToString());
                    
                }
                objConn.Close();
            }

            return Result;
        }

        public List<string> ActualizarEstadoUsuarioCargado(string p_id, string p_estado)
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_ACTUALIZAR_ESTADO_USU_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ID", OracleDbType.Varchar2).Value = p_id;
                objCmd.Parameters.Add("P_ESTADO", OracleDbType.Varchar2).Value = p_estado;
              


                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    msg = objCmd.Parameters["V_MSG"].Value.ToString();
                    result = objCmd.Parameters["V_RESULT"].Value.ToString();


                   
                    Result.Add(result);
                    Result.Add(msg);

                }
                catch (Exception ex)
                {
                    Result.Add("500");
                    Result.Add(ex.Message.ToString());

                }
                objConn.Close();
            }

            return Result;
        }


        public List<string> SincronizarUsuarioCargado(string p_migrados, string p_nomigrados, string p_usuario, string p_modo_migracion="M")
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_SINCRONIZAR_USUARIO_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_MIGRADOS", OracleDbType.Varchar2).Value = p_migrados;
                objCmd.Parameters.Add("P_NOMIGRADOS", OracleDbType.Varchar2).Value = p_nomigrados;
                objCmd.Parameters.Add("P_USUARIO", OracleDbType.Varchar2).Value = p_usuario;
                objCmd.Parameters.Add("P_MODO_MIGRACION", OracleDbType.Varchar2).Value = p_modo_migracion;
                




                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    msg = objCmd.Parameters["V_MSG"].Value.ToString();
                    result = objCmd.Parameters["V_RESULT"].Value.ToString();


                    Result.Add(msg);
                    Result.Add(result);


                }
                catch (Exception ex)
                {
                    Result.Add("500");
                    Result.Add(ex.Message.ToString());

                }
                objConn.Close();
            }

            return Result;
        }


        public ResponseDB getCuentasUsuario()
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_CUENTAS_USUARIOS";
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

                    if (dt == null)
                    {
                        return new ResponseDB { success = (int)ResponseCode.R300, message = "No hay datos"};
                    }

                    if (dt.Rows.Count == 0)
                    {
                        return new ResponseDB { success = (int)ResponseCode.R300, message = "No hay datos"};
                    }

                    return new ResponseDB { success = success_, message = message_, datatable = dt };
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
