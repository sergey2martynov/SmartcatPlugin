<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BusketModal.aspx.cs" Inherits="SmartcatPlugin.sitecore_modules.shell.BusketModal" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Your Custom Dialog</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TreeView ID="TreeView1" runat="server" ShowCheckBoxes="All"></asp:TreeView>
            <br />            
        </div>'
        <div>
            <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" />
        </div>
    </form>
</body>
</html>
