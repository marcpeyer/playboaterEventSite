<%@ Page language="c#" Codebehind="AccessDenied.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.AccessDenied" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>AccessDenied</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<h1 id="pageTitle" style="COLOR: red">Der Zugriff verweigert</h1>
			<h3>Der Zugriff auf die gew�nschte Seite wurde verweigert. Du hast nicht gen�gend 
				Rechte um diese Seite anzuzeigen. Bitte "Zur�ck"-Knopf des Browsers verwenden 
				um auf die vorangegangene Seite zur�ckzukehren oder
				<asp:LinkButton id="LogoutButton" runat="server" style="font-weight:bold;">ausloggen</asp:LinkButton>
				um dich mit einem anderen Benutzer anzumelden.
			</h3>
		</form>
	</body>
</HTML>
