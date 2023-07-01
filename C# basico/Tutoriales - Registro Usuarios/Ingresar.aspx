<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Ingresar.aspx.cs" Inherits="Ingresar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Login.css" rel="stylesheet" type="text/css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="FormaLogin">
        <div class="LeftFormaLogin">
        </div>
        <div class="CenterFormaLogin">
            <div class="cPosRel" style="width: 370px; height: 90px; margin: 34px auto 0px auto; text-align:justify;">
                <span class="TextoBienvenido">Al ingresar al Sistema ústed está de acuerdo en aceptar
                    nuestros Términos y condiciones legales, cualquier cambio que realize en esta página
                    será monitoreado.</span>
                <br />
                <span>
                    <%= Request.ServerVariables["REMOTE_ADDR"]%></span><br />
                <span>
                    <asp:Literal ID="ltMac" runat="server"></asp:Literal></span>
            </div>
            <div class="cFL cPosRel" style="width: 450px; height: 150px;">
                <div class="cFL cPosRel" style="width: 100px; height: 25px;">
                    <span class="cFR cPosRel TextoLogin" style="margin-top: 7px;">Usuario:</span>
                </div>
                <div class="cFL cPosRel" style="width: 290px; height: 25px;">
                    <asp:TextBox ID="txtUsuario" CssClass="txtControl" runat="server"></asp:TextBox>
                </div>
                <div class="cFL cPosRel" style="width: 100px; height: 25px; margin-top: 15px;">
                    <span class="cFR cPosRel TextoLogin" style="margin-top: 7px;">Password:</span>
                </div>
                <div class="cFL cPosRel" style="width: 290px; height: 25px; margin-top: 15px;">
                    <asp:TextBox ID="txtContraseña" TextMode="Password" CssClass="txtControl" runat="server"></asp:TextBox>
                </div>
                <div class="cFL cPosRel" style="width: 460px;">
                    <div style="width: 120px; height: 30px; margin-left: auto; margin-right: auto; margin-top: 15px;">
                        <asp:ImageButton ID="btnIniciar" ImageUrl="~/publico/include/imagenes/login/ingresarsistema.jpg" runat="server" OnClick="btnIniciar_Click" />
                    </div>
                </div>
            </div>
            <asp:Label ID="lblMensaje" CssClass="cFL" runat="server" ForeColor="#996600"></asp:Label>
        </div>
        <div class="RightFormaLogin">
        </div>
    </div>
    </form>
</body>
</html>
