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
    public class PeriodoDAO
    {

        public PeriodoDAO(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }



        //public ResponseDB GetPeriodoCargado(String p_anio, String p_nivel, String p_ind_migrado)
        //{
        //    int success_;
        //    string message_;

        //    Conexion objConexion = new Conexion(Configuration);
        //    using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
        //    {
        //        OracleCommand objCmd = new OracleCommand();

        //        objCmd.Connection = objConn;
        //        objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_PERIODOS_CARG";
        //        objCmd.CommandType = CommandType.StoredProcedure;
        //        objCmd.Parameters.Add("P_ANIO", OracleDbType.Varchar2).Value = p_anio;
        //        objCmd.Parameters.Add("P_NIVEL", OracleDbType.Varchar2).Value = p_nivel;
        //        objCmd.Parameters.Add("P_IND_MIGRADO", OracleDbType.Varchar2).Value = p_ind_migrado;

        //        objCmd.Parameters.Add("CURSOR_OUT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
        //        try
        //        {
        //            objConn.Open();
        //            OracleDataReader objReader = objCmd.ExecuteReader();
        //            DataTable dt = new DataTable();
        //            dt.Load(objReader);
        //            success_ = (int)ResponseCode.R200;
        //            message_ = "OK";
        //            objConn.Close();


        //            if (dt == null) { return new ResponseDB { success = (int)ResponseCode.R500, message = "No hay datos" }; }
        //            if (dt.Rows.Count == 0) { return new ResponseDB { success = (int)ResponseCode.R500, message = "No hay datos" }; }

        //            return new ResponseDB { success = success_, message = message_, data = Conversiones.DataTableToJson(dt), datatable = dt };
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Console.WriteLine("Exception: {0}", ex.ToString());
        //            success_ = (int)ResponseCode.R500;
        //            message_ = ex.Message;
        //            objConn.Close();
        //            return new ResponseDB { success = success_, message = message_ };
        //        }

        //    }

        //}
        public DataTable GetPeriodoCargado(String p_code, String p_parte_periodo)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_PERIODO";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_CODE", OracleDbType.Varchar2).Value = p_code;
                objCmd.Parameters.Add("P_PARTE", OracleDbType.Varchar2).Value = p_parte_periodo;



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


        public DataTable GetPeriodoCargadoInterfaz(String p_anio, String p_nivel, String p_ind_migrado)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_PERIODOS_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ANIO", OracleDbType.Varchar2).Value = p_anio;
                objCmd.Parameters.Add("P_NIVEL", OracleDbType.Varchar2).Value = p_nivel;
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




        public List<string> SincronizarPeriodoCargado(string p_migrados, string p_nomigrados, string p_usuario, string p_modo_migracion = "M")
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_SINCRONIZAR_PERIODO_CARG";
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



        public DataTable GetPeriodoCargado(String p_code)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_PERIODO_CARG";
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


        public List<string> ActualizarPeriodoCargado(string p_accion, string p_data, string p_codes)
        {
            List<string> Result = new List<string>();
            String msg, result, newid;


            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_ACTUALIZAR_PERIODO_CARG";
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



        public DataTable GetPeriodosNoCargados(String p_anio, String p_nivel)
        {

            DataTable dataTable = null;

            Conexion objConexion = new Conexion(Configuration);
            //COMMENT_BDTEST: using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                //COMMENT_BDTEST: objCmd.CommandText = "COMUMGR.PKC_UPAO_CANVAS_CSV.PKC_GET_CSV";
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_PERIODOS_PROG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ANIO", OracleDbType.Varchar2).Value = p_anio;
                objCmd.Parameters.Add("P_NIVEL", OracleDbType.Varchar2).Value = p_nivel;
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


        public List<string> SincronizarBannerPeriodos(String p_anio, String p_nivel/*, String p_usuario*/)
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
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_PERIODOS_SINC_BANNER";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ANIO", OracleDbType.Varchar2).Value = p_anio;
                objCmd.Parameters.Add("P_NIVEL", OracleDbType.Varchar2).Value = p_nivel;
               // objCmd.Parameters.Add("P_USUARIO", OracleDbType.Varchar2).Value = p_usuario;
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


    }
}
