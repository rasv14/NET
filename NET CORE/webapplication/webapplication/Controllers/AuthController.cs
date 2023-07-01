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
using Microsoft.AspNetCore.Cors;
using webapplication.Helpers;

namespace webapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration;

        public AuthController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }


       // [EnableCors("EnableCORS")]
        [HttpPost, Route("login")]
        public ResponseApi Login([FromBody]LoginModel user)//IActionResult
        {
            if (user == null)
            {
                //return BadRequest("Invalid client request");
                return new ResponseApi { success = (int)ResponseCode.R900, message = "No ingreso datos." };
            }

            String result1 = "0";
            String result ="0";
            String msg = "ERROR";
            String msg_result = "ERROR";

            String user_id="0";
            String user_nombre="ERROR";
                    
            //string dbConn = configuration.GetSection("MySettings").GetSection("DbConnection").Value;

            

            string PasswordKeyJWT = configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value;


            Conexion objConexion = new Conexion(configuration);

        //    using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
        //    {
        //        OracleCommand objCmd = new OracleCommand();

        //        objCmd.Connection = objConn;
        //        //objCmd.CommandType = CommandType.StoredProcedure;

        //        //objCmd.CommandText = "BANINST1.FG_UPAO_VALIDATE_USER";

        //        //objCmd.BindByName = true;

        //        //objCmd.Parameters.Add("Return_Value", OracleDbType.Varchar2,
        //        //    ParameterDirection.ReturnValue);
        //        //objCmd.Parameters.Add("USERNAME", OracleDbType.Varchar2, 1000,
        //        //    user.UserName,
        //        //    ParameterDirection.Input);
        //        //objCmd.Parameters.Add("PASSWORD", OracleDbType.Varchar2, 1000,
        //        //    user.Password,
        //        //    ParameterDirection.Input);



        //        objCmd.CommandText =
        //@"begin
        //            :prm_Result := BANINST1.FG_UPAO_VALIDATE_USER(:prm_Argument,:prm_Argument2);
        //          end;";

        //        objCmd.Parameters.Add(":prm_Result", OracleDbType.Varchar2, ParameterDirection.Output);
        //        objCmd.Parameters.Add(":prm_Argument", OracleDbType.Varchar2).Value = user.UserName;
        //        objCmd.Parameters.Add(":prm_Argument2", OracleDbType.Varchar2).Value = user.Password;


        //        //objCmd.Connection = objConn;
        //        //objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.FG_UPAO_VALIDATE_USER";
        //        //objCmd.CommandType = CommandType.StoredProcedure;
        //        //objCmd.Parameters.Add("USERNAME", OracleDbType.Varchar2).Value = user.UserName;
        //        //objCmd.Parameters.Add("PASSWORD", OracleDbType.Varchar2).Value = user.Password;

        //        //objCmd.Parameters.Add("V_PIDM_ACTIVE", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;

        //        try
        //        {

        //            //objConn.Open();
        //            //using (var dr = objCmd.ExecuteReader())
        //            //{
        //            //    // do some work here
        //            //}

        //            objConn.Open();
        //            objCmd.ExecuteNonQuery();


        //            result1 = objCmd.Parameters[0].Value.ToString();

        //            //objConn.Open();
        //            //objCmd.ExecuteNonQuery();

        //            //result1 = objCmd.Parameters["V_PIDM_ACTIVE"].Value.ToString();

        //        }
        //        catch (Exception ex)
        //        {
        //            System.Console.WriteLine("Exception: {0}", ex.ToString());
        //            msg = ex.ToString();

        //        }
        //        objConn.Close();
        //    }





            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("campus")))
            {
                OracleCommand objCmd = new OracleCommand();
                
                objCmd.Connection = objConn;
               // objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_LOGIN_USER";
                objCmd.CommandText = "COMUMGR.PYG_SEGUR_USUARIO.PC_LOGIN_USER_CANVAS";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_USER", OracleDbType.Varchar2).Value = user.UserName;
                objCmd.Parameters.Add("P_PASSWORD", OracleDbType.Varchar2).Value = user.Password;

                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2,1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2,1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_ID", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                objCmd.Parameters.Add("V_NOMBRE", OracleDbType.Varchar2, 1000).Direction = ParameterDirection.Output;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                    msg = objCmd.Parameters["V_MSG"].Value.ToString();
                    msg_result = objCmd.Parameters["V_MSG"].Value.ToString();
                    result = objCmd.Parameters["V_RESULT"].Value.ToString();
                    user_id = objCmd.Parameters["V_ID"].Value.ToString();
                    user_nombre = objCmd.Parameters["V_NOMBRE"].Value.ToString();

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                    msg = "Error de Base de Datos";
                    msg_result = ex.ToString();

                }
                objConn.Close();
            }



            using (OracleConnection objConn = new OracleConnection(objConexion.GetConexion("banner")))
            {
                OracleCommand objCmd = new OracleCommand();

                objCmd.Connection = objConn;
                // objCmd.CommandText = "CANVMGR.PKC_INTEGRACION_CANVAS.PC_LOGIN_USER";
                objCmd.CommandText = " CANVMGR.PKC_INTEGRACION_CANVAS.PC_INSERT_LOG_LOGIN2";
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add("P_USER", OracleDbType.Varchar2).Value = user.UserName;
                objCmd.Parameters.Add("V_MSG", OracleDbType.Varchar2).Value = msg_result;
                objCmd.Parameters.Add("V_RESULT", OracleDbType.Varchar2).Value = result;
                objCmd.Parameters.Add("P_IP", OracleDbType.Varchar2).Value = user.Ip;
                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();

                   // msg = objCmd.Parameters["V_MSG"].Value.ToString();
                  //  result = objCmd.Parameters["V_RESULT"].Value.ToString();
                   

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                   

                }
                objConn.Close();
            }

            if (result == "200" )// (user.UserName == "johndoe" && user.Password == "def@123")
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PasswordKeyJWT));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim> 
                { 
                    new Claim(ClaimTypes.Name, user.UserName), 
                    new Claim(ClaimTypes.Role, "Operator"),
                    new Claim("canvas_userid","97"),
                    new Claim("user_id",user_id),
                    new Claim("user_nombre",user_nombre),
                };

                var tokeOptions = new JwtSecurityToken(
                    issuer: configuration.GetSection("MySettings").GetSection("APIUrlServerJWT").Value,// "http://localhost:5000",
                    audience: configuration.GetSection("MySettings").GetSection("APIUrlServerJWT").Value,//"http://localhost:5000",
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                //return Ok(new { Token = tokenString });
                return new ResponseApi { success = (int)ResponseCode.R200, message = msg,data = tokenString };
            }
            else
            {
               return new ResponseApi { success = (int)ResponseCode.R900, message = msg };
                //return Unauthorized();

            }
        }
    }
}