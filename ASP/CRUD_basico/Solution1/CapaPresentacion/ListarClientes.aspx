<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListarClientes.aspx.cs" Inherits="CapaPresentacion.ListarClientes" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style>
        body
        {
            background:aqua;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
      <p><label>Ingrese Apellidos</label><asp:TextBox ID="txtApellidosCliente" runat="server"></asp:TextBox>
          <asp:Button ID="Buscar" runat="server" Text="Buscar" OnClick="Buscar_Click" />
          <asp:Button ID="NuevoCliente" runat="server" Text="Nuevo" OnClick="NuevoCliente_Click" />
      </p>  
        <p>
            <asp:GridView ID="GridViewDatos" runat="server" AllowPaging="True" OnPageIndexChanging="GridViewDatos_PageIndexChanging" OnRowCommand="GridViewDatos_RowCommand" OnRowDeleting="GridViewDatos_RowDeleting" Width="126px">
                <Columns>
               
                    <asp:CommandField ShowDeleteButton="True" />
                   
                    <asp:buttonfield buttontype="Link" commandname="Actualizar" text="Actualizar"/>
                     
               
                </Columns>
            </asp:GridView>  
        </p>
    </form>
</body>
</html>
