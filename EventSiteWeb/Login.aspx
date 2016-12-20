<%@ Page Title="EventSite - Login" Language="C#" AutoEventWireup="false" MasterPageFile="~/SiteAnonym.Master" CodeBehind="Login.aspx.cs" Inherits="kcm.ch.EventSite.Web.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
		<!--<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">-->
			<table align="center" style="TABLE-LAYOUT: fixed; WIDTH: 400px">
				<tr>
					<td colspan="2">
						<h2><asp:Label id="MandatorTitle" runat="server"></asp:Label></h2>
						<div style="margin-bottom: 20px;"><a href="/">Mandant wechseln</a></div>
					</td>
				</tr>
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
							hier klicken</a>
					</td>
				</tr>
			</table>
			
		<script language="javascript" type="text/javascript">
			try
			{
				document.forms[0].MainContent_LoginBox.focus();
			}
			catch(e)
			{
				//alert(e);
			}
		</script>
</asp:Content>