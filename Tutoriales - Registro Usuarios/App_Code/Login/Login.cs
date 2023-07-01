using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public static class LoginService
{
    public static bool Autenticar(string usuario, string password)
    {
        //Declaramos la sentencia SQL
        string sql = @"SELECT COUNT(*)
                       FROM Usuario
                       WHERE Username = @usuario AND Contraseña = @password";

        //utilizamos using para indicarle al compilador que dentro de este bloque se llame al Método Dispose.
        //para así liberar recursos cuanto antes mejor. en este caso no ocupamos decirle que cierre la conexión a la base de datos.
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadenaConexion"].ToString()))
        {
            conn.Open();

            SqlCommand command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@usuario", usuario);
            string hash = Helper.EncodePassword(string.Concat(usuario, password));
            command.Parameters.AddWithValue("@password", hash);

            int count = Convert.ToInt32(command.ExecuteScalar());
            
            if (count == 0)
                return false;
            else
                return true;

        }
    }
    public static void Security(int userid, string usuario, DateTime ultimoacc, string ip)
    {
        string sql = @"INSERT INTO UsuarioSecurity(
                            UsuarioID
                           ,Username
                           ,UltimoAcceso
                           ,IPAcceso)
                        VALUES(
                            @UsuarioID,
                            @Username,
                            @UltimoAcceso,
                            @IPAcceso)
                            SELECT SCOPE_IDENTITY()";
        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadenaConexion"].ToString()))
        {
            SqlCommand command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@UsuarioID", userid);
            command.Parameters.AddWithValue("@Username", usuario);
            command.Parameters.AddWithValue("@UltimoAcceso", ultimoacc);
            command.Parameters.AddWithValue("@IPAcceso", ip);

            conn.Open();

            int resultado = Convert.ToInt32(command.ExecuteScalar());
        }
    }
    public static DataTable prConsultaUsuario(string usuario, string password)
    {
        string sql = @"SELECT UsuarioID
                            FROM Usuario
                            WHERE Username = @Username AND Contraseña = @password";


        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadenaConexion"].ToString()))
        {
            SqlCommand command = new SqlCommand(sql, conn);

            command.Parameters.AddWithValue("@Username", usuario);
            string hash = Helper.EncodePassword(string.Concat(usuario, password));
            command.Parameters.AddWithValue("@password", hash);

            conn.Open();
            SqlDataAdapter daAdaptador = new SqlDataAdapter(command);
            DataTable dtDatos = new DataTable();
            daAdaptador.Fill(dtDatos);
            return dtDatos;
        }
    }
}

