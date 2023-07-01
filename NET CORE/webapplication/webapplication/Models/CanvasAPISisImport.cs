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
    public class CanvasAPISisImport
    {

        public CanvasAPISisImport(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

       
        public ResponseApi get_sisimport(String id)
        {
            string respuesta;
            try
            {
                HttpWebResponse response2;
                StreamReader reader2;


                string url_api = Configuration.GetSection("MySettings").GetSection("urlCANVASUPAO").Value + "/api/v1/accounts/1/sis_imports/" + id; //"https://upao.instructure.com/api/v1/courses"; 

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

                var splashInfo = JsonConvert.DeserializeObject<SisImportCanvas>(respuesta);

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



        
      
    }
}
