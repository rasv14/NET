<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Registrar.aspx.cs" Inherits="Registrar" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>Bienvenido</h1>
        <asp:Label ID="LabelN" runat="server" Text="Label"></asp:Label><br />
        <asp:Button ID="Button1" runat="server" Text="Logout" OnClick="Button1_Click" />
    </div>
    </form>
</body>
</html>
