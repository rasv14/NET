using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using CapaNegocio;
using CapaEntidad; 

namespace CapaPresentacion
{
    public partial class ListarClientes : System.Web.UI.Page
    {
        ClientesNegocio ClientNego = new ClientesNegocio();
        ClientesEntidad ClientEnti = new ClientesEntidad();


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    ListarDatos();
                }
            }
            catch (Exception)
            {

            }
        }
        private void ListarDatos()
        {
            try
            {
                GridViewDatos.DataSource = ClientNego.ListarClientes(txtApellidosCliente.Text);
                GridViewDatos.DataBind();
            }
            catch (Exception)
            {

            }
        }       

     

        protected void NuevoCliente_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/InsertarActualizarClientes.aspx");
        }

        protected void Buscar_Click(object sender, EventArgs e)
        {
            ListarDatos();
        }

        protected void GridViewDatos_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridViewRow row = GridViewDatos.Rows[e.RowIndex];
                string strcod = Convert.ToString(row.Cells[2].Text);

                {
                    ClientEnti.codigoCliente = strcod;
                    ClientEnti.estadoCliente = 0;
                }
                if (ClientNego.EliminarCliente(ClientEnti) == true)
                {
                    ListarDatos();
                }
                else
                {
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void GridViewDatos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                short indicefila;
                indicefila = Convert.ToInt16(e.CommandArgument);
                string strcod;
                if (indicefila >= 0 & indicefila < GridViewDatos.Rows.Count)
                {
                    strcod = GridViewDatos.Rows[indicefila].Cells[2].Text;
                    if (e.CommandName == "Actualizar")
                    {
                        Session["CodigoCliente"] = strcod;
                        Response.Redirect("~/InsertarActualizarClientes.aspx");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        protected void GridViewDatos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridViewDatos.PageIndex = e.NewPageIndex;
                GridViewDatos.DataBind();
                ListarDatos();
            }
            catch (Exception)
            {
            }
        }
    }
}