<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InsertarActualizarClientes.aspx.cs" Inherits="CapaPresentacion.InsertarActualizarClientes" %>

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
        <h1>Demo Programacion en Capas</h1>
    
      <label>Codigo: </label>  <asp:TextBox ID="txtCodigo" runat="server"></asp:TextBox><br />
       <label>Nombre: </label>  <asp:TextBox ID="txtNombres" runat="server"></asp:TextBox><br />
       <label>Apellidos: </label>  <asp:TextBox ID="txtApellidos" runat="server"></asp:TextBox><br />
       <label>Correo:</label>  <asp:TextBox ID="txtCorreo" runat="server"></asp:TextBox><br />
        <p>
            <asp:Button ID="btnGrabar" runat="server" Text="Grabar" OnClick="btnGrabar_Click" /> 
            <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" OnClick="btnActualizar_Click" />
            <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
            <asp:Button ID="btnSalir" runat="server" Text="Salir" OnClick="btnSalir_Click" />
        </p>
        <p>
            <asp:Label ID="lblMensaje" runat="server" Text="lblMensaje"></asp:Label>
        </p>
    </form>
</body>
</html>
