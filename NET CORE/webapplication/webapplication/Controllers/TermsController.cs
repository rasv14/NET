using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using webapplication.clases;
using webapplication.Handler;
using webapplication.Helpers;
using webapplication.Models;

namespace webapplication.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TermsController : ControllerBase
    {
        private IConfiguration configuration;

        public TermsController(IConfiguration iConfig)
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
            PeriodoDAO objPeriodoDAO = new PeriodoDAO(configuration);
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



                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;
                String p_anio_objt = jsondata_objt.p_anio;
                String p_nivel_objt = jsondata_objt.p_nivel;
                //String p_tipo_curso_objt = jsondata_objt.p_tipo_curso;
                //String p_curso_departamento_objt = !String.IsNullOrEmpty(jsondata_objt.p_curso_departamento)? jsondata_objt.p_curso_departamento.ToUpper(): jsondata_objt.p_curso_departamento;
                String p_ind_migrado_objt = "";
                

                if (jsondata.p_tabla == "terms")
                {
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                 //   p_parte_periodo_objt = jsondata_objt.p_parte_periodo;
                }

              else if (jsondata.p_tabla == "terms_nocargados")
                {
                    
                  //  p_parte_periodo_objt = jsondata_objt.p_parte_periodo;
                }
                
                else if (jsondata.p_tabla == "sections")
                {
                   // p_sis = jsondata_objt.p_sis;
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                    //p_parte_periodo_objt = jsondata_objt.p_parte_periodo;
                }

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_anio", p_anio_objt);
                parametros.Add("p_nivel", p_nivel_objt);
               
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                

                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;


                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


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
                    if (jsondata.p_codes == "[]")
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                        Result = "2";
                        msjResult = "No ha seleccionado ningun elemento";


                        ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                        ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                        return JsonResult;
                    }
                    p_codesString = (string)jsondata.p_codes.ToString();


                }
                else if(jsondata.p_accion == "sincronizar_BANNER")
                {

                    if (String.IsNullOrEmpty(p_anio_objt)) { JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha ingresado el Año", "[]"); return JsonResult; }

                    lstSincronizarBanner = objPeriodoDAO.SincronizarBannerPeriodos(p_anio_objt, p_nivel_objt);
                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");
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
                        if (jsondata.p_tabla == "terms") {

                             dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, jsoncodes);

                           // dataTable_merge = objCursoDAO.GetCursosCargados(p_periodo_objt, p_curso_departamento_objt);

                            
                        }
                        //else if (jsondata.p_tabla == "courses_nocargados")
                        //{

                        //    // dataTable_merge = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, jsoncodes);

                        //    dataTable_merge = objCursoDAO.GetCursosNoCargados(p_periodo_objt, p_curso_departamento_objt);


                        //}
                        else if (jsondata.p_tabla == "sections")
                        {

                            // dataTable_merge = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, jsoncodes);

                            dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, jsoncodes);


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

                        if(jsondata.p_tabla == "terms")
                        {
                            var newDataTable = dataTable.AsEnumerable()
                             .OrderBy(r => r.Field<String>("term_id"))
                             .ThenBy(r => r.Field<String>("name"))
                             .CopyToDataTable();
                            dataTable = null;
                            dataTable = newDataTable;
                        }

                        if (jsondata.p_tabla == "courses")
                        {
                            var newDataTable = dataTable.AsEnumerable()
                             .OrderBy(r => r.Field<String>("short_name"))
                             .ThenBy(r => r.Field<String>("course_id"))
                             .CopyToDataTable();
                            dataTable = null;
                            dataTable = newDataTable;

                            //DataView dv = dataTable.DefaultView;
                            //dv.Sort = "short_name asc";
                            //DataTable sortedDT = dv.ToTable();
                            //dataTable = null;
                            //dataTable = sortedDT;

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
                    if (jsondata.p_tabla == "terms")
                    {
                        // dataTable = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, "");
                        dataTable = objPeriodoDAO.GetPeriodoCargadoInterfaz(p_anio_objt, p_nivel_objt,p_ind_migrado_objt);
                    }
                    else if (jsondata.p_tabla == "terms_nocargados")
                    {
                        // dataTable = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, "");
                        dataTable = objPeriodoDAO.GetPeriodosNoCargados(p_anio_objt, p_nivel_objt);
                    }
                    //else if (jsondata.p_tabla == "sections")
                    // {
                    //     // dataTable = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, "");
                    //     dataTable = objCursoDAO.GetSeccionesCargados2(p_periodo_objt, p_parte_periodo_objt,p_curso_departamento_objt, p_ind_migrado_objt, p_sis);
                    // }
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



        [HttpPost, Route("Tabla_reportes")]
        public String Tabla_reportes([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            CursoDAO objCursoDAO = new CursoDAO(configuration);
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



                var converter2 = new ExpandoObjectConverter();
                var jsondata_objt = JsonConvert.DeserializeObject<ExpandoObject>(p_datosString, converter2) as dynamic;
                String p_anio_objt = jsondata_objt.p_anio;
                String p_periodo_objt = jsondata_objt.p_periodo;
                String p_tipo_curso_objt = jsondata_objt.p_tipo_curso;
                String p_curso_departamento_objt = !String.IsNullOrEmpty(jsondata_objt.p_curso_departamento) ? jsondata_objt.p_curso_departamento.ToUpper() : jsondata_objt.p_curso_departamento;
                String p_ind_migrado_objt = "";
                String p_sis = "";
                String p_parte_periodo_objt = "";

                if (jsondata.p_tabla == "courses")
                {
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                }
                else if (jsondata.p_tabla == "sections")
                {
                    p_sis = jsondata_objt.p_sis;
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;
                }
                else if (jsondata.p_tabla == "courses_reporte_inconsistencias") {
                    p_parte_periodo_objt = jsondata_objt.p_parte_periodo;

                }

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_anio", p_anio_objt);
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_parte_periodo", p_parte_periodo_objt);
                parametros.Add("p_tipo_curso", p_tipo_curso_objt);
                parametros.Add("p_curso_departamento", p_curso_departamento_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_sis", p_sis);

                var claseJSON = new utilidadesJSON();

                string p_datosString2 = claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;


                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, "No se Envio que tabla se desea exportar.", "[]");
                }


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

                if (jsondata.p_accion == "descargar_CSV")
                {
                    if (jsondata.p_codes == "[]")
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                        Result = "2";
                        msjResult = "No ha seleccionado ningun elemento";


                        ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                        ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                        return JsonResult;
                    }
                    p_codesString = (string)jsondata.p_codes.ToString();


                }
                else if (jsondata.p_accion == "sincronizar_BANNER")
                {

                    if (String.IsNullOrEmpty(p_periodo_objt)) { JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha ingresado el Periodo", "[]"); return JsonResult; }

                    lstSincronizarBanner = objCursoDAO.SincronizarBannerCursos(p_periodo_objt, p_curso_departamento_objt);
                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");
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
                                jsoncodes += code;
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
                        if (jsondata.p_tabla == "courses")
                        {

                            dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, jsoncodes);

                            // dataTable_merge = objCursoDAO.GetCursosCargados(p_periodo_objt, p_curso_departamento_objt);


                        }
                        else if (jsondata.p_tabla == "courses_nocargados")
                        {

                            // dataTable_merge = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, jsoncodes);

                            dataTable_merge = objCursoDAO.GetCursosNoCargados(p_periodo_objt, p_curso_departamento_objt);


                        }
                        else if (jsondata.p_tabla == "sections")
                        {

                            // dataTable_merge = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, jsoncodes);

                            dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, p_datosString, jsoncodes);


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

                    if (dataTable != null)
                    {

                        if (jsondata.p_tabla == "terms")
                        {
                            var newDataTable = dataTable.AsEnumerable()
                             .OrderBy(r => r.Field<String>("term_id"))
                             .ThenBy(r => r.Field<String>("name"))
                             .CopyToDataTable();
                            dataTable = null;
                            dataTable = newDataTable;
                        }

                        if (jsondata.p_tabla == "courses")
                        {
                            var newDataTable = dataTable.AsEnumerable()
                             .OrderBy(r => r.Field<String>("short_name"))
                             .ThenBy(r => r.Field<String>("course_id"))
                             .CopyToDataTable();
                            dataTable = null;
                            dataTable = newDataTable;

                            //DataView dv = dataTable.DefaultView;
                            //dv.Sort = "short_name asc";
                            //DataTable sortedDT = dv.ToTable();
                            //dataTable = null;
                            //dataTable = sortedDT;

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
                else
                {

                    dataTable = null;
                    //COMENTARIO_BDtest: dataTable = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, p_codesString);

                    //AGREGADO PARA CARGA INICIAL***
                  if (jsondata.p_tabla == "courses_reporte_inconsistencias")
                    {
                        // dataTable = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, "");
                        dataTable = objCursoDAO.GetReporteInconsistencias(p_periodo_objt, p_parte_periodo_objt, p_curso_departamento_objt);
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


        [HttpPost, Route("Sincronizar")]
        public String Sincronizar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPICourse objCanvasAPICourse= new CanvasAPICourse(configuration);
            CanvasAPITerm objCanvasAPITerm = new CanvasAPITerm(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            PeriodoDAO objPeriodoDAO = new PeriodoDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog, lstSincronizarBanner,lstSincronizarCursos;
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

                    if (ReferenceEquals(null, jsondata.p_codes))
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se Selecciono ningun elemento.", "[]");
                    }

                    if (jsondata.p_codes == "[]")
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                        Result = "2";
                        msjResult = "No ha seleccionado ningun elemento";


                        ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                        ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                        return JsonResult;
                    }
                    p_codesString = (string)jsondata.p_codes.ToString();


                }




                //ResponseApi rpt_curso_API = objCanvasAPICourse.cursos_canvas();
                //if (rpt_curso_API.success != (int)ResponseCode.R200)
                //{
                //    return FormatRespuestaJSON(2, rpt_curso_API.message, "[]");
                //}

                //List<CursoCanvas> lstCursosCanvas = rpt_curso_API.data;



                //Obtener el periodo de canvas

                //String p_periodo = jsondata_objt.p_periodo;
                //String p_parte_periodo = jsondata_objt.p_parte_periodo;

                //DataTable dt_periodo = new PeriodoDAO(configuration).GetPeriodoCargado(p_periodo, p_parte_periodo);

                //if (dt_periodo == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Periodo", "[]"); }
                //if (dt_periodo.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Periodo", "[]"); }



                //foreach (DataRow dr in dt_periodo.Rows)
                //{
                //    string periodo_id = dr["ID_CANVAS"].ToString();
                //    string periodo_externo_sis = dr["PERIODO_EXTERNO"].ToString();
                //    if (String.IsNullOrEmpty(periodo_id)) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se encontro el ID de CANVAS del periodo " + periodo_externo_sis, "[]"); }

                //    ResponseApi rpt_periodo_API = objCanvasAPITerm.periodo_by_sis_term_id(periodo_externo_sis);
                //    if (rpt_periodo_API.success != (int)ResponseCode.R200)
                //    {
                //        return FormatRespuestaJSON((int)ResponseCode.R500, "No se encontro el periodo " + periodo_externo_sis + " en CANVAS", "[]");
                //    }
                //}


                //PeriodoCanvas PeriodoCanvas = rpt_periodo_API.data;
                //int periodo_id = PeriodoCanvas.id;

                //Obtener el periodo de canvas--FIN


                String codes_json = "{\"Codes\":" + p_codesString + "}";
                var s_lstCodes = JsonConvert.DeserializeObject<RootObject>(codes_json);
                List<p_codesModel> lstCodes = s_lstCodes.Codes;

                List<p_codesModelCanvas_Periodo> lstCodes_API = new List<p_codesModelCanvas_Periodo>();
                List<p_codesModelCanvas_Periodo> lstCodes_Migrados = new List<p_codesModelCanvas_Periodo>();
                List<p_codesModel> lstCodes_No_Migrados = new List<p_codesModel>();


                Int32 numero_codigos = lstCodes.Count;

                //if (numero_codigos > 101)
                //{

                     
                        int i_c = 1;

                    do
                    {
                        //   ResponseApi rpt_curso_API = objCanvasAPICourse.cursos_canvas_paginados(i.ToString());
                        ResponseApi rpt_periodo_API = objCanvasAPITerm.periodos_canvas_paginados(i_c.ToString());



                        if (rpt_periodo_API.success == (int)ResponseCode.R200)
                        {

                            List<PeriodoCanvas> lista_periodo_api = rpt_periodo_API.data.enrollment_terms;


                            if (lista_periodo_api.Count() == 0) { break; }

                            foreach (PeriodoCanvas periodo_api in lista_periodo_api)
                            {
                                String code = periodo_api.sis_term_id;
                               String id_canvas = periodo_api.id.ToString();

                                DateTime dateTime = Convert.ToDateTime(periodo_api.created_at);
                                string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                DateTime dateTime_i = Convert.ToDateTime(periodo_api.start_at);
                                string date_s_i = dateTime_i.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                DateTime dateTime_f = Convert.ToDateTime(periodo_api.end_at);
                                string date_s_f = dateTime_f.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                p_codesModelCanvas_Periodo p_code2 = new p_codesModelCanvas_Periodo { p_sis = code, p_id_canvas= id_canvas,  p_fecha_migracion = date_s , p_fecha_inicio  = date_s_i, p_fecha_fin = date_s_f };

                                lstCodes_API.Add(p_code2);

                            }


                        }
                         i_c++;

                    } while (i_c < 101);


             


                    foreach (p_codesModel p_code in lstCodes)
                    {
                        int match = 0;

                        String p_sis = "";
                        String p_id_canvas = "";
                        String p_fecha = "";
                        String p_fecha_i = "";
                        String p_fecha_f = "";

                        foreach (p_codesModelCanvas_Periodo periodo_api in lstCodes_API)
                        {
                            if (p_code.p_code == periodo_api.p_sis)
                            {
                                match++;
                                p_sis = periodo_api.p_sis;
                                p_id_canvas = periodo_api.p_id_canvas;
                                p_fecha = periodo_api.p_fecha_migracion;
                                p_fecha_i = periodo_api.p_fecha_inicio;
                                p_fecha_f = periodo_api.p_fecha_fin;
                            }
                        
                        }

                        if (match > 0)
                        {
                            p_codesModelCanvas_Periodo p_code2 = new p_codesModelCanvas_Periodo { p_sis = p_sis, p_id_canvas= p_id_canvas, p_fecha_migracion = p_fecha, p_fecha_inicio = p_fecha_i, p_fecha_fin = p_fecha_f };
                            lstCodes_Migrados.Add(p_code2);
                        }
                        else {
                            lstCodes_No_Migrados.Add(p_code);
                        }


                     }






                //}
                //else
                //{


                //    foreach (p_codesModel p_code in lstCodes)
                //    {
                //        String code = p_code.p_code;
                //        //int count_match = 0;

                //        ResponseApi rpt_curso_API = objCanvasAPICourse.get_course_by_sis(code);
                //        ResponseApi rpt_periodo_API = objCanvasAPITerm.periodos_canvas_paginados(i.ToString());

                //        if (rpt_curso_API.success == (int)ResponseCode.R200)
                //        {

                //            CursoCanvas curso_api = rpt_curso_API.data;



                //            DateTime dateTime = Convert.ToDateTime(curso_api.created_at);
                //            string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                //            p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = code, p_fecha_migracion = date_s };
                //            lstCodes_Migrados.Add(p_code2);


                //        }
                //        else
                //        {

                //            lstCodes_No_Migrados.Add(p_code);
                //        }

                //    }
                //}

                List<List<p_codesModelCanvas_Periodo>> list_ListCodes_Migrados = SplitList(lstCodes_Migrados, 70);
               
                foreach (List<p_codesModelCanvas_Periodo> list_p_code in list_ListCodes_Migrados)
                {
                     String jsoncodes = "[";
                    
                    int i = 0;
                    foreach (p_codesModelCanvas_Periodo p_code in list_p_code)
                    {
                        i++;
                        String code = p_code.p_sis;
                        String id_canvas = p_code.p_id_canvas;
                        String fecha = p_code.p_fecha_migracion;
                        String fecha_i = p_code.p_fecha_inicio;
                        String fecha_f = p_code.p_fecha_fin;

                        if (i == list_p_code.Count)
                        {
                             jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_canvas_id\":\"" + id_canvas + "\",\"p_fecha_migracion\":\"" + fecha + "\",\"p_fecha_inicio\":\"" + fecha_i + "\",\"p_fecha_fin\":\"" + fecha_f + "\"}";
                            //jsoncodes += code;
                        }
                        else
                        {
                            jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_canvas_id\":\"" + id_canvas + "\",\"p_fecha_migracion\":\"" + fecha + "\",\"p_fecha_inicio\":\"" + fecha_i + "\",\"p_fecha_fin\":\"" + fecha_f + "\"}" + ",";

                            //jsoncodes += code + ",";

                        }


                    }
                    jsoncodes += "]";

                    
                    lstSincronizarCursos = objPeriodoDAO.SincronizarPeriodoCargado(jsoncodes, "[]", jsondata.p_usuario);


                }

                List<List<p_codesModel>> list_ListCodes_No_Migrados = SplitList(lstCodes_No_Migrados, 150);

                foreach (List<p_codesModel> list_p_code in list_ListCodes_No_Migrados)
                {
                    String jsoncodes = "[";

                    int i = 0;
                    foreach (p_codesModel p_code in list_p_code)
                    {
                        i++;
                        String code = p_code.p_code;
                        

                        if (i == list_p_code.Count)
                        {
                            jsoncodes += "{\"p_sis\":\"" + code + "\"}";
                            //jsoncodes += code;
                        }
                        else
                        {
                            jsoncodes += "{\"p_sis\":\"" + code + "\"}" + ",";

                            //jsoncodes += code + ",";

                        }


                    }
                    jsoncodes += "]";


                    lstSincronizarCursos = objPeriodoDAO.SincronizarPeriodoCargado("[]", jsoncodes, jsondata.p_usuario);


                }
                





                return  FormatRespuestaJSON((int)ResponseCode.R200, "Se sincronizo con Éxito", "[]"); ;

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
            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CanvasAPITerm objCanvasAPITerm = new CanvasAPITerm(configuration);

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            PeriodoDAO objPeriodoDAO = new PeriodoDAO(configuration);
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

                    if (ReferenceEquals(null, jsondata.p_codes))
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "No se Selecciono ningun elemento.", "[]");
                    }

                    if (jsondata.p_codes == "[]")
                    {
                        JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha seleccionado ningun elemento", "[]");
                        Result = "2";
                        msjResult = "No ha seleccionado ningun elemento";
                        return JsonResult;
                    }
                    p_codesString = (string)jsondata.p_codes.ToString();




                    DataTable dataTable = null;

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
                                jsoncodes += code;
                            }
                            else
                            {
                                //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}" + ",";

                                jsoncodes += code + ",";

                            }


                        }

                        //COMENTARIO_BDtest: jsoncodes += "]";

                        DataTable dataTable_merge = null;

                        dataTable_merge = objCSVDAO.GetDatosCSV(jsondata.accion, "", jsoncodes);



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

                            //  System.Threading.Thread.Sleep(30000);

                            ///SE SINCRONIZARAN LOS CURSOS PARA VER SI ESTAN MIGRADOS


                            #region sincronizar




                            String codes_json = "{\"Codes\":" + p_codesString + "}";
                            var s_lstCodes = JsonConvert.DeserializeObject<RootObject>(codes_json);
                            List<p_codesModel> lstCodes = s_lstCodes.Codes;

                            List<p_codesModelCanvas_Periodo> lstCodes_API = new List<p_codesModelCanvas_Periodo>();
                            List<p_codesModelCanvas_Periodo> lstCodes_Migrados = new List<p_codesModelCanvas_Periodo>();
                            List<p_codesModel> lstCodes_No_Migrados = new List<p_codesModel>();


                            Int32 numero_codigos = lstCodes.Count;

                            //if (numero_codigos > 101)
                            //{


                            int i_c = 1;

                            do
                            {
                                //   ResponseApi rpt_curso_API = objCanvasAPICourse.cursos_canvas_paginados(i.ToString());
                                ResponseApi rpt_periodo_API = objCanvasAPITerm.periodos_canvas_paginados(i_c.ToString());



                                if (rpt_periodo_API.success == (int)ResponseCode.R200)
                                {

                                    List<PeriodoCanvas> lista_periodo_api = rpt_periodo_API.data.enrollment_terms;


                                    if (lista_periodo_api.Count() == 0) { break; }

                                    foreach (PeriodoCanvas periodo_api in lista_periodo_api)
                                    {
                                        String code = periodo_api.sis_term_id;
                                        String id_canvas = periodo_api.id.ToString();

                                        DateTime dateTime = Convert.ToDateTime(periodo_api.created_at);
                                        string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                        DateTime dateTime_i = Convert.ToDateTime(periodo_api.start_at);
                                        string date_s_i = dateTime_i.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                        DateTime dateTime_f = Convert.ToDateTime(periodo_api.end_at);
                                        string date_s_f = dateTime_f.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                        p_codesModelCanvas_Periodo p_code2 = new p_codesModelCanvas_Periodo { p_sis = code, p_id_canvas = id_canvas, p_fecha_migracion = date_s, p_fecha_inicio = date_s_i, p_fecha_fin = date_s_f };

                                        lstCodes_API.Add(p_code2);

                                    }


                                }
                                i_c++;

                            } while (i_c < 101);





                            foreach (p_codesModel p_code in lstCodes)
                            {
                                int match = 0;

                                String p_sis = "";
                                String p_id_canvas = "";
                                String p_fecha = "";
                                String p_fecha_i = "";
                                String p_fecha_f = "";

                                foreach (p_codesModelCanvas_Periodo periodo_api in lstCodes_API)
                                {
                                    if (p_code.p_code == periodo_api.p_sis)
                                    {
                                        match++;
                                        p_sis = periodo_api.p_sis;
                                        p_id_canvas = periodo_api.p_id_canvas;
                                        p_fecha = periodo_api.p_fecha_migracion;
                                        p_fecha_i = periodo_api.p_fecha_inicio;
                                        p_fecha_f = periodo_api.p_fecha_fin;
                                    }

                                }

                                if (match > 0)
                                {
                                    p_codesModelCanvas_Periodo p_code2 = new p_codesModelCanvas_Periodo { p_sis = p_sis, p_id_canvas = p_id_canvas, p_fecha_migracion = p_fecha, p_fecha_inicio = p_fecha_i, p_fecha_fin = p_fecha_f };
                                    lstCodes_Migrados.Add(p_code2);
                                }
                                else
                                {
                                    lstCodes_No_Migrados.Add(p_code);
                                }


                            }






                            //}
                            //else
                            //{


                            //    foreach (p_codesModel p_code in lstCodes)
                            //    {
                            //        String code = p_code.p_code;
                            //        //int count_match = 0;

                            //        ResponseApi rpt_curso_API = objCanvasAPICourse.get_course_by_sis(code);
                            //        ResponseApi rpt_periodo_API = objCanvasAPITerm.periodos_canvas_paginados(i.ToString());

                            //        if (rpt_curso_API.success == (int)ResponseCode.R200)
                            //        {

                            //            CursoCanvas curso_api = rpt_curso_API.data;



                            //            DateTime dateTime = Convert.ToDateTime(curso_api.created_at);
                            //            string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                            //            p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = code, p_fecha_migracion = date_s };
                            //            lstCodes_Migrados.Add(p_code2);


                            //        }
                            //        else
                            //        {

                            //            lstCodes_No_Migrados.Add(p_code);
                            //        }

                            //    }
                            //}

                            List<List<p_codesModelCanvas_Periodo>> list_ListCodes_Migrados = SplitList(lstCodes_Migrados, 70);

                            foreach (List<p_codesModelCanvas_Periodo> list_p_code in list_ListCodes_Migrados)
                            {
                                String jsoncodes = "[";

                                int i = 0;
                                foreach (p_codesModelCanvas_Periodo p_code in list_p_code)
                                {
                                    i++;
                                    String code = p_code.p_sis;
                                    String id_canvas = p_code.p_id_canvas;
                                    String fecha = p_code.p_fecha_migracion;
                                    String fecha_i = p_code.p_fecha_inicio;
                                    String fecha_f = p_code.p_fecha_fin;

                                    if (i == list_p_code.Count)
                                    {
                                        jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_canvas_id\":\"" + id_canvas + "\",\"p_fecha_migracion\":\"" + fecha + "\",\"p_fecha_inicio\":\"" + fecha_i + "\",\"p_fecha_fin\":\"" + fecha_f + "\"}";
                                        //jsoncodes += code;
                                    }
                                    else
                                    {
                                        jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_canvas_id\":\"" + id_canvas + "\",\"p_fecha_migracion\":\"" + fecha + "\",\"p_fecha_inicio\":\"" + fecha_i + "\",\"p_fecha_fin\":\"" + fecha_f + "\"}" + ",";

                                        //jsoncodes += code + ",";

                                    }


                                }
                                jsoncodes += "]";


                                lstSincronizarCursos = objPeriodoDAO.SincronizarPeriodoCargado(jsoncodes, "[]", jsondata.p_usuario,"A");


                            }

                            List<List<p_codesModel>> list_ListCodes_No_Migrados = SplitList(lstCodes_No_Migrados, 150);

                            foreach (List<p_codesModel> list_p_code in list_ListCodes_No_Migrados)
                            {
                                String jsoncodes = "[";

                                int i = 0;
                                foreach (p_codesModel p_code in list_p_code)
                                {
                                    i++;
                                    String code = p_code.p_code;


                                    if (i == list_p_code.Count)
                                    {
                                        jsoncodes += "{\"p_sis\":\"" + code + "\"}";
                                        //jsoncodes += code;
                                    }
                                    else
                                    {
                                        jsoncodes += "{\"p_sis\":\"" + code + "\"}" + ",";

                                        //jsoncodes += code + ",";

                                    }


                                }
                                jsoncodes += "]";


                                lstSincronizarCursos = objPeriodoDAO.SincronizarPeriodoCargado("[]", jsoncodes, jsondata.p_usuario,"A");


                            }


                            Int32 numero_migrados = lstCodes_Migrados.Count;
                            Int32 numero_no_migrados = lstCodes_No_Migrados.Count;
                            Int32 numero_enviados = lstCodes.Count;

                            String respuesta = "";
                            if (numero_migrados == 0) { respuesta = "No se migro ningún Periodo."; }
                            if (numero_migrados > 0 && numero_no_migrados == 0) { respuesta = "Se migraron todos los Periodos enviados" + "(" + numero_migrados + ")."; }
                            else { respuesta = "Se migraron " + numero_migrados.ToString() + " de " + numero_enviados.ToString() + " Periodos enviados."; }

                            #endregion

                            //**********************
                            JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, respuesta, "[]");
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

                    String p_code_periodo = jsondata_objt.SIS;

                    DataTable dt_datos = null;

                    dt_datos = objPeriodoDAO.GetPeriodoCargado(p_code_periodo);

                    if (dt_datos == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Periodo", "[]"); }
                    if (dt_datos.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Periodo", "[]"); }

                    string datos_nombre= dt_datos.Rows[0]["DESCRIPCION"].ToString();
                    string datos_sis = dt_datos.Rows[0]["SIS"].ToString();
                    string datos_fecha_inicio = dt_datos.Rows[0]["FECHA_INICIO_M"].ToString();
                    string datos_fecha_fin = dt_datos.Rows[0]["FECHA_FIN_M"].ToString();
                    int datos_account_id = 1;
                    //int course_account_id = String.IsNullOrEmpty(dt_course.Rows[0]["ACCOUNT_ID_CANVAS"].ToString())? 1:Convert.ToInt32(dt_course.Rows[0]["ACCOUNT_ID_CANVAS"].ToString());
                   
                    




                    BodyFormPeriodo body_curso = new BodyFormPeriodo
                    {
                        name = datos_nombre,
                        start_at = datos_fecha_inicio,
                        end_at = datos_fecha_fin,
                        sis_term_id = datos_sis,
                       
                    };

                    ResponseApi rpt_periodo_API = objCanvasAPITerm.crear_periodo(body_curso, datos_account_id.ToString());
                    if (rpt_periodo_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_periodo_API.message, "[]");
                    }

                    PeriodoCanvas PeriodoCanvaS = rpt_periodo_API.data;

                    DateTime dateTime = Convert.ToDateTime(PeriodoCanvaS.created_at);
                    string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                    


                    String p_ind_migrado_objt = "S";
                    String p_ind_modo_migrado_objt = "A";

                    var parametros = new Dictionary<string, string>();
                    parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                    parametros.Add("p_fecha_migracion", date_s);
                    parametros.Add("p_usuario", jsondata.p_usuario);
                    parametros.Add("p_ind_modo_migrado", p_ind_modo_migrado_objt);
                    parametros.Add("p_canvas_id", PeriodoCanvaS.id.ToString());


                    var claseJSON = new utilidadesJSON();

                    string p_datosString2 = claseJSON.DictionaryToString(parametros);

                    p_datosString = p_datosString2;

                    lstUpdateCurso = objPeriodoDAO.ActualizarPeriodoCargado("ACTUALIZAR_IND_MIGRADO", p_datosString, p_code_periodo);




                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstUpdateCurso[1]), lstUpdateCurso[0], "[]");

                }





                else
                {

                    //Buscar Curso por Codigo SIS

                    String p_code_periodo = jsondata_objt.SIS;
                    ResponseApi rpt_periodo_API = objCanvasAPITerm.periodo_by_sis_term_id(p_code_periodo);
                    if (rpt_periodo_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_periodo_API.message, "[]");
                    }

                    PeriodoCanvas PeriodoCanvas = rpt_periodo_API.data;

                    DataTable dt_datos = null;

                    dt_datos = objPeriodoDAO.GetPeriodoCargado(p_code_periodo);

                    if (dt_datos == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Periodo", "[]"); }
                    if (dt_datos.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Periodo", "[]"); }

                   

                    string datos_nombre = dt_datos.Rows[0]["DESCRIPCION"].ToString();
                    string datos_sis = dt_datos.Rows[0]["SIS"].ToString();
                    string datos_id_canvas = dt_datos.Rows[0]["ID_CANVAS"].ToString();
                    string datos_usuario_migro = dt_datos.Rows[0]["USUARIO_MIGRO"].ToString();
                    string datos_modo_migracion = dt_datos.Rows[0]["MODO_MIGRACION"].ToString();
                    

                    


                    string tipo_migracion;
                    if (datos_modo_migracion == "A")
                    {
                        tipo_migracion = "Automático";
                    }
                    else { tipo_migracion = "Manual"; }

                    DateTime dateTime = Convert.ToDateTime(PeriodoCanvas.created_at);
                    string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                    DateTime dateTime_i = Convert.ToDateTime(PeriodoCanvas.start_at);
                    string date_s_i = dateTime_i.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                    DateTime dateTime_f = Convert.ToDateTime(PeriodoCanvas.end_at);
                    string date_s_f = dateTime_f.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                    PeriodoVer periodo = new PeriodoVer
                    {
                        nombre = PeriodoCanvas.name,
                        id = (PeriodoCanvas.id).ToString(),
                        sis = p_code_periodo,
                        
                        
                        estado = PeriodoCanvas.workflow_state,
                        
                        usuario_migro = datos_usuario_migro,
                        fecha_migracion = date_s,
                        tipo_migracion = tipo_migracion,
                        fecha_inicio = date_s_i,
                        fecha_fin = date_s_f


                    };

                     

        string json = JsonConvert.SerializeObject(periodo);





                    //string json = JsonConvert.SerializeObject(CursoCanvas);

                    JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "Se encontro el Curso", json);

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










    }
}