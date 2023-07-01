using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using webapplication.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

using System.Web;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;

namespace webapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CSVAPIController : ControllerBase
    {
        private IConfiguration configuration;

        

        public CSVAPIController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }


        public string FormatRespuestaJSON(int success, string message, string data)
        {
            return "{\"success\": \"" + success + "\" ,\"message\": \"" + message + "\", \"data\": " + data + "}";
        }

        public static string ToCSV( DataTable table)
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



        [HttpPost, Route("Tabla")]
        public String Tabla([FromBody] dynamic data)
        {

            CSVDAO objCSVDAO = new CSVDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog;
            try
            {

                String JsonResult, msjResult, Result;
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

                //SE inserta el LOG
                //p_data_i_log = objCSVDAO.GetDataInsetCSVLog2("dd", jsondata.jwt, "GET_TERMS","","","");
                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "", "");
                lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, "");


                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(jsondata.jwt))
                {
                    JsonResult = FormatRespuestaJSON(2, "No tiene permisos para esta accion", "[]");
                    Result = "2";
                    msjResult = "No tiene permisos para esta accion";


                    p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);

                    return JsonResult;
                }





                //string PasswordKeyJWT = configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value;

                DataTable dataTable = null;
                dataTable = objCSVDAO.GetDataCSV(jsondata.accion, JsonString,"");



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
                        p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult, fileResult);
                        lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);



                        //SUbir el cssv generado anteriormente a CANVAS mediante un llamado a su API
                        string token = "17977~LKqXdglXgLXMTH1szp3FB6V4gQWGMtQi2rYsrXzMj6QVhFSjmyNfgymylzAbjn7I";
                        string urlcanvas = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value;
                        string accountid = "97";

                        string result = CanvasAPICSV.UploadCSVFile(token, file_path, urlcanvas, accountid).Result;
                        //**********************
                        JsonResult = FormatRespuestaJSON(1, "OK", "[]");
                        Result = "1";
                        msjResult = "OK";



                    }
                }
                else
                { //return Ok(new { success = 3, message = "No hay registros para mostrar" });
                  //return FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                    JsonResult = FormatRespuestaJSON(3, "No hay registros para mostrar", "[]");
                    Result = "3";
                    msjResult = "No hay registros para mostrar";
                }


                p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult, fileResult);

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

                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, "", "2", ex.Message);
                lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }
        }


        [HttpPost, Route("Terms")]
        public String Terms([FromBody] dynamic data)
        {


            CSVDAO objCSVDAO = new CSVDAO(configuration);
            String p_data_i_log, p_data_u_log;
            List<string> lstInsertLog, lstUpdateLog;
            try
            {

                String JsonResult, msjResult, Result;


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

                        JsonResult = FormatRespuestaJSON(1, "OK", "[]");
                        Result = "1";
                        msjResult = "OK";

                        //Generar el archivo CSV en el servidor
                        String csvString = ToCSV(dataTable);
                        String file_name = "terms.csv";
                        String file_path = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV").Value + file_name;
                        String file_history = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV_history").Value + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file_name;

                        System.IO.File.WriteAllText(file_path, csvString);
                        System.IO.File.WriteAllText(file_history, csvString);//guardar un historial de archivo
                        //**********

                        //SUbir el cssv generado anteriormente a CANVAS mediante un llamado a su API
                        string token = "17977~LKqXdglXgLXMTH1szp3FB6V4gQWGMtQi2rYsrXzMj6QVhFSjmyNfgymylzAbjn7I";
                        string urlcanvas = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value;
                        string accountid = "97";

                        string result = CanvasAPICSV.UploadCSVFile(token, file_path, urlcanvas, accountid).Result;
                        //**********************


                    }
                }
                else
                {
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
                lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", jsonresult);

                return JsonResult;

            }
            catch (Exception ex)
            {
                var converter = new ExpandoObjectConverter();
                var jsondata = JsonConvert.DeserializeObject<ExpandoObject>(data.ToString(), converter) as dynamic;
                String JsonString = (string)data.ToString();
                String jsonResult = FormatRespuestaJSON(2, ex.Message, "[]");

                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_TERMS", "", "2", ex.Message);
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
            try
            {

                String JsonResult, msjResult, Result;


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

                        JsonResult = FormatRespuestaJSON(1, "OK", "[]");
                        Result = "1";
                        msjResult = "OK";

                        //Generar el archivo CSV en el servidor
                        String csvString = ToCSV(dataTable);
                        String file_name = "courses.csv";
                        String file_path = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV").Value + file_name;
                        String file_history = configuration.GetSection("MySettings").GetSection("UbicacionFilesCSV_history").Value + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file_name;

                        System.IO.File.WriteAllText(file_path, csvString);
                        System.IO.File.WriteAllText(file_history, csvString);//guardar un historial de archivo
                        //**********

                        //SUbir el cssv generado anteriormente a CANVAS mediante un llamado a su API
                        string token = "17977~LKqXdglXgLXMTH1szp3FB6V4gQWGMtQi2rYsrXzMj6QVhFSjmyNfgymylzAbjn7I";
                        string urlcanvas = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value;
                        string accountid = "97";

                        string result = CanvasAPICSV.UploadCSVFile(token, file_path, urlcanvas, accountid).Result;
                        //**********************


                    }
                }
                else
                {
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

                p_data_i_log = objCSVDAO.GetDataInsetCSVLog2(data, "GET_COURSES", "", "2", ex.Message);
                lstInsertLog = objCSVDAO.SetCSVLog("ADD_CSVLOG", p_data_i_log, JsonString, jsonResult);

                return jsonResult;
            }



        }



    }
}