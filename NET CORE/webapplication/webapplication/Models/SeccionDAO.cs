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
    public class SeccionDAO
    {

        public SeccionDAO(IConfiguration configuration)
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
        public DataTable GetCursosCargados(String p_periodo, String p_carrera, String p_ind_migrado = "")
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();
             
                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_CURSOS_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_CARRERA", OracleDbType.Varchar2).Value = p_carrera;
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

        public DataTable GetCursosNoCargados(String p_periodo, String p_carrera)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_CURSOS_PROG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_CARRERA", OracleDbType.Varchar2).Value = p_carrera;
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


        public List<string> SincronizarBannerCursos(String p_periodo, String p_carrera)
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
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_SINCRONIZAR_BANNER";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_CARRERA", OracleDbType.Varchar2).Value = p_carrera;
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


        public DataTable GetCursoCargado(String p_code)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_CURSO_CARG";
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


        public DataTable GetSeccionCargado(String p_code)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_SEC_CARG";
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



        public List<string> ActualizarCursoCargado(string p_accion, string p_data, string p_codes)
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_ACTUALIZAR_SEC_CARG";
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


        public List<string> SincronizarCursoCargado(string p_migrados, string p_nomigrados, string p_usuario, string p_modo_migracion="M")
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_SINCRONIZAR_SEC_CARG";
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



    }
}
