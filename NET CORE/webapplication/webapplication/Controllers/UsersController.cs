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
    public class UsersController : ControllerBase
    {
        private IConfiguration configuration;

        public UsersController(IConfiguration iConfig)
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
            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
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
                String p_id_objt = "";// jsondata_objt.p_id;
                String p_tipo_objt = "";// jsondata_objt.p_tipo;
                String p_ind_migrado_objt = "";// jsondata_objt.p_ind_migrado;
                String p_periodo_objt = "";// jsondata_objt.p_periodo;


                if (jsondata.p_accion == "sincronizar_BANNER" || jsondata.p_tabla == "users_nocargados")
                {
                    p_periodo_objt = jsondata_objt.p_periodo;
                    p_tipo_objt = jsondata_objt.p_tipo;

                }
                else if ( jsondata.p_accion == "ver_INDIVIDUAL") {

                    p_id_objt = jsondata_objt.p_id;
                    p_tipo_objt = jsondata_objt.p_tipo;
                }


                else {
                    p_id_objt = jsondata_objt.p_id;
                    p_tipo_objt = jsondata_objt.p_tipo;
                    p_ind_migrado_objt = jsondata_objt.p_ind_migrado;

                }




                var parametros = new Dictionary<string, string>();
                parametros.Add("p_id", p_id_objt);
                parametros.Add("p_tipo", p_tipo_objt);
                parametros.Add("p_ind_migrado", p_ind_migrado_objt);
                parametros.Add("p_periodo", p_periodo_objt);


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

                    if (String.IsNullOrEmpty(p_periodo_objt) || String.IsNullOrEmpty(p_tipo_objt)) { JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha ingresado el Periodo o el tipo de Usuario", "[]"); return JsonResult; }

                    lstSincronizarBanner = objUsuarioDAO.SincronizarBannerUsuariosMasivo(p_periodo_objt, p_tipo_objt, configuration.GetSection("MySettings").GetSection("entornoCANVAS").Value, jsondata.p_usuario);
                    JsonResult = FormatRespuestaJSON(Convert.ToInt32(lstSincronizarBanner[0]), lstSincronizarBanner[1], "[]");
                    return JsonResult;

                }

                else if (jsondata.p_accion == "ver_INDIVIDUAL")
                {
                    if (String.IsNullOrEmpty(p_id_objt) || String.IsNullOrEmpty(p_tipo_objt)) { JsonResult = FormatRespuestaJSON((int)ResponseCode.R500, "No ha ingresado el ID o el tipo de Usuario", "[]"); return JsonResult; }

                    ResponseDB responseDB = new UsuarioDAO(configuration).getUsuarioIndividual(p_id_objt, p_tipo_objt);

                    if (responseDB.success == (int)ResponseCode.R200)
                    {
                        string json = JsonConvert.SerializeObject(responseDB.data, Formatting.Indented);
                        JsonResult = FormatRespuestaJSON(responseDB.success, responseDB.message,json);
                        return JsonResult;
                    }
                    else
                    {
                        JsonResult = FormatRespuestaJSON(responseDB.success, responseDB.message, "[]");
                        return JsonResult;
                    }

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
                    if (jsondata.p_tabla == "users")
                    {
                        
                        dataTable = objUsuarioDAO.GetUsuariosCargados(p_id_objt, p_tipo_objt, p_ind_migrado_objt);
                    }
                    else if (jsondata.p_tabla == "users_nocargados")
                    {
                        

                        dataTable = objUsuarioDAO.GetUsuariosNoCargados(p_periodo_objt, p_tipo_objt);
                    }
                   else if (jsondata.p_tabla == "sections")
                    {
                        
                        //dataTable = objCursoDAO.GetSeccionesCargados(p_periodo_objt, p_curso_departamento_objt, p_ind_migrado_objt, p_sis);
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





        [HttpPost, Route("Sincronizar")]
        public String Sincronizar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPIUser objCanvasAPIUser= new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
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

                if (jsondata.p_accion == "sincronizar_USUARIOS")
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


                String codes_json = "{\"Codes\":" + p_codesString + "}";
                var s_lstCodes = JsonConvert.DeserializeObject<RootObject>(codes_json);
                List<p_codesModel> lstCodes = s_lstCodes.Codes;

                List<p_codesModelCanvas> lstCodes_API = new List<p_codesModelCanvas>();
                List<p_codesModelCanvas> lstCodes_Migrados = new List<p_codesModelCanvas>();
                List<p_codesModel> lstCodes_No_Migrados = new List<p_codesModel>();


                ResponseDB responseDB = new UsuarioDAO(configuration).getCuentasUsuario();

                if (responseDB.success != (int)ResponseCode.R200)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, responseDB.message, "[]");
                }

                DataTable dt_cuentas = responseDB.datatable;


                Int32 numero_codigos = lstCodes.Count;

                if (numero_codigos > 290)
                {

                    foreach (DataRow dr in dt_cuentas.Rows) {

                        String id_cuenta_canvas = dr["CUENTA_CANVAS"].ToString();

                    


                    int i = 1;

                    do
                    {
                        ResponseApi rpt_usuario_API = objCanvasAPIUser.usuarios_canvas_paginados(i.ToString(),"100", id_cuenta_canvas);


                        if (rpt_usuario_API.success == (int)ResponseCode.R200)
                        {

                            List<UsuarioCanvas> lista_curso_api = rpt_usuario_API.data;

                            foreach (UsuarioCanvas curso_api in lista_curso_api)
                            {
                                String code = curso_api.sis_user_id;

                                DateTime dateTime = Convert.ToDateTime(curso_api.created_at);
                                string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = code, p_fecha_migracion = date_s };

                                lstCodes_API.Add(p_code2);

                            }


                        }
                        i++;

                    } while (i < 401);


                    foreach (p_codesModel p_code in lstCodes)
                    {
                        int match = 0;

                        String p_sis = "";
                        String p_fecha = "";

                        foreach (p_codesModelCanvas curso_api in lstCodes_API)
                        {
                            if (p_code.p_code == curso_api.p_sis)
                            {
                                match++;
                                p_sis = curso_api.p_sis;
                                p_fecha = curso_api.p_fecha_migracion;
                            }

                        }

                        if (match > 0)
                        {
                            p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = p_sis, p_fecha_migracion = p_fecha };
                            lstCodes_Migrados.Add(p_code2);
                        }
                        else {
                            lstCodes_No_Migrados.Add(p_code);
                        }


                    }



                }


                }
                else
                {



                    foreach (p_codesModel p_code in lstCodes)
                    {
                        String code = p_code.p_code;
                        //int count_match = 0;


                        ResponseApi rpt_usuario_API = objCanvasAPIUser.get_user_by_id(code);


                        if (rpt_usuario_API.success == (int)ResponseCode.R200)
                        {

                            UsuarioCanvas usuario_api = rpt_usuario_API.data;

                            DateTime dateTime = Convert.ToDateTime(usuario_api.created_at);
                            string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                            p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = code, p_fecha_migracion = date_s };
                            lstCodes_Migrados.Add(p_code2);


                        }
                        else
                        {

                            lstCodes_No_Migrados.Add(p_code);
                        }

                    }
                }

                List<List<p_codesModelCanvas>> list_ListCodes_Migrados = SplitList(lstCodes_Migrados, 150);
               
                foreach (List<p_codesModelCanvas> list_p_code in list_ListCodes_Migrados)
                {
                     String jsoncodes = "[";
                    
                    int i = 0;
                    foreach (p_codesModelCanvas p_code in list_p_code)
                    {
                        i++;
                        String code = p_code.p_sis;
                        String fecha = p_code.p_fecha_migracion;

                        if (i == list_p_code.Count)
                        {
                             jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_fecha_migracion\":\"" + fecha + "\"}";
                            //jsoncodes += code;
                        }
                        else
                        {
                            jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_fecha_migracion\":\"" + fecha + "\"}" + ",";

                            //jsoncodes += code + ",";

                        }


                    }
                    jsoncodes += "]";

                    
                    lstSincronizarCursos = objUsuarioDAO.SincronizarUsuarioCargado(jsoncodes, "[]", jsondata.p_usuario);


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


                    lstSincronizarCursos = objUsuarioDAO.SincronizarUsuarioCargado("[]", jsoncodes, jsondata.p_usuario);


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
            CanvasAPIUser objCanvasAPIUser = new CanvasAPIUser(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            UsuarioDAO objUsuarioDAO = new UsuarioDAO(configuration);
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
                                        if (import_canvas_r.workflow_state == "failed_with_messages") {
                                            return FormatRespuestaJSON((int)ResponseCode.R500, "La importacion Falló, revise el archivo CSV.", "[]");
                                        }

                                        i_imp = import_canvas_r.progress;
                                    }

                                }
                            }
                            //    }
                            //Consultar si el SisImport ya proceso al 100%--FIN

                            //System.Threading.Thread.Sleep(30000);

                            ///SE SINCRONIZARAN LOS CURSOS PARA VER SI ESTAN MIGRADOS


                            #region sincronizar

                            String codes_json = "{\"Codes\":" + p_codesString + "}";
                            var s_lstCodes = JsonConvert.DeserializeObject<RootObject>(codes_json);
                            List<p_codesModel> lstCodes = s_lstCodes.Codes;

                            List<p_codesModelCanvas> lstCodes_API = new List<p_codesModelCanvas>();
                            List<p_codesModelCanvas> lstCodes_Migrados = new List<p_codesModelCanvas>();
                            List<p_codesModel> lstCodes_No_Migrados = new List<p_codesModel>();


                            ResponseDB responseDB = new UsuarioDAO(configuration).getCuentasUsuario();

                            if (responseDB.success != (int)ResponseCode.R200)
                            {
                                return FormatRespuestaJSON((int)ResponseCode.R500, responseDB.message, "[]");
                            }

                            DataTable dt_cuentas = responseDB.datatable;


                            Int32 numero_codigos = lstCodes.Count;

                            if (numero_codigos > 290)
                            {

                                foreach (DataRow dr in dt_cuentas.Rows)
                                {

                                    String id_cuenta_canvas = dr["CUENTA_CANVAS"].ToString();




                                    int i = 1;

                                    do
                                    {
                                        ResponseApi rpt_usuario_API = objCanvasAPIUser.usuarios_canvas_paginados(i.ToString(), "100", id_cuenta_canvas);


                                        if (rpt_usuario_API.success == (int)ResponseCode.R200)
                                        {

                                            List<UsuarioCanvas> lista_curso_api = rpt_usuario_API.data;

                                            foreach (UsuarioCanvas curso_api in lista_curso_api)
                                            {
                                                String code = curso_api.sis_user_id;

                                                DateTime dateTime = Convert.ToDateTime(curso_api.created_at);
                                                string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                                p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = code, p_fecha_migracion = date_s };

                                                lstCodes_API.Add(p_code2);

                                            }


                                        }
                                        i++;

                                    } while (i < 401);


                                    foreach (p_codesModel p_code in lstCodes)
                                    {
                                        int match = 0;

                                        String p_sis = "";
                                        String p_fecha = "";

                                        foreach (p_codesModelCanvas curso_api in lstCodes_API)
                                        {
                                            if (p_code.p_code == curso_api.p_sis)
                                            {
                                                match++;
                                                p_sis = curso_api.p_sis;
                                                p_fecha = curso_api.p_fecha_migracion;
                                            }

                                        }

                                        if (match > 0)
                                        {
                                            p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = p_sis, p_fecha_migracion = p_fecha };
                                            lstCodes_Migrados.Add(p_code2);
                                        }
                                        else
                                        {
                                            lstCodes_No_Migrados.Add(p_code);
                                        }


                                    }



                                }


                            }
                            else
                            {



                                foreach (p_codesModel p_code in lstCodes)
                                {
                                    String code = p_code.p_code;
                                    //int count_match = 0;


                                    ResponseApi rpt_usuario_API = objCanvasAPIUser.get_user_by_id(code);


                                    if (rpt_usuario_API.success == (int)ResponseCode.R200)
                                    {

                                        UsuarioCanvas usuario_api = rpt_usuario_API.data;

                                        DateTime dateTime = Convert.ToDateTime(usuario_api.created_at);
                                        string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                                        p_codesModelCanvas p_code2 = new p_codesModelCanvas { p_sis = code, p_fecha_migracion = date_s };
                                        lstCodes_Migrados.Add(p_code2);


                                    }
                                    else
                                    {

                                        lstCodes_No_Migrados.Add(p_code);
                                    }

                                }
                            }

                            List<List<p_codesModelCanvas>> list_ListCodes_Migrados = SplitList(lstCodes_Migrados, 150);

                            foreach (List<p_codesModelCanvas> list_p_code in list_ListCodes_Migrados)
                            {
                                String jsoncodes = "[";

                                int i = 0;
                                foreach (p_codesModelCanvas p_code in list_p_code)
                                {
                                    i++;
                                    String code = p_code.p_sis;
                                    String fecha = p_code.p_fecha_migracion;

                                    if (i == list_p_code.Count)
                                    {
                                        jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_fecha_migracion\":\"" + fecha + "\"}";
                                        //jsoncodes += code;
                                    }
                                    else
                                    {
                                        jsoncodes += "{\"p_sis\":\"" + code + "\",\"p_fecha_migracion\":\"" + fecha + "\"}" + ",";

                                        //jsoncodes += code + ",";

                                    }


                                }
                                jsoncodes += "]";


                                lstSincronizarCursos = objUsuarioDAO.SincronizarUsuarioCargado(jsoncodes, "[]", jsondata.p_usuario);


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


                                lstSincronizarCursos = objUsuarioDAO.SincronizarUsuarioCargado("[]", jsoncodes, jsondata.p_usuario);


                            }

                            Int32 numero_migrados = lstCodes_Migrados.Count;
                            Int32 numero_no_migrados = lstCodes_No_Migrados.Count;
                            Int32 numero_enviados = lstCodes.Count;

                            String respuesta = "";
                            if (numero_migrados == 0) { respuesta = "No se migro ningún Usuario."; }
                            if (numero_migrados > 0 && numero_no_migrados == 0) { respuesta = "Se migraron todos los Usuarios enviados" + "(" + numero_migrados + ")."; }
                            else { respuesta = "Se migraron " + numero_migrados.ToString() + " de " + numero_enviados.ToString() + " Usuarios enviados."; }

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

                    ResponseApi rpt_api = migrar_usuario_individual(jsondata_objt.USER_ID, jsondata.p_usuario, p_datosString);


                    return FormatRespuestaJSON(rpt_api.success, rpt_api.message, rpt_api.data);

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

                    String p_code_user = jsondata_objt.USER_ID;
                    ResponseApi rpt_usuario_API = objCanvasAPIUser.get_user_by_id(p_code_user);
                    if (rpt_usuario_API.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, rpt_usuario_API.message, "[]");
                    }

                    UsuarioCanvas UsuarioCanvas = rpt_usuario_API.data;

                    DataTable dt_user = null;

                    dt_user = objUsuarioDAO.GetUsuarioCargado(p_code_user);

                    if (dt_user == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }
                    if (dt_user.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }


                    string nombre_usuario = dt_user.Rows[0]["NOMBRE"].ToString();
                    string short_name_usuario = dt_user.Rows[0]["SHORT_NOMBRE"].ToString();
                    string sortable_name_usuario = dt_user.Rows[0]["SORTABLE_NOMBRE"].ToString();

                    string unique_id_usuario = dt_user.Rows[0]["USER_ID"].ToString();

                    string sis_user_id_usuario = dt_user.Rows[0]["USER_ID"].ToString();
                    string address_usuario = dt_user.Rows[0]["CORREO"].ToString();
                    string authentication_provider_id_usuario = dt_user.Rows[0]["AUTHENTIC_PROVIDER_ID"].ToString();

                    string user_account_id_usuario = String.IsNullOrEmpty(dt_user.Rows[0]["CUENTA_CANVAS"].ToString()) ? "1" : dt_user.Rows[0]["CUENTA_CANVAS"].ToString();

                    string usuario_migro = dt_user.Rows[0]["USUARIO_MIGRO"].ToString();


                    string usuario_modo_migracion = dt_user.Rows[0]["MODO_MIGRACION"].ToString();
                    string tipo_migracion;


                    if (usuario_modo_migracion == "A")
                    {
                        tipo_migracion = "Automático";
                    }
                    else { tipo_migracion = "Manual"; }

                    DateTime dateTime = Convert.ToDateTime(UsuarioCanvas.created_at);
                    string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);






                    //OBTENER CURSOS

                    List<UsuarioCursoVer> lista_cursos_usuario = new List<UsuarioCursoVer>();

                    int i = 1;

                    do
                    {
                        ResponseApi rpt_curso_API = objCanvasAPIUser.cursos_usuario_canvas_paginados(i.ToString(), (UsuarioCanvas.id).ToString());


                        if (rpt_curso_API.success == (int)ResponseCode.R200)
                        {

                            List<CursoCanvas> lista_curso_api = rpt_curso_API.data;

                            foreach (CursoCanvas curso_api in lista_curso_api)
                            {
                                string nombre_show = curso_api.name;
                                string codigo_show = "Código: " + curso_api.course_code + ", " + "SIS: " + curso_api.sis_course_id;
                                string periodo_show = "";
                                string roles_show = "";

                                if (curso_api.sis_course_id != null && curso_api.sis_course_id != "")
                                {

                                    periodo_show = "Periodo " + curso_api.sis_course_id.Substring(0, 6);

                                }


                                if (curso_api.enrollments != null)
                                {
                                    if (curso_api.enrollments.Count > 0)
                                    {

                                        int i_r = 1;
                                        foreach (Enrollments enrollment in curso_api.enrollments)
                                        {
                                            string estado_r = "";
                                            string tipo_r = "";

                                            if (enrollment.enrollment_state == "active")
                                            {
                                                estado_r = "Activo";
                                            }
                                            else { estado_r = "Inactivo"; }
                                            if (enrollment.type == "student")
                                            {
                                                tipo_r = "Estudiante";
                                            }
                                            else { tipo_r = "Docente"; }


                                            if (curso_api.enrollments.Count == i_r)
                                            {
                                                roles_show += estado_r + ", " + "Inscrito como " + tipo_r;
                                            }
                                            else
                                            {
                                                roles_show += estado_r + ", " + "Inscrito como " + tipo_r + ". ";
                                            }
                                            i++;
                                            break;
                                        }
                                    }

                                }




                                UsuarioCursoVer curso_ver = new UsuarioCursoVer
                                {

                                    id = curso_api.id,
                                    titulo = nombre_show,
                                    periodo = periodo_show,
                                    rol = roles_show,
                                    codigo = codigo_show

                                };

                                lista_cursos_usuario.Add(curso_ver);

                            }


                        }
                        i++;

                    } while (i < 5);


                    UsuarioVer usuario = new UsuarioVer
                    {

                        name = UsuarioCanvas.name,
                        id = (UsuarioCanvas.id).ToString(),
                        sortable_name = sortable_name_usuario,
                        short_name = short_name_usuario,
                        sis_user_id = UsuarioCanvas.sis_user_id,
                        usuario_migro = usuario_migro,
                        fecha_migracion = date_s,
                        tipo_migracion = tipo_migracion,
                        email = address_usuario,
                        imagen_url = UsuarioCanvas.avatar_url,
                        login_id = UsuarioCanvas.login_id,
                        integration_id = UsuarioCanvas.integration_id,
                        id_cuenta_canvas = user_account_id_usuario,
                        lst_cursos = lista_cursos_usuario
                    };




                    string json = JsonConvert.SerializeObject(usuario);





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










    }
}