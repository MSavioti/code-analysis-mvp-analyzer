<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ContatoPage.aspx.cs" Inherits="ExemploMVP_WebForm.ContatoPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label runat="server" Text="Nome"></asp:Label>
            <asp:TextBox ID="CaixaTexto" runat="server"></asp:TextBox>
        </div>
        <asp:Button ID="BuscarNomeBotao" runat="server" Text="Mostrar Nome" OnClick="BuscarNome"/>
        <asp:TextBox ID="NomeBuscado" runat="server"></asp:TextBox>
    </form>
</body>
</html>
