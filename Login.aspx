<%@ Page language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.Login" %>
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
					<td>Benutzername:</td>
					<td>
						<asp:TextBox id="LoginBox" runat="server" Width="160px"></asp:TextBox></td>
				</tr>
				<tr>
					<td>Passwort:</td>
					<td>
						<asp:TextBox id="PasswordBox" runat="server" TextMode="Password" Width="160px"></asp:TextBox></td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:Button id="LoginButton" runat="server" Text="Anmelden"></asp:Button></td>
				</tr>
				<tr>
					<td colspan="2" style="WHITE-SPACE: normal">Dieselben Login-Daten wie im KCM 
						Administrations Web verwenden! Falls&nbsp;du&nbsp;in der EventSite unter diesem 
						Mandanten (<asp:Label id="MandatorLabel" runat="server"></asp:Label>) noch 
						keinen Account hast, kannst du dich
						<asp:HyperLink id="RegisterLink" runat="server">hier</asp:HyperLink>&nbsp;neu 
						registrieren. Falls du die Login-Daten vergessen hast: <a href="/admin/sendpass.asp">
							hier klicken</a></td>
				</tr>
			</table>
		</form>
		<script language="javascript" type="text/javascript">
			try
			{
				document.Form1.LoginBox.focus();
			}
			catch(e)
			{
				//alert(e);
			}
		</script>
	</body>
</HTML>
