using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using nmConexion;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        Conexion miConexion = new Conexion();
        SqlConnection con = new SqlConnection(miConexion.GetConexion());
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "spLogin4";
        cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name.Text;
        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = Password.Text;

        cmd.ExecuteNonQuery();
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {

            foreach (DataRow dr in dt.Rows)
            {

                Session["Name"] = dr["Name"].ToString();

                Response.Redirect("Menu.aspx");

            }

            con.Close();

        }

        else {

            con.Close();
            Response.Write("<script>alert('Usuario o Contraseña Incorrectos');</script>");
        
        }


    }
}