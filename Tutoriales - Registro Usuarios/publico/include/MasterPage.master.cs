using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class publico_include_MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Menu();
        }
    }
    protected void Menu()
    {
        string html = string.Format(@"
            <li><a href='#'>Opcion 0</a>
                <ul>
                    <li class='dcjq-current-parent'><a href='#'>Sub Opcion 1</a></li>
                    <li class='dcjq-current-parent'><a href='#'>Sub Opcion 2</a></li>
                    <li class='dcjq-current-parent'><a href='#'>Sub Opcion 4</a></li>
                </ul>
            </li>
            <li><a href='#'>Opcion 1</a></li>
            <li><a href='#'>Opcion 2</a></li>
        ");
        this.ltMenu.Text = html;
    }
    protected void btnSalir_Click(object sender, EventArgs e)
    {
        Session["UsuarioID"] = null;
        Response.Redirect("~/ingresar.aspx");
    }
}
