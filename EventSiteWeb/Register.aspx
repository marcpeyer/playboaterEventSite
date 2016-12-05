<%@ Page language="c#" Codebehind="Register.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.Register" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>EventSite - Registrieren</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<h1 id="pageTitle" runat="server">EventSite - Registrieren</h1>
			<table align="center" style="TABLE-LAYOUT: fixed; WIDTH: 900px">
				<tr>
					<td vAlign="top">Name:</td>
					<td>
						<asp:TextBox id="Name" runat="server" Width="160px"></asp:TextBox><br>
						<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="Name darf nicht leer sein!"
							ControlToValidate="Name" EnableClientScript="False"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td vAlign="top">Email:</td>
					<td>
						<asp:TextBox id="Email" runat="server" Width="160px"></asp:TextBox><br>
						<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" ErrorMessage="Email darf nicht leer sein!"
							ControlToValidate="Email" EnableClientScript="False"></asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td vAlign="top">Handy:</td>
					<td>
						<asp:TextBox id="MobilePhone" runat="server" Width="160px"></asp:TextBox></td>
				</tr>
				<tr id="LiftMgmtRow" runat="server">
					<td>SMS Nachrichten beim<br>Mitfahrten-Management:</td>
					<td>
						<asp:CheckBox id="LiftMgmtSmsOn" runat="server" Text="Ein"></asp:CheckBox>
					</td>
				</tr>
				<tr id="EventMgmtRow" runat="server">
					<td>SMS Nachrichten beim<br>Event-Management (Benachrichtigungen für Anlässe / Anmeldungen):</td>
					<td>
						<asp:CheckBox id="EventMgmtSmsOn" runat="server" Text="Ein"></asp:CheckBox>
					</td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:Button id="RegisterButton" runat="server" Text="Registrieren"></asp:Button></td>
				</tr>
				<tr>
					<td colspan="2" style="WHITE-SPACE: normal">
						<span style="COLOR: red; font-weight: bold;">Wichtig! Bitte lesen:</span>&nbsp;Die Registration muss nach dem Absenden noch vom 
						Administrator vervollständigt werden. Du erhälst ein Email sobald dies erfolgt 
						ist. Danach kannst du dich hier auf der EventSite anmelden. <span id="SendPassContainer" runat="server">Falls du diese Login-Daten
						vergessen hast: <asp:HyperLink id="SendPassLink" runat="server">hier klicken</asp:HyperLink></span><BR>
						<asp:HyperLink id="LoginLink" runat="server">Hier gehts zurück zum Login</asp:HyperLink></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
