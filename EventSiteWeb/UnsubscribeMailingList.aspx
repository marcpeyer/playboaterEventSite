<%@ Page language="c#" Codebehind="UnsubscribeMailingList.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.UnsubscribeMailingList" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Aus Mailingliste austragen</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<h1 id="pageTitle">Aus Mailingliste austragen</h1>
			<asp:Label id="EmailLabel" runat="server">Email-Adresse:</asp:Label>
			<asp:TextBox id="EmailTextbox" runat="server"></asp:TextBox><br>
			<asp:Button id="UnsubscribeButton" runat="server" Text="Austragen" style="margin-top: 10px; margin-bottom: 10px;"></asp:Button>
			<asp:Panel id="StatusPanel" runat="server">Panel</asp:Panel>
		</form>
	</body>
</HTML>
