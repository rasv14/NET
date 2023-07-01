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
    public class SecurityController : ControllerBase
    {
        private IConfiguration configuration;

        public SecurityController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }



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


        [HttpPost, Route("permisovista")]
        public ResponseApi permisovista([FromQuery] HeadersParameters parameters, [FromBody] dynamic data)
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

               // string p_datosString = jsondata.p_datos;



                String p_usuario = jsondata.p_usuario.ToString();
                String p_forma = jsondata.p_forma.ToString();
               

               




                //Se Valida el JWT para que solo se acept2en las solicitudes de un usuario que se ha logueado
                Token objToken = new Token(configuration);

                if (!objToken.ValidateJwtToken(p_jwt))
                {


                    return new ResponseApi { success = (int)ResponseCode.R500, message = "Solicitud no autorizada." };
                    ///Comentado: p_data_u_log = objCSVDAO.GetDataInsetCSVLog2(data, jsondata.accion, lstInsertLog[2], Result, msjResult);
                    ///Comentado: lstUpdateLog = objCSVDAO.SetCSVLog("UPD_CSVLOG", p_data_u_log, "", JsonResult);


                }



                ResponseDB responseDB = new SecurityDAO(configuration).verificaAccesoForma(p_usuario, p_forma);


                //   lstUpdateProcesoEvalDoc = objEvalDocDAO.ActivarDesactivarProcesoEvalDoc(p_code,p_estado,p_usuario);


                //  return new ResponseApi { success = Convert.ToInt32(lstUpdateProcesoEvalDoc[0]), message = lstUpdateProcesoEvalDoc[1], data = "[]" };

                return new ResponseApi { success = responseDB.success, message = responseDB.message };


            }
            catch (Exception ex)
            {
                return new ResponseApi { success = (int)ResponseCode.R500, message = "ERROR", error = ex.Message };
            }
        }
















    }
}