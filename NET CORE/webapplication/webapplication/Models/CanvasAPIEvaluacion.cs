using System;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using webapplication.clases;
using Microsoft.Extensions.Configuration;
using webapplication.Helpers;

namespace webapplication.Models
{
    public class CanvasAPIEvaluacion
    {

        public CanvasAPIEvaluacion(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public ResponseApi cursos_canvas()
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses"; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

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


        public ResponseApi modulos_paginados_con_items(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;

                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/modules?include[]=items&include[]=content_details&page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<ModuloCanvas>>(respuesta);

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



        public ResponseApi get_secciones(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/sections?page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<SeccionCanvas>>(respuesta);

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

        public ResponseApi examenes_paginados(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/quizzes?page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<ExamenCanvas>>(respuesta);

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

        public ResponseApi tareas_paginados(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/assignments?&include[]=overrides&page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<AssignmentCanvas>>(respuesta);

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



        public ResponseApi foros_paginados(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;

                //https://upao.instructure.com/api/v1/courses/3468/discussion_topics?&include[]=overrides&page=1&per_page=100


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/discussion_topics?&include[]=overrides&page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<ForoCanvas>>(respuesta);

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


        public ResponseApi files_paginados(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;

                // https://upao.instructure.com/api/v1/courses/sis_course_id:2021102138/files?&include[]=user&exclude_content_types[]=image/jpeg&exclude_content_types[]=image/png&page=1&per_page=100

                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/files?&include[]=user&page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<FileCanvas>>(respuesta);

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


        public ResponseApi files_paginados_noimagenes(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;

                // https://upao.instructure.com/api/v1/courses/sis_course_id:2021102138/files?&include[]=user&exclude_content_types[]=image/jpeg&exclude_content_types[]=image/png&page=1&per_page=100

                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/files?&include[]=user&exclude_content_types[]=image/jpeg&exclude_content_types[]=image/png&page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<FileCanvas>>(respuesta);

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


        public ResponseApi docentes_roles_paginados(String sis, String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;

                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:"+sis+"/enrollments?page=" + numeropaginas + "&per_page=" + numeroxpagina+ "&role[]=TeacherEnrollment"; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<EnrollmentCanvas>>(respuesta);

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


        public ResponseApi items_paginados(String sis, String modulo_id,String numeropaginas = "1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


      
                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + sis + "/modules/"+modulo_id+"/items?page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<ItemCanvas>>(respuesta);

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

        public ResponseApi modulos_paginados(String sis,String numeropaginas="1", String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;
               
                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:"+sis+"/modules?page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<List<ModuloCanvas>>(respuesta);

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

        public ResponseApi get_course_by_sis(String p_code_course)
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/courses/sis_course_id:" + p_code_course; //"https://upao.instructure.com/api/v1/courses"; 

                HttpWebRequest tRequest;
                tRequest = WebRequest.Create(url_api) as HttpWebRequest;
                tRequest.Method = "GET";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                response2 = tRequest.GetResponse() as HttpWebResponse;
                reader2 = new StreamReader(response2.GetResponseStream());
                var buffer = reader2.ReadToEnd();
                respuesta = buffer.ToString();
                reader2.Close();

                var splashInfo = JsonConvert.DeserializeObject<CursoCanvas>(respuesta);

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




        public ResponseApi crear_curso(BodyFormCurso curso)
        {
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            //return new ResponseApi {success = 200, message = curso.name + "-" + curso.account_id };

            try
            {
                string URL_API_CANVAS = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/" + curso.account_id + "/courses";
                // string URL_API_CANVAS = "https://upao.beta.instructure.com/api/v1/accounts/" + curso.account_id + "/courses";
                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

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
                    //return new ResponseApi { success = (int)ResponseCode.R200, message = "OK"};
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


        public ResponseApi crear_curso2(BodyFormCurso curso)
        {
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            //return new ResponseApi {success = 200, message = curso.name + "-" + curso.account_id };

            try
            {
                string URL_API_CANVAS = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/" + curso.account_id + "/courses";
                // string URL_API_CANVAS = "https://upao.beta.instructure.com/api/v1/accounts/" + curso.account_id + "/courses";
                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

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
                    //CursoCanvas jsonCurso = JsonConvert.DeserializeObject<CursoCanvas>(respuesta);
                    return new ResponseApi { success = (int)ResponseCode.R200, message = "OK" };
                    // return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonCurso };
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


        public static async Task<string> UploadCSVFile(String token, String file, String urlcanvas, String accountid)
        {


            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), urlcanvas + "/api/v1/accounts/" + accountid + "/sis_imports.json?import_type=instructure_csv"))
                {
                    //request.Headers.TryAddWithoutValidation("Authorization", "Bearer <token>");
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + token);

                    var multipartContent = new MultipartFormDataContent();
                    // multipartContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes("<filename>")), "attachment", Path.GetFileName("<filename>"));
                    multipartContent.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(file)), "attachment", Path.GetFileName(file));
                    request.Content = multipartContent;

                    using (var result = await httpClient.SendAsync(request))
                    {
                        var response = result;
                        string content = await result.Content.ReadAsStringAsync();
                        return content;
                    }

                    // var response = await httpClient.SendAsync(request);
                    //return response;
                }
            }
        }
    }
}
