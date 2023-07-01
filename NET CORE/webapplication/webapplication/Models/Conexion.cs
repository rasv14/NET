using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace webapplication.Models
{
    public class Conexion
    {


        public Conexion(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public String GetConexion(String db) {
            String Conexion = "";

            switch(db)
            {
                case "campus":
                    Conexion = Configuration.GetSection("DBConnection").GetSection("DbCampus").Value;
                    break;
                case "banner":
                    Conexion = Configuration.GetSection("DBConnection").GetSection("DbBanner").Value;
                    break;
                case "PROD":
                    Conexion = Configuration.GetSection("DBConnection").GetSection("PROD").Value;
                    break;
                default:
                    Conexion = Configuration.GetSection("DBConnection").GetSection("DbCampus").Value;
                    break;
            }

            return Conexion;
        }


    }
  
}
