<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
<link href="style.css" rel="stylesheet"/>
    <title>Login en ASP</title>

<script>
    $(document).ready(function () {
        $("#Button1").hover(
            function () {
                $(this).animate({ opacity: "0.3" },"slow")

            }
        , function () {
            $(this).animate({ opacity: "1.0" },"slow")
        }
        )

    });

</script>

</head>

<body>
    <header><img src="imagenes/logo_wow.png" alt="WoW"/></header>
    <div id="formulario">
        <h1>Ingreso de Usuarios</h1>
    <form id="form1" runat="server">
        <fieldset>
           <legend>Datos</legend>
    <p><label for="Nombre">Nombre: </label>
        <asp:TextBox ID="Nombre" runat="server"></asp:TextBox></p>
    <p><label for="Password">Contraseña: </label>
        <asp:TextBox ID="Password" runat="server"></asp:TextBox></p>
     <p>
         <asp:Button ID="Button1" runat="server" Text="Enviar" OnClick="Button1_Click" /></p>
        <p>
            <asp:Button ID="Button2" runat="server" Text="Enviar_sp" OnClick="Button2_Click" />
        </p>
            </fieldset>
    </form>

        </div>
    <footer><img src="imagenes/Paladin_crest.png" alt="Paladindes"/></footer>
</body>
</html>
