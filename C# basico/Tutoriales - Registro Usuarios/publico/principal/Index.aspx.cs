using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class publico_principal_Index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string userid = (string)Session["UsuarioID"];
        if (!IsPostBack)
        {
            if (userid == "" || userid == null)
                Response.Redirect("~/Ingresar.aspx");
        }
    }
}