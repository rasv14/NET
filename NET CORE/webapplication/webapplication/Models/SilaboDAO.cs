using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using webapplication.Helpers;

namespace webapplication.Models
{
    public class SilaboDAO
    {
        public SilaboDAO(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public ResponseDB getSilabos(string p_periodo, string p_carrera, string p_ind_migrado)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_SILB_CARG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_CARRERA", OracleDbType.Varchar2).Value = p_carrera;
                objCmd.Parameters.Add("P_IND_MIGRADO", OracleDbType.Varchar2).Value = p_ind_migrado;
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

                    return new ResponseDB { success = success_, message = message_, data = Conversiones.DataTableToJson(dt) };
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



        public ResponseDB getSilabosActualizar(string p_periodo, string p_carrera)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_LISTA_SILB_ACT";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;
                objCmd.Parameters.Add("P_CARRERA", OracleDbType.Varchar2).Value = p_carrera;
                //objCmd.Parameters.Add("P_IND_MIGRADO", OracleDbType.Varchar2).Value = p_ind_migrado;
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

                    return new ResponseDB { success = success_, message = message_, data = Conversiones.DataTableToJson(dt) };
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
        public ResponseDB getCursos(string p_periodo, string p_carrera, string p_ind_migrado ) 
        {
            int success_;
            string message_;            

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
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
                    DataTable dt = new DataTable();
                    dt.Load(objReader);
                    success_ = (int)ResponseCode.R200;
                    message_ = "OK";
                    objConn.Close();
                    
                    return new ResponseDB { success = success_, message = message_, data = Conversiones.DataTableToJson(dt) };
                }
                catch (Exception ex)
                {                    
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                    success_ = (int)ResponseCode.R500;
                    message_ = ex.Message;
                    objConn.Close();
                    return new ResponseDB { success = success_, message = message_};
                }
                
            }
            
        }

        public ResponseDB getSilabosUrl(string p_lista_codigos)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_SILABOS";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_CODES", OracleDbType.Varchar2).Value = p_lista_codigos;
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

        public ResponseDB getSilabosMulticarreraUrl(string p_lista_codigos)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_URL_SILABO";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_CODES", OracleDbType.Varchar2).Value = p_lista_codigos;
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

        public ResponseDB getSilabosMigradosUrl(string p_lista_codigos)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_GET_ITEM_SILB_MIGRADOS";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_CODES", OracleDbType.Varchar2).Value = p_lista_codigos;
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

        public ResponseDB actualizarSilabo(string p_accion, string p_data)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_ACTUALIZAR_SILABO";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = p_accion;//SILABO_MIGRO
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;
                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    message_ = objCmd.Parameters["V_MSG"].Value.ToString();

                    success_ = (int)ResponseCode.R200;                    
                    objConn.Close();

                    return new ResponseDB { success = success_, message = message_};
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

        public ResponseDB actualizarUrlSilaboMigrado(string p_accion, string p_data)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_ACTUALIZAR_URL_SIL_MIG";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = p_accion;//SILABO_MIGRO
                objCmd.Parameters.Add("P_DATA", OracleDbType.Varchar2).Value = p_data;
                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    message_ = objCmd.Parameters["V_MSG"].Value.ToString();

                    success_ = (int)ResponseCode.R200;
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


        public ResponseDB getNombreSilabo(string p_carrera)
        {
            int success_;
            string message_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_GET_NOMBRE_CARRERA";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_CARRERA", OracleDbType.Varchar2).Value = p_carrera;//SILABO_MIGRO
                
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    message_ = objCmd.Parameters["V_RESULT"].Value.ToString();

                    success_ = (int)ResponseCode.R200;
                    objConn.Close();

                    return new ResponseDB { success = success_, message = message_, data = message_ };
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                    success_ = (int)ResponseCode.R500;
                    message_ = ex.Message;
                    objConn.Close();
                    return new ResponseDB { success = success_, message = message_, data ="-" };
                }

            }
        }



        public ResponseDB aprobarSilabos(string p_periodo)
        {
            int success_;
            string message_,cantidad_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_APROBAR_SILABOS";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;//SILABO_MIGRO
               
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_NRO_ACT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    message_ = objCmd.Parameters["V_RESULT"].Value.ToString();
                    cantidad_ = objCmd.Parameters["V_NRO_ACT"].Value.ToString();

                    success_ = (int)ResponseCode.R200;
                    objConn.Close();

                    return new ResponseDB { success = success_, message = message_,data= cantidad_ };
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

        public ResponseDB aprobarSilabos_agr(string p_periodo)
        {
            int success_;
            string message_, cantidad_;

            Conexion objConexion = new Conexion(Configuration);
            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("PROD")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_CANV_APROBAR_SILABOS_AGR";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_PERIODO", OracleDbType.Varchar2).Value = p_periodo;//SILABO_MIGRO

                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_NRO_ACT", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                    message_ = objCmd.Parameters["V_RESULT"].Value.ToString();
                    cantidad_ = objCmd.Parameters["V_NRO_ACT"].Value.ToString();

                    success_ = (int)ResponseCode.R200;
                    objConn.Close();

                    return new ResponseDB { success = success_, message = message_, data = cantidad_ };
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
