using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using nmConexion;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Conexion miConexion = new Conexion();
        SqlConnection cnx = new SqlConnection(miConexion.GetConexion());
        cnx.Open();
        SqlCommand cmd = new SqlCommand("Select * From tblUser where name='"+ Nombre.Text +"' and password='"+Password.Text+"' ");
        cmd.Connection = cnx;
        cmd.ExecuteNonQuery();

        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
       // int count = Convert.ToInt32(cmd.ExecuteScalar());
        if (dt.Rows.Count >0)
        {

            foreach (DataRow dr in dt.Rows) {


                Session["name"] = dr["name"].ToString();
                Response.Redirect("Registrar.aspx");
                cnx.Close();
            }

            
        }
        else {

            Response.Write("<script>alert('Nombre o Contraseña incorrectos')</script>");
            cnx.Close();
        }


    }
    protected void Button2_Click(object sender, EventArgs e)
    {

        Conexion Miconexion = new Conexion();
        SqlConnection con = new SqlConnection(Miconexion.GetConexion());
        con.Open();
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "spLogin3";

        cmd.Parameters.Add("@Name",SqlDbType.NVarChar).Value = Nombre.Text;
        cmd.Parameters.Add("@Password",SqlDbType.NVarChar).Value = Password.Text;

        cmd.ExecuteNonQuery();
        DataTable dt= new DataTable();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(dt);
       // int count = Convert.ToInt32(cmd.ExecuteScalar());
       // if (count > 0)
        if(dt.Rows.Count > 0)
        {

            foreach (DataRow dr in dt.Rows)
            {
                Session["Name"] = dr["Name"].ToString();
                Response.Redirect("Registrar.aspx");
                con.Close();

            }
        }
        else {

            Response.Write("<script>alert('Usuario o contraseña incorrecta')</script>");
            con.Close();
        }

    }
}