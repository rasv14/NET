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
using System.IO;
using System.Linq;
using System.Net;
using webapplication.clases;
using webapplication.Handler;
using webapplication.Helpers;
using webapplication.Models;

namespace webapplication.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SilabosController : ControllerBase
    {
        private IConfiguration configuration;

        public SilabosController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }

        public static List<List<T>> SplitList<T>(List<T> me, int size = 50)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < me.Count; i += size)
                list.Add(me.GetRange(i, Math.Min(size, me.Count - i)));
            return list;
        }

        [Route("cursos")]
        [HttpPost]
        public ResponseApi cursos([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            /*******************************************************/
            string p_jwt = Conversiones.getTokenFromHeader(parameters);
            /*******************************************************/


            dynamic jsondata = JsonConvert.DeserializeObject(data.ToString());
            dynamic jsonDataForm = JsonConvert.DeserializeObject(jsondata.p_form.ToString());
            try
            {
                string p_periodo = (string)jsonDataForm.p_periodo.ToString();
                string p_carrera = jsonDataForm.p_curso_departamento.ToString();
                string p_ind_migrado = (string)jsonDataForm.p_ind_migrado.ToString();

                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                //DAO
                p_carrera = p_carrera == "" ? "%" : p_carrera;
                //ResponseDB responseDB = new SilaboDAO(configuration).getCursos(p_periodo, p_carrera, p_ind_migrado);
                ResponseDB responseDB = new SilaboDAO(configuration).getSilabos(p_periodo, p_carrera, p_ind_migrado);

                if (responseDB.success == (int)ResponseCode.R200)
                {
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = responseDB.data };
                }
                else
                {
                    return new ResponseApi { success = (int)ResponseCode.R300, message = responseDB.message };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR: " + ex.ToString(), error = ex.ToString() };
            }


        }

        [Route("lista_actualizar")]
        [HttpPost]
        public ResponseApi lista_actualizar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            /*******************************************************/
            string p_jwt = Conversiones.getTokenFromHeader(parameters);
            /*******************************************************/


            dynamic jsondata = JsonConvert.DeserializeObject(data.ToString());
            dynamic jsonDataForm = JsonConvert.DeserializeObject(jsondata.p_form.ToString());
            try
            {
                string p_periodo = (string)jsonDataForm.p_periodo.ToString();
                string p_carrera = jsonDataForm.p_curso_departamento.ToString();
                //string p_ind_migrado = (string)jsonDataForm.p_ind_migrado.ToString();

                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                //DAO
                p_carrera = p_carrera == "" ? "%" : p_carrera;
                //ResponseDB responseDB = new SilaboDAO(configuration).getCursos(p_periodo, p_carrera, p_ind_migrado);
                ResponseDB responseDB = new SilaboDAO(configuration).getSilabosActualizar(p_periodo, p_carrera);

                if (responseDB.success == (int)ResponseCode.R200)
                {
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = responseDB.data };
                }
                else
                {
                    return new ResponseApi { success = (int)ResponseCode.R300, message = responseDB.message };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR: " + ex.ToString(), error = ex.ToString() };
            }


        }

        [Route("migrar")]
        [HttpPost]
        public ResponseApi migrar([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            /*******************************************************/
            string p_jwt = Conversiones.getTokenFromHeader(parameters);
            /*******************************************************/

            //DECLARAMOS ESTA VARIABLE AQUÍ POR SI SALTA UNA EXCEPTION PARA GENERAR EL LOG EN FORMATO .txt
            List<CursoMigradoCanvas> list_cursos_migrados = new List<CursoMigradoCanvas>();

            try
            {
                dynamic jsondata = JsonConvert.DeserializeObject(data.ToString());

                List<SisCourseId> list_sis_course_id = JsonConvert.DeserializeObject<List<SisCourseId>>(jsondata.p_lista_codigos.ToString());

                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                TokenData tokenData = objToken.GetDataFromToken(p_jwt);

                string P_ID_USUARIO = tokenData.user_id;

                List<CursoResultTable> lstCursoResult = new List<CursoResultTable>();
                String messageHTML = "";
                string message_ = "";
                HttpWebResponse response;
                StreamReader reader;
                string respuesta;
                dynamic jsonResponse = null;
                string P_SIS_COURSE_ID;
                int P_CANVAS_COURSE_ID, P_CANVAS_MODULE_ID;
                int total_cursos_migrados = 0;

                if (list_sis_course_id.Count == 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R300, message = "Lista de códigos de curso vacíos." };
                }

                /************* ************************/
                List<string> list_strings_sis_course_id = list_sis_course_id.Select(value => value.sis_course_id).ToList();

                string codes_joined = string.Join(",", list_strings_sis_course_id);

                //DAO
                DataTable dt_silabos = null;

                //String codesjson = "{\"Codes\":" + (string)jsondata.p_lista_codigos.ToString() + "}";

                //var result = JsonConvert.DeserializeObject<RootObject>(codesjson);

                List<List<string>> list_list_strings_sis_course_id = SplitList(list_strings_sis_course_id, 150);

                int i_dt = 0;
                foreach (List<string> list_strings_sis_course_id_l in list_list_strings_sis_course_id)
                {
                    //COMENTARIO_BDtest:  String jsoncodes = "[";
                    String jsoncodes = "";
                    int i = 0;
                    foreach (string p_code in list_strings_sis_course_id_l)
                    {
                        i++;
                        String code = p_code;

                        if (i == list_strings_sis_course_id_l.Count)
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


                    ResponseDB responseDB = new SilaboDAO(configuration).getSilabosUrl(jsoncodes);
                    if (responseDB.success != (int)ResponseCode.R200)
                    {
                        //return new ResponseApi {success = 200, message = "OK", data = responseDB.data };
                        return new ResponseApi { success = responseDB.success, message = responseDB.message };
                    }

                    dataTable_merge = responseDB.datatable;
                    // dataTable_merge = objCursoDAO.GetCursosCargados(p_periodo_objt, p_curso_departamento_objt);



                    //////*********/
                    ///dataTable_merge = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, jsoncodes);

                    if (i_dt == 0)
                    {
                        dt_silabos = dataTable_merge;
                    }
                    else
                    {
                        dt_silabos.Merge(dataTable_merge);
                    }


                    i_dt++;

                }





                //ResponseDB responseDB = new SilaboDAO(configuration).getSilabosUrl(codes_joined);

                //if(responseDB.success != (int)ResponseCode.R200)
                //{
                //    //return new ResponseApi {success = 200, message = "OK", data = responseDB.data };
                //    return new ResponseApi { success = responseDB.success, message = responseDB.message };
                //}

                //DataTable dt_silabos = responseDB.datatable;


                /********************************** INICIO FOREACH ****************************************************/

                foreach (var code in list_sis_course_id)
                {
                    P_SIS_COURSE_ID = code.sis_course_id;


                    string ind_silabo_migrado = extraerDato_Columna_xSIS_FromDatatable(P_SIS_COURSE_ID, dt_silabos, "SILABO_MIGRADO");
                    string IND_SILABO_MULTICARRERA = extraerDato_Columna_xSIS_FromDatatable(P_SIS_COURSE_ID, dt_silabos, "SILABO_MULTICARRERA");

                    string silabo_file = "-";

                    string silabo_file_multicarrera = "";
                    string silabo_file_multicarrera_html = "";
                    DataTable silabo_file_dt = null;

                    if (IND_SILABO_MULTICARRERA.Equals("Y"))
                    {
                        ResponseDB responseDB = new SilaboDAO(configuration).getSilabosMulticarreraUrl(P_SIS_COURSE_ID);
                        if (responseDB.success != (int)ResponseCode.R200)
                        {
                            //return new ResponseApi {success = 200, message = "OK", data = responseDB.data };
                            return new ResponseApi { success = responseDB.success, message = responseDB.message };
                        }
                        silabo_file_dt = responseDB.datatable;

                        if (silabo_file_dt == null)
                        {
                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                            //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                            continue;
                        }
                        if (silabo_file_dt.Rows.Count == 0)
                        {
                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                            //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                            continue;
                        }


                        int i = 0;
                        string link_silabo, nombre_silabo;
                        foreach (DataRow row in silabo_file_dt.Rows)
                        {
                            i++;
                            String silabo_url_multicarrera = row["SILABO_URL"].ToString();

                            if (i == silabo_file_dt.Rows.Count)
                            {
                                //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}";
                                silabo_file_multicarrera += silabo_url_multicarrera;
                            }
                            else
                            {
                                //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}" + ",";

                                silabo_file_multicarrera += silabo_url_multicarrera + ",";

                            }
                            link_silabo = "https://static.upao.edu.pe/upload/silabo/" + row["SILABO_URL"].ToString();
                            nombre_silabo = row["SILABO_NOMBRE"].ToString();
                            silabo_file_multicarrera_html += "<p>";
                            silabo_file_multicarrera_html += "<span style='font-size: 14pt;' ><a title = 'Enlace' target = '_blank' href ='" + link_silabo + "' ><img src = 'https://campusvirtual.upao.edu.pe/recursos/img/pdf.jpg' alt = 'Formato de Archivo' width = '21' height = '20' style = 'max-width: 21px;' >" + nombre_silabo + "</a></span>";
                            silabo_file_multicarrera_html += "</p>";
                        }


                    }
                    else
                    {
                        silabo_file = extraerDatoFromDatatable(P_SIS_COURSE_ID, dt_silabos);
                        if (silabo_file.Equals("-"))
                        {

                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                            //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                            continue;
                        }

                    }




                    if (ind_silabo_migrado.Equals("S"))
                    {

                        lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "El Silabo ya esta migrado" });
                        //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                        continue;
                    }

                    string SILABO_URL = "https://static.upao.edu.pe/upload/silabo/" + silabo_file;



                    //*** OBTENEMOS EL CURSO POR EL SIS_COURSE_ID
                    string URL_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + P_SIS_COURSE_ID;

                    HttpWebRequest tRequest;
                    tRequest = WebRequest.Create(URL_CANVAS) as HttpWebRequest;
                    tRequest.Method = "GET";
                    tRequest.ContentType = "application/json";
                    tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                    try
                    {
                        response = tRequest.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException we)
                    {
                        response = (HttpWebResponse)we.Response;
                    }

                    reader = new StreamReader(response.GetResponseStream());
                    var buffer = reader.ReadToEnd();
                    respuesta = buffer.ToString();
                    reader.Close();

                    //*** VERIFICAMOS SI EL CURSO EXISTE EN CANVAS (FUE MIGRADO)
                    if ((int)response.StatusCode == 200)
                    {
                        message_ = "LISTA DE MODULOS";
                        CursoCanvas jsonCurso = JsonConvert.DeserializeObject<CursoCanvas>(respuesta);

                        P_CANVAS_COURSE_ID = jsonCurso.id;

                        //*** PROCEDEMOS A CONECTARNOS A CANVAS CON EL CÓDIGO DEL CURSO PARA OBTENER LOS MÓDULOS
                        URL_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + P_CANVAS_COURSE_ID + "/modules";

                        tRequest = WebRequest.Create(URL_CANVAS) as HttpWebRequest;
                        tRequest.Method = "GET";
                        tRequest.ContentType = "application/json";
                        tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                        try
                        {
                            response = tRequest.GetResponse() as HttpWebResponse;
                        }
                        catch (WebException we)
                        {
                            response = (HttpWebResponse)we.Response;
                        }

                        reader = new StreamReader(response.GetResponseStream());
                        var buffer2 = reader.ReadToEnd();
                        respuesta = buffer2.ToString();
                        reader.Close();

                        //*** VERIFICAMOS SI TIENE MÓDULOS CREADOS
                        if ((int)response.StatusCode == 200)
                        {

                            if (respuesta.Equals("[]"))//*** NO EXISTE MODULO
                            {
                                ResponseApi responseModule = ApiCrearModuloCanvas(P_CANVAS_COURSE_ID, "Información General", 1);

                                //*** VERIFICAMOS SI EL MÓDULO SE REGISTRÓ CORRECTAMENTE
                                if (responseModule.success == (int)ResponseCode.R200)
                                {
                                    ModuloCanvas moduloCanvas = (ModuloCanvas)responseModule.data;

                                    
                                    if (!IND_SILABO_MULTICARRERA.Equals("Y")) //VERIFICAMOS SI EL SILABO NO ES MULTICARRERA
                                    {
                                        ResponseApi responseItem = ApiCrearItem(P_CANVAS_COURSE_ID, moduloCanvas.id, SILABO_URL, "Sílabo del Curso", 1);

                                        if (responseItem.success == (int)ResponseCode.R200)
                                        {
                                            list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItem.data, url = silabo_file, ind_multicarrera = "N" });
                                            total_cursos_migrados++;
                                        }
                                    }
                                    else
                                    {

                                        ResponseApi responsePage = ApiCrearPage(P_CANVAS_COURSE_ID, "Sílabo del Curso", silabo_file_multicarrera_html);


                                        if (responsePage.success != (int)ResponseCode.R200)
                                        {
                                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Página para Silabo Multicarrera" });
                                            continue;
                                        }

                                        PageCanvas Pageitem = responsePage.data;

                                        ResponseApi responseItemPage = ApiCrearItemPage(P_CANVAS_COURSE_ID, moduloCanvas.id, Pageitem.url, "Sílabo del Curso", 1);


                                        if (responseItemPage.success == (int)ResponseCode.R200)
                                        {
                                            list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItemPage.data, url = silabo_file_multicarrera, ind_multicarrera = "Y", page = Pageitem });
                                            total_cursos_migrados++;
                                        }
                                        else
                                        {
                                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Item para Silabo Multicarrera" });
                                            continue;
                                        }



                                    }
                                }

                                //*** EL MODULO NO SE CREÓ
                                if (responseModule.success != (int)ResponseCode.R200)
                                {
                                    lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                                    //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>el Módulo no se Creo en CANVAS.</p>";
                                    //return responseModule;
                                }
                            }
                            else //*** EXISTE MODULO
                            {

                                List<ModuloCanvas> splashInfo = JsonConvert.DeserializeObject<List<ModuloCanvas>>(respuesta);


                                ModuloCanvas moduloCanvas_existente = null;
                                int id_modulo_existente = 0;

                                foreach (ModuloCanvas modulo_canvas in splashInfo)
                                {

                                    String nombre_modulo = modulo_canvas.name;
                                    if (nombre_modulo.ToUpper() == "INFORMACION GENERAL" || nombre_modulo.ToUpper() == "INFORMACIÓN GENERAL" || nombre_modulo.ToUpper() == "GENERAL")
                                    {
                                        id_modulo_existente = modulo_canvas.id;
                                        moduloCanvas_existente = modulo_canvas;
                                        break;
                                    }

                                }

                                if (id_modulo_existente > 0)
                                {

                                    //respuesta.Equals("[]"

                                    ResponseApi responseItem_1 = ApiGetItems(P_CANVAS_COURSE_ID, id_modulo_existente);
                                    if (responseItem_1.success != (int)ResponseCode.R200)
                                    {
                                        lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al obtener Items." });
                                        continue;
                                    }

                                    if (responseItem_1.data.Equals("[]"))//No Existen items
                                    {
                                        //Si no existen items se crea uno para el silabo
                                        ModuloCanvas moduloCanvas = moduloCanvas_existente;
                                      

                                        if (!IND_SILABO_MULTICARRERA.Equals("Y")) //VERIFICAMOS SI EL SILABO NO ES MULTICARRERA
                                        {
                                            ResponseApi responseItem = ApiCrearItem(P_CANVAS_COURSE_ID, moduloCanvas.id, SILABO_URL, "Sílabo del Curso", 1);

                                            if (responseItem.success == (int)ResponseCode.R200)
                                            {
                                                list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItem.data, url = silabo_file, ind_multicarrera = "N" });
                                                total_cursos_migrados++;
                                            }
                                        }
                                        else
                                        {

                                            ResponseApi responsePage = ApiCrearPage(P_CANVAS_COURSE_ID, "Sílabo del Curso", silabo_file_multicarrera_html);


                                            if (responsePage.success != (int)ResponseCode.R200)
                                            {
                                                lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Página para Silabo Multicarrera" });
                                                continue;
                                            }

                                            PageCanvas Pageitem = responsePage.data;

                                            ResponseApi responseItemPage = ApiCrearItemPage(P_CANVAS_COURSE_ID, moduloCanvas.id, Pageitem.url, "Sílabo del Curso", 1);


                                            if (responseItemPage.success == (int)ResponseCode.R200)
                                            {
                                                list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItemPage.data, url = silabo_file_multicarrera, ind_multicarrera = "Y", page = Pageitem });
                                                total_cursos_migrados++;
                                            }
                                            else
                                            {
                                                lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Item para Silabo Multicarrera" });
                                                continue;
                                            }



                                        }

                                    }
                                    else
                                    {
                                        //EXISTEN ITEMS

                                        List<ItemCanvas> splashInfo_1 = JsonConvert.DeserializeObject<List<ItemCanvas>>(responseItem_1.data);

                                        //SE VERIFICA SI EXISTE UN ITEM CON EL NOMBRE DE SILABO DEL CURSO
                                        ItemCanvas itenCanvas_existente = null;
                                        int id_item_existente = 0;

                                        foreach (ItemCanvas item_canvas in splashInfo_1)
                                        {

                                            String nombre_modulo = item_canvas.title;
                                            if (nombre_modulo.ToUpper() == "SÍLABO DEL CURSO" || nombre_modulo.ToUpper() == "SILABO DEL CURSO")
                                            {
                                                id_item_existente = item_canvas.id;
                                                itenCanvas_existente = item_canvas;
                                                break;
                                            }

                                        }
                                        if (id_item_existente > 0)//SI EXISTE UN ITEM PARA SILABO, SOLO SE AGREGA PARA ACTUALIZAR LA BD***ACTUALIZACION*** SI ES MULTICARRERA SE CREA UNA PAGINA Y SE ACTUALIZA EL ITEM
                                        {
                                            ModuloCanvas moduloCanvas = moduloCanvas_existente;
                                            //list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = itenCanvas_existente, url = silabo_file, ind_multicarrera = "N" });
                                            //total_cursos_migrados++;
                                            if (!IND_SILABO_MULTICARRERA.Equals("Y")) //VERIFICAMOS SI EL SILABO NO ES MULTICARRERA
                                            {
                                                
                                                    list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = itenCanvas_existente, url = silabo_file, ind_multicarrera = "N" });
                                                    total_cursos_migrados++;
                                                
                                            }
                                            else
                                            {

                                                ResponseApi responseDeleteItem = ApiDeleteItem(P_CANVAS_COURSE_ID, moduloCanvas.id, id_item_existente);

                                                if (responseDeleteItem.success != (int)ResponseCode.R200)
                                                {
                                                    lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Reemplazar el Item" });
                                                    continue;
                                                }


                                                ResponseApi responsePage = ApiCrearPage(P_CANVAS_COURSE_ID, "Sílabo del Curso", silabo_file_multicarrera_html);


                                                if (responsePage.success != (int)ResponseCode.R200)
                                                {
                                                    lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Página para Silabo Multicarrera" });
                                                    continue;
                                                }

                                                PageCanvas Pageitem = responsePage.data;

                                                ResponseApi responseItemPage = ApiCrearItemPage(P_CANVAS_COURSE_ID, moduloCanvas.id, Pageitem.url, "Sílabo del Curso", 1);


                                                if (responseItemPage.success == (int)ResponseCode.R200)
                                                {
                                                    list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItemPage.data, url = silabo_file_multicarrera, ind_multicarrera = "Y", page = Pageitem });
                                                    total_cursos_migrados++;
                                                }
                                                else
                                                {
                                                    lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Item para Silabo Multicarrera" });
                                                    continue;
                                                }




                                            }

                                        }
                                        else
                                        {//SI NO EXISTE SE CREA EL ITEM PARA EL SILABO

                                            ModuloCanvas moduloCanvas = moduloCanvas_existente;


                                            if (!IND_SILABO_MULTICARRERA.Equals("Y")) //VERIFICAMOS SI EL SILABO NO ES MULTICARRERA
                                            {
                                                ResponseApi responseItem = ApiCrearItem(P_CANVAS_COURSE_ID, moduloCanvas.id, SILABO_URL, "Sílabo del Curso", 1);

                                                if (responseItem.success == (int)ResponseCode.R200)
                                                {
                                                    list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItem.data, url = silabo_file, ind_multicarrera = "N" });
                                                    total_cursos_migrados++;
                                                }
                                            }
                                            else
                                            {

                                                ResponseApi responsePage = ApiCrearPage(P_CANVAS_COURSE_ID, "Sílabo del Curso", silabo_file_multicarrera_html);


                                                if (responsePage.success != (int)ResponseCode.R200)
                                                {
                                                    lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Página para Silabo Multicarrera" });
                                                    continue;
                                                }

                                                PageCanvas Pageitem = responsePage.data;

                                                ResponseApi responseItemPage = ApiCrearItemPage(P_CANVAS_COURSE_ID, moduloCanvas.id, Pageitem.url, "Sílabo del Curso", 1);


                                                if (responseItemPage.success == (int)ResponseCode.R200)
                                                {
                                                    list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItemPage.data, url = silabo_file_multicarrera, ind_multicarrera = "Y", page = Pageitem });
                                                    total_cursos_migrados++;
                                                }
                                                else
                                                {
                                                    lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Item para Silabo Multicarrera" });
                                                    continue;
                                                }



                                            }




                                        }





                                    }








                                }
                                else
                                {


                                    /*message_ = "LISTA DE ITEMS";
                                    List<ModuloCanvas> jsonModulos = JsonConvert.DeserializeObject<List<ModuloCanvas>>(respuesta);

                                    //OBTENEMOS EL PRIMER MODULO ID
                                    P_CANVAS_MODULE_ID = jsonModulos[0].id;

                                    //PROCEDEMOS A CONECTARNOS A CANVAS PARA OBTENER LOS ITEMS DEL PRIMER MÓDULO
                                    URL_CANVAS = "https://upao.beta.instructure.com/api/v1/courses/" + P_CANVAS_COURSE_ID + "/modules/" + P_CANVAS_MODULE_ID + "/items";

                                    tRequest = WebRequest.Create(URL_CANVAS) as HttpWebRequest;
                                    tRequest.Method = "GET";
                                    tRequest.ContentType = "application/json";
                                    tRequest.Headers.Add("Authorization", "Bearer 17977~j0Uh3PvknF2wNUj7a1dGPHosa6mbqRQQBr2pT6v3F3FNKCCQD2zyNUG3uTXftCeL");

                                    try
                                    {
                                        response = tRequest.GetResponse() as HttpWebResponse;
                                    }
                                    catch (WebException we)
                                    {
                                        response = (HttpWebResponse)we.Response;
                                    }

                                    reader = new StreamReader(response.GetResponseStream());
                                    var buffer3 = reader.ReadToEnd();
                                    respuesta = buffer3.ToString();
                                    reader.Close();

                                    //*** VERIFICAMOS SI LA RESPUESTA ES 200 PARA PROCEDER A CREAR EL ITEM
                                    if ((int)response.StatusCode == 200)
                                    {
                                        ResponseApi responseItem = ApiCrearItem(P_CANVAS_COURSE_ID, P_CANVAS_MODULE_ID, SILABO_URL, "ITEM SILABO", 1);

                                        if (responseItem.success == (int)ResponseCode.R200)
                                        {
                                            list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItem.data });
                                            list_silabos_migrados.Add(new SisCourseId() { sis_course_id = P_SIS_COURSE_ID });
                                            total_cursos_migrados++;
                                        }

                                    }*/
                                    ResponseApi responseModule = ApiCrearModuloCanvas(P_CANVAS_COURSE_ID, "Información General", 1);

                                    //*** VERIFICAMOS SI EL MÓDULO SE REGISTRÓ CORRECTAMENTE
                                    if (responseModule.success == (int)ResponseCode.R200)
                                    {
                                        ModuloCanvas moduloCanvas = (ModuloCanvas)responseModule.data;




                                        if (!IND_SILABO_MULTICARRERA.Equals("Y")) //VERIFICAMOS SI EL SILABO NO ES MULTICARRERA
                                        {
                                            ResponseApi responseItem = ApiCrearItem(P_CANVAS_COURSE_ID, moduloCanvas.id, SILABO_URL, "Sílabo del Curso", 1);

                                            if (responseItem.success == (int)ResponseCode.R200)
                                            {
                                                list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItem.data, url = silabo_file, ind_multicarrera = "N" });
                                                total_cursos_migrados++;
                                            }
                                        }
                                        else
                                        {

                                            ResponseApi responsePage = ApiCrearPage(P_CANVAS_COURSE_ID, "Sílabo del Curso", silabo_file_multicarrera_html);


                                            if (responsePage.success != (int)ResponseCode.R200)
                                            {
                                                lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Página para Silabo Multicarrera" });
                                                continue;
                                            }

                                            PageCanvas Pageitem = responsePage.data;

                                            ResponseApi responseItemPage = ApiCrearItemPage(P_CANVAS_COURSE_ID, moduloCanvas.id, Pageitem.url, "Sílabo del Curso", 1);


                                            if (responseItemPage.success == (int)ResponseCode.R200)
                                            {
                                                list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = moduloCanvas, item = (ItemCanvas)responseItemPage.data, url = silabo_file_multicarrera, ind_multicarrera = "Y", page = Pageitem });
                                                total_cursos_migrados++;
                                            }
                                            else
                                            {
                                                lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Crear Item para Silabo Multicarrera" });
                                                continue;
                                            }



                                        }




                                        
                                    }

                                    //*** EL MODULO NO SE CREÓ
                                    if (responseModule.success != (int)ResponseCode.R200)
                                    {
                                        lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                                        //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>el Módulo no se Creo en CANVAS.</p>";
                                        //return responseModule;
                                    }
                                }
                            }

                        }//*** FIN DE VERIFICAR SI EXISTE MÓDULO


                    }//*** FIN DE VERIFICAR SI EXISTE CURSO EN CANVAS
                    else
                    {

                        lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Curso no encontrado en Canvas." });
                        //  lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                        // messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>El curso no está migrado en CANVAS.</p>";

                    }
                    //*** EL CURSO NO EXISTE EN CANVAS - NO  FUE MIGRADO
                    if ((int)response.StatusCode == 404)
                    {
                        //message_ = "Curso no encontrado/migrado.";
                        //jsonResponse = JsonConvert.DeserializeObject<NotFoundCanvas>(respuesta);
                        //return new ResponseApi { success = (int)ResponseCode.R404, message = message_, data = jsonResponse };
                    }

                }//*************
                 // FIN FOR EACH

                /************************************* FIN FOREACH ****************************************************/

                //*** ACTUALIZAMOS LOS SILABOS CON ESTADO MIGRADO EN LA BD


                List<List<CursoMigradoCanvas>> list_list_cursos_migrados = SplitList(list_cursos_migrados, 3);

                foreach (List<CursoMigradoCanvas> list_cursos_migrados_l in list_list_cursos_migrados)
                {
                    string jsonCursosMigrados = JsonConvert.SerializeObject(list_cursos_migrados_l).ToString();

                    var json_data = new
                    {
                        p_id_usuario = P_ID_USUARIO,
                        p_cursos_migrados = jsonCursosMigrados
                    };

                    string P_DATA = JsonConvert.SerializeObject(json_data);

                    ResponseDB responseSilabos = new SilaboDAO(configuration).actualizarSilabo("SILABO_MIGRO", P_DATA);



                }
                return new ResponseApi { success = (int)ResponseCode.R200, message = "Número de sílabos migrados a canvas: " + total_cursos_migrados, error = "", data = lstCursoResult };

                //string jsonCursosMigrados = JsonConvert.SerializeObject(list_cursos_migrados).ToString();

                //var json_data = new {
                //    p_id_usuario = P_ID_USUARIO,
                //    p_cursos_migrados = jsonCursosMigrados
                //};

                //string P_DATA = JsonConvert.SerializeObject(json_data);

                //ResponseDB responseSilabos = new SilaboDAO(configuration).actualizarSilabo("SILABO_MIGRO", P_DATA);

                //if(responseSilabos.success == (int)ResponseCode.R200)
                //{
                //    return new ResponseApi { success = (int)ResponseCode.R200, message = "Número de sílabos migrados a canvas: " + total_cursos_migrados , error = responseSilabos.message,data = lstCursoResult };
                //}
                //else
                //{
                //    return new ResponseApi {success = responseSilabos.success, message = responseSilabos.message };
                //}                

            }
            catch (Exception ex)
            {
                string jsonCursosMigrados = JsonConvert.SerializeObject(list_cursos_migrados).ToString();
                genera_archivo_plano(jsonCursosMigrados);
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR", error = ex.Message };
            }
        }


        [Route("actualizar_silabo")]
        [HttpPost]
        public ResponseApi actualizar_silabo([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            /*******************************************************/
            string p_jwt = Conversiones.getTokenFromHeader(parameters);
            /*******************************************************/

            //DECLARAMOS ESTA VARIABLE AQUÍ POR SI SALTA UNA EXCEPTION PARA GENERAR EL LOG EN FORMATO .txt
            List<CursoMigradoCanvas> list_cursos_migrados = new List<CursoMigradoCanvas>();

            try
            {
                dynamic jsondata = JsonConvert.DeserializeObject(data.ToString());

                List<SisCourseId> list_sis_course_id = JsonConvert.DeserializeObject<List<SisCourseId>>(jsondata.p_lista_codigos.ToString());

                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {
                    return new ResponseApi { success = (int)ResponseCode.R900, message = "Solicitud no autorizada." };
                }

                TokenData tokenData = objToken.GetDataFromToken(p_jwt);

                string P_ID_USUARIO = tokenData.user_id;

                List<CursoResultTable> lstCursoResult = new List<CursoResultTable>();
                String messageHTML = "";
                string message_ = "";
                HttpWebResponse response;
                StreamReader reader;
                string respuesta;
                dynamic jsonResponse = null;
                string P_SIS_COURSE_ID;
                int P_CANVAS_COURSE_ID, P_CANVAS_MODULE_ID;
                int total_cursos_migrados = 0;

                if (list_sis_course_id.Count == 0)
                {
                    return new ResponseApi { success = (int)ResponseCode.R300, message = "Lista de códigos de curso vacíos." };
                }

                /************* ************************/
                List<string> list_strings_sis_course_id = list_sis_course_id.Select(value => value.sis_course_id).ToList();

                string codes_joined = string.Join(",", list_strings_sis_course_id);

                //DAO
                DataTable dt_silabos = null;
                DataTable dt_silabos_migrados = null;

                //String codesjson = "{\"Codes\":" + (string)jsondata.p_lista_codigos.ToString() + "}";

                //var result = JsonConvert.DeserializeObject<RootObject>(codesjson);

                List<List<string>> list_list_strings_sis_course_id = SplitList(list_strings_sis_course_id, 150);

                int i_dt = 0;
                foreach (List<string> list_strings_sis_course_id_l in list_list_strings_sis_course_id)
                {
                    //COMENTARIO_BDtest:  String jsoncodes = "[";
                    String jsoncodes = "";
                    int i = 0;
                    foreach (string p_code in list_strings_sis_course_id_l)
                    {
                        i++;
                        String code = p_code;

                        if (i == list_strings_sis_course_id_l.Count)
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
                    DataTable dataTable_merge_migrados = null;

                    //AGREGADO PARA CARGA INICIAL***


                    ResponseDB responseDB = new SilaboDAO(configuration).getSilabosUrl(jsoncodes);
                    if (responseDB.success != (int)ResponseCode.R200)
                    {
                        //return new ResponseApi {success = 200, message = "OK", data = responseDB.data };
                        return new ResponseApi { success = responseDB.success, message = responseDB.message };
                    }

                    ResponseDB responseDB_migrados = new SilaboDAO(configuration).getSilabosMigradosUrl(jsoncodes);
                    if (responseDB_migrados.success != (int)ResponseCode.R200)
                    {
                        //return new ResponseApi {success = 200, message = "OK", data = responseDB.data };
                        return new ResponseApi { success = responseDB_migrados.success, message = responseDB_migrados.message };
                    }

                    dataTable_merge = responseDB.datatable;
                    dataTable_merge_migrados = responseDB_migrados.datatable;


                    // dataTable_merge = objCursoDAO.GetCursosCargados(p_periodo_objt, p_curso_departamento_objt);



                    //////*********/
                    ///dataTable_merge = objCSVDAO.GetDataCSV(jsondata.accion, p_datosString, jsoncodes);

                    if (i_dt == 0)
                    {
                        dt_silabos = dataTable_merge;
                        dt_silabos_migrados = dataTable_merge_migrados;
                    }
                    else
                    {
                        dt_silabos.Merge(dataTable_merge);
                        dt_silabos_migrados.Merge(dataTable_merge_migrados);
                    }


                    i_dt++;

                }





                //ResponseDB responseDB = new SilaboDAO(configuration).getSilabosUrl(codes_joined);

                //if(responseDB.success != (int)ResponseCode.R200)
                //{
                //    //return new ResponseApi {success = 200, message = "OK", data = responseDB.data };
                //    return new ResponseApi { success = responseDB.success, message = responseDB.message };
                //}

                //DataTable dt_silabos = responseDB.datatable;


                /********************************** INICIO FOREACH ****************************************************/

                foreach (var code in list_sis_course_id)
                {
                    P_SIS_COURSE_ID = code.sis_course_id;

                    string ind_silabo_migrado = extraerDato_Columna_xSIS_FromDatatable(P_SIS_COURSE_ID, dt_silabos, "SILABO_MIGRADO");
                    string IND_SILABO_MULTICARRERA = extraerDato_Columna_xSIS_FromDatatable(P_SIS_COURSE_ID, dt_silabos, "SILABO_MULTICARRERA");

                    string silabo_file = "-";

                    string silabo_file_multicarrera = "";
                    string silabo_file_multicarrera_html = "";
                    DataTable silabo_file_dt = null;

                    if (IND_SILABO_MULTICARRERA.Equals("Y"))
                    {
                        ResponseDB responseDB = new SilaboDAO(configuration).getSilabosMulticarreraUrl(P_SIS_COURSE_ID);
                        if (responseDB.success != (int)ResponseCode.R200)
                        {
                            //return new ResponseApi {success = 200, message = "OK", data = responseDB.data };
                            return new ResponseApi { success = responseDB.success, message = responseDB.message };
                        }
                        silabo_file_dt = responseDB.datatable;

                        if (silabo_file_dt == null)
                        {
                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                            //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                            continue;
                        }
                        if (silabo_file_dt.Rows.Count == 0)
                        {
                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                            //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                            continue;
                        }


                        int i = 0;
                        string link_silabo, nombre_silabo;
                        foreach (DataRow row in silabo_file_dt.Rows)
                        {
                            i++;
                            String silabo_url_multicarrera = row["SILABO_URL"].ToString();

                            if (i == silabo_file_dt.Rows.Count)
                            {
                                //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}";
                                silabo_file_multicarrera += silabo_url_multicarrera;
                            }
                            else
                            {
                                //COMENTARIO_BDtest:  jsoncodes += "{\"p_code\":\"" + code + "\"}" + ",";

                                silabo_file_multicarrera += silabo_url_multicarrera + ",";

                            }
                            link_silabo = "https://static.upao.edu.pe/upload/silabo/" + row["SILABO_URL"].ToString();
                            nombre_silabo = row["SILABO_NOMBRE"].ToString();
                            silabo_file_multicarrera_html += "<p>";
                            silabo_file_multicarrera_html += "<span style='font-size: 14pt;' ><a title = 'Enlace' target = '_blank' href ='" + link_silabo + "' ><img src = 'https://campusvirtual.upao.edu.pe/recursos/img/pdf.jpg' alt = 'Formato de Archivo' width = '21' height = '20' style = 'max-width: 21px;' >" + nombre_silabo + "</a></span>";
                            silabo_file_multicarrera_html += "</p>";
                        }


                    }
                    else
                    {
                        silabo_file = extraerDatoFromDatatable(P_SIS_COURSE_ID, dt_silabos);
                        if (silabo_file.Equals("-"))
                        {

                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                            //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                            continue;
                        }

                    }




                     if (!ind_silabo_migrado.Equals("S"))
                    {

                        lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "El Silabo NO esta migrado" });
                        //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                        continue;
                    }

                    string SILABO_URL = "https://static.upao.edu.pe/upload/silabo/" + silabo_file;


                   // string silabo_file = extraerDatoFromDatatable(P_SIS_COURSE_ID, dt_silabos);
                   // string ind_silabo_migrado = extraerDato_Columna_xSIS_FromDatatable(P_SIS_COURSE_ID, dt_silabos, "SILABO_MIGRADO");


                    string silabo_file_migrado = extraerDatoFromDatatable(P_SIS_COURSE_ID, dt_silabos_migrados);
                    string P_MODULE_ID = extraerDato_Columna_xSIS_FromDatatable(P_SIS_COURSE_ID, dt_silabos_migrados, "MODULO_ID");
                    string P_ITEM_ID = extraerDato_Columna_xSIS_FromDatatable(P_SIS_COURSE_ID, dt_silabos_migrados, "ITEM_ID");




                    //if (silabo_file_migrado.Equals(silabo_file))
                    //{

                    //    lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "El Silabo de CANVAS es el mismo que el de CAMPUS" });
                    //    //messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>Archivo de Silabo no encontrado.</p>";
                    //    continue;
                    //}



                    //string SILABO_URL = "https://static.upao.edu.pe/upload/silabo/" + silabo_file;

                    //*** OBTENEMOS EL CURSO POR EL SIS_COURSE_ID
                    string URL_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + P_SIS_COURSE_ID;

                    HttpWebRequest tRequest;
                    tRequest = WebRequest.Create(URL_CANVAS) as HttpWebRequest;
                    tRequest.Method = "GET";
                    tRequest.ContentType = "application/json";
                    tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                    try
                    {
                        response = tRequest.GetResponse() as HttpWebResponse;
                    }
                    catch (WebException we)
                    {
                        response = (HttpWebResponse)we.Response;
                    }

                    reader = new StreamReader(response.GetResponseStream());
                    var buffer = reader.ReadToEnd();
                    respuesta = buffer.ToString();
                    reader.Close();

                    //*** VERIFICAMOS SI EL CURSO EXISTE EN CANVAS (FUE MIGRADO)
                    if ((int)response.StatusCode == 200)
                    {
                        message_ = "ITEM DE CANVAS";
                        CursoCanvas jsonCurso = JsonConvert.DeserializeObject<CursoCanvas>(respuesta);

                        P_CANVAS_COURSE_ID = jsonCurso.id;

                        ResponseApi responseItem = ApiGetItem(P_CANVAS_COURSE_ID, Int32.Parse(P_MODULE_ID), Int32.Parse(P_ITEM_ID));

                        if (responseItem.success != (int)ResponseCode.R200)
                        {
                            lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al obtener el item de CANVAS" });
                            continue;
                        }
                        ItemCanvas itemCanvas = responseItem.data;

                        if (!IND_SILABO_MULTICARRERA.Equals("Y")) //VERIFICAR SI NO ES MULTICARRERA
                        {

                            ResponseApi responseItemAct = ApiUpdateUrlItem(P_CANVAS_COURSE_ID, Int32.Parse(P_MODULE_ID), itemCanvas.id, SILABO_URL);

                            if (responseItemAct.success != (int)ResponseCode.R200)
                            {
                                lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Actualizar URL del Silabo" });
                                continue;
                            }

                            list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = new ModuloCanvas(), item = (ItemCanvas)responseItemAct.data, url = silabo_file, ind_multicarrera = "N" });
                            total_cursos_migrados++;

                        }
                        else {
                            //Como es Multicarrera se Actualiza la pagina, no el Item
                            ResponseApi responsePageAct = ApiUpdatePage(P_CANVAS_COURSE_ID, itemCanvas.page_url, silabo_file_multicarrera_html);

                            if (responsePageAct.success != (int)ResponseCode.R200)
                            {
                                lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Error al Actualizar PAGINA del Silabo" });
                                continue;
                            }

                            PageCanvas PageItem = responsePageAct.data;

                            list_cursos_migrados.Add(new CursoMigradoCanvas { curso = jsonCurso, modulo = new ModuloCanvas(), item = itemCanvas, url = silabo_file_multicarrera, ind_multicarrera = "Y", page = PageItem });
                            total_cursos_migrados++;

                            


                        }








                    }//*** FIN DE VERIFICAR SI EXISTE CURSO EN CANVAS
                    else
                    {

                        lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Curso no encontrado en Canvas." });
                        //  lstCursoResult.Add(new CursoResultTable { curso_sis = code.sis_course_id, descripcion = "Archivo de Silabo no encontrado." });
                        // messageHTML += "<p> <strong>" + code.sis_course_id + ": </strong>El curso no está migrado en CANVAS.</p>";

                    }
                    //*** EL CURSO NO EXISTE EN CANVAS - NO  FUE MIGRADO
                    if ((int)response.StatusCode == 404)
                    {
                        //message_ = "Curso no encontrado/migrado.";
                        //jsonResponse = JsonConvert.DeserializeObject<NotFoundCanvas>(respuesta);
                        //return new ResponseApi { success = (int)ResponseCode.R404, message = message_, data = jsonResponse };
                    }

                }//*************
                 // FIN FOR EACH

                /************************************* FIN FOREACH ****************************************************/

                //*** ACTUALIZAMOS LOS SILABOS CON ESTADO MIGRADO EN LA BD


                List<List<CursoMigradoCanvas>> list_list_cursos_migrados = SplitList(list_cursos_migrados, 3);

                foreach (List<CursoMigradoCanvas> list_cursos_migrados_l in list_list_cursos_migrados)
                {
                    string jsonCursosMigrados = JsonConvert.SerializeObject(list_cursos_migrados_l).ToString();

                    var json_data = new
                    {
                        p_id_usuario = P_ID_USUARIO,
                        p_cursos_migrados = jsonCursosMigrados
                    };

                    string P_DATA = JsonConvert.SerializeObject(json_data);

                    ResponseDB responseSilabos = new SilaboDAO(configuration).actualizarUrlSilaboMigrado("SILABO_MIGRO", P_DATA);



                }
                return new ResponseApi { success = (int)ResponseCode.R200, message = "Número de sílabos Actualizados: " + total_cursos_migrados, error = "", data = lstCursoResult };

                //string jsonCursosMigrados = JsonConvert.SerializeObject(list_cursos_migrados).ToString();

                //var json_data = new {
                //    p_id_usuario = P_ID_USUARIO,
                //    p_cursos_migrados = jsonCursosMigrados
                //};

                //string P_DATA = JsonConvert.SerializeObject(json_data);

                //ResponseDB responseSilabos = new SilaboDAO(configuration).actualizarSilabo("SILABO_MIGRO", P_DATA);

                //if(responseSilabos.success == (int)ResponseCode.R200)
                //{
                //    return new ResponseApi { success = (int)ResponseCode.R200, message = "Número de sílabos migrados a canvas: " + total_cursos_migrados , error = responseSilabos.message,data = lstCursoResult };
                //}
                //else
                //{
                //    return new ResponseApi {success = responseSilabos.success, message = responseSilabos.message };
                //}                

            }
            catch (Exception ex)
            {
                string jsonCursosMigrados = JsonConvert.SerializeObject(list_cursos_migrados).ToString();
                genera_archivo_plano(jsonCursosMigrados);
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR", error = ex.Message };
            }
        }



        [HttpPost, Route("ver_canvas")]
        public String ver_canvas([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            CursoDAO objCursoDAO = new CursoDAO(configuration);
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




                //Buscar Curso por Codigo SIS

                String p_code_course = jsondata_objt.CURSO_ID_CANV;
                ResponseApi rpt_curso_API = objCanvasAPICourse.get_course_by_sis(p_code_course);
                if (rpt_curso_API.success != (int)ResponseCode.R200)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, rpt_curso_API.message, "[]");
                }





                CursoCanvas CursoCanvas = rpt_curso_API.data;

                //***OBTENER  URL SILABO

                ResponseDB responseDB_migrados = new SilaboDAO(configuration).getSilabosMigradosUrl(p_code_course);
                if (responseDB_migrados.success != (int)ResponseCode.R200)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, responseDB_migrados.message, "[]");
                }
                string P_MODULE_ID = extraerDato_Columna_xSIS_FromDatatable(p_code_course, responseDB_migrados.datatable, "MODULO_ID");
                string P_ITEM_ID = extraerDato_Columna_xSIS_FromDatatable(p_code_course, responseDB_migrados.datatable, "ITEM_ID");

                string IND_MULTICARRERA = extraerDato_Columna_xSIS_FromDatatable(p_code_course, responseDB_migrados.datatable, "IND_MULTICARRERA");

                string url_silabos = "";

                if (!IND_MULTICARRERA.Equals("Y"))
                {
                    ResponseApi responseItem = ApiGetItem(CursoCanvas.id, Int32.Parse(P_MODULE_ID), Int32.Parse(P_ITEM_ID));

                    if (responseItem.success != (int)ResponseCode.R200)
                    {
                        return FormatRespuestaJSON((int)ResponseCode.R500, "Error al obtener Silabo", "[]");
                    }
                    ItemCanvas itemCanvas = responseItem.data;

                    url_silabos = itemCanvas.external_url;


                }
                else {
                    string url_silabo_individual = extraerDato_Columna_xSIS_FromDatatable(p_code_course, responseDB_migrados.datatable, "SILABO_URL");
                    string s = url_silabo_individual;
                    string[] nums = s.Split(',');
                    List<string> list_ss = new List<string>(nums);
                    string link_silabo, nombre_silabo;
                    foreach ( string url in list_ss) {

                        string str = url.Substring(0, 25);
                        var result = str.Substring(str.Length - 4);

                        // ResponseDB responseDB_silabo = new SilaboDAO(configuration).getNombreSilabo(result);
                        //  nombre_silabo = responseDB_silabo.data;
                       
                        nombre_silabo = url;

                          link_silabo = "https://static.upao.edu.pe/upload/silabo/" + url;
                       // nombre_silabo = url;
                        url_silabos += "<p>";
                        url_silabos += "<span style='font-size: 14pt;' ><a title = 'Enlace' target = '_blank' href ='" + link_silabo + "' ><img src = 'https://campusvirtual.upao.edu.pe/recursos/img/pdf.jpg' alt = 'Formato de Archivo' width = '21' height = '20' style = 'max-width: 21px;' >" + nombre_silabo + "</a></span>";
                        url_silabos += "</p>";
                    }

                }




                
                //*********

                DataTable dt_course = null;

                dt_course = objCursoDAO.GetCursoCargado(p_code_course);

                if (dt_course == null) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }
                if (dt_course.Rows.Count <= 0) { return FormatRespuestaJSON((int)ResponseCode.R500, "No se pudo obtener los datos del Curso", "[]"); }

                string name_curso = dt_course.Rows[0]["NOM_CURSO"].ToString();
                string curso_code = dt_course.Rows[0]["CURSO_CANV"].ToString();
                string curso_carrera = dt_course.Rows[0]["CARRERA"].ToString();
                string curso_periodo = dt_course.Rows[0]["PERIODO"].ToString();
                string curso_usuario_migro = dt_course.Rows[0]["USUARIO_MIGRO"].ToString();
                string curso_fecha_migracion = dt_course.Rows[0]["FECHA_MIGRACION"].ToString();
                string curso_modo_migracion = dt_course.Rows[0]["MODO_MIGRACION"].ToString();
                string tipo_migracion;
                if (curso_modo_migracion == "A")
                {
                    tipo_migracion = "Automático";
                }
                else { tipo_migracion = "Manual"; }

                DateTime dateTime = Convert.ToDateTime(CursoCanvas.created_at);
                string date_s = dateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                CursoVer curso = new CursoVer
                {
                    nombre = CursoCanvas.name,
                    id = (CursoCanvas.id).ToString(),
                    sis = p_code_course,
                    codigo = CursoCanvas.course_code,
                    carrera = curso_carrera,
                    periodo = curso_periodo,
                    estado = CursoCanvas.workflow_state,
                    cuenta = "",
                    usuario_migro = curso_usuario_migro,
                    fecha_migracion = date_s,
                    tipo_migracion = tipo_migracion,
                    ind_multicarrera= IND_MULTICARRERA,
                    url_silabo = url_silabos



                };

                string json = JsonConvert.SerializeObject(curso);





                //string json = JsonConvert.SerializeObject(CursoCanvas);

                JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "Se encontro el Curso", json);




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



        [HttpPost, Route("aprobar_silabos")]
        public String aprobar_silabos([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
        {
            CanvasAPICourse objCanvasAPICourse = new CanvasAPICourse(configuration);
            CSVDAO objCSVDAO = new CSVDAO(configuration);
            CursoDAO objCursoDAO = new CursoDAO(configuration);
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




              

                String p_periodo_objt = jsondata_objt.p_periodo;

                int aprobados_total = 0;

                int aprobados = 0;
                int aprogados_agr = 0;

                ResponseDB responseDB_aprobar= new SilaboDAO(configuration).aprobarSilabos(p_periodo_objt);
                if (responseDB_aprobar.success != (int)ResponseCode.R200)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, responseDB_aprobar.message, "[]");
                }
                aprobados =  Convert.ToInt32(responseDB_aprobar.data);

                ResponseDB responseDB_aprobar_agr = new SilaboDAO(configuration).aprobarSilabos_agr(p_periodo_objt);
                if (responseDB_aprobar_agr.success != (int)ResponseCode.R200)
                {
                    return FormatRespuestaJSON((int)ResponseCode.R500, responseDB_aprobar_agr.message, "[]");
                }
                aprogados_agr = Convert.ToInt32(responseDB_aprobar_agr.data);

                aprobados_total = aprobados + aprogados_agr;


                

                JsonResult = FormatRespuestaJSON((int)ResponseCode.R200, "Se aprobaron "+ aprobados_total + " curso(s).", "[]");




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
        public string FormatRespuestaJSON(int success, string message, string data)
        {
            return "{\"success\": \"" + success + "\" ,\"message\": \"" + message + "\", \"data\": " + data + "}";
        }

        private string extraerDatoFromDatatable(string evaluar, DataTable dt)
        {
            string silabo_url = "";
            foreach (DataRow row in dt.Rows)
            {
                if (evaluar.Equals(row["CURSO_ID_CANVAS"].ToString()))
                {
                    silabo_url = row["SILABO_URL"].ToString();
                }
            }
            return silabo_url;
        }

        private string extraerDato_Columna_xSIS_FromDatatable(string evaluar, DataTable dt, string columna)
        {
            string silabo_url = "";
            foreach (DataRow row in dt.Rows)
            {
                if (evaluar.Equals(row["CURSO_ID_CANVAS"].ToString()))
                {
                    silabo_url = row[columna].ToString();
                }
            }
            return silabo_url;
        }

        private ResponseApi ApiCrearModuloCanvas(int course_id, string title_module, int position)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules";

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("module[name]={0}&module[position]={1}", title_module, position);
                //byte[] dataStream = Encoding.UTF8.GetBytes(postString);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    ModuloCanvas jsonModulo = JsonConvert.DeserializeObject<ModuloCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonModulo };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }

        private ResponseApi ApiCrearItem(int course_id, int module_id, string external_url, string title_item, int position)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules/" + module_id + "/items";

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("module_item[type]=ExternalUrl&module_item[external_url]={0}&module_item[title]={1}&module_item[published]=true&module_item[position]={2}", external_url, title_item, position);
                //byte[] dataStream = Encoding.UTF8.GetBytes(postString);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    ItemCanvas jsonModulo = JsonConvert.DeserializeObject<ItemCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonModulo };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }

        private ResponseApi ApiCrearItemPage(int course_id, int module_id, string page_url, string title_item, int position)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules/" + module_id + "/items";

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("module_item[type]=Page&module_item[page_url]={0}&module_item[title]={1}&module_item[published]=true&module_item[position]={2}", page_url, title_item, position);
                //byte[] dataStream = Encoding.UTF8.GetBytes(postString);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    ItemCanvas jsonModulo = JsonConvert.DeserializeObject<ItemCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonModulo };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }


        private ResponseApi ApiCrearPage(int course_id, string title_item, string contenido_html)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/pages";

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("wiki_page[title]={0}&wiki_page[body]={1}&wiki_page[editing_roles]={2}&wiki_page[notify_of_update]={3}&wiki_page[published]={4}&wiki_page[front_page]={5}", title_item, contenido_html, "teachers", true, true, false);
                //byte[] dataStream = Encoding.UTF8.GetBytes(postString);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    PageCanvas jsonModulo = JsonConvert.DeserializeObject<PageCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonModulo };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }


        private ResponseApi ApiUpdatePage(int course_id, string page_url, string contenido_html)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/pages/" + page_url;

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "PUT";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("wiki_page[body]={0}", contenido_html);
                //byte[] dataStream = Encoding.UTF8.GetBytes(postString);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    PageCanvas jsonModulo = JsonConvert.DeserializeObject<PageCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonModulo };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }

        private ResponseApi ApiUpdateUrlItem(int course_id, int module_id, int item_id, string external_url)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules/" + module_id + "/items/" + item_id;

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "PUT";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("module_item[external_url]={0}", external_url);
                //byte[] dataStream = Encoding.UTF8.GetBytes(postString);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    ItemCanvas jsonModulo = JsonConvert.DeserializeObject<ItemCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonModulo };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }


        private ResponseApi ApiUpdateItemExternalToPage(int course_id, int module_id, int item_id, string page_url)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules/" + module_id + "/items/" + item_id;

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "PUT";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("module_item[type]={0}&module_item[page_url]={1}", "Page", page_url);
                //byte[] dataStream = Encoding.UTF8.GetBytes(postString);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    ItemCanvas jsonModulo = JsonConvert.DeserializeObject<ItemCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonModulo };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }

        private ResponseApi ApiGetItem(int course_id, int module_id, int item_id)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            HttpWebResponse response2;
            StreamReader reader2;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules/" + module_id + "/items/" + item_id;

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<ItemCanvas>(respuesta);

                return new ResponseApi
                {
                    success = 200,
                    message = "OK",
                    data = splashInfo
                };



            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }

        private ResponseApi ApiDeleteItem(int course_id, int module_id, int item_id)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            HttpWebResponse response2;
            StreamReader reader2;

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules/" + module_id + "/items/" + item_id;

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "DELETE";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<ItemCanvas>(respuesta);

                return new ResponseApi
                {
                    success = 200,
                    message = "OK",
                    data = splashInfo
                };



            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }



        private ResponseApi ApiGetItems(int course_id, int module_id)
        {
            //int success_ = 0;
            string message_ = "";
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            HttpWebResponse response2;
            StreamReader reader2;

            try
            {
                message_ = "LISTA DE ITEMS";


                //*** PROCEDEMOS A CONECTARNOS A CANVAS CON EL CÓDIGO DEL CURSO PARA OBTENER LOS MÓDULOS
                string URL_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/" + course_id + "/modules/" + module_id + "/items";

                tRequest = WebRequest.Create(URL_CANVAS) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer2 = reader.ReadToEnd();
                respuesta = buffer2.ToString();
                reader.Close();

                return new ResponseApi
                {
                    success = 200,
                    message = "OK",
                    data = respuesta
                };





            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }

        [Route("crear_curso")]
        [HttpPost]
        public ResponseApi ApiCrearCurso([FromForm] BodyFormCurso curso)
        {
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            //return new ResponseApi {success = 200, message = curso.name + "-" + curso.account_id };

            try
            {
                string URL_API_CANVAS = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/" + curso.account_id + "/courses";

                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("course[name]={0}&course[course_code]={1}&course[is_public_to_auth_users]={2}&course[public_syllabus]={3}&course[public_syllabus_to_auth]={4}&course[sis_course_id]={5}&course[default_view]={6}", curso.name, curso.course_code, curso.is_public_to_auth_users, curso.public_syllabus, curso.public_syllabus_to_auth, curso.sis_course_id, curso.default_view);

                tRequest.ContentLength = postString.Length;
                writer = new StreamWriter(tRequest.GetRequestStream());
                writer.Write(postString);
                writer.Close();

                try
                {
                    response = tRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    string error = we.Message;
                    response = (HttpWebResponse)we.Response;
                }

                reader = new StreamReader(response.GetResponseStream());
                var buffer = reader.ReadToEnd();
                respuesta = buffer.ToString();
                reader.Close();

                if ((int)response.StatusCode == 200)
                {
                    CursoCanvas jsonCurso = JsonConvert.DeserializeObject<CursoCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonCurso };
                }

                if ((int)response.StatusCode == 400)//BAD REQUEST
                {
                    return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                }
                else
                {
                    return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
                }

            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
            }
        }

        [Route("cursos_canvas")]
        [HttpGet]
        public ResponseApi cursos_canvas()
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses";

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<CursoCanvas>>(respuesta);

                return new ResponseApi
                {
                    success = 200,
                    message = "OK",
                    data = splashInfo
                };
            }
            catch (WebException ex)
            {
                respuesta = ex.Message;
                return new ResponseApi
                {
                    success = 500,
                    message = respuesta
                };
            }
        }

        private void genera_archivo_plano(string str)
        {
            string nombre_archivo = DateTime.Now.ToString("yyyyMMdd hh:MM:ss") + ".txt";
            nombre_archivo = nombre_archivo.Replace(":", "");
            string path = @"c:\temp\" + nombre_archivo;
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine(str);
                }
            }
        }
    }
}
