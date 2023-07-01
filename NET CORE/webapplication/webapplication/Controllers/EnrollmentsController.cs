using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webapplication.clases;
using webapplication.Handler;
using webapplication.Helpers;
using webapplication.Models;

namespace webapplication.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IConfiguration configuration;

        public EnrollmentsController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }



        //internal Dictionary<string, object> GetDict(DataTable dt)
        //{
        //    return dt.AsEnumerable()
        //      .ToDictionary<DataRow, string, object>(row => row.Field<string>(0),
        //                                row => row.Field<object>(1));
        //}

        //public static List<dynamic> ToDynamic( DataTable dt)
        //{
        //    var dynamicDt = new List<dynamic>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        dynamic dyn = new ExpandoObject();
        //        dynamicDt.Add(dyn);
        //        //--------- change from here
        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            var dic = (IDictionary<string, object>)dyn;
        //            dic[column.ColumnName] = row[column];
        //        }
        //        //--------- change up to here
        //    }
        //    return dynamicDt;
        //}
        //public static IEnumerable<dynamic> AsDynamicEnumerable(DataTable table)
        //    {
        //        // Validate argument here..

        //        return table.AsEnumerable().Select(row => new DynamicRow(row));
        //    }

        //    private sealed class DynamicRow : DynamicObject
        //    {
        //        private readonly DataRow _row;

        //        internal DynamicRow(DataRow row) { _row = row; }

        //        // Interprets a member-access as an indexer-access on the 
        //        // contained DataRow.
        //        public override bool TryGetMember(GetMemberBinder binder, out object result)
        //        {
        //            var retVal = _row.Table.Columns.Contains(binder.Name);
        //            result = retVal ? _row[binder.Name] : null;
        //            return retVal;
        //        }
       //     }

        public string FormatRespuestaJSON(int success, string message, string data)
        {
            return "{\"success\": \"" + success + "\" ,\"message\": \"" + message + "\", \"data\": " + data + "}";
        }
        public static string ToCSV(DataTable table)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString());
                    result.Append(i == table.Columns.Count - 1 ? "\n" : ",");
                }
            }

            return result.ToString();
        }


        public static List<List<T>> SplitList<T>( List<T> me, int size = 50)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            return list;
        }


        [HttpPost, Route("Tabla")]
        public String Tabla([FromQuery] HeadersParameters parameters,  [FromBody] dynamic data)
        {

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objUsuarioDAO = new EvalDocDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);

                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;

                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;
                String p_ind_estado_objt = "";// jsondata_objt.p_id;
               
                String p_periodo_objt = "";// jsondata_objt.p_tipo;


                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                //String p_periodo_objt = "";
                String p_carrera_objt = "";
                String p_ind_migrado_objt = "";
                String p_curso_objt = "";
                String p_id_objt = "";

               


                if (jsondata.p_accion == "ver_INDIVIDUAL")
                {
                    p_periodo_objt = jsondata_objt.p_periodo;
                  //  p_ind_estado_objt = jsondata_objt.p_ind_estado;
                }
                else {
                    p_tipo_objt = jsondata_objt.p_tipo;
                    p_periodo_objt = jsondata_objt.p_periodo;
                    p_carrera_objt = jsondata_objt.p_carrera;
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                    p_curso_objt = jsondata_objt.p_curso;
                    p_id_objt = jsondata_objt.p_id;
                }



                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;


               


                //SE inserta el LOG

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "", "");

                if (JsonString.Length > 4000)
                {
                    JsonString = JsonString.Substring(0, 4000);
                }

                ///Comentado: lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, "");


                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

                if (jsondata.p_accion == "descargar_CSV" || jsondata.p_accion == "enviar_APICANVAS")
                {
                    //if (jsondata.p_codes == "[]")
                    //{
                    //    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                    //    Result = "2";
                    //    msjResult = "No ha seleccionado ningun elemento";


                    //    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    //    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    //    return JsonResult;
                    //}
                    //p_codesString = (string)jsondata.p_codes.ToString();


                }
                else if(jsondata.p_accion == "sincronizar_BANNER")
                {

                    if (String.IsNullOrEmpty(p_periodo_objt)) { JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha ingresado el Periodo o el tipo de Usuario", "[]"); return JsonResult; }

                    lstSincronizarBanner = objUsuarioDAO.SincronizarBannerUsuariosMasivo(p_periodo_objt, "", configuration.GetSection("MySettings").GetSection("entornoCANVAS").Value, jsondata.p_usuario);
                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");
                    return JsonResult;

                }

                else if (jsondata.p_accion == "ver_INDIVIDUAL")
                {
                    if (String.IsNullOrEmpty(jsondata.p_code)) { JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha ingresado el Codigo de la Evaluacion docente", "[]"); return JsonResult; }

                    DataTable dt_proceso = null;

                    dt_proceso = objUsuarioDAO.GetProcesoEvalDoc(jsondata.p_code);

                    if (dt_proceso == null)
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No hay datos", "[]");
                        return JsonResult;
                    }
                    if (dt_proceso.Rows.Count <= 0)
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No hay datos", "[]");
                        return JsonResult;
                    }


                    string p_code_proceso = dt_proceso.Rows[0]["CODE"].ToString();
                    string p_periodo_proceso = dt_proceso.Rows[0]["PERIODO"].ToString();
                    string p_titulo_proceso = dt_proceso.Rows[0]["TITULO"].ToString();
                    string p_ind_estado_proceso = dt_proceso.Rows[0]["IND_ESTADO"].ToString();

                        EvalDocVer evaldocver = new EvalDocVer
                        {
                           code = p_code_proceso,
                           titulo = p_titulo_proceso,
                           periodo = p_periodo_proceso,
                           ind_estado = p_ind_estado_proceso
                        };

                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "OK", JsonConvert.SerializeObject(evaldocver));
                    return JsonResult;


                }




                DataTable dataTable = null;

                if (p_codesString != "[]")
                {
                    String codesjson = "{\"Codes\":" + p_codesString + "}";

                    var result = JsonConvert.DeserializeObject<RootObject>(codesjson);

                    List<List<p_codesModel>> list_ListCodes = SplitList(result.Codes, 150);

                    int i_dt = 0;
                    foreach (List<p_codesModel> list_p_code in list_ListCodes)
                    {
                        //COMENTARIO_BDtest:  String jsoncodes = "[";
                        String jsoncodes = "";
                        int i = 0;
                        foreach (p_codesModel p_code in list_p_code)
                        {
                            i++;
                            String code = p_code.p_code;

                            if (i == list_p_code.Count)
                            {
                                //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}";
                                jsoncodes += code ;
                            }
                            else
                            {
                                //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}" + ",";

                                jsoncodes += code + ",";

                            }


                        }

                        //COMENTARIO_BDtest: jsoncodes += "]";

                        DataTable dataTable_merge = null;


                        //AGREGADO PARA CARGA INICIAL***
                        if (jsondata.p_tabla == "users") {

                             dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, jsoncodes);

                           // dataTable_merge = objCursoDAO.GetCursosCargados(p_periodo_objt, p_curso_departamento_objt);

                            
                        }
                        else if (jsondata.p_tabla == "courses_nocargados")
                        {

                           
                          //  dataTable_merge = objCursoDAO.GetCursosNoCargados(p_periodo_objt, p_curso_departamento_objt);


                        }
                        else if (jsondata.p_tabla == "sections")
                        {

                            
                          //  dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, jsoncodes);


                        }
                        else
                        {
                            dataTable_merge = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, jsoncodes);
                        }
                        //////*********/
                        ///dataTable_merge = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, jsoncodes);

                        if (i_dt == 0)
                        {
                            dataTable = dataTable_merge;
                        }
                        else
                        {
                            dataTable.Merge(dataTable_merge);
                        }


                        i_dt++;

                    }

                    if(dataTable != null)
                    {

                        

                        if (jsondata.p_tabla == "users")
                        {


                            DataView dv = dataTable.DefaultView;
                            dv.Sort = "full_name asc";
                            DataTable sortedDT = dv.ToTable();
                            dataTable = null;
                            dataTable = sortedDT;

                        }
                        if (jsondata.p_tabla == "sections")
                        {
                            var newDataTable = dataTable.AsEnumerable()
                             .OrderBy(r => r.Field<String>("name"))
                             .ThenBy(r => r.Field<String>("section_id"))
                             .CopyToDataTable();
                            dataTable = null;
                            dataTable = newDataTable;

                            //DataView dv = dataTable.DefaultView;
                            //dv.Sort = "short_name asc";
                            //DataTable sortedDT = dv.ToTable();
                            //dataTable = null;
                            //dataTable = sortedDT;

                        }




                    }
                        





                }
                else {

                    dataTable = null;
                    //COMENTARIO_BDtest: dataTable = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, p_codesString);
                   
                    //AGREGADO PARA CARGA INICIAL***
                    if (jsondata.p_tabla == "evaldoc")
                    {
                        
                        dataTable = objUsuarioDAO.GetListaProcesos(p_periodo_objt, p_ind_estado_objt);
                    }
                    else if (jsondata.p_tabla == "users_nocargados")
                    {
                        

                       // dataTable = objUsuarioDAO.GetUsuariosNoCargados(p_periodo_objt, p_tipo_objt);
                    }
                   else if (jsondata.p_tabla == "sections")
                    {
                        
                        //dataTable = objCursoDAO.GetSeccionesCargados(p_periodo_objt, p_curso_departamento_objt, p_ind_migrado_objt, p_sis);
                    }

                    else if (jsondata.p_tabla == "enrollments")
                    {

                        dataTable =  objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, "");
                    }
                    else if (jsondata.p_tabla == "enrollments_eliminados")
                    {
                        if (p_tipo_objt == "E")
                        {
                            dataTable = objCSVDAO.GetDatosCSV(jsondata.accion+"_E", p_datosString, "");
                        }

                        else if (p_tipo_objt == "D")
                        {
                            dataTable = objCSVDAO.GetDatosCSV(jsondata.accion + "_D", p_datosString, "");
                        }

                       
                    }                  
                    else
                    {
                        dataTable = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, "");
                    }
                    //
                    //////*********/
                    ///dataTable = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, "");

                }



                //string PasswordKeyJWT = configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value;


                if (dataTable != null)
                {
                    if (dataTable.Rows.Count <= 0)
                    {


                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "No hay registros para mostrar", "[]");
                        Result = "3";
                        msjResult = "No hay registros para mostrar";





                        // return JsonResult;
                    }
                    else
                    {
                        if (jsondata.p_accion == "enviar_APICANVAS")
                        {

                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "En proceso de envio", "[]");
                            Result = "3";
                            msjResult = "En proceso de envio";

                            //Generar el archivo CSV en el servidor
                            String csvString = ToCSV(dataTable);
                            String file_name = jsondata.p_tabla + ".csv";
                            String file_path = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV").Value + file_name;
                            String file_history = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV_history").Value + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file_name;

                            System.IO.File.WriteAllText(file_path, csvString);
                            System.IO.File.WriteAllText(file_history, csvString);//guardar un historial de archivo
                                                                                 //**********

                            fileResult = file_history;
                            ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult, fileResult);
                            ///Comentado:  lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);



                            //SUbir el cssv generado anteriormente a CANVAS mediante un llamado a su API
                            string token = "17977~pzJqcU6RQMZeDoaE8r5PJxhvheSVzcGk6AZ1HPHL9lYRdy0lMxm2Fe8PB7ULlprO";
                            string urlcanvas = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value;
                            string accountid = "97";

                            string result = CanvasAPICSV.UploadCSVFile(token, file_path, urlcanvas, accountid).Result;
                            //**********************
                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "OK", "[]");
                            Result = "1";
                            msjResult = "OK";

                        }

                        else
                        {
                            string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
                            //var jsonobjt = JsonConvert.SerializeObject(dataTable.AsEnumerable().Select(r => r.ItemArray));

                            //var dynamicTable = AsDynamicEnumerable(dataTable);
                            //var dynamic2 = ToDynamic(dataTable);

                            //List<object> lst = dataTable.AsEnumerable().ToList<object>();
                            //var json2 = JsonConvert.SerializeObject(lst);

                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "OK", json);
                            Result = "1";
                            msjResult = "OK";
                        }
                        //return JsonResult;


                    }
                }
                else
                { //return Ok(new { success = 3, message = "No hay registros para mostrar" });
                  //return FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "No hay registros para mostrar", "[]");
                    Result = "3";
                    msjResult = "No hay registros para mostrar";
                }


                
                ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult, fileResult);

                String jsonresult = JsonResult;
                if (jsonresult.Length > 4000)
                {
                    jsonresult = JsonResult.Substring(0, 4000);
                }
                ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", jsonresult);

                return JsonResult;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }


        [HttpPost, Route("Procesar")]
        public ResponseApi Procesar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CanvasAPIEvaluacion objCanvasAPIEvaluacion = new CanvasAPIEvaluacion(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objEvalDocDAO = new EvalDocDAO(configuration);
            
            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstSincronizarCursos, lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {

                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se envió Parametros" };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;



                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;
                

                String p_code= jsondata_objt.CODE.ToString();
                String p_usuario = jsondata.p_usuario;

                //String p_periodo_objt = jsondata_objt.p_periodo;
                //String p_tipo_curso_objt = jsondata_objt.p_tipo_curso;
                //String p_curso_departamento_objt = !String.IsNullOrEmpty(jsondata_objt.p_curso_departamento) ? jsondata_objt.p_curso_departamento.ToUpper() : jsondata_objt.p_curso_departamento;

                //var parametros = new Dictionary<string, string>();
                //parametros.Add("p_anio", p_anio_objt);
                //parametros.Add("p_periodo", p_periodo_objt);
                //parametros.Add("p_tipo_curso", p_tipo_curso_objt);
                //parametros.Add("p_curso_departamento", p_curso_departamento_objt);

                //var claseJSON = new utilidadesJSON();

                //string p_datosString2 = claseJSON.DictionaryToString(parametros);

                //p_datosString = p_datosString2;





                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {


                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Solicitud no autorizada." };
                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);


                }

                if (p_code == "")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se ingreso Codigo del Proceso." };

                }





                DataTable dt_proceso = null;

                dt_proceso = objEvalDocDAO.GetProcesoEvalDoc(p_code);




                if (dt_proceso == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se encontro data del proceso." };

                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se encontro data del proceso." };
                }

                string p_ind_estado_proceso = dt_proceso.Rows[0]["IND_ESTADO"].ToString();
                string p_ind_cerrado_proceso = dt_proceso.Rows[0]["IND_CERRADO"].ToString();
                string ind_activo = dt_proceso.Rows[0]["IND_ACTIVO"].ToString();


                if (p_ind_estado_proceso == "P")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Este proceso esta en ejecucion no se puede procesar hasta que termine" };
                }
                if (p_ind_cerrado_proceso == "S")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Este proceso esta cerrado, no se puede ejecutar" };
                }
                if (ind_activo != "S")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Este proceso esta desactivado, no se puede ejecutar" };
                }


                DateTime fecha_inicio_dt = DateTime.Now;
                String fecha_inicio = fecha_inicio_dt.ToString("dd/MM/yyyy");
                String hora_inicio = fecha_inicio_dt.ToString("H:mm");


                var parametros = new Dictionary<string, string>();
                parametros.Add("p_fecha_inicio", fecha_inicio);
                parametros.Add("p_hora_inicio", hora_inicio);
                parametros.Add("p_usuario_ejecuto", jsondata.p_usuario);


                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);



                lstUpdateProcesoEvalDoc = objEvalDocDAO.ActualizarProcesoEvalDoc(p_datosString2, p_code, "ACTUALIZAR_INICIO_EJECUCION");



               // lstUpdateProcesoEvalDoc = objEvalDocDAO.ActualizarEstadoProcesoEvalDoc(p_code, "P");

                //string path = "D:\\2021\\CANVAS\\console_project\\ConsoleEvalDoc\\ConsoleEvalDoc\\bin\\Debug\\netcoreapp3.1\\ConsoleEvalDoc.exe";

                string path = configuration.GetSection("MySettings").GetSection("rutaConsoleProcesarEvaluacionDocente").Value;


                // string arguments = "param1 param2 param3";

      

                string arguments = p_code + " " + p_usuario;

                Process process = new Process();


                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = arguments;


                // These two optional flags ensure that no DOS window
                // appears
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;


                // Capture the output of the Console application
                process.StartInfo.RedirectStandardOutput = true;


                process.Start();



                return new ResponseApi { success = (int)ResponseCode.R200, message = "Se esta Procesando, este proceso puede durar varios minutos", data = "[]" };

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR", error = ex.Message };
            }
        }

        [HttpPost, Route("ReProcesar")]
        public ResponseApi ReProcesar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CanvasAPIEvaluacion objCanvasAPIEvaluacion = new CanvasAPIEvaluacion(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objEvalDocDAO = new EvalDocDAO(configuration);

            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstSincronizarCursos, lstUpdateProcesoEvalDoc;

            List<EvaluacionDocente> lst_EvaluacionDocente = new List<EvaluacionDocente>();
            List<CursoResultTable> lstCursoResult = new List<CursoResultTable>();

            List<CursoResultTable> lstCursoResult_procesados = new List<CursoResultTable>();

            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";

                int total_cursos_procesados = 0;


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {

                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se envió Parametros" };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;



                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;


                String p_code = jsondata.p_code.ToString(); 
                String p_code_detalle = jsondata.p_code_detalle.ToString();
                String p_usuario = jsondata.p_usuario;

                
                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {


                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Solicitud no autorizada." };
                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);


                }

                if (p_code == "")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se ingreso Codigo del Proceso." };

                }



                DateTime fecha_inicio_dt = DateTime.Now;
                String fecha_inicio = fecha_inicio_dt.ToString("dd/MM/yyyy");
                String hora_inicio = fecha_inicio_dt.ToString("H:mm");


      

                DataTable dt_proceso = null;

                dt_proceso = objEvalDocDAO.GetProcesoEvalDoc(p_code);

                


                if (dt_proceso == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se encontro data del proceso." };

                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se encontro data del proceso." };
                }

                string p_ind_estado_proceso = dt_proceso.Rows[0]["IND_ESTADO"].ToString();
                string p_ind_cerrado_proceso = dt_proceso.Rows[0]["IND_CERRADO"].ToString();
                string ind_activo = dt_proceso.Rows[0]["IND_ACTIVO"].ToString();



                if (p_ind_estado_proceso  == "P")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Este proceso esta en ejecucion no se puede reprocesar hasta que termine" };
                }
                if (p_ind_cerrado_proceso == "S")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Este proceso esta cerrado, no se puede ejecutar" };
                }
                     
                if (ind_activo != "S")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Este proceso esta desactivado, no se puede ejecutar" };
                }



                DataTable dt_cursos = objEvalDocDAO.GetSubDetalleNoProceso(p_code_detalle);

                if (dt_cursos == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se encontraron los cursos a procesar." };
                }
                if (dt_cursos.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se encontraron los cursos a procesar." };
                }

                // ObtenerDataCanvas(dt_cursos, ref lst_EvaluacionDocente, ref lstCursoResult, ref total_cursos_procesados, ref lstCursoResult_procesados);
                //  CanvasAPIEvaluacion objCanvasAPIEvaluacion = new CanvasAPIEvaluacion(configuration);



                List<EvaluacionDocente> lst_EvaluacionDocente_sin_agrupar_total = new List<EvaluacionDocente>();



                foreach (DataRow dr in dt_cursos.Rows)
                {
                    List<EvaluacionDocente> lst_EvaluacionDocente_sin_agrupar = new List<EvaluacionDocente>();


                    String code = dr["CURSO_SIS"].ToString();


                    try
                    {
                        //int count_match = 0;

                        // File, Page, Discussion, Assignment, Quiz, SubHeader, ExternalUrl, ExternalTool

                        int total_foros = 0;//Discussion
                    int total_trabajos = 0;  //Assignment
                    int total_examenes = 0; //Quiz
                    int total_materiales = 0; //ExternalTool //File //ExternalTool
                    int total_paginas = 0; //Page


                    //obtener los docentes del curso*******

                    List<DocenteCanvas> lst_docentes_roles = new List<DocenteCanvas>();
                    List<EnrollmentCanvas> lst_enrollments = null;

                    List<int?> lst_tareas_ya_consideradas = new List<int?>();

                    ResponseApi rpt_docentes_API = objCanvasAPIEvaluacion.docentes_roles_paginados(code);
                    if (rpt_docentes_API.success == (int)ResponseCode.R200)
                    {
                        lst_enrollments = rpt_docentes_API.data;
                        foreach (EnrollmentCanvas enrollment in lst_enrollments)
                        {

                            DocenteCanvas docente = new DocenteCanvas { id_canvas = enrollment.user_id, id = enrollment.sis_user_id };
                            lst_docentes_roles.Add(docente);

                        }


                    }

                    else
                    {
                        lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener Docentes del Curso" });
                        continue;

                    }

                    List<DocenteCanvas> lst_docentes = new List<DocenteCanvas>();
                    int count = 0;

                    foreach (DocenteCanvas enrollment in lst_docentes_roles)
                    {

                        if (count == 0)
                        {
                            lst_docentes.Add(enrollment);
                        }

                        int count_repetido = 0;
                        foreach (DocenteCanvas docente in lst_docentes)
                        {
                            if (enrollment.id_canvas == docente.id_canvas)
                            {
                                count_repetido++;
                            }


                        }

                        if (count_repetido == 0)
                        {
                            lst_docentes.Add(enrollment);
                        }



                        count++;

                    }


                    //  List<DocenteCanvas> lst_docentes = lst_docentes_roles.Distinct().ToList();
                    //*****************************************



                    //obtener las SECCIONES del curso*******

                    List<SeccionCanvas> lst_secciones = null;

                    ResponseApi rpt_secciones_API = objCanvasAPIEvaluacion.get_secciones(code);
                    if (rpt_secciones_API.success == (int)ResponseCode.R200)
                    {
                        lst_secciones = rpt_secciones_API.data;

                    }

                    else
                    {
                        lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener Secciones del Curso" });
                        continue;

                    }
                    //*****************************************

                    //obtener las ARCHIVOS del curso*******

                    List<FileCanvas> lst_archivos = new List<FileCanvas>();


                    int i_files = 1;
                        while (i_files <= 5)
                        {
                            List<FileCanvas> lst_archivos_p = null;
                            ResponseApi rpt_archivos_API = objCanvasAPIEvaluacion.files_paginados(code, i_files.ToString());
                            if (rpt_archivos_API.success == (int)ResponseCode.R200)
                            {
                                lst_archivos_p = rpt_archivos_API.data;
                                if (lst_archivos_p.Count() == 0) { break; }
                            }

                            else
                            {
                                lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener los Archivos del Curso" });
                                continue;

                            }
                            foreach (FileCanvas file in lst_archivos_p)
                            {
                                lst_archivos.Add(file);
                            }

                            i_files++;
                        }



                        //*****************************************



                        //obtener las FOROS del curso*******

                        List<ForoCanvas> lst_foros = null;

                    ResponseApi rpt_foros_API = objCanvasAPIEvaluacion.foros_paginados(code);
                    if (rpt_foros_API.success == (int)ResponseCode.R200)
                    {
                        lst_foros = rpt_foros_API.data;

                        //Agregar los foros a la lista 

                        foreach (ForoCanvas foro in lst_foros)
                        {
                            if (foro.title == "Foro de consultas")
                            {//Descartar los Foros de Consulta que se crean por Defecto
                                if (foro.author != null)
                                {
                                    if (foro.author.id == 0)
                                    {
                                        continue;
                                    }

                                }
                                else
                                {
                                    continue;
                                }
                            }


                            if (foro.published == true)//Solo si el foro esta publicado
                            {
                                //   if (foro.assignment != null) {

                                String id_docente_author = "";

                                foreach (DocenteCanvas docente in lst_docentes)
                                {
                                    if (foro.author.id == docente.id_canvas)
                                    {
                                        id_docente_author = docente.id;
                                    }


                                }
                                if (String.IsNullOrEmpty(id_docente_author))
                                {
                                    continue;
                                }
                                int num_character_periodo2 = 6;
                                string periodo2 = code.Substring(0, num_character_periodo2);
                                string nrc2 = code.Substring(num_character_periodo2, code.Length - num_character_periodo2);
                                EvaluacionDocente evaluacion = new EvaluacionDocente
                                {
                                    docente_id = id_docente_author,
                                    tipo = "Foros",
                                    tipo_code = "F",
                                    curso_sis = code,
                                    periodo = periodo2,
                                    curso_nrc = nrc2,
                                    cantidad = 1
                                };
                                lst_EvaluacionDocente_sin_agrupar.Add(evaluacion);

                                if (foro.assignment_id != null)
                                {
                                    lst_tareas_ya_consideradas.Add(foro.assignment_id); //para ya no considerarlo estemos en las Tareas
                                }
                                //   if (foro.assignment.overrides.Count== 0)//Si es 0 entonces el foro se asigno a todas las secciones


                                //  }

                            }

                        }

                    }

                    else
                    {
                        lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener los Foros del Curso" });
                        continue;

                    }
                        //*****************************************

                        //obtener las TAREAS del curso*******

                        List<AssignmentCanvas> lst_tareas = new List<AssignmentCanvas>();


                        int i_tareas = 1;
                        while (i_tareas <= 3)
                        {
                            List<AssignmentCanvas> lst_tareas_p = null;
                            ResponseApi rpt_tareas_API = objCanvasAPIEvaluacion.tareas_paginados(code, i_tareas.ToString());
                            if (rpt_tareas_API.success == (int)ResponseCode.R200)
                            {
                                lst_tareas_p = rpt_tareas_API.data;
                                if (lst_tareas_p.Count() == 0) { break; }
                            }

                            else
                            {
                                lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener las tareas del Curso" });
                                continue;

                            }
                            foreach (AssignmentCanvas tarea in lst_tareas_p)
                            {
                                lst_tareas.Add(tarea);
                            }

                            i_tareas++;
                        }
                        //*****************************************


                        //obtener los Examenes del curso*******

                        List<ExamenCanvas> lst_examenes = null;

                    ResponseApi rpt_examenes_API = objCanvasAPIEvaluacion.examenes_paginados(code);
                    if (rpt_examenes_API.success == (int)ResponseCode.R200)
                    {
                        lst_examenes = rpt_examenes_API.data;


                        foreach (ExamenCanvas examen in lst_examenes)
                        {

                            if (examen.published == true)
                            {  //La tarea tiene que estar Publicada


                                int? id_asignacion = 0;
                                id_asignacion = examen.assignment_id;

                                AssignmentCanvas tarea = null;
                                List<int> lista_secciones_beneficiadas = new List<int>();

                                foreach (AssignmentCanvas asig in lst_tareas)
                                {
                                    if (asig.id == id_asignacion)
                                    {
                                        tarea = asig;
                                    }

                                }

                                if (tarea != null)
                                {

                                    if (tarea.overrides.Count == 0)
                                    {//Si es 0 entonces el exmen se asigno a todas las secciones y beneficia a todos los docentes

                                        foreach (DocenteCanvas docente in lst_docentes)
                                        {
                                            int num_character_periodo2 = 6;
                                            string periodo2 = code.Substring(0, num_character_periodo2);
                                            string nrc2 = code.Substring(num_character_periodo2, code.Length - num_character_periodo2);
                                            EvaluacionDocente evaluacion = new EvaluacionDocente
                                            {
                                                docente_id = docente.id,
                                                tipo = "Examen",
                                                tipo_code = "E",
                                                curso_sis = code,
                                                periodo = periodo2,
                                                curso_nrc = nrc2,
                                                cantidad = 1
                                            };
                                            lst_EvaluacionDocente_sin_agrupar.Add(evaluacion);

                                            if (examen.assignment_id != null)
                                            {
                                                lst_tareas_ya_consideradas.Add(examen.assignment_id);
                                            }

                                        }


                                    }
                                    else
                                    {

                                        List<String> lst_docentes_beneficiados = new List<String>();


                                        foreach (OverrideCanvas ov in tarea.overrides)
                                        {
                                            foreach (EnrollmentCanvas enrollment in lst_enrollments)
                                            {
                                                if (ov.course_section_id == enrollment.course_section_id)
                                                {

                                                    lst_docentes_beneficiados.Add(enrollment.sis_user_id);
                                                }


                                            }


                                        }

                                        lst_docentes_beneficiados = lst_docentes_beneficiados.Distinct().ToList();

                                        foreach (String id_docente in lst_docentes_beneficiados)
                                        {
                                            int num_character_periodo2 = 6;
                                            string periodo2 = code.Substring(0, num_character_periodo2);
                                            string nrc2 = code.Substring(num_character_periodo2, code.Length - num_character_periodo2);
                                            EvaluacionDocente evaluacion = new EvaluacionDocente
                                            {
                                                docente_id = id_docente,
                                                tipo = "Examen",
                                                tipo_code = "E",
                                                curso_sis = code,
                                                periodo = periodo2,
                                                curso_nrc = nrc2,
                                                cantidad = 1
                                            };
                                            lst_EvaluacionDocente_sin_agrupar.Add(evaluacion);

                                            if (examen.assignment_id != null)
                                            {
                                                lst_tareas_ya_consideradas.Add(examen.assignment_id);
                                            }

                                        }



                                    }

                                }




                            }

                        }


                    }

                    else
                    {
                        lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener los Examenes del Curso" });
                        continue;

                    }
                    //*****************************************



                    //obtener las TAREAS ASIGNADAS del curso*******
                    lst_tareas_ya_consideradas = lst_tareas_ya_consideradas.Distinct().ToList();

                    foreach (AssignmentCanvas tarea in lst_tareas)
                    {

                        int count_asignado = 0;
                        foreach (int id_tarea_ya_asignada in lst_tareas_ya_consideradas) //verificar que la tarea no haya sido asignada antes como foro o examen
                        {
                            if (tarea.id == id_tarea_ya_asignada)
                            {
                                count_asignado++;
                            }

                        }
                        if (count_asignado > 0)
                        {
                            continue;
                        }


                        if (tarea.published == true)
                        {  //La tarea tiene que estar Publicada

                            if (tarea.overrides.Count == 0)
                            {//Si es 0 entonces la tarea se asigno a todas las secciones y beneficia a todos los docentes

                                foreach (DocenteCanvas docente in lst_docentes)
                                {
                                    int num_character_periodo2 = 6;
                                    string periodo2 = code.Substring(0, num_character_periodo2);
                                    string nrc2 = code.Substring(num_character_periodo2, code.Length - num_character_periodo2);
                                    EvaluacionDocente evaluacion = new EvaluacionDocente
                                    {
                                        docente_id = docente.id,
                                        tipo = "Tarea",
                                        tipo_code = "T",
                                        curso_sis = code,
                                        periodo = periodo2,
                                        curso_nrc = nrc2,
                                        cantidad = 1
                                    };
                                    lst_EvaluacionDocente_sin_agrupar.Add(evaluacion);

                                }


                            }
                            else
                            {

                                List<String> lst_docentes_beneficiados = new List<String>();


                                foreach (OverrideCanvas ov in tarea.overrides)
                                {
                                    foreach (EnrollmentCanvas enrollment in lst_enrollments)
                                    {
                                        if (ov.course_section_id == enrollment.course_section_id)
                                        {

                                            lst_docentes_beneficiados.Add(enrollment.sis_user_id);
                                        }


                                    }


                                }

                                lst_docentes_beneficiados = lst_docentes_beneficiados.Distinct().ToList();

                                foreach (String id_docente in lst_docentes_beneficiados)
                                {
                                    int num_character_periodo2 = 6;
                                    string periodo2 = code.Substring(0, num_character_periodo2);
                                    string nrc2 = code.Substring(num_character_periodo2, code.Length - num_character_periodo2);
                                    EvaluacionDocente evaluacion = new EvaluacionDocente
                                    {
                                        docente_id = id_docente,
                                        tipo = "Tarea",
                                        tipo_code = "T",
                                        curso_sis = code,
                                        periodo = periodo2,
                                        curso_nrc = nrc2,
                                        cantidad = 1
                                    };
                                    lst_EvaluacionDocente_sin_agrupar.Add(evaluacion);

                                }



                            }
                        }

                    }


                    //*****************************************










                        List<ModuloCanvas> lst_modulos = new List<ModuloCanvas>();
                        int i_modules = 1;
                        while (i_modules <= 2)
                        {
                            List<ModuloCanvas> lst_modulos_p = null;

                            ResponseApi rpt_modulos_API = objCanvasAPIEvaluacion.modulos_paginados_con_items(code, i_modules.ToString());

                            if (rpt_modulos_API.success == (int)ResponseCode.R200)
                            {
                                lst_modulos_p = rpt_modulos_API.data;

                            }

                            else
                            {
                                lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener los Archivos del Curso" });
                                continue;

                            }
                            foreach (ModuloCanvas modulo in lst_modulos_p)
                            {
                                lst_modulos.Add(modulo);
                            }

                            i_modules++;
                        }

                        //  ResponseApi rpt_modulos_API = objCanvasAPIEvaluacion.modulos_paginados_con_items(code);


                        //if (rpt_modulos_API.success == (int)ResponseCode.R200)
                        //{
                        //    List<ModuloCanvas> lst_modulos = rpt_modulos_API.data;

                        foreach (ModuloCanvas modulo in lst_modulos)
                        {
                            if (modulo.published == false) { continue; }
                            if (modulo.items == null) { continue; }

                            //ResponseApi rpt_items_API = objCanvasAPIEvaluacion.items_paginados(code, modulo.id.ToString());
                            //  if (rpt_modulos_API.success == (int)ResponseCode.R200)
                            if (modulo.items.Count > 0)
                            {
                                //List<ItemCanvas> lst_items = rpt_items_API.data;

                                List<ItemCanvas> lst_items = modulo.items;

                                foreach (ItemCanvas item in lst_items)
                                {
                                    if (item.published == false) { continue; }

                                    if (!(item.title.ToUpper().Contains("SILABO")) && !(item.title.ToUpper().Contains("SÍLABO")))
                                    {


                                        if (item.type == "Discussion")
                                        {
                                            //  total_foros++;

                                            //foreach(ForoCanvas foro in lst_foros){

                                            //    if (item.content_id == foro.id)
                                            //    {
                                            //        foro.assignment




                                            //    }


                                            //}




                                        }
                                        else if (item.type == "Assignment")
                                        {
                                            total_trabajos++;
                                        }
                                        else if (item.type == "Quiz")
                                        {
                                            total_examenes++;
                                        }

                                        else if (item.type == "Page" || item.type == "ExternalUrl") //|| item.type == "ExternalTool"
                                        {
                                            total_materiales++;
                                        }

                                        else if (item.type == "File")
                                        {
                                            //total_examenes++;

                                            foreach (FileCanvas file in lst_archivos)
                                            {

                                                if (file.id == item.content_id)
                                                {
                                                    if (file.user != null)
                                                    {

                                                        String id_docente_author = "";
                                                        foreach (DocenteCanvas docente in lst_docentes)
                                                        {

                                                            if (file.user.id == docente.id_canvas)
                                                            {
                                                                id_docente_author = docente.id;
                                                            }
                                                        }
                                                        if (!String.IsNullOrEmpty(id_docente_author))
                                                        {
                                                            int num_character_periodo2 = 6;
                                                            string periodo2 = code.Substring(0, num_character_periodo2);
                                                            string nrc2 = code.Substring(num_character_periodo2, code.Length - num_character_periodo2);
                                                            EvaluacionDocente evaluacion = new EvaluacionDocente
                                                            {
                                                                docente_id = id_docente_author,
                                                                tipo = "Archivo",
                                                                tipo_code = "A",
                                                                curso_sis = code,
                                                                periodo = periodo2,
                                                                curso_nrc = nrc2,
                                                                cantidad = 1
                                                            };
                                                            lst_EvaluacionDocente_sin_agrupar.Add(evaluacion);

                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        //else if (item.type == "Page") {

                                        //    total_paginas++;
                                        //}


                                    }

                                }
                            }
                            //else
                            //{
                            //    lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener los items del módulo: " + modulo.id.ToString() + "-" + modulo.name });
                            //    continue;
                            //}

                        }

                      


                    }
                    catch (Exception ex)
                    {
                        lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error de Codigo" + ":" + ex.Message.ToString() });
                        continue;

                    }

                    foreach (EvaluacionDocente evdl in lst_EvaluacionDocente_sin_agrupar)
                    {

                        lst_EvaluacionDocente_sin_agrupar_total.Add(evdl);
                    }



                    int encontro_data_curso = 0;

                    foreach (EvaluacionDocente evdc in lst_EvaluacionDocente_sin_agrupar) {

                        if (evdc.curso_sis == code)
                        {
                            encontro_data_curso++;
                        }
                    }

                    if (encontro_data_curso > 0)
                    {
                        lstCursoResult_procesados.Add(new CursoResultTable { curso_sis = code, descripcion = "Se procesó con Éxito" });
                        total_cursos_procesados++;
                    }
                    else {
                        lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "No se encontró Material en el Curso" });
                    }

                   

                }





                
                if (lst_EvaluacionDocente_sin_agrupar_total.Count > 0)
                {

                    var query_lst_group = lst_EvaluacionDocente_sin_agrupar_total.GroupBy(d => new { d.curso_sis, d.periodo, d.curso_nrc, d.docente_id, d.tipo_code, d.tipo })
                            .Select(
                                g => new EvaluacionDocente
                                {
                            // Key = g.Key,
                            cantidad = g.Sum(s => s.cantidad),
                                    curso_sis = g.First().curso_sis,
                                    periodo = g.First().periodo,
                                    curso_nrc = g.First().curso_nrc,
                                    docente_id = g.First().docente_id,
                                    tipo_code = g.First().tipo_code,
                                    tipo = g.First().tipo,
                                });

                    var list_query = query_lst_group.ToList();

                    lst_EvaluacionDocente = list_query;
                }



 
                List<List<EvaluacionDocente>> list_list_cursos_migrados = SplitList(lst_EvaluacionDocente, 15);
       
                foreach (List<EvaluacionDocente> list_cursos_migrados_l in list_list_cursos_migrados)
                {
                    string jsonCursosMigrados = JsonConvert.SerializeObject(list_cursos_migrados_l).ToString();


                    List<string> lstRegistrarDataEvalDoc = objEvalDocDAO.RegistrarDataEvalDoc_RP(jsonCursosMigrados, p_code, p_code_detalle,p_usuario);
             

                }

                //List<List<CursoResultTable>> list_lstCursoResult = SplitList(lstCursoResult_procesados, 15);


                //foreach (List<CursoResultTable> list_lstCursoResult_l in list_lstCursoResult)
                //{
                //    string jsonCursosErrores = JsonConvert.SerializeObject(list_lstCursoResult_l).ToString();

                //    List<string> lstRegistrarSubDetalleEvalDoc = objEvalDocDAO.RegistrarProcSubDetalleEjecucionEvalDoc(jsonCursosErrores, p_code_detalle, p_usuario);

                //}

                String p_estado = p_ind_estado_proceso;

                ///verificar si todos los cursos ya se procesaron
            
                DataTable dt_cursos_procesados= objEvalDocDAO.GetSubDetalleNoProceso(p_code_detalle);

                if (dt_cursos_procesados == null)
                {
                    p_estado = "T";
                }
                if (dt_cursos_procesados.Rows.Count <= 0)
                {
                    p_estado = "T";
                }

                String fecha_fin = DateTime.Now.ToString("dd/MM/yyyy");
                DateTime fecha_fin_dt = DateTime.Now;
                String hora_fin = DateTime.Now.ToString("H:mm");
                String tiempo_ejecucion = (Convert.ToInt32(fecha_fin_dt.Subtract(fecha_inicio_dt).TotalMinutes)).ToString();

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_ind_estado", p_estado);
                parametros.Add("p_usuario_ejecuto", p_usuario);
                parametros.Add("p_fecha_inicio", fecha_inicio);
                parametros.Add("p_fecha_fin", fecha_fin);
                parametros.Add("p_hora_inicio", hora_inicio);
                parametros.Add("p_hora_fin", hora_fin);
                parametros.Add("p_tiempo_ejecucion", tiempo_ejecucion);

                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);
              
                lstUpdateProcesoEvalDoc = objEvalDocDAO.ActualizarProcesoEvalDoc(p_datosString2, p_code, "ACTUALIZAR_EJECUCION");






                return new ResponseApi { success = (int)ResponseCode.R200, message = "Número de Cursos Procesados: " + total_cursos_procesados, error = "", data = lstCursoResult };








         

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR", error = ex.Message };
            }
        }


        //public  void ObtenerDataCanvas(DataTable dt_cursos, ref List<EvaluacionDocente> lst_EvaluacionDocente, ref List<CursoResultTable> lstCursoResult, ref int total_cursos_procesados, ref List<CursoResultTable> lstCursoResult_procesados)
        //{
        //    CanvasAPIEvaluacion objCanvasAPIEvaluacion = new CanvasAPIEvaluacion(configuration);



        //    foreach (DataRow dr in dt_cursos.Rows)
        //    {


        //        String code = dr["CURSO_SIS"].ToString();
        //        //int count_match = 0;

        //        // File, Page, Discussion, Assignment, Quiz, SubHeader, ExternalUrl, ExternalTool

        //        int total_foros = 0;//Discussion
        //        int total_trabajos = 0;  //Assignment
        //        int total_examenes = 0; //Quiz
        //        int total_materiales = 0; //ExternalTool //File //ExternalTool
        //        int total_paginas = 0; //Page


        //        //obtener los docentes del curso

        //        List<String> lst_docentes_roles = new List<String>();


        //        ResponseApi rpt_docentes_API = objCanvasAPIEvaluacion.docentes_roles_paginados(code);
        //        if (rpt_docentes_API.success == (int)ResponseCode.R200)
        //        {
        //            List<EnrollmentCanvas> lst_enrollments = rpt_docentes_API.data;
        //            foreach (EnrollmentCanvas enrollment in lst_enrollments)
        //            {
        //                lst_docentes_roles.Add(enrollment.sis_user_id);

        //            }


        //        }

        //        else
        //        {
        //            lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener Docentes del Curso" });
        //            continue;

        //        }

        //        List<String> lst_docentes = lst_docentes_roles.Distinct().ToList();




        //        ResponseApi rpt_modulos_API = objCanvasAPIEvaluacion.modulos_paginados(code);


        //        if (rpt_modulos_API.success == (int)ResponseCode.R200)
        //        {
        //            List<ModuloCanvas> lst_modulos = rpt_modulos_API.data;

        //            foreach (ModuloCanvas modulo in lst_modulos)
        //            {
        //                ResponseApi rpt_items_API = objCanvasAPIEvaluacion.items_paginados(code, modulo.id.ToString());
        //                if (rpt_modulos_API.success == (int)ResponseCode.R200)
        //                {
        //                    List<ItemCanvas> lst_items = rpt_items_API.data;

        //                    foreach (ItemCanvas item in lst_items)
        //                    {
        //                        if (!(item.title.ToUpper().Contains("SILABO")) && !(item.title.ToUpper().Contains("SÍLABO")))
        //                        {


        //                            if (item.type == "Discussion")
        //                            {
        //                                total_foros++;
        //                            }
        //                            else if (item.type == "Assignment")
        //                            {
        //                                total_trabajos++;
        //                            }
        //                            else if (item.type == "Quiz")
        //                            {
        //                                total_examenes++;
        //                            }
        //                            else if (item.type == "Page" || item.type == "File" || item.type == "ExternalUrl") //|| item.type == "ExternalTool"
        //                            {
        //                                total_materiales++;
        //                            }
        //                            //else if (item.type == "Page") {

        //                            //    total_paginas++;
        //                            //}


        //                        }

        //                    }
        //                }
        //                else
        //                {
        //                    lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener los items del módulo: " + modulo.id.ToString() + "-" + modulo.name });
        //                    continue;
        //                }

        //            }

        //        }
        //        else
        //        {
        //            lstCursoResult.Add(new CursoResultTable { curso_sis = code, descripcion = "Error al Obtener los Módulos del Curso" });
        //            continue;
        //        }

        //        int num_character_periodo = 6;
        //        string periodo = code.Substring(0, num_character_periodo);
        //        string nrc = code.Substring(num_character_periodo, code.Length - num_character_periodo);


        //        foreach (String docente_id in lst_docentes)
        //        {
        //            if (total_foros > 0)
        //            {
        //                EvaluacionDocente evaluacion = new EvaluacionDocente
        //                {
        //                    docente_id = docente_id,
        //                    tipo = "Foros",
        //                    tipo_code = "F",
        //                    curso_sis = code,
        //                    periodo = periodo,
        //                    curso_nrc = nrc,
        //                    cantidad = total_foros
        //                };
        //                lst_EvaluacionDocente.Add(evaluacion);

        //            }

        //            if (total_trabajos > 0)
        //            {
        //                EvaluacionDocente evaluacion = new EvaluacionDocente
        //                {
        //                    docente_id = docente_id,
        //                    tipo = "Trabajos",
        //                    tipo_code = "T",
        //                    curso_sis = code,
        //                    periodo = periodo,
        //                    curso_nrc = nrc,
        //                    cantidad = total_trabajos
        //                };
        //                lst_EvaluacionDocente.Add(evaluacion);
        //            }

        //            if (total_examenes > 0)
        //            {
        //                EvaluacionDocente evaluacion = new EvaluacionDocente
        //                {
        //                    docente_id = docente_id,
        //                    tipo = "Examenes",
        //                    tipo_code = "E",
        //                    curso_sis = code,
        //                    periodo = periodo,
        //                    curso_nrc = nrc,
        //                    cantidad = total_examenes
        //                };
        //                lst_EvaluacionDocente.Add(evaluacion);
        //            }


        //            if (total_materiales > 0)
        //            {
        //                EvaluacionDocente evaluacion = new EvaluacionDocente
        //                {
        //                    docente_id = docente_id,
        //                    tipo = "Materiales",
        //                    tipo_code = "M",
        //                    curso_sis = code,
        //                    periodo = periodo,
        //                    curso_nrc = nrc,
        //                    cantidad = total_materiales
        //                };
        //                lst_EvaluacionDocente.Add(evaluacion);
        //            }

        //            if (total_paginas > 0)
        //            {
        //                EvaluacionDocente evaluacion = new EvaluacionDocente
        //                {
        //                    docente_id = docente_id,
        //                    tipo = "Paginas",
        //                    tipo_code = "P",
        //                    curso_sis = code,
        //                    periodo = periodo,
        //                    curso_nrc = nrc,
        //                    cantidad = total_paginas
        //                };
        //                lst_EvaluacionDocente.Add(evaluacion);
        //            }

        //        }


        //        lstCursoResult_procesados.Add(new CursoResultTable { curso_sis = code, descripcion = "Se proceso son Exito" });
        //        total_cursos_procesados++;
        //    }


        //}



        [HttpPost, Route("activar_desactivar_proceso")]
        public ResponseApi activar_desactivar_proceso([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CanvasAPIEvaluacion objCanvasAPIEvaluacion = new CanvasAPIEvaluacion(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objEvalDocDAO = new EvalDocDAO(configuration);

            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstSincronizarCursos, lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {

                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se envió Parametros" };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;



                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;


                String p_code = jsondata.p_code.ToString();
                String p_estado = jsondata.p_estado.ToString();
                String p_usuario = jsondata.p_usuario;

               




                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {


                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Solicitud no autorizada." };
                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);


                }

                if (p_code == "")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se ingreso Codigo del Proceso." };

                }



               



                lstUpdateProcesoEvalDoc = objEvalDocDAO.ActivarDesactivarProcesoEvalDoc(p_code,p_estado,p_usuario);


                return new ResponseApi { success = Convert.ToInt32(lstUpdateProcesoEvalDoc[0]), message = lstUpdateProcesoEvalDoc[1], data = "[]" };

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR", error = ex.Message };
            }
        }


        [HttpPost, Route("cerrar_proceso")]
        public ResponseApi cerrar_proceso([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CanvasAPIEvaluacion objCanvasAPIEvaluacion = new CanvasAPIEvaluacion(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objEvalDocDAO = new EvalDocDAO(configuration);

            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstSincronizarCursos, lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {

                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se envió Parametros" };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;



                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;


                String p_code = jsondata.p_code.ToString();
                String p_estado = jsondata.p_estado.ToString();
                String p_usuario = jsondata.p_usuario;






                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {


                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Solicitud no autorizada." };
                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);


                }

                if (p_code == "")
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se ingreso Codigo del Proceso." };

                }







                lstUpdateProcesoEvalDoc = objEvalDocDAO.CerrarProcesoEvalDoc(p_code, p_estado, p_usuario);


                return new ResponseApi { success = Convert.ToInt32(lstUpdateProcesoEvalDoc[0]), message = lstUpdateProcesoEvalDoc[1], data = "[]" };

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR", error = ex.Message };
            }
        }



        [HttpPost, Route("Sincronizar")]
        public String Sincronizar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CanvasAPITerm objCanvasAPITerm = new CanvasAPITerm(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            CursoDAO objCursoDAO = new CursoDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstSincronizarCursos;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;

                //string p_datosString = jsondata.p_datos;



                //var converter2 = new ExpandoObjectConverter();
                //var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;
                //String p_anio_objt = jsondata_objt.p_anio;
                //String p_periodo_objt = jsondata_objt.p_periodo;
                //String p_tipo_curso_objt = jsondata_objt.p_tipo_curso;
                //String p_curso_departamento_objt = !String.IsNullOrEmpty(jsondata_objt.p_curso_departamento) ? jsondata_objt.p_curso_departamento.ToUpper() : jsondata_objt.p_curso_departamento;

                //var parametros = new Dictionary<string, string>();
                //parametros.Add("p_anio", p_anio_objt);
                //parametros.Add("p_periodo", p_periodo_objt);
                //parametros.Add("p_tipo_curso", p_tipo_curso_objt);
                //parametros.Add("p_curso_departamento", p_curso_departamento_objt);

                //var claseJSON = new utilidadesJSON();

                //string p_datosString2 = claseJSON.DictionaryToString(parametros);

                //p_datosString = p_datosString2;


                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


                //SE inserta el LOG

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "", "");

                //if (JsonString.Length > 4000)
                //{
                //    JsonString = JsonString.Substring(0, 4000);
                //}

                ///Comentado: lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, "");


                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

                if (jsondata.p_accion == "sincronizar_CURSOS")
                {

                    //if (ReferenceEquals(null, jsondata.p_codes))
                    //{
                    //    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Selecciono ningun elemento.", "[]");
                    //}

                    //if (jsondata.p_codes == "[]")
                    //{
                    //    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                    //    Result = "2";
                    //    msjResult = "No ha seleccionado ningun elemento";


                    //    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    //    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    //    return JsonResult;
                    //}
                    //p_codesString = (string)jsondata.p_codes.ToString();


                }
                DataTable dataTable = null;

                String p_periodo_objt = "";// jsondata_objt.p_tipo;
                String p_parte_periodo_objt = "";

                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                //String p_periodo_objt = "";
                String p_carrera_objt = "";
                String p_ind_migrado_objt = "";
                String p_curso_objt = "";
                String p_id_objt = "";

                p_tipo_objt = jsondata_objt.p_tipo;
                p_periodo_objt = jsondata_objt.p_periodo;
                p_parte_periodo_objt = jsondata_objt.p_parte_periodo;
                p_carrera_objt = jsondata_objt.p_carrera;
                p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                p_curso_objt = jsondata_objt.p_curso;
                p_id_objt = jsondata_objt.p_id;

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_parte_periodo", p_parte_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;



                dataTable = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, "");

                if (dataTable == null) {
                    return FormatRespuestaJSON((int)ResponseCode.R200, "No existen registros para migrar", "[]"); ;
                }

                if (dataTable.Rows.Count ==0)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R200, "No existen registros para migrar", "[]"); ;
                }


                JsonResult = Sincronizar_Canvas( "SINCRONIZAR", p_tipo_objt, dataTable, "course_id", "section_id", "user_id", jsondata.p_usuario);



                return JsonResult;

           //         return FormatRespuestaJSON((int)ResponseCode.R200, "Se sincronizo con Éxito", "[]"); ;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }



        [HttpPost, Route("Sincronizar_eliminados")]
        public String Sincronizar_eliminados([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CanvasAPITerm objCanvasAPITerm = new CanvasAPITerm(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            CursoDAO objCursoDAO = new CursoDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstSincronizarCursos;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;

                //string p_datosString = jsondata.p_datos;



                //var converter2 = new ExpandoObjectConverter();
                //var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;
                //String p_anio_objt = jsondata_objt.p_anio;
                //String p_periodo_objt = jsondata_objt.p_periodo;
                //String p_tipo_curso_objt = jsondata_objt.p_tipo_curso;
                //String p_curso_departamento_objt = !String.IsNullOrEmpty(jsondata_objt.p_curso_departamento) ? jsondata_objt.p_curso_departamento.ToUpper() : jsondata_objt.p_curso_departamento;

                //var parametros = new Dictionary<string, string>();
                //parametros.Add("p_anio", p_anio_objt);
                //parametros.Add("p_periodo", p_periodo_objt);
                //parametros.Add("p_tipo_curso", p_tipo_curso_objt);
                //parametros.Add("p_curso_departamento", p_curso_departamento_objt);

                //var claseJSON = new utilidadesJSON();

                //string p_datosString2 = claseJSON.DictionaryToString(parametros);

                //p_datosString = p_datosString2;


                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


                //SE inserta el LOG

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "", "");

                //if (JsonString.Length > 4000)
                //{
                //    JsonString = JsonString.Substring(0, 4000);
                //}

                ///Comentado: lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, "");


                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

                if (jsondata.p_accion == "sincronizar_CURSOS")
                {

                    //if (ReferenceEquals(null, jsondata.p_codes))
                    //{
                    //    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Selecciono ningun elemento.", "[]");
                    //}

                    //if (jsondata.p_codes == "[]")
                    //{
                    //    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                    //    Result = "2";
                    //    msjResult = "No ha seleccionado ningun elemento";


                    //    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    //    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    //    return JsonResult;
                    //}
                    //p_codesString = (string)jsondata.p_codes.ToString();


                }
                DataTable dataTable = null;

                String p_periodo_objt = "";// jsondata_objt.p_tipo;


                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                //String p_periodo_objt = "";
                String p_carrera_objt = "";
                String p_ind_migrado_objt = "";
                String p_curso_objt = "";
                String p_id_objt = "";

                p_tipo_objt = jsondata_objt.p_tipo;
                p_periodo_objt = jsondata_objt.p_periodo;
                p_carrera_objt = jsondata_objt.p_carrera;
                p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                p_curso_objt = jsondata_objt.p_curso;
                p_id_objt = jsondata_objt.p_id;

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;



                if (p_tipo_objt == "E")
                {
                    dataTable = objCSVDAO.GetDatosCSV(jsondata.accion + "_E", p_datosString, "");
                }

                else if (p_tipo_objt == "D")
                {
                    dataTable = objCSVDAO.GetDatosCSV(jsondata.accion + "_D", p_datosString, "");
                }


             

                if (dataTable == null)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R200, "No existen registros para sincronizar", "[]"); ;
                }

                if (dataTable.Rows.Count == 0)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R200, "No existen registros para sincronizar", "[]"); ;
                }


                JsonResult = Sincronizar_Canvas("SINCRONIZAR_ELIMINADOS", p_tipo_objt, dataTable, "course_id", "section_id", "user_id", jsondata.p_usuario);



                return JsonResult;

                //         return FormatRespuestaJSON((int)ResponseCode.R200, "Se sincronizo con Éxito", "[]"); ;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }



        public String Sincronizar_Canvas(String accion, String rol, DataTable dt_enroll_bd, String nombre_columna_dt_sis_curso, String nombre_columna_dt_sis_seccion, String nombre_columna_dt_id_usuario, String usuario) {

            String JsonResult = "";

            try
            {
                List<UsuarioCanvas> list_total_API = new List<UsuarioCanvas>();

                List<EnrollmentUser> list_migrados = new List<EnrollmentUser>();
                List<EnrollmentUser> list_no_migrados = new List<EnrollmentUser>();

                List<String> cursos_list = new List<String>();
                List<EnrollmentUser> enrollmentuser_list = new List<EnrollmentUser>();




                foreach (DataRow dr in dt_enroll_bd.Rows)
                {
                    String sis_curso = dr[nombre_columna_dt_sis_curso].ToString();
                    cursos_list.Add(sis_curso);

                    String sis_user = dr[nombre_columna_dt_id_usuario].ToString();

                    String sis_seccion = dr[nombre_columna_dt_sis_seccion].ToString();

                    EnrollmentUser enrollment_user = new EnrollmentUser {
                        user_sis = sis_user,
                        seccion_sis = sis_seccion
                    };
                    enrollmentuser_list.Add(enrollment_user);


                }

                cursos_list = cursos_list.Distinct().ToList();



                String rol_api = "student";
              
                if (rol == "D")
                {
                    rol_api = "teacher";
                }
                else if(rol == "E") { rol_api = "student"; }



                foreach (String sis_curso in cursos_list) {


                    int per_page = 100;
                    int page = 1;

                    while (per_page == 100)
                    {

                        ResponseApi responseAPI_enrollments = new CanvasAPIEnrollments(configuration).get_users_enrollments_by_course_rol_paginado(sis_curso,rol_api, page.ToString());

                        if (responseAPI_enrollments.success == (int)ResponseCode.R200)
                        {

                            List<UsuarioCanvas> list_API = responseAPI_enrollments.data;

                            foreach (UsuarioCanvas user_enroll_API in list_API)
                            {

                                list_total_API.Add(user_enroll_API);
                            }
                            if (list_API != null)
                            {
                                per_page = list_API.Count;
                            }
                            else { break; }

                            
                        }
                        else {
                            break;
                        }
                        page++;
                    }




                }


                foreach (EnrollmentUser user_enroll_bd in enrollmentuser_list)

                {
                    int coincide = 0;

                    foreach (UsuarioCanvas user_enroll in list_total_API) {

                        

                        if (user_enroll.enrollments != null)
                        {

                            foreach (EnrollmentCanvas enrollment in user_enroll.enrollments) {

                                if (enrollment.sis_section_id == user_enroll_bd.seccion_sis && enrollment.sis_user_id == user_enroll_bd.user_sis) {


                                    DateTime dateTime = Convert.ToDateTime(enrollment.created_at);
                                    string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    EnrollmentUser enrollment_user = new EnrollmentUser
                                    {
                                        user_sis = user_enroll_bd.user_sis,
                                        seccion_sis = user_enroll_bd.seccion_sis,
                                        fecha_migracion = date_s
                                    };

                                    list_migrados.Add(enrollment_user);
                                    coincide ++;
                                    break;
                                }


                            }


                        }

                        if (coincide>0) { break; }
                    
                    
                    }

                    if (coincide == 0) { list_no_migrados.Add(user_enroll_bd); }


                }


                List<List<EnrollmentUser>> list_list_migrados = SplitList(list_migrados, 150);

                foreach (List<EnrollmentUser> list_migrados_lst in list_list_migrados)
                {

                    string jsonCursosMigrados = JsonConvert.SerializeObject(list_migrados_lst).ToString();


                    String modo_migracion = "";
                    if (accion.Contains("SINCRONIZAR"))
                    { modo_migracion = "M"; }
                    else if (accion.Contains("MIGRAR"))
                    {
                        modo_migracion = "A";
                    }

                    List<String> responseBD= new EnrollmentsDAO(configuration).SincronizarEnrollmentCargado(jsonCursosMigrados, "[]", rol,usuario, modo_migracion);


                    //var json_data = new
                    //{
                    //    p_id_usuario = P_ID_USUARIO,
                    //    p_cursos_migrados = jsonCursosMigrados
                    //};

                    //string P_DATA = JsonConvert.SerializeObject(json_data);

                    //    ResponseDB responseSilabos = new SilaboDAO(configuration).actualizarSilabo("SILABO_MIGRO", P_DATA);



                    //    lstSincronizarCursos = objUsuarioDAO.SincronizarUsuarioCargado(jsoncodes, "[]", jsondata.p_usuario);


                }

                List<List<EnrollmentUser>> list_list_no_migrados = SplitList(list_no_migrados, 150);

                foreach (List<EnrollmentUser> list_no_migrados_lst in list_list_no_migrados)
                {

                    string jsonCursosNoMigrados = JsonConvert.SerializeObject(list_no_migrados_lst).ToString();


                    String modo_migracion = "";
                    if ( accion.Contains("SINCRONIZAR"))
                    { modo_migracion = "M"; }
                    else if (accion.Contains("MIGRAR"))
                    {
                        modo_migracion = "A";
                    }

                    if (accion.Contains("ELIMINADOS"))
                    {
                        List<String> responseBD = new EnrollmentsDAO(configuration).SincronizarEnrollmentEliminado(jsonCursosNoMigrados, "[]", rol, usuario, modo_migracion);

                    }
                    else {
                        List<String> responseBD = new EnrollmentsDAO(configuration).SincronizarEnrollmentCargado("[]", jsonCursosNoMigrados, rol, usuario, modo_migracion);

                    }







                    //var json_data = new
                    //{
                    //    p_id_usuario = P_ID_USUARIO,
                    //    p_cursos_migrados = jsonCursosMigrados
                    //};

                    //string P_DATA = JsonConvert.SerializeObject(json_data);

                    //    ResponseDB responseSilabos = new SilaboDAO(configuration).actualizarSilabo("SILABO_MIGRO", P_DATA);



                    //    lstSincronizarCursos = objUsuarioDAO.SincronizarUsuarioCargado(jsoncodes, "[]", jsondata.p_usuario);


                }





                Int32 numero_migrados = list_migrados.Count;
                Int32 numero_no_migrados = list_no_migrados.Count;
                Int32 numero_enviados = enrollmentuser_list.Count;

                String respuesta = "";
                
                if (accion.Contains("SINCRONIZAR"))
                {

                    respuesta = "Se sincronizó con Éxito";
                }
                else if (accion.Contains("MIGRAR")) {
                    if (numero_migrados == 0) { respuesta = "No se migro ninguna Inscripcion."; }
                    if (numero_migrados > 0 && numero_no_migrados == 0) { respuesta = "Se migraron todas las inscripciones" + "(" + numero_migrados + ")."; }
                    else { respuesta = "Se migraron " + numero_migrados.ToString() + " de " + numero_enviados.ToString() + " Inscripciones Enviadas."; }
                }

                //**********************
                JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, respuesta, "[]");










            }
            catch (Exception ex)
            {

          return      FormatRespuestaJSON((int)ResponseCode.R500, ex.Message.ToString(), "[]");
           
            }


            return JsonResult;

        }





        [HttpPost, Route("SincronizarBanner")]
        public String SincronizarBanner([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CanvasAPITerm objCanvasAPITerm = new CanvasAPITerm(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            CursoDAO objCursoDAO = new CursoDAO(configuration);
            EnrollmentsDAO objEnrollmentDAO = new EnrollmentsDAO(configuration);

            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstSincronizarCursos;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;

               


                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


                //SE inserta el LOG

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "", "");

                //if (JsonString.Length > 4000)
                //{
                //    JsonString = JsonString.Substring(0, 4000);
                //}

                ///Comentado: lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, "");


                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

               
             
                String p_periodo_objt = "";// jsondata_objt.p_tipo;


                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                //String p_periodo_objt = "";
                String p_carrera_objt = "";
                String p_ind_migrado_objt = "";
                String p_curso_objt = "";
                String p_id_objt = "";

                p_tipo_objt = jsondata_objt.p_tipo;
                p_periodo_objt = jsondata_objt.p_periodo;
                p_carrera_objt = jsondata_objt.p_carrera;
                p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                p_curso_objt = jsondata_objt.p_curso;
                p_id_objt = jsondata_objt.p_id;

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;


                if (p_tipo_objt == "E")
                {
                    lstSincronizarBanner = objEnrollmentDAO.SincronizarBannerEnrollment_E(p_periodo_objt, jsondata.p_usuario);
                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");
                    return JsonResult;
                }

                else if (p_tipo_objt == "D")
                {
                    lstSincronizarBanner = objEnrollmentDAO.SincronizarBannerEnrollment_D(p_periodo_objt, jsondata.p_usuario);
                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");
                    return JsonResult;
                    
                }

                



                JsonResult = FormatRespuestaJSON(Convert.ToInt32("500"), "No hay Acción Establecida", "[]");
                return JsonResult;



            

                //         return FormatRespuestaJSON((int)ResponseCode.R200, "Se sincronizo con Éxito", "[]"); ;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }

        [HttpPost, Route("Migrar")]
        public String Migrar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);

            CanvasAPIEnrollments objCanvasAPIEnrollments = new CanvasAPIEnrollments(configuration);
            EnrollmentsDAO objEnrollmentsDAO = new EnrollmentsDAO(configuration);

            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CanvasAPISection objCanvasAPISection = new CanvasAPISection(configuration);

            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                
                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;


               

                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

                if (jsondata.p_accion =="actualizar_ESTADO") {

                    lstUpdateCurso = objUsuarioDAO.ActualizarEstadoUsuarioCargado(jsondata_objt.USER_ID, jsondata.p_ind_estado);

                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstUpdateCurso[0]), lstUpdateCurso[1], "[]");
                    return JsonResult;

                    
                }



               else if (jsondata.p_accion == "cargar_INDIVIDUAL" || jsondata.p_accion == "cargar_migrar_INDIVIDUAL")
                {


                    String p_id = "";// jsondata_objt.p_id;
                    String p_integration_id = "";// jsondata_objt.p_tipo;
                    String p_login_id = "";// jsondata_objt.p_ind_migrado;
                    String p_primer_nombre = "";// jsondata_objt.p_periodo;
                    String p_nombres = "";
                    String p_apellidos = "";
                    String p_full_nombre = "";
                    String p_short_nombre = "";
                    String p_correo = "";
                    String p_alias = "";
                    String p_estado = "";
                    String p_auth = "";
                    String p_estado_migrar = "";
                    String p_usuario = "";
                    String p_instancia = "";
                    String p_cuenta_canvas = "";

                    String p_tipo = "";

                    p_tipo = jsondata_objt.p_tipo;
                    p_id = jsondata_objt.p_id;
                    p_integration_id = jsondata_objt.p_integration_id;
                    p_login_id = jsondata_objt.p_login_id;
                    p_primer_nombre = jsondata_objt.p_primer_nombre;
                    p_nombres = jsondata_objt.p_primer_nombre + " " + jsondata_objt.p_segundo_nombre;
                    p_apellidos = jsondata_objt.p_apellidos;
                    p_full_nombre = jsondata_objt.p_primer_nombre + " " + jsondata_objt.p_segundo_nombre + " " + jsondata_objt.p_apellidos;
                    p_short_nombre = jsondata_objt.p_primer_nombre + " " + jsondata_objt.p_apellidos;
                    p_correo = jsondata_objt.p_correo;
                    p_alias = jsondata_objt.p_alias;
                    p_estado = jsondata_objt.p_estado;
                    p_auth = jsondata_objt.p_auth;

                    p_estado_migrar = jsondata_objt.p_estado;
                    p_usuario = jsondata.p_usuario;
                    p_instancia = configuration.GetSection("MySettings").GetSection("entornoCANVAS").Value;
                    p_cuenta_canvas = jsondata_objt.p_cuenta_canvas;


                    if (jsondata.p_accion == "cargar_migrar_INDIVIDUAL" && p_estado_migrar != "A")
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "Para migrar el Usuario tiene que seleccionar el estado ACTIVO", "[]");
                        return JsonResult;
                    }

                    if (String.IsNullOrEmpty(p_id) || String.IsNullOrEmpty(p_tipo)) { JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha ingresado el ID o el tipo de Usuario", "[]"); return JsonResult; }

                    ResponseDB responseDB = new UsuarioDAO(configuration).getUsuarioIndividual(p_id, p_tipo);

                    if (responseDB.success != (int)ResponseCode.R200)
                    {
                        JsonResult = FormatRespuestaJSON(responseDB.success, responseDB.message, "[]");
                        return JsonResult;
                    }
                    
                    var parametros = new Dictionary<string, string>();
                    parametros.Add("p_id", p_id);
                    parametros.Add("p_integration_id", p_integration_id);
                    parametros.Add("p_login_id", p_login_id);
                    parametros.Add("p_primer_nombre", p_primer_nombre);
                    parametros.Add("p_nombres", p_nombres);
                    parametros.Add("p_apellidos", p_apellidos);
                    parametros.Add("p_full_nombre", p_full_nombre);
                    parametros.Add("p_short_nombre", p_short_nombre);
                    parametros.Add("p_correo", p_correo);
                    parametros.Add("p_alias", p_alias);
                    parametros.Add("p_auth", p_auth);
                    parametros.Add("p_estado", p_estado_migrar);
                    parametros.Add("p_estado_migrar", "active");
                    parametros.Add("p_usuario", p_usuario);
                    parametros.Add("p_instancia", p_instancia);
                    parametros.Add("p_cuenta_canvas", p_cuenta_canvas);

                    var claseJSON = new utilidadesJSON();

                    string p_datosString2 = claseJSON.DictionaryToString(parametros);


                    if (jsondata.p_accion == "cargar_INDIVIDUAL"  )
                    {
                        lstSincronizarBanner = objUsuarioDAO.SincronizarBannerUsuariosIndividual(p_datosString2);
                        JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");
                        return JsonResult;
                    }
                    else  {
                        lstSincronizarBanner = objUsuarioDAO.SincronizarBannerUsuariosIndividual(p_datosString2);
                        JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");


                        if (Convert.ToInt32(lstSincronizarBanner[0]) == (int)ResponseCode.R200)
                        {
                            ResponseApi rpt_api_c = migrar_usuario_individual(p_id, jsondata.p_usuario, p_datosString);

                            if (rpt_api_c.success != (int)ResponseCode.R200)
                            {
                                return FormatRespuestaJSON((int)ResponseCode.R500, "El Usuario se CARGO pero no se pudo MIGRAR", "[]");
                            }
                            else
                            {

                                return FormatRespuestaJSON(rpt_api_c.success, rpt_api_c.message, rpt_api_c.data);
                            }
                        }
                        else
                        { return JsonResult; }
                      


                    
                    }


                       


                }

                else if (jsondata.p_accion == "migrar_CSV")
                {

                    //if (ReferenceEquals(null, jsondata.p_codes))
                    //{
                    //    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Selecciono ningun elemento.", "[]");
                    //}

                    //if (jsondata.p_codes == "[]")
                    //{
                    //    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                    //    Result = "2";
                    //    msjResult = "No ha seleccionado ningun elemento";
                    //    return JsonResult;
                    //}
                    //p_codesString = (string)jsondata.p_codes.ToString();




                    //DataTable dataTable = null;

                    //String codesjson = "{\"Codes\":" + p_codesString + "}";

                    //var result = JsonConvert.DeserializeObject<RootObject>(codesjson);

                    //List<List<p_codesModel>> list_ListCodes = SplitList(result.Codes, 150);

                    //int i_dt = 0;
                    //foreach (List<p_codesModel> list_p_code in list_ListCodes)
                    //{
                    //    //COMENTARIO_BDtest:  String jsoncodes = "[";
                    //    String jsoncodes = "";
                    //    int i = 0;
                    //    foreach (p_codesModel p_code in list_p_code)
                    //    {
                    //        i++;
                    //        String code = p_code.p_code;

                    //        if (i == list_p_code.Count)
                    //        {
                    //            //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}";
                    //            jsoncodes += code;
                    //        }
                    //        else
                    //        {
                    //            //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}" + ",";

                    //            jsoncodes += code + ",";

                    //        }


                    //    }

                    //    //COMENTARIO_BDtest: jsoncodes += "]";

                    //    DataTable dataTable_merge = null;

                    //    dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, "", jsoncodes);



                    //    if (i_dt == 0)
                    //    {
                    //        dataTable = dataTable_merge;
                    //    }
                    //    else
                    //    {
                    //        dataTable.Merge(dataTable_merge);
                    //    }


                    //    i_dt++;

                    //}

                     DataTable dataTable = null;
                    String p_periodo_objt = "";// jsondata_objt.p_tipo;
                    String p_parte_periodo_objt = "";

                    String p_tipo_objt = "";// jsondata_objt.p_tipo;
                                            //String p_periodo_objt = "";
                    String p_carrera_objt = "";
                    String p_ind_migrado_objt = "";
                    String p_curso_objt = "";
                    String p_id_objt = "";

                    p_tipo_objt = jsondata_objt.p_tipo;
                    p_periodo_objt = jsondata_objt.p_periodo;
                    p_parte_periodo_objt = jsondata_objt.p_parte_periodo;
                    p_carrera_objt = jsondata_objt.p_carrera;
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                    p_curso_objt = jsondata_objt.p_curso;
                    p_id_objt = jsondata_objt.p_id;
                



                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_parte_periodo", p_parte_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;


                dataTable = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, "");


                if (dataTable != null)
                    {
                        if (dataTable.Rows.Count <= 0)
                        {


                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "No hay registros para mostrar", "[]");
                            Result = "3";
                            msjResult = "No hay registros para mostrar";





                            // return JsonResult;
                        }
                        else
                        {


                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "En proceso de envio", "[]");
                            Result = "3";
                            msjResult = "En proceso de envio";

                            //Generar el archivo CSV en el servidor
                            String csvString = ToCSV(dataTable);
                            String file_name = jsondata.p_tabla + ".csv";
                            String file_path = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV").Value + file_name;
                            String file_history = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV_history").Value + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file_name;

                            System.IO.File.WriteAllText(file_path, csvString);
                            System.IO.File.WriteAllText(file_history, csvString);//guardar un historial de archivo
                                                                                 //**********

                            fileResult = file_history;
                            ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult, fileResult);
                            ///Comentado:  lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);



                            //SUbir el cssv generado anteriormente a CANVAS mediante un llamado a su API
                            string token = configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value;
                            string urlcanvas = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value;
                            string accountid = "1";

                            //string resultAPI = CanvasAPICSV.UploadCSVFile(token, file_path, urlcanvas, accountid).Result;
                            ResponseApi rpt_curso_API = CanvasAPICSV.UploadCSVFileCanvas(token, file_path, urlcanvas, accountid).Result;

                            if (rpt_curso_API.success != (int)ResponseCode.R200)
                            {
                                return FormatRespuestaJSON((int)ResponseCode.R500, rpt_curso_API.message, "[]");
                            }

                            SisImportCanvas import_canvas = rpt_curso_API.data;

                            //Consultar si el SisImport ya proceso al 100%
                            // System.Threading.Thread.Sleep(10000);

                            //ResponseApi rpt_sisimport_API_s = new CanvasAPISisImport(configuration).get_sisimport(import_canvas.id.ToString());

                            //if (rpt_sisimport_API_s.success == (int)ResponseCode.R200)
                            //{
                            SisImportCanvas import_canvas_s = rpt_curso_API.data;
                            if (import_canvas_s.progress != 100)
                            {

                                int i_imp = 0;


                                while (i_imp < 100)
                                {
                                    System.Threading.Thread.Sleep(10000);

                                    ResponseApi rpt_sisimport_API = new CanvasAPISisImport(configuration).get_sisimport(import_canvas.id.ToString());

                                    if (rpt_sisimport_API.success == (int)ResponseCode.R200)
                                    {
                                        SisImportCanvas import_canvas_r = rpt_sisimport_API.data;
                                        if (import_canvas_r.workflow_state == "failed_with_messages")
                                        {
                                            return FormatRespuestaJSON((int)ResponseCode.R500, "La importacion Falló, revise el archivo CSV.", "[]");
                                        }

                                        i_imp = import_canvas_r.progress;
                                    }

                                }
                            }
                            //    }
                            //Consultar si el SisImport ya proceso al 100%--FIN

                            ///SE SINCRONIZARAN LOS CURSOS PARA VER SI ESTAN MIGRADOS


                            #region sincronizar

                            JsonResult = Sincronizar_Canvas("MIGRAR", p_tipo_objt, dataTable, "course_id", "section_id", "user_id", jsondata.p_usuario);

                            #endregion

                            //**********************
                         //   JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, respuesta, "[]");
                            Result = "1";
                            msjResult = "OK";




                        }
                    }
                    else
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "No hay registros para mostrar", "[]");
                        Result = "3";
                        msjResult = "No hay registros para mostrar";
                    }




                }

                //else if (jsondata.p_accion == "migrar_MASIVO")
                //{
                //    if (ReferenceEquals(null, jsondata.p_codes))
                //    {
                //        return FormatRespuestaJSON((int)ResponseCode.R500, "No se Selecciono ningun elemento.", "[]");
                //    }

                //    if (jsondata.p_codes == "[]")
                //    {
                //        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                //        Result = "2";
                //        msjResult = "No ha seleccionado ningun elemento";


                //        ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                //        ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                //        return JsonResult;
                //    }
                //    p_codesString = (string)jsondata.p_codes.ToString();





                //    String codes_json = "{\"Codes\":" + p_codesString + "}";
                //    var s_lstCodes = JsonConvert.DeserializeObject<RootObject>(codes_json);
                //    List<p_codesModel> lstCodes = s_lstCodes.Codes;
                //    List<p_codesModel> lstCodes_Migrados = new List<p_codesModel>();
                //    List<p_codesModel> lstCodes_No_Migrados = new List<p_codesModel>();

                //    foreach (p_codesModel p_code in lstCodes)
                //    {


                //        String p_code_course = p_code.p_code;

                //        DataTable dt_course = null;

                //        dt_course = objCursoDAO.GetCursoCargado(p_code_course);

                //        string name_curso = dt_course.Rows[0]["NOM_CURSO"].ToString();
                //        string curso_code = dt_course.Rows[0]["CURSO_CANV"].ToString();
                //        string curso_sis = dt_course.Rows[0]["CURSO_ID_CANV"].ToString();
                //        int course_account_id = String.IsNullOrEmpty(dt_course.Rows[0]["ACCOUNT_ID_CANVAS"].ToString()) ? 1 : Convert.ToInt32(dt_course.Rows[0]["ACCOUNT_ID_CANVAS"].ToString());
                //        //int course_account_id = Convert.ToInt32(configuration.GetSection("MySettings").GetSection("account_id_CANVASUPAO").Value);

                //        BodyFormCurso body_curso = new BodyFormCurso
                //        {
                //            account_id = course_account_id,
                //            name = name_curso,
                //            course_code = curso_code,
                //            is_public_to_auth_users = true,
                //            public_syllabus = false,
                //            public_syllabus_to_auth = true,
                //            sis_course_id = curso_sis,
                //            default_view = "assignments"
                //        };

                //        ResponseApi rpt_curso_API = objCanvasAPICourse.crear_curso2(body_curso);
                //        if (rpt_curso_API.success != (int)ResponseCode.R200)
                //        {
                //            lstCodes_No_Migrados.Add(p_code);
                //        }

                //        else
                //        {

                //            DateTime dateTime = DateTime.Today;
                //            string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                //            String p_ind_migrado_objt = "S";
                //            String p_ind_modo_migrado_objt = "A";

                //            var parametros = new Dictionary<string, string>();
                //            parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                //            parametros.Add("p_fecha_migracion", date_s);
                //            parametros.Add("p_usuario", jsondata.p_usuario);
                //            parametros.Add("p_ind_modo_migrado", p_ind_modo_migrado_objt);


                //            var claseJSON = new utilidadesJSON();

                //            string p_datosString2 = claseJSON.DictionaryToString(parametros);

                //            p_datosString = p_datosString2;

                //            lstUpdateCurso = objCursoDAO.ActualizarCursoCargado("ACTUALIZAR_IND_MIGRADO", p_datosString, p_code_course);



                //            lstCodes_Migrados.Add(p_code);

                //        }




                //    }

                //    Int32 numero_migrados = lstCodes_Migrados.Count;
                //    Int32 numero_no_migrados = lstCodes_No_Migrados.Count;
                //    Int32 numero_enviados = lstCodes.Count;

                //    String respuesta="";
                //    if (numero_migrados == 0) { respuesta = "No se migro ningún Curso."; }
                //    if (numero_migrados > 0 && numero_no_migrados == 0) { respuesta = "Se migraron todos los Cursos enviados" + "(" + numero_migrados + ")."; }
                //    else { respuesta = "Se migraron " + numero_migrados.ToString() + " de " + numero_enviados.ToString() + " Cursos enviados."; }

                //    JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, respuesta, "[]");


                //}

                else if (jsondata.p_accion == "migrar_UNO")
                {
                    String p_code_enrollment = jsondata_objt.CODE.ToString();

                    String p_curso_sis = jsondata_objt.CURSO_SIS;
                    String p_seccion_sis = jsondata_objt.SECCION_SIS;
                    String p_persona_id = jsondata_objt.PERSONA_ID;
                    String p_rol_code = jsondata_objt.ROL_CODE;

                    ResponseApi rpt_curso_API_m = objCanvasAPICourse.get_course_by_sis(p_curso_sis);
                    if (rpt_curso_API_m.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener Datos del Curso", "[]");
                    }

                    ResponseApi rpt_seccion_API_m = objCanvasAPISection.get_section_by_sis(p_seccion_sis);
                    if (rpt_seccion_API_m.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener Datos de la Sección", "[]");
                    }

                    ResponseApi rpt_usuario_API_m = objCanvasAPIUser.get_user_by_id(p_persona_id);
                    if (rpt_usuario_API_m.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener Datos del Usuario", "[]");
                    }

                    CursoCanvas curso_canvas = rpt_curso_API_m.data;

                    UsuarioCanvas usuario_canvas = rpt_usuario_API_m.data;

                    SeccionCanvas seccion_canvas = rpt_seccion_API_m.data;

                    String type = "StudentEnrollment";

                    if (p_rol_code == "D")
                    {
                        type = "TeacherEnrollment";
                    }
                    else if(p_rol_code == "E") {
                        type = "StudentEnrollment";
                    }


                    BodyFormEnrollment body_enrollment = new BodyFormEnrollment
                    {
                        user_id= usuario_canvas.id,
                        type= type,
                        enrollment_state = "active",
                        course_section_id = seccion_canvas.id

                    };


                    ResponseApi rpt_enrollment_API = objCanvasAPIEnrollments.crear_enrollment2(body_enrollment, curso_canvas.id);
                    if (rpt_enrollment_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_enrollment_API.message, "[]");
                    }



                    EnrollmentCanvas UsuarioCanvaS = rpt_enrollment_API.data;

                    DateTime dateTime = Convert.ToDateTime(UsuarioCanvaS.created_at);
                    string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                    String p_ind_migrado_objt = "S";
                    String p_ind_modo_migrado_objt = "A";

                    var parametros = new Dictionary<string, string>();
                    parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                    parametros.Add("p_fecha_migracion", date_s);
                    parametros.Add("p_usuario", jsondata.p_usuario);
                    parametros.Add("p_ind_modo_migrado", p_ind_modo_migrado_objt);

                   



                    var claseJSON = new utilidadesJSON();

                    string p_datosString2 = claseJSON.DictionaryToString(parametros);

                    p_datosString = p_datosString2;

                    lstUpdateCurso = objEnrollmentsDAO.ActualizarEnrollmentCargado("ACTUALIZAR_IND_MIGRADO", p_datosString, p_code_enrollment);




                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstUpdateCurso[1]), lstUpdateCurso[0], "[]");


                    //            String p_code_user = jsondata_objt.USER_ID;

                    //            DataTable dt_user = null;

                    //            dt_user = objUsuarioDAO.GetUsuarioCargado(p_code_user);

                    //            if (dt_user == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }
                    //            if (dt_user.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }

                    //            string login_id_usuario = dt_user.Rows[0]["LOGIN_ID"].ToString();
                    //            string integration_id_usuario = dt_user.Rows[0]["INTEGRATION_ID"].ToString();


                    //            string nombre_usuario = dt_user.Rows[0]["NOMBRE"].ToString();
                    //            string short_name_usuario = dt_user.Rows[0]["SHORT_NOMBRE"].ToString();
                    //            string sortable_name_usuario = dt_user.Rows[0]["SORTABLE_NOMBRE"].ToString();

                    //            string unique_id_usuario = dt_user.Rows[0]["USER_ID"].ToString();

                    //            string sis_user_id_usuario = dt_user.Rows[0]["USER_ID"].ToString();
                    //            string address_usuario = dt_user.Rows[0]["CORREO"].ToString(); 
                    //            string authentication_provider_id_usuario = dt_user.Rows[0]["AUTHENTIC_PROVIDER_ID"].ToString();

                    //            string user_account_id_usuario = String.IsNullOrEmpty(dt_user.Rows[0]["CUENTA_CANVAS"].ToString())? "1":dt_user.Rows[0]["CUENTA_CANVAS"].ToString();

                    //            //int course_account_id = Convert.ToInt32(configuration.GetSection("MySettings").GetSection("account_id_CANVASUPAO").Value);




                    //BodyFormUsuario body_usuario = new BodyFormUsuario
                    //            {
                    //    name  = nombre_usuario,
                    //    short_name = short_name_usuario,
                    //    sortable_name = sortable_name_usuario,
                    //    unique_id = login_id_usuario,
                    //    sis_user_id = unique_id_usuario,
                    //    send_confirmation = true,
                    //    address = address_usuario,
                    //    terms_of_use = true,
                    //    skip_registration = true,
                    //    authentication_provider_id = authentication_provider_id_usuario,
                    //    integration_id = integration_id_usuario
                    //};

                    //            ResponseApi rpt_usuario_API = objCanvasAPIUser.crear_usuario(body_usuario, user_account_id_usuario);
                    //            if (rpt_usuario_API.success != (int)ResponseCode.R200)
                    //            {
                    //                return FormatRespuestaJSON((int)ResponseCode.R500, rpt_usuario_API.message, "[]");
                    //            }

                    //            UsuarioCanvas UsuarioCanvaS = rpt_usuario_API.data;

                    //            DateTime dateTime = Convert.ToDateTime(UsuarioCanvaS.created_at);
                    //            string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                    //            String p_ind_migrado_objt = "S";
                    //            String p_ind_modo_migrado_objt = "A";

                    //            var parametros = new Dictionary<string, string>();
                    //            parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                    //            parametros.Add("p_fecha_migracion", date_s);
                    //            parametros.Add("p_usuario", jsondata.p_usuario);
                    //            parametros.Add("p_ind_modo_migrado", p_ind_modo_migrado_objt);



                    //            var claseJSON = new utilidadesJSON();

                    //            string p_datosString2 = claseJSON.DictionaryToString(parametros);

                    //            p_datosString = p_datosString2;

                    //            lstUpdateCurso = objUsuarioDAO.ActualizarUsuarioCargado("ACTUALIZAR_IND_MIGRADO", p_datosString, p_code_user);




                    //            JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstUpdateCurso[1]), lstUpdateCurso[0], "[]");

                }





                else
                {

                    //Buscar Curso por Codigo SIS

                    String p_code = jsondata_objt.CODE.ToString();
                    String p_persona_id = jsondata_objt.PERSONA_ID;
                    String p_curso_sis = jsondata_objt.CURSO_SIS;
                    String p_seccion_sis = jsondata_objt.SECCION_SIS;

                    String p_fecha_migracion_string = "";

                    String p_rol_desc = jsondata_objt.ROL_DESC;
                    String p_rol_code = jsondata_objt.ROL_CODE;

                    String p_persona_nombre = "";

                    int coincide = 0;


                    String rol_api = "student";

                    if (p_rol_code == "D")
                    {
                        rol_api = "teacher";
                    }
                    else if (p_rol_code == "E") { rol_api = "student"; }


                    ResponseApi responseAPI_enrollments = new CanvasAPIEnrollments(configuration).get_users_enrollments_by_course_rol_paginado(p_curso_sis, rol_api);
                    if (responseAPI_enrollments.success != (int)ResponseCode.R200) {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]");
                    }

                    List<UsuarioCanvas> list_API_enrollments = responseAPI_enrollments.data;



                    foreach (UsuarioCanvas usuario_canvas in list_API_enrollments) {

                        if (usuario_canvas.enrollments != null)
                        {

                            foreach (EnrollmentCanvas enrollment in usuario_canvas.enrollments)
                            {

                                if (enrollment.sis_section_id == p_seccion_sis && enrollment.sis_user_id == p_persona_id)
                                {


                                    DateTime dateTime_ver = Convert.ToDateTime(enrollment.created_at);
                                     p_fecha_migracion_string = dateTime_ver.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    p_persona_nombre = usuario_canvas.name;

                                    coincide++;
                                    break;
                                }


                            }


                        }

                    }


                    if (coincide == 0)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos de inscripcion", "[]");
                    }


                    ResponseApi rpt_curso_API = objCanvasAPICourse.get_course_by_sis(p_curso_sis);
                    if (rpt_curso_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_curso_API.message, "[]");
                    }

                    CursoCanvas CursoCanvas = rpt_curso_API.data;

                    ResponseApi rpt_seccion_API = objCanvasAPISection.get_section_by_sis(p_seccion_sis);
                    if (rpt_curso_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_curso_API.message, "[]");
                    }

                    SeccionCanvas SeccionCanvas = rpt_seccion_API.data;



                    DataTable dt_enrollment = null;

                    dt_enrollment = objEnrollmentsDAO.GetEnrollmentCargado(p_code);

                    if (dt_enrollment == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }
                    if (dt_enrollment.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }
                    

                    string usuario_migro = dt_enrollment.Rows[0]["USUARIO_MIGRO"].ToString();


                    string usuario_modo_migracion = dt_enrollment.Rows[0]["MODO_MIGRACION"].ToString();
                    string tipo_migracion;


                    if (usuario_modo_migracion == "A")
                    {
                        tipo_migracion = "Automático";
                    }
                    else { tipo_migracion = "Manual"; }

                    




                    EnrollmentVer enrollment_ver = new EnrollmentVer
                    {

                     curso_nombre = CursoCanvas.name ,
                     curso_sis= CursoCanvas.sis_course_id,
                     curso_id = CursoCanvas.id,
                     curso_code = CursoCanvas.course_code,

                    seccion_nombre = SeccionCanvas.name,
                    seccion_sis = SeccionCanvas.sis_section_id,
                     seccion_id = SeccionCanvas.id,

                    //public string estado { get; set; }
                  usuario_migro = usuario_migro,
                  fecha_migracion = p_fecha_migracion_string,
                  tipo_migracion = tipo_migracion,



                      persona_sis = p_persona_id,

                      persona_nombre = p_persona_nombre,

                       rol = p_rol_desc
                    };




                    string json = JsonConvert.SerializeObject(enrollment_ver);





                    //string json = JsonConvert.SerializeObject(CursoCanvas);

                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "Se encontro la inscripcion", json);

                }


                return JsonResult;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }


        [HttpPost, Route("Migrar_eliminados")]
        public String Migrar_eliminados([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);

            CanvasAPIEnrollments objCanvasAPIEnrollments = new CanvasAPIEnrollments(configuration);
            EnrollmentsDAO objEnrollmentsDAO = new EnrollmentsDAO(configuration);

            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CanvasAPISection objCanvasAPISection = new CanvasAPISection(configuration);

            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;




                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

                if (jsondata.p_accion == "migrar_CSV")
                {

                   

                    DataTable dataTable = null;
                    String p_periodo_objt = "";// jsondata_objt.p_tipo;


                    String p_tipo_objt = "";// jsondata_objt.p_tipo;
                                            //String p_periodo_objt = "";
                    String p_carrera_objt = "";
                    String p_ind_migrado_objt = "";
                    String p_curso_objt = "";
                    String p_id_objt = "";

                    p_tipo_objt = jsondata_objt.p_tipo;
                    p_periodo_objt = jsondata_objt.p_periodo;
                    p_carrera_objt = jsondata_objt.p_carrera;
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                    p_curso_objt = jsondata_objt.p_curso;
                    p_id_objt = jsondata_objt.p_id;




                    var parametros = new Dictionary<string, string>();
                    parametros.Add("p_periodo", p_periodo_objt);
                    parametros.Add("p_tipo", p_tipo_objt);
                    parametros.Add("p_carrera", p_carrera_objt);
                    parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                    parametros.Add("p_curso", p_curso_objt);
                    parametros.Add("p_id", p_id_objt);



                    var claseJSON = new utilidadesJSON();

                    string p_datosString2 = claseJSON.DictionaryToString(parametros);

                    p_datosString = p_datosString2;




                    if (p_tipo_objt == "E")
                    {
                        dataTable = objCSVDAO.GetDatosCSV(jsondata.accion + "_E", p_datosString, "");
                    }

                    else if (p_tipo_objt == "D")
                    {
                        dataTable = objCSVDAO.GetDatosCSV(jsondata.accion + "_D", p_datosString, "");
                    }




                    if (dataTable != null)
                    {
                        if (dataTable.Rows.Count <= 0)
                        {


                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "No hay registros para mostrar", "[]");
                            Result = "3";
                            msjResult = "No hay registros para mostrar";





                            // return JsonResult;
                        }
                        else
                        {


                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "En proceso de envio", "[]");
                            Result = "3";
                            msjResult = "En proceso de envio";

                            //Generar el archivo CSV en el servidor
                            String csvString = ToCSV(dataTable);
                            String file_name = "enrollments" + ".csv";
                            String file_path = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV").Value + file_name;
                            String file_history = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV_history").Value + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file_name;

                            System.IO.File.WriteAllText(file_path, csvString);
                            System.IO.File.WriteAllText(file_history, csvString);//guardar un historial de archivo
                                                                                 //**********

                            fileResult = file_history;
                            ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult, fileResult);
                            ///Comentado:  lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);



                            //SUbir el cssv generado anteriormente a CANVAS mediante un llamado a su API
                            string token = configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value;
                            string urlcanvas = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value;
                            string accountid = "1";

                            //string resultAPI = CanvasAPICSV.UploadCSVFile(token, file_path, urlcanvas, accountid).Result;
                            ResponseApi rpt_curso_API = CanvasAPICSV.UploadCSVFileCanvas(token, file_path, urlcanvas, accountid).Result;

                            if (rpt_curso_API.success != (int)ResponseCode.R200)
                            {
                                return FormatRespuestaJSON((int)ResponseCode.R500, rpt_curso_API.message, "[]");
                            }

                            SisImportCanvas import_canvas = rpt_curso_API.data;

                            //Consultar si el SisImport ya proceso al 100%
                            // System.Threading.Thread.Sleep(10000);

                            //ResponseApi rpt_sisimport_API_s = new CanvasAPISisImport(configuration).get_sisimport(import_canvas.id.ToString());

                            //if (rpt_sisimport_API_s.success == (int)ResponseCode.R200)
                            //{
                            SisImportCanvas import_canvas_s = rpt_curso_API.data;
                            if (import_canvas_s.progress != 100)
                            {

                                int i_imp = 0;


                                while (i_imp < 100)
                                {
                                    System.Threading.Thread.Sleep(10000);

                                    ResponseApi rpt_sisimport_API = new CanvasAPISisImport(configuration).get_sisimport(import_canvas.id.ToString());

                                    if (rpt_sisimport_API.success == (int)ResponseCode.R200)
                                    {
                                        SisImportCanvas import_canvas_r = rpt_sisimport_API.data;
                                        if (import_canvas_r.workflow_state == "failed_with_messages")
                                        {
                                            return FormatRespuestaJSON((int)ResponseCode.R500, "La importacion Falló, revise el archivo CSV.", "[]");
                                        }

                                        i_imp = import_canvas_r.progress;
                                    }

                                }
                            }
                            //    }
                            //Consultar si el SisImport ya proceso al 100%--FIN

                            ///SE SINCRONIZARAN LOS CURSOS PARA VER SI ESTAN MIGRADOS


                            #region sincronizar

                           // JsonResult = Sincronizar_Canvas("MIGRAR_ELIMINADOS", p_tipo_objt, dataTable, "course_id", "section_id", "user_id", jsondata.p_usuario);


                            List<EnrollmentUser> enrollmentuser_list = new List<EnrollmentUser>();

                            foreach (DataRow dr in dataTable.Rows)
                            {
                              
                                String sis_user = dr["user_id"].ToString();

                                String sis_seccion = dr["section_id"].ToString();

                                EnrollmentUser enrollment_user = new EnrollmentUser
                                {
                                    user_sis = sis_user,
                                    seccion_sis = sis_seccion
                                };
                                enrollmentuser_list.Add(enrollment_user);


                            }

                            List<List<EnrollmentUser>> list_list_migrados = SplitList(enrollmentuser_list, 150);

                            foreach (List<EnrollmentUser> list_migrados_lst in list_list_migrados)
                            {

                                string jsonCursosMigrados = JsonConvert.SerializeObject(list_migrados_lst).ToString();


                                String modo_migracion = "A";
                                

                                List<String> responseBD = new EnrollmentsDAO(configuration).SincronizarEnrollmentEliminado(jsonCursosMigrados, "[]", p_tipo_objt, jsondata.p_usuario, modo_migracion);


                             

                            }

                            #endregion

                            //**********************
                            //   JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, respuesta, "[]");
                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "Se migró con Éxito", "[]");
                            Result = "200";
                            msjResult = "Se migró con Éxito";




                        }
                    }
                    else
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R300, "No hay registros para mostrar", "[]");
                        Result = "3";
                        msjResult = "No hay registros para mostrar";
                    }




                }


                else if (jsondata.p_accion == "migrar_UNO")
                {
                    String p_code_enrollment = jsondata_objt.CODE.ToString();

                    String p_curso_sis = jsondata_objt.CURSO_SIS;
                    String p_seccion_sis = jsondata_objt.SECCION_SIS;
                    String p_persona_id = jsondata_objt.PERSONA_ID;
                    String p_rol_code = jsondata_objt.ROL_CODE;

                    ResponseApi rpt_curso_API_m = objCanvasAPICourse.get_course_by_sis(p_curso_sis);
                    if (rpt_curso_API_m.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener Datos del Curso", "[]");
                    }

                    ResponseApi rpt_seccion_API_m = objCanvasAPISection.get_section_by_sis(p_seccion_sis);
                    if (rpt_seccion_API_m.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener Datos de la Sección", "[]");
                    }

                    ResponseApi rpt_usuario_API_m = objCanvasAPIUser.get_user_by_id(p_persona_id);
                    if (rpt_usuario_API_m.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener Datos del Usuario", "[]");
                    }

                    CursoCanvas curso_canvas = rpt_curso_API_m.data;

                    UsuarioCanvas usuario_canvas = rpt_usuario_API_m.data;

                    SeccionCanvas seccion_canvas = rpt_seccion_API_m.data;

                    String type = "StudentEnrollment";

                    if (p_rol_code == "D")
                    {
                        type = "TeacherEnrollment";
                    }
                    else if (p_rol_code == "E")
                    {
                        type = "StudentEnrollment";
                    }


                    BodyFormEnrollment body_enrollment = new BodyFormEnrollment
                    {
                        user_id = usuario_canvas.id,
                        type = type,
                        enrollment_state = "active",
                        course_section_id = seccion_canvas.id

                    };


                    ResponseApi rpt_enrollment_API = objCanvasAPIEnrollments.crear_enrollment2(body_enrollment, curso_canvas.id);
                    if (rpt_enrollment_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_enrollment_API.message, "[]");
                    }



                    EnrollmentCanvas UsuarioCanvaS = rpt_enrollment_API.data;

                    DateTime dateTime = Convert.ToDateTime(UsuarioCanvaS.created_at);
                    string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                    String p_ind_migrado_objt = "S";
                    String p_ind_modo_migrado_objt = "A";

                    var parametros = new Dictionary<string, string>();
                    parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                    parametros.Add("p_fecha_migracion", date_s);
                    parametros.Add("p_usuario", jsondata.p_usuario);
                    parametros.Add("p_ind_modo_migrado", p_ind_modo_migrado_objt);





                    var claseJSON = new utilidadesJSON();

                    string p_datosString2 = claseJSON.DictionaryToString(parametros);

                    p_datosString = p_datosString2;

                    lstUpdateCurso = objEnrollmentsDAO.ActualizarEnrollmentCargado("ACTUALIZAR_IND_MIGRADO", p_datosString, p_code_enrollment);




                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstUpdateCurso[1]), lstUpdateCurso[0], "[]");



                }





                else
                {

                    //Buscar Curso por Codigo SIS

                    String p_code = jsondata_objt.CODE.ToString();
                    String p_persona_id = jsondata_objt.PERSONA_ID;
                    String p_curso_sis = jsondata_objt.CURSO_SIS;
                    String p_seccion_sis = jsondata_objt.SECCION_SIS;

                    String p_fecha_migracion_string = "";

                    String p_rol_desc = jsondata_objt.ROL_DESC;
                    String p_rol_code = jsondata_objt.ROL_CODE;

                    String p_persona_nombre = "";

                    int coincide = 0;


                    String rol_api = "student";

                    if (p_rol_code == "D")
                    {
                        rol_api = "teacher";
                    }
                    else if (p_rol_code == "E") { rol_api = "student"; }


                    ResponseApi responseAPI_enrollments = new CanvasAPIEnrollments(configuration).get_users_enrollments_by_course_rol_paginado(p_curso_sis, rol_api);
                    if (responseAPI_enrollments.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]");
                    }

                    List<UsuarioCanvas> list_API_enrollments = responseAPI_enrollments.data;



                    foreach (UsuarioCanvas usuario_canvas in list_API_enrollments)
                    {

                        if (usuario_canvas.enrollments != null)
                        {

                            foreach (EnrollmentCanvas enrollment in usuario_canvas.enrollments)
                            {

                                if (enrollment.sis_section_id == p_seccion_sis && enrollment.sis_user_id == p_persona_id)
                                {


                                    DateTime dateTime_ver = Convert.ToDateTime(enrollment.created_at);
                                    p_fecha_migracion_string = dateTime_ver.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                    p_persona_nombre = usuario_canvas.name;

                                    coincide++;
                                    break;
                                }


                            }


                        }

                    }


                    if (coincide == 0)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos de inscripcion", "[]");
                    }


                    ResponseApi rpt_curso_API = objCanvasAPICourse.get_course_by_sis(p_curso_sis);
                    if (rpt_curso_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_curso_API.message, "[]");
                    }

                    CursoCanvas CursoCanvas = rpt_curso_API.data;

                    ResponseApi rpt_seccion_API = objCanvasAPISection.get_section_by_sis(p_seccion_sis);
                    if (rpt_curso_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_curso_API.message, "[]");
                    }

                    SeccionCanvas SeccionCanvas = rpt_seccion_API.data;



                    DataTable dt_enrollment = null;

                    dt_enrollment = objEnrollmentsDAO.GetEnrollmentCargado(p_code);

                    if (dt_enrollment == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }
                    if (dt_enrollment.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }


                    string usuario_migro = dt_enrollment.Rows[0]["USUARIO_MIGRO"].ToString();


                    string usuario_modo_migracion = dt_enrollment.Rows[0]["MODO_MIGRACION"].ToString();
                    string tipo_migracion;


                    if (usuario_modo_migracion == "A")
                    {
                        tipo_migracion = "Automático";
                    }
                    else { tipo_migracion = "Manual"; }






                    EnrollmentVer enrollment_ver = new EnrollmentVer
                    {

                        curso_nombre = CursoCanvas.name,
                        curso_sis = CursoCanvas.sis_course_id,
                        curso_id = CursoCanvas.id,
                        curso_code = CursoCanvas.course_code,

                        seccion_nombre = SeccionCanvas.name,
                        seccion_sis = SeccionCanvas.sis_section_id,
                        seccion_id = SeccionCanvas.id,

                        //public string estado { get; set; }
                        usuario_migro = usuario_migro,
                        fecha_migracion = p_fecha_migracion_string,
                        tipo_migracion = tipo_migracion,



                        persona_sis = p_persona_id,

                        persona_nombre = p_persona_nombre,

                        rol = p_rol_desc
                    };




                    string json = JsonConvert.SerializeObject(enrollment_ver);





                    //string json = JsonConvert.SerializeObject(CursoCanvas);

                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "Se encontro la inscripcion", json);

                }


                return JsonResult;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }


        public ResponseApi migrar_usuario_individual(String id, String p_usuario, String p_datosString) {

            try
            {
                UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
                CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
                List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;

                String p_code_user = id;

                DataTable dt_user = null;

                dt_user = objUsuarioDAO.GetUsuarioCargado(p_code_user);

                if (dt_user == null) { return new ResponseApi { success = (int)ResponseCode.R500, message = "No se pudo obtener los datos del Curso", data = "[]" }; }
                if (dt_user.Rows.Count <= 0) { return new ResponseApi { success = (int)ResponseCode.R500, message = "No se pudo obtener los datos del Curso", data = "[]" }; }

                string login_id_usuario = dt_user.Rows[0]["LOGIN_ID"].ToString();
                string integration_id_usuario = dt_user.Rows[0]["INTEGRATION_ID"].ToString();


                string nombre_usuario = dt_user.Rows[0]["NOMBRE"].ToString();
                string short_name_usuario = dt_user.Rows[0]["SHORT_NOMBRE"].ToString();
                string sortable_name_usuario = dt_user.Rows[0]["SORTABLE_NOMBRE"].ToString();

                string unique_id_usuario = dt_user.Rows[0]["USER_ID"].ToString();

                string sis_user_id_usuario = dt_user.Rows[0]["USER_ID"].ToString();
                string address_usuario = dt_user.Rows[0]["CORREO"].ToString();
                string authentication_provider_id_usuario = dt_user.Rows[0]["AUTHENTIC_PROVIDER_ID"].ToString();

                string user_account_id_usuario = String.IsNullOrEmpty(dt_user.Rows[0]["CUENTA_CANVAS"].ToString()) ? "1" : dt_user.Rows[0]["CUENTA_CANVAS"].ToString();

                //int course_account_id = Convert.ToInt32(configuration.GetSection("MySettings").GetSection("account_id_CANVASUPAO").Value);




                BodyFormUsuario body_usuario = new BodyFormUsuario
                {
                    name = nombre_usuario,
                    short_name = short_name_usuario,
                    sortable_name = sortable_name_usuario,
                    unique_id = login_id_usuario,
                    sis_user_id = unique_id_usuario,
                    send_confirmation = true,
                    address = address_usuario,
                    terms_of_use = true,
                    skip_registration = true,
                    authentication_provider_id = authentication_provider_id_usuario,
                    integration_id = integration_id_usuario
                };

                ResponseApi rpt_usuario_API = objCanvasAPIUser.crear_usuario(body_usuario, user_account_id_usuario);
                if (rpt_usuario_API.success != (int)ResponseCode.R200)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = rpt_usuario_API.message, data = "[]" };
                }

                UsuarioCanvas UsuarioCanvaS = rpt_usuario_API.data;

                DateTime dateTime = Convert.ToDateTime(UsuarioCanvaS.created_at);
                string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                String p_ind_migrado_objt = "S";
                String p_ind_modo_migrado_objt = "A";

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_fecha_migracion", date_s);
                parametros.Add("p_usuario", p_usuario);
                parametros.Add("p_ind_modo_migrado", p_ind_modo_migrado_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;

                lstUpdateCurso = objUsuarioDAO.ActualizarUsuarioCargado("ACTUALIZAR_IND_MIGRADO", p_datosString, p_code_user);



                return new ResponseApi
                {
                    success = Convert.ToInt32(lstUpdateCurso[1]),
                    message = lstUpdateCurso[0],
                    data = "[]"
                };
            }
            catch (Exception ex) {

                return new ResponseApi
                {
                    success = 500,
                    message = ex.Message,
                    data ="[]"
                    
                };

            }
        
        }


        public async Task importar_evaldoc(List<p_codesModel> lstCodes) {

            CanvasAPIEvaluacion objCanvasAPIEvaluacion = new CanvasAPIEvaluacion(configuration);

            List<EvaluacionDocente> lst_EvaluacionDocente = new List<EvaluacionDocente>();





            foreach (p_codesModel p_code in lstCodes)
            {
                String code = p_code.p_code;
                //int count_match = 0;

                // File, Page, Discussion, Assignment, Quiz, SubHeader, ExternalUrl, ExternalTool

                int total_foros = 0;//Discussion
                int total_trabajos = 0;  //Assignment
                int total_examenes = 0; //Quiz
                int total_materiales = 0; //ExternalTool //File //ExternalTool
                int total_paginas = 0; //Page


                //obtener los docentes del curso

                List<String> lst_docentes_roles = new List<String>();


                ResponseApi rpt_docentes_API = objCanvasAPIEvaluacion.docentes_roles_paginados(code);
                if (rpt_docentes_API.success == (int)ResponseCode.R200)
                {
                    List<EnrollmentCanvas> lst_enrollments = rpt_docentes_API.data;
                    foreach (EnrollmentCanvas enrollment in lst_enrollments)
                    {
                        lst_docentes_roles.Add(enrollment.sis_user_id);

                    }


                }

                else
                {
                    continue;

                }

                List<String> lst_docentes = lst_docentes_roles.Distinct().ToList();




                ResponseApi rpt_modulos_API = objCanvasAPIEvaluacion.modulos_paginados(code);


                if (rpt_modulos_API.success == (int)ResponseCode.R200)
                {
                    List<ModuloCanvas> lst_modulos = rpt_modulos_API.data;

                    foreach (ModuloCanvas modulo in lst_modulos)
                    {
                        ResponseApi rpt_items_API = objCanvasAPIEvaluacion.items_paginados(code, modulo.id.ToString());
                        if (rpt_modulos_API.success == (int)ResponseCode.R200)
                        {
                            List<ItemCanvas> lst_items = rpt_items_API.data;

                            foreach (ItemCanvas item in lst_items)
                            {
                                if (!(item.title.ToUpper().Contains("SILABO")) && !(item.title.ToUpper().Contains("SÍLABO")))
                                {


                                    if (item.type == "Discussion")
                                    {
                                        total_foros++;
                                    }
                                    else if (item.type == "Assignment")
                                    {
                                        total_trabajos++;
                                    }
                                    else if (item.type == "Quiz")
                                    {
                                        total_examenes++;
                                    }
                                    else if (item.type == "Page" || item.type == "File" || item.type == "ExternalUrl") //|| item.type == "ExternalTool"
                                    {
                                        total_materiales++;
                                    }
                                    //else if (item.type == "Page") {

                                    //    total_paginas++;
                                    //}


                                }

                            }
                        }

                    }

                }
                else
                {

                    continue;
                }

                int num_character_periodo = 6;
                string periodo = code.Substring(0, num_character_periodo);
                string nrc = code.Substring(num_character_periodo, code.Length - num_character_periodo);


                foreach (String docente_id in lst_docentes)
                {
                    if (total_foros > 0)
                    {
                        EvaluacionDocente evaluacion = new EvaluacionDocente
                        {
                            docente_id = docente_id,
                            tipo = "Foros",
                            tipo_code = "F",
                            curso_sis = code,
                            periodo = periodo,
                            curso_nrc = nrc,
                            cantidad = total_foros
                        };
                        lst_EvaluacionDocente.Add(evaluacion);

                    }

                    if (total_trabajos > 0)
                    {
                        EvaluacionDocente evaluacion = new EvaluacionDocente
                        {
                            docente_id = docente_id,
                            tipo = "Trabajos",
                            tipo_code = "T",
                            curso_sis = code,
                            periodo = periodo,
                            curso_nrc = nrc,
                            cantidad = total_trabajos
                        };
                        lst_EvaluacionDocente.Add(evaluacion);
                    }

                    if (total_examenes > 0)
                    {
                        EvaluacionDocente evaluacion = new EvaluacionDocente
                        {
                            docente_id = docente_id,
                            tipo = "Examenes",
                            tipo_code = "E",
                            curso_sis = code,
                            periodo = periodo,
                            curso_nrc = nrc,
                            cantidad = total_examenes
                        };
                        lst_EvaluacionDocente.Add(evaluacion);
                    }


                    if (total_materiales > 0)
                    {
                        EvaluacionDocente evaluacion = new EvaluacionDocente
                        {
                            docente_id = docente_id,
                            tipo = "Materiales",
                            tipo_code = "M",
                            curso_sis = code,
                            periodo = periodo,
                            curso_nrc = nrc,
                            cantidad = total_materiales
                        };
                        lst_EvaluacionDocente.Add(evaluacion);
                    }

                    if (total_paginas > 0)
                    {
                        EvaluacionDocente evaluacion = new EvaluacionDocente
                        {
                            docente_id = docente_id,
                            tipo = "Paginas",
                            tipo_code = "P",
                            curso_sis = code,
                            periodo = periodo,
                            curso_nrc = nrc,
                            cantidad = total_paginas
                        };
                        lst_EvaluacionDocente.Add(evaluacion);
                    }

                }





            }



        }




        [HttpPost, Route("AgregarProceso")]
        public String AgregarProceso([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objUsuarioDAO = new EvalDocDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;



             

                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

   

               
                String p_periodo_objt = "";// jsondata_objt.p_tipo;
                String p_titulo_objt = "";

                p_periodo_objt = jsondata_objt.p_periodo;
                p_titulo_objt = jsondata_objt.p_titulo;
                

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_titulo", p_titulo_objt);
                parametros.Add("p_usuario", jsondata.p_usuario);

               
                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);



                lstUpdateProcesoEvalDoc = objUsuarioDAO.RegistrarProcesoEvalDoc(p_datosString2);

                
                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstUpdateProcesoEvalDoc[0]), lstUpdateProcesoEvalDoc[1], "[]");
                    return JsonResult;

                

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }



        [HttpPost, Route("EditarProceso")]
        public String EditarProceso([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objUsuarioDAO = new EvalDocDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio Parametros", "[]");
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;



              

                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }


                String p_estado = jsondata_objt.p_estado;

                String p_periodo_objt = "";// jsondata_objt.p_tipo;
                String p_titulo_objt = "";
                String p_code = jsondata_objt.p_code;

                p_periodo_objt = jsondata_objt.p_periodo;
                p_titulo_objt = jsondata_objt.p_titulo;


                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_titulo", p_titulo_objt);
                parametros.Add("p_usuario", jsondata.p_usuario);


                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);


                if (p_estado == "C")
                {
                    lstUpdateProcesoEvalDoc = objUsuarioDAO.ActualizarProcesoEvalDoc(p_datosString2, p_code, "ACTUALIZAR_DATOS_ESTADO_CREADO");

                }
                else {
                    lstUpdateProcesoEvalDoc = objUsuarioDAO.ActualizarProcesoEvalDoc(p_datosString2, p_code, "ACTUALIZAR_DATOS");

                }




                

                JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstUpdateProcesoEvalDoc[0]), lstUpdateProcesoEvalDoc[1], "[]");
                return JsonResult;



            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }


        [HttpPost, Route("GetDetalle")]
        public ResponseApi GetDetalle([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objUsuarioDAO = new EvalDocDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se enviaron parametros." };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;





                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }



                String p_code =  (string)jsondata.p_code.ToString();


                //String p_periodo_objt = "";// jsondata_objt.p_tipo;
                //String p_titulo_objt = "";
                //String p_code = jsondata_objt.p_code;

                //p_periodo_objt = jsondata_objt.p_periodo;
                //p_titulo_objt = jsondata_objt.p_titulo;


                //var parametros = new Dictionary<string, string>();
                //parametros.Add("p_periodo", p_periodo_objt);
                //parametros.Add("p_titulo", p_titulo_objt);
                //parametros.Add("p_usuario", jsondata.p_usuario);


                //var claseJSON = new utilidadesJSON();

                //string p_datosString2 = claseJSON.DictionaryToString(parametros);

                DataTable dt_proceso = null;

                dt_proceso = objUsuarioDAO.GetProcesoEvalDoc(p_code);

                if (dt_proceso == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }


                string p_code_proceso = dt_proceso.Rows[0]["CODE"].ToString();
                string p_periodo_proceso = dt_proceso.Rows[0]["PERIODO"].ToString();
                string p_titulo_proceso = dt_proceso.Rows[0]["TITULO"].ToString();
                string p_ind_estado_proceso = dt_proceso.Rows[0]["IND_ESTADO"].ToString();

                string fecha_registro = dt_proceso.Rows[0]["FECHA_REGISTRO"].ToString();
                string usuario_registro = dt_proceso.Rows[0]["USUARIO_REGISTRO"].ToString();
                string fecha_inicio = dt_proceso.Rows[0]["FECHA_INICIO"].ToString();
                string fecha_fin = dt_proceso.Rows[0]["FECHA_FIN"].ToString();
                string usuario_ejecuto = dt_proceso.Rows[0]["USUARIO_EJECUTO"].ToString();
                string tiempo_ejecucion = dt_proceso.Rows[0]["TIEMPO_EJECUCION"].ToString();

                string hora_inicio = dt_proceso.Rows[0]["HORA_INICIO"].ToString();
                string hora_fin = dt_proceso.Rows[0]["HORA_FIN"].ToString();

                string ind_cerrado = dt_proceso.Rows[0]["IND_CERRADO"].ToString();
                
                string ind_activo = dt_proceso.Rows[0]["IND_ACTIVO"].ToString();

               


                EvalDocVer evaldocver = new EvalDocVer
                {
                    code = p_code_proceso,
                    titulo = p_titulo_proceso,
                    periodo = p_periodo_proceso,
                    ind_estado = p_ind_estado_proceso,
                 fecha_registro = fecha_registro,
                 usuario_registro = usuario_registro,
                 fecha_inicio = fecha_inicio,
                 fecha_fin = fecha_fin,
                 usuario_ejecuto = usuario_ejecuto,
                 tiempo_ejecucion = tiempo_ejecucion,
                 hora_inicio = hora_inicio,
                 hora_fin = hora_fin,
                    ind_cerrado= ind_cerrado,
                    ind_activo = ind_activo
                };


                DataTable dt_detalle_proceso = null;

                dt_detalle_proceso = objUsuarioDAO.GetDetalleProceso(p_code);

                if (dt_detalle_proceso == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }
                if (dt_detalle_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }

                //string json = JsonConvert.SerializeObject(dt_proceso, Formatting.Indented);

                return new ResponseApi { success = (int)ResponseCode.R200, message = "OK",data = evaldocver, data2 = Conversiones.DataTableToJson(dt_detalle_proceso) };



            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return new ResponseApi { success = (int)ResponseCode.R500, message = ex.Message.ToString() };
            }
        }




        [HttpPost, Route("GetSubDetalle")]
        public ResponseApi GetSubDetalle([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objUsuarioDAO = new EvalDocDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se enviaron parametros." };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;





                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }



                String p_code = (string)jsondata.p_code.ToString();


                //String p_periodo_objt = "";// jsondata_objt.p_tipo;
                //String p_titulo_objt = "";
                //String p_code = jsondata_objt.p_code;

                //p_periodo_objt = jsondata_objt.p_periodo;
                //p_titulo_objt = jsondata_objt.p_titulo;


                //var parametros = new Dictionary<string, string>();
                //parametros.Add("p_periodo", p_periodo_objt);
                //parametros.Add("p_titulo", p_titulo_objt);
                //parametros.Add("p_usuario", jsondata.p_usuario);


                //var claseJSON = new utilidadesJSON();

                //string p_datosString2 = claseJSON.DictionaryToString(parametros);

                DataTable dt_proceso = null;

                dt_proceso = objUsuarioDAO.GetSubDetalleProceso(p_code);

                if (dt_proceso == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }

                //string json = JsonConvert.SerializeObject(dt_proceso, Formatting.Indented);

                return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = Conversiones.DataTableToJson(dt_proceso) };



            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return new ResponseApi { success = (int)ResponseCode.R500, message = ex.Message.ToString() };
            }
        }



        [HttpPost, Route("GetPasos")]
        public ResponseApi GetPasos([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objUsuarioDAO = new EvalDocDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se enviaron parametros." };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;





                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }



                String p_code = (string)jsondata.p_code.ToString();


                //String p_periodo_objt = "";// jsondata_objt.p_tipo;
                //String p_titulo_objt = "";
                //String p_code = jsondata_objt.p_code;

                //p_periodo_objt = jsondata_objt.p_periodo;
                //p_titulo_objt = jsondata_objt.p_titulo;


                //var parametros = new Dictionary<string, string>();
                //parametros.Add("p_periodo", p_periodo_objt);
                //parametros.Add("p_titulo", p_titulo_objt);
                //parametros.Add("p_usuario", jsondata.p_usuario);


                //var claseJSON = new utilidadesJSON();

                //string p_datosString2 = claseJSON.DictionaryToString(parametros);

                DataTable dt_pasos_proceso = null;

                dt_pasos_proceso = objUsuarioDAO.GetPasosProceso(p_code);

                if (dt_pasos_proceso == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }
                if (dt_pasos_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }


                string p_code_proceso = dt_pasos_proceso.Rows[0]["PROCESO_CODE"].ToString();
                string p_estado = dt_pasos_proceso.Rows[0]["ESTADO"].ToString();
                string p_nro_cursos = dt_pasos_proceso.Rows[0]["NRO_CURSOS"].ToString();
                string p_paso_1 = dt_pasos_proceso.Rows[0]["PASO_1"].ToString();

                string p_nro_cursos_importados = dt_pasos_proceso.Rows[0]["NRO_CURSOS_IMPORTADOS"].ToString();
                string p_paso_2 = dt_pasos_proceso.Rows[0]["PASO_2"].ToString();

                string p_nro_cursos_insertados = dt_pasos_proceso.Rows[0]["NRO_CURSOS_INSERTADOS"].ToString();
                string p_paso_3 = dt_pasos_proceso.Rows[0]["PASO_3"].ToString();

                string p_titulo_proceso = dt_pasos_proceso.Rows[0]["PROCESO_TITULO"].ToString();

                


                EvalDocPasos evaldocpasos = new EvalDocPasos
                {
                    code_proceso = p_code_proceso,
                    titulo_proceso = p_titulo_proceso,
                    estado = p_estado,
                    nro_cursos = p_nro_cursos,
                    paso_1 = p_paso_1,
                    nro_cursos_importados = p_nro_cursos_importados,
                    paso_2 = p_paso_2,
                    nro_cursos_insertados = p_nro_cursos_insertados,
                    paso_3 = p_paso_3,
                   
                };


               

                return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = evaldocpasos };



            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return new ResponseApi { success = (int)ResponseCode.R500, message = ex.Message.ToString() };
            }
        }



        [HttpPost, Route("GetResultados")]
        public ResponseApi GetResultados([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EvalDocDAO objUsuarioDAO = new EvalDocDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No se enviaron parametros." };
                }


                //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();

                string p_datosString = jsondata.p_datos;
                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;





                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                String p_periodo_objt = "";// jsondata_objt.p_tipo;
                String p_id_objt = "";


                p_periodo_objt = jsondata_objt.p_periodo;
                p_id_objt = jsondata_objt.p_id;


                // String p_code = (string)jsondata.p_code.ToString();


                //String p_periodo_objt = "";// jsondata_objt.p_tipo;
                //String p_titulo_objt = "";
                //String p_code = jsondata_objt.p_code;

                //p_periodo_objt = jsondata_objt.p_periodo;
                //p_titulo_objt = jsondata_objt.p_titulo;


                //var parametros = new Dictionary<string, string>();
                //parametros.Add("p_periodo", p_periodo_objt);
                //parametros.Add("p_titulo", p_titulo_objt);
                //parametros.Add("p_usuario", jsondata.p_usuario);


                //var claseJSON = new utilidadesJSON();

                //string p_datosString2 = claseJSON.DictionaryToString(parametros);

                DataTable dt_proceso = null;

                dt_proceso = objUsuarioDAO.GetResultados(p_periodo_objt, p_id_objt);

                if (dt_proceso == null)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R500, message = "No hay Datos." };
                }

                //string json = JsonConvert.SerializeObject(dt_proceso, Formatting.Indented);

                return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = Conversiones.DataTableToJson(dt_proceso) };



            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return new ResponseApi { success = (int)ResponseCode.R500, message = ex.Message.ToString() };
            }
        }



        [HttpGet, Route("GetResultadosPaginados")]
        public ResponseApi_paginado GetResultadosPaginados([FromQuery] HeadersParameters parameters, String tipo = "", String periodo = "", String parte_periodo = "", String carrera = "", String ind_migrado = "", String curso = "", String id = "", String page = "1", String limit = "10")
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EnrollmentsDAO objUsuarioDAO = new EnrollmentsDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";






                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                String p_periodo_objt = "";
                String p_parte_periodo_objt = "";
                String p_carrera_objt = "";
                String p_ind_migrado_objt = "";
                String p_curso_objt = "";
                String p_id_objt = "";

                p_tipo_objt = tipo;// jsondata_objt.p_tipo;
                p_periodo_objt = periodo;
                p_parte_periodo_objt = parte_periodo;
                p_carrera_objt = carrera;
                p_ind_migrado_objt = ind_migrado;
                p_curso_objt = curso;
                p_id_objt = id;

                // String p_code = (string)jsondata.p_code.ToString();


                //String p_periodo_objt = "";// jsondata_objt.p_tipo;
                //String p_titulo_objt = "";
                //String p_code = jsondata_objt.p_code;

                //p_periodo_objt = jsondata_objt.p_periodo;
                //p_titulo_objt = jsondata_objt.p_titulo;



                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_parte_periodo", p_parte_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString = claseJSON.DictionaryToString(parametros);

                DataTable dt_proceso = null;

                // dt_proceso = objUsuarioDAO.GetResultados(p_periodo_objt, p_id_objt);
                dt_proceso = objUsuarioDAO.GetCargados_paginados(p_datosString, page, limit);


                if (dt_proceso == null)
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R300, message = "No hay Datos." };
                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R300, message = "No hay Datos." };
                }

                String numero_registros_actual = (dt_proceso.Rows.Count).ToString();

                String numero_registros = dt_proceso.Rows[0]["TOTAL_REG"].ToString();

                decimal ultima_pagina = (Convert.ToInt32(numero_registros) / Convert.ToInt32(limit));

                //string json = JsonConvert.SerializeObject(dt_proceso, Formatting.Indented);

                return new ResponseApi_paginado
                {
                    success = (int)ResponseCode.R200,
                    message = "OK",
                    data = Conversiones.DataTableToJson(dt_proceso)

                    ,
                    links = new ResponseApi_paginado.clslinks
                    {
                        first = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=1" + "&limit=" + limit,
                        previous = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso,
                        next = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=" + (Convert.ToInt32(page) + 1).ToString() + "&limit=" + limit,
                        last = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=" + (Math.Ceiling(ultima_pagina)).ToString() + "&limit=" + limit
                    }
                    ,
                    meta = new ResponseApi_paginado.clsmeta
                    {
                        currentPage = page,
                        itemCount = numero_registros_actual,
                        itemsPerPage = limit,
                        totalItems = numero_registros,
                        totalPages = (Math.Ceiling(ultima_pagina)).ToString()
                    }


                };



            }
            catch (Exception ex)
            {
                //var converter = new ExpandoObjectConverter();
                //var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                ////String JsonString = (string)data.ToString();
                //String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return new ResponseApi_paginado { success = (int)ResponseCode.R500, message = ex.Message.ToString() };
            }
        }



        [HttpGet, Route("GetResultadosPaginados_carga")]
        public ResponseApi_paginado GetResultadosPaginados_carga([FromQuery] HeadersParameters parameters, String tipo = "", String periodo = "", String carrera = "", String ind_migrado = "", String curso = "", String id = "", String page = "1", String limit = "10")
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EnrollmentsDAO objUsuarioDAO = new EnrollmentsDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";






                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                String p_periodo_objt = "";
                String p_carrera_objt = "";
                String p_ind_migrado_objt = "";
                String p_curso_objt = "";
                String p_id_objt = "";

                p_tipo_objt = tipo;// jsondata_objt.p_tipo;
                p_periodo_objt = periodo;
                p_carrera_objt = carrera;
                p_ind_migrado_objt = ind_migrado;
                p_curso_objt = curso;
                p_id_objt = id;

                // String p_code = (string)jsondata.p_code.ToString();


                //String p_periodo_objt = "";// jsondata_objt.p_tipo;
                //String p_titulo_objt = "";
                //String p_code = jsondata_objt.p_code;

                //p_periodo_objt = jsondata_objt.p_periodo;
                //p_titulo_objt = jsondata_objt.p_titulo;



                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString = claseJSON.DictionaryToString(parametros);

                DataTable dt_proceso = null;


                if (p_tipo_objt == "E")
                {
                    dt_proceso = objUsuarioDAO.GetCargados_paginados_carga_estudiantes(p_datosString, page, limit);
                }

                else if (p_tipo_objt == "D")
                {
                    
                         dt_proceso = objUsuarioDAO.GetCargados_paginados_carga_docentes(p_datosString, page, limit);

                }

                    // dt_proceso = objUsuarioDAO.GetResultados(p_periodo_objt, p_id_objt);



                    if (dt_proceso == null)
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R300, message = "No hay Datos." };
                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R300, message = "No hay Datos." };
                }

                String numero_registros_actual = (dt_proceso.Rows.Count).ToString();

                String numero_registros = dt_proceso.Rows[0]["TOTAL_REG"].ToString();

                decimal ultima_pagina = (Convert.ToInt32(numero_registros) / Convert.ToInt32(limit));

                //string json = JsonConvert.SerializeObject(dt_proceso, Formatting.Indented);

                return new ResponseApi_paginado
                {
                    success = (int)ResponseCode.R200,
                    message = "OK",
                    data = Conversiones.DataTableToJson(dt_proceso)

                    ,
                    links = new ResponseApi_paginado.clslinks
                    {
                        first = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=1" + "&limit=" + limit,
                        previous = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso,
                        next = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=" + (Convert.ToInt32(page) + 1).ToString() + "&limit=" + limit,
                        last = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=" + (Math.Ceiling(ultima_pagina)).ToString() + "&limit=" + limit
                    }
                    ,
                    meta = new ResponseApi_paginado.clsmeta
                    {
                        currentPage = page,
                        itemCount = numero_registros_actual,
                        itemsPerPage = limit,
                        totalItems = numero_registros,
                        totalPages = (Math.Ceiling(ultima_pagina)).ToString()
                    }


                };



            }
            catch (Exception ex)
            {
                //var converter = new ExpandoObjectConverter();
                //var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                ////String JsonString = (string)data.ToString();
                //String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return new ResponseApi_paginado { success = (int)ResponseCode.R500, message = ex.Message.ToString() };
            }
        }


        [HttpGet, Route("GetResultadosPaginados_carga_eliminados")]
        public ResponseApi_paginado GetResultadosPaginados_carga_eliminados([FromQuery] HeadersParameters parameters, String tipo = "", String periodo = "", String carrera = "", String ind_migrado = "", String curso = "", String id = "", String page = "1", String limit = "10")
        {
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            EnrollmentsDAO objUsuarioDAO = new EnrollmentsDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner, lstUpdateCurso, lstSincronizarCursos;
            List<string> lstUpdateProcesoEvalDoc;
            try
            {
                String p_jwt = Conversiones.getTokenFromHeader(parameters);
                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";






                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                String p_periodo_objt = "";
                String p_carrera_objt = "";
                String p_ind_migrado_objt = "";
                String p_curso_objt = "";
                String p_id_objt = "";

                p_tipo_objt = tipo;// jsondata_objt.p_tipo;
                p_periodo_objt = periodo;
                p_carrera_objt = carrera;
                p_ind_migrado_objt = ind_migrado;
                p_curso_objt = curso;
                p_id_objt = id;

                // String p_code = (string)jsondata.p_code.ToString();


                //String p_periodo_objt = "";// jsondata_objt.p_tipo;
                //String p_titulo_objt = "";
                //String p_code = jsondata_objt.p_code;

                //p_periodo_objt = jsondata_objt.p_periodo;
                //p_titulo_objt = jsondata_objt.p_titulo;



                var parametros = new Dictionary<string, string>();
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_carrera", p_carrera_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_curso", p_curso_objt);
                parametros.Add("p_id", p_id_objt);



                var claseJSON = new utilidadesJSON();

                string p_datosString = claseJSON.DictionaryToString(parametros);

                DataTable dt_proceso = null;


                if (p_tipo_objt == "E")
                {
                    dt_proceso = objUsuarioDAO.GetCargados_paginados_eliminados_estudiantes(p_datosString, page, limit);
                }

                else if (p_tipo_objt == "D")
                {
                    dt_proceso = objUsuarioDAO.GetCargados_paginados_eliminados_docentes(p_datosString, page, limit);
                    
                }

                // dt_proceso = objUsuarioDAO.GetResultados(p_periodo_objt, p_id_objt);



                if (dt_proceso == null)
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R300, message = "No hay Datos." };
                }
                if (dt_proceso.Rows.Count <= 0)
                {
                    return new ResponseApi_paginado { success = (int)ResponseCode.R300, message = "No hay Datos." };
                }

                String numero_registros_actual = (dt_proceso.Rows.Count).ToString();

                String numero_registros = dt_proceso.Rows[0]["TOTAL_REG"].ToString();

                decimal ultima_pagina = (Convert.ToInt32(numero_registros) / Convert.ToInt32(limit));

                //string json = JsonConvert.SerializeObject(dt_proceso, Formatting.Indented);

                return new ResponseApi_paginado
                {
                    success = (int)ResponseCode.R200,
                    message = "OK",
                    data = Conversiones.DataTableToJson(dt_proceso)

                    ,
                    links = new ResponseApi_paginado.clslinks
                    {
                        first = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=1" + "&limit=" + limit,
                        previous = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso,
                        next = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=" + (Convert.ToInt32(page) + 1).ToString() + "&limit=" + limit,
                        last = "http://localhost:5000/api/Evaluacion/GetResultadosPaginados" + "?periodo=" + periodo + "&id=" + id + "&curso_code=" + curso + "&page=" + (Math.Ceiling(ultima_pagina)).ToString() + "&limit=" + limit
                    }
                    ,
                    meta = new ResponseApi_paginado.clsmeta
                    {
                        currentPage = page,
                        itemCount = numero_registros_actual,
                        itemsPerPage = limit,
                        totalItems = numero_registros,
                        totalPages = (Math.Ceiling(ultima_pagina)).ToString()
                    }


                };



            }
            catch (Exception ex)
            {
                //var converter = new ExpandoObjectConverter();
                //var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                ////String JsonString = (string)data.ToString();
                //String jsonResult = FormatRespuestaJSON((int)ResponseCode.R500, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return new ResponseApi_paginado { success = (int)ResponseCode.R500, message = ex.Message.ToString() };
            }
        }


    }
}