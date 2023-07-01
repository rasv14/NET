using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using webapplication.Models;

namespace webapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSVController : ControllerBase
    {
        private IConfiguration configuration;

        public CSVController(IConfiguration iConfig)
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
        public String Tabla([FromBody] dynamic data)
        {

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog;
            try
            {

                String JsonResult, msjResult, Result;
                String p_codesString = "[]";
                String fileResult = "";


                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
                {
                    return FormatRespuestaJSON(2, "No se Envio Parametros", "[]");
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
                String p_curso_departamento_objt = jsondata_objt.p_curso_departamento;

                var parametros = new Dictionary<string, string>();
                parametros.Add("p_anio", p_anio_objt);
                parametros.Add("p_periodo", p_periodo_objt);
                parametros.Add("p_tipo_curso", p_tipo_curso_objt);
                parametros.Add("p_curso_departamento", p_curso_departamento_objt);

                //var claseJSON = new utilidadesJSON();

                string p_datosString2 = "";// claseJSON.DictionaryToString(parametros);

                p_datosString = p_datosString2;


                if (ReferenceEquals(null, jsondata.p_tabla))
                {
                    return FormatRespuestaJSON(2, "No se Envio que tabla se desea exportar.", "[]");
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

                if (!objToken.ValidateJwtToken(jsondata.jwt))
                {
                    JsonResult = FormatRespuestaJSON(2, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }

                if (jsondata.p_accion == "descargar_CSV" || jsondata.p_accion == "enviar_APICANVAS")
                {
                    if (jsondata.p_codes == "")
                    {
                        JsonResult = FormatRespuestaJSON(2, "No ha seleccionado ningun elemento", "[]");
                        Result = "2";
                        msjResult = "No ha seleccionado ningun elemento";


                        ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                        ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                        return JsonResult;
                    }
                    p_codesString = (string)jsondata.p_codes.ToString();




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
                        if (jsondata.p_accion == "ver_CARGA") {

                            dataTable_merge = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, jsoncodes);
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
                            DataView dv = dataTable.DefaultView;
                            dv.Sort = "short_name asc";
                            DataTable sortedDT = dv.ToTable();
                            dataTable = null;
                            dataTable = sortedDT;

                        }

                       


                    }
                        





                }
                else {

                    dataTable = null;
                    //COMENTARIO_BDtest: dataTable = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, p_codesString);
                   
                    //AGREGADO PARA CARGA INICIAL***
                    if (jsondata.p_accion == "ver_CARGA")
                    {
                        dataTable = objCSVDAO.GetDataCARGA(jsondata.accion, p_datosString, "");
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


                        JsonResult = FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                        Result = "3";
                        msjResult = "No hay registros para mostrar";





                        // return JsonResult;
                    }
                    else
                    {
                        if (jsondata.p_accion == "enviar_APICANVAS")
                        {

                            JsonResult = FormatRespuestaJSON(3, "En proceso de envio", "[]");
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
                            JsonResult = FormatRespuestaJSON(1, "OK", "[]");
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

                            JsonResult = FormatRespuestaJSON(1, "OK", json);
                            Result = "1";
                            msjResult = "OK";
                        }
                        //return JsonResult;


                    }
                }
                else
                { //return Ok(new { success = 3, message = "No hay registros para mostrar" });
                  //return FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                    JsonResult = FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
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
                String jsonResult = FormatRespuestaJSON(2, ex.Message, "[]");

                ///Comentado: p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                ///Comentado:  lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }







        [HttpPost, Route("Terms")]
        public String Terms([FromBody] dynamic data)
        {

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog;
            try {
             
            String JsonResult,msjResult,Result;
            

                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
            {
                return FormatRespuestaJSON(2, "No se Envio Parametros", "[]");
            }
            

            //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
            var converter = new ExpandoObjectConverter();
            var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
            String JsonString = (string)data.ToString();

                //SE inserta el LOG
                //p_data_i_log = objCSVDAO.GetDataInsetCSVLog2("dd", jsondata.jwt, "GET_TERMS","","","");
                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_TERMS", "", "", "");
                lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, "");
             

                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

            if (!objToken.ValidateJwtToken(jsondata.jwt))
            {
                    JsonResult = FormatRespuestaJSON(2, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_TERMS", lstInsertLog[2], Result, msjResult);
                    lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
            }





                //string PasswordKeyJWT = configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value;
                
                DataTable dataTable = null;
                dataTable = objCSVDAO.GetDataCSV("GET_TERMS", JsonString,"");



                if (dataTable != null)
            {
                if (dataTable.Rows.Count <= 0)
                {


                        JsonResult = FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
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

                        JsonResult = FormatRespuestaJSON(1, "OK", json);
                        Result = "1";
                        msjResult = "OK";

                        //return JsonResult;
  

                }
            }
            else { //return Ok(new { success = 3, message = "No hay registros para mostrar" });
                //return FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                    JsonResult = FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                    Result = "3";
                    msjResult = "No hay registros para mostrar";
                }


                p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_TERMS", lstInsertLog[2], Result, msjResult);

                String jsonresult = JsonResult;
                if (jsonresult.Length > 4000)
                {
                    jsonresult = JsonResult.Substring(0, 4000);
                }
                ///Comentado:  lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", jsonresult);

                return JsonResult;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON(2, ex.Message, "[]");

                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_TERMS","","2", ex.Message);
                lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }


        [HttpPost, Route("Courses")]
        public String Courses([FromBody] dynamic data)
        {
                      CSVDAO objCSVDAO = new CSVDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog;
            try {
             
            String JsonResult,msjResult,Result;
            

                //Se valida el envio de parametros
                if (ReferenceEquals(null, data))
            {
                return FormatRespuestaJSON(2, "No se Envio Parametros", "[]");
            }
            

            //Convertir el obj dinamico para poder obtener sus atributor, asi mismo se convierte en un jsostring para enviarlo a la bd
            var converter = new ExpandoObjectConverter();
            var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
            String JsonString = (string)data.ToString();

                //SE inserta el LOG
                //p_data_i_log = objCSVDAO.GetDataInsetCSVLog2("dd", jsondata.jwt, "GET_TERMS","","","");
                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_COURSES", "", "", "");
                lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, "");
             

                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

            if (!objToken.ValidateJwtToken(jsondata.jwt))
            {
                    JsonResult = FormatRespuestaJSON(2, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_COURSES", lstInsertLog[2], Result, msjResult);
                    lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
            }





                //string PasswordKeyJWT = configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value;
                
                DataTable dataTable = null;
                dataTable = objCSVDAO.GetDataCSV("GET_COURSES", JsonString,"");



                if (dataTable != null)
            {
                if (dataTable.Rows.Count <= 0)
                {


                        JsonResult = FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                        Result = "3";
                        msjResult = "No hay registros para mostrar";





                       // return JsonResult;
                }
                else
                {
                    string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
            

                        JsonResult = FormatRespuestaJSON(1, "OK", json);
                        Result = "1";
                        msjResult = "OK";

                        //return JsonResult;
  

                }
            }
            else { 
                    JsonResult = FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                    Result = "3";
                    msjResult = "No hay registros para mostrar";
                }


                p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_COURSES", lstInsertLog[2], Result, msjResult);

                String jsonresult = JsonResult;
                if (jsonresult.Length > 4000)
                {
                    jsonresult = JsonResult.Substring(0, 4000);
                }
                lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", jsonresult);

                return JsonResult;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON(2, ex.Message, "[]");

                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_COURSES","","2", ex.Message);
                lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }







    }
}