<%@ Page language="c#" Codebehind="RemoteLogin.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.RemoteLogin" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>EventSite - Login</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
	</HEAD>
	<body>
		<form id="Form1" name="Form1" method="post" runat="server">
			<h1 id="pageTitle" runat="server">EventSite - Login</h1>
			<table align="center" style="TABLE-LAYOUT: fixed; WIDTH: 400px">
				<tr>
					<td colspan="2" style="WHITE-SPACE: normal">
						Falls&nbsp;du&nbsp;in der EventSite unter diesem Mandanten (<asp:Label id="MandatorLabel" runat="server"></asp:Label>) 
						noch keinen Account hast, kannst du dich
						<asp:HyperLink id="RegisterLink" runat="server">hier</asp:HyperLink>&nbsp;neu 
						registrieren. Falls du die Login-Daten vergessen hast: <a href="http://www.kcm.ch/admin/sendpass.asp">
							hier klicken</a></td>
				</tr>
			</table>
		</form>
		<asp:Label id="Label1" runat="server">Label</asp:Label><br>
		<asp:Label id="Label2" runat="server">Label</asp:Label>
	</body>
</HTML>
