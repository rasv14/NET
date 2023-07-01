using System;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using webapplication.Helpers;
using webapplication.clases;
using Newtonsoft.Json;

namespace webapplication.Models
{
    public class CanvasAPICSV
    {

        public CanvasAPICSV()
        {

        }


        //public ResponseApi UploadCSVinCANVAS(String token, String file, String urlcanvas, String accountid)
        //{

        //    string respuesta;
        //    HttpWebResponse response;
        //    StreamReader reader;
        //    StreamWriter writer;
        //    HttpWebRequest tRequest;

        //    //return new ResponseApi {success = 200, message = curso.name + "-" + curso.account_id };

        //    try
        //    {
        //        string URL_API_CANVAS = urlcanvas + "/api/v1/accounts/" + accountid + "/sis_imports.json?import_type=instructure_csv";

        //        tRequest = WebRequest.Create(URL_API_CANVAS) as HttpWebRequest;
        //        tRequest.Method = "POST";
        //        tRequest.ContentType = "application/x-www-form-urlencoded";
        //        tRequest.Headers.Add("Authorization", "Bearer "+ token);

        //        string postString = string.Format("course[name]={0}&course[course_code]={1}&course[is_public_to_auth_users]={2}&course[public_syllabus]={3}&course[public_syllabus_to_auth]={4}&course[sis_course_id]={5}&course[default_view]={6}", curso.name, curso.course_code, curso.is_public_to_auth_users, curso.public_syllabus, curso.public_syllabus_to_auth, curso.sis_course_id, curso.default_view);

        //        tRequest.ContentLength = postString.Length;
        //        writer = new StreamWriter(tRequest.GetRequestStream());
        //        writer.Write(postString);
        //        writer.Close();

        //        try
        //        {
        //            response = tRequest.GetResponse() as HttpWebResponse;
        //        }
        //        catch (WebException we)
        //        {
        //            string error = we.Message;
        //            response = (HttpWebResponse)we.Response;
        //        }

        //        reader = new StreamReader(response.GetResponseStream());
        //        var buffer = reader.ReadToEnd();
        //        respuesta = buffer.ToString();
        //        reader.Close();

        //        if ((int)response.StatusCode == 200)
        //        {
        //            CursoCanvas jsonCurso = JsonConvert.DeserializeObject<CursoCanvas>(respuesta);
        //            //return new ResponseApi { success = (int)ResponseCode.R200, message = "OK"};
        //            return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonCurso };
        //        }

        //        if ((int)response.StatusCode == 400)//BAD REQUEST
        //        {
        //            return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
        //        }
        //        else
        //        {
        //            return new ResponseApi { success = (int)response.StatusCode, message = respuesta };
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseApi { success = (int)ResponseCode.R500, message = "Error", error = ex.Message };
        //    }

        //}



        public static async Task<ResponseApi> UploadCSVFileCanvas(String token, String file, String urlcanvas, String accountid)
        {

            try
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


                            if ((int)response.StatusCode == 200)
                            {
                                SisImportCanvas jsonCurso = JsonConvert.DeserializeObject<SisImportCanvas>(content);
                                return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonCurso };
                                // return new ResponseApi { success = (int)ResponseCode.R200, message = "OK", data = jsonCurso };
                            }

                            if ((int)response.StatusCode == 400)//BAD REQUEST
                            {
                                return new ResponseApi { success = (int)ResponseCode.R400, message = "Solicitud incorrecta." };
                            }
                            else
                            {
                                return new ResponseApi { success = (int)response.StatusCode, message = content };
                            }



                        }

                        // var response = await httpClient.SendAsync(request);
                        //return response;
                    }
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
                        var resultado = response.StatusCode;
                        var resultado2 = (int)response.StatusCode;


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
