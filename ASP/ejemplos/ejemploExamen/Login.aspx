<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <link href="style.css" rel="stylesheet"/>
    <title>Login Xirects Solution</title>

    <script>
        $(document).ready(function() {
            $("#Button1").hover(function() {
                $(this).animate({ opacity: "0.3" }, "slow");
            },
            function() {
                $(this).animate({ opacity: "1.0" }, "slow");
            }
                )
        });

    </script>
</head>
<body>
    <header>
    <img src="imagenes/logo_wow.png" alt="Logo"/>
    </header>

    <div id="formulario">
    <form id="form1" runat="server">
        <h1>Ingreso de Datos</h1>
        <fieldset>
            <legend></legend>
    <p>
        <asp:Label ID="Label1" runat="server" Text="Label">Nombre: </asp:Label><br />
        <asp:TextBox ID="Name" runat="server"></asp:TextBox></p>
    <p>
        <asp:Label ID="Label2" runat="server" Text="Label">Password: </asp:Label><br />
        <asp:TextBox ID="Password" runat="server" Type="Password"></asp:TextBox></p>

            </fieldset>
    <p id="btn">
        <asp:Button ID="Button1" runat="server" Text="Enviar" OnClick="Button1_Click" /></p>
    </form>
        </div>

    <footer><img src="imagenes/Paladin_crest.png" alt="Footer"/></footer>
</body>
</html>
