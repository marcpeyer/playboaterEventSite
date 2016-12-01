<%@ Page language="c#" Codebehind="SendPass.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.SendPass" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>EventSite - Passwort vergessen</title>
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<h1 id="pageTitle" runat="server">EventSite - Passwort zusenden</h1>
			<div>Um sich ein vergessenes Passwort zusenden zu lassen bitte hier die Email Adresse
				des Kontos angeben.</div>
			<table align="center" style="TABLE-LAYOUT: fixed; WIDTH: 900px">
				<tr>
					<td vAlign="top">Email:</td>
					<td>
						<asp:TextBox id="Email" runat="server" Width="160px"></asp:TextBox><br>
						<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" ErrorMessage="Email darf nicht leer sein!"
							ControlToValidate="Email" EnableClientScript="False"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:Button id="SendButton" runat="server" Text="Passwort zusenden"></asp:Button></td>
				</tr>
				<tr>
					<td colspan="2" style="WHITE-SPACE: normal">
						<asp:HyperLink id="LoginLink" runat="server">Hier gehts zurück zum Login</asp:HyperLink></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
