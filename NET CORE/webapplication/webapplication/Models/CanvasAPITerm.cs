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
    public class CanvasAPITerm
    {

        public CanvasAPITerm(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }


        public ResponseApi crear_periodo(BodyFormPeriodo periodo , String cuenta)
        {
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            //return new ResponseApi {success = 200, message = curso.name + "-" + curso.account_id };

            try
            {
                string URL_API_CANVAS = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/" + cuenta + "/terms";
                // string URL_API_CANVAS = "https://upao.beta.instructure.com/api/v1/accounts/" + curso.account_id + "/courses";
                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("enrollment_term[name]={0}&enrollment_term[start_at]={1}&enrollment_term[end_at]={2}&enrollment_term[sis_term_id]={3}", periodo.name, periodo.start_at, periodo.end_at, periodo.sis_term_id);

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
                    PeriodoCanvas jsonCurso = JsonConvert.DeserializeObject<PeriodoCanvas>(respuesta);
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

        public ResponseApi periodos_canvas_paginados(String numeropaginas, String numeroxpagina = "100")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/1/terms?page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

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

                var splashInfo = JsonConvert.DeserializeObject<EnrollmentTermsCanvas>(respuesta);

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

        public ResponseApi usuarios_canvas_paginados(String numeropaginas, String numeroxpagina = "100", String id_cuenta_Canvas="1")
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/"+ id_cuenta_Canvas + "/users?state[]=all&page=" + numeropaginas + "&per_page=" + numeroxpagina; //"https://upao.instructure.com/api/v1/courses"; 

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

                var splashInfo = JsonConvert.DeserializeObject<List<UsuarioCanvas>>(respuesta);

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


        public ResponseApi periodo_by_sis_term_id(String periodo, String id_usuario= "1" )
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/" + id_usuario + "/terms/sis_term_id:" + periodo; 

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

                var splashInfo = JsonConvert.DeserializeObject<PeriodoCanvas>(respuesta);

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




        public ResponseApi get_user_by_id(String p_code_user)
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/users/sis_user_id:" + p_code_user; //"https://upao.instructure.com/api/v1/courses"; 

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

                var splashInfo = JsonConvert.DeserializeObject<UsuarioCanvas>(respuesta);

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




        public ResponseApi crear_usuario(BodyFormUsuario usuario, string id_cuenta_canvas="1")
        {
            string respuesta;
            HttpWebResponse response;
            StreamReader reader;
            StreamWriter writer;
            HttpWebRequest tRequest;

            //return new ResponseApi {success = 200, message = curso.name + "-" + curso.account_id };

            try
            {
                string URL_API_CANVAS = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/"+ id_cuenta_canvas + "/users";
                // string URL_API_CANVAS = "https://upao.beta.instructure.com/api/v1/accounts/" + curso.account_id + "/courses";
                tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
                tRequest.Method = "POST";
                tRequest.ContentType = "application/x-www-form-urlencoded";
                tRequest.Headers.Add("Authorization", "Bearer " + Configuration.GetSection("MySettings").GetSection("tokenCANVASUPAO").Value);

                string postString = string.Format("user[name]={0}&user[short_name]={1}&user[sortable_name]={2}&pseudonym[unique_id]={3}&pseudonym[sis_user_id]={4}&pseudonym[send_confirmation]={5}&communication_channel[address]={6}&user[terms_of_use]={7}&user[skip_registration]={8}&pseudonym[authentication_provider_id]={9}&pseudonym[integration_id]={10}", usuario.name, usuario.short_name, usuario.sortable_name, usuario.unique_id, usuario.sis_user_id, usuario.send_confirmation, usuario.address,usuario.terms_of_use,usuario.skip_registration, usuario.authentication_provider_id,usuario.integration_id);

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
                    UsuarioCanvas jsonCurso = JsonConvert.DeserializeObject<UsuarioCanvas>(respuesta);
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
