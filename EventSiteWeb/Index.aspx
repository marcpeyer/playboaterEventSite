<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="kcm.ch.EventSite.Web.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Playboater's EventSite - Mandanten-Übersicht</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <h1>Bitte einen Mandanten wählen</h1>
    </div>
    	<asp:Repeater ID="MandatorList" runat="server">
				<ItemTemplate><div><%# Eval("MandatorName") %></div></ItemTemplate>
			</asp:Repeater>
    </form>
</body>
</html>
