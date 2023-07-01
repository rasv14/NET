using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Ingresar : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnIniciar_Click(object sender, EventArgs e)
    {
        //se declara la variable usuario de tipo string y se le indica que reemplaze los carácteres que sean:
        // ; y -- para evitar sql inyection lo mismo para contraseña.
        string usuario = this.txtUsuario.Text.Replace(";","").Replace("--","");
        string contraseña = this.txtContraseña.Text.Replace(";","").Replace("--","");
        //Se manda llamar al método Autenticar que está parametrizado y se le pasan los valores correspondientes.
        if (LoginService.Autenticar(usuario, contraseña)==true)
        {
            //Se verifica en la base de datos el UsuarioID y se almacena en la variable tblUsuario.
            DataTable tblUsuario = LoginService.prConsultaUsuario(usuario,contraseña);
            //Como seguridad se almacena en la base de datos el usuarioID, el usuario,  la fecha de ingreso y el ip
            //de todas las veces que el usuario ha ingresado de manera correcta.
            LoginService.Security(Convert.ToInt32(tblUsuario.Rows[0]["UsuarioID"]), usuario, DateTime.Now, Request.ServerVariables["REMOTE_ADDR"]);
            //se declara y se le da el valor a la variable de sesión UsuarioID
            Session["UsuarioID"] = tblUsuario.Rows[0]["UsuarioID"].ToString();
            //Manda a la principal en caso de ser correcto el login
            Response.Redirect("~/publico/principal/index.aspx");
        }
        else
        {
            //Mensaje de error en caso de no ser usuario registrado
            lblMensaje.Text = "Usuario/Contraseña incorrecta verifique por favor.";
        }
    }
}