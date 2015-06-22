<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="Xamarin.Forms.Player.Status" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Status</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%= typeof(Microsoft.AspNet.SignalR.Hub).AssemblyQualifiedName %>
    </div>
    </form>
</body>
</html>
