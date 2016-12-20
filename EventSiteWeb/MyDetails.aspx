<%@ Page Title="Meine Details" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="MyDetails.aspx.cs" Inherits="kcm.ch.EventSite.Web.MyDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<!--		<title runat="server" id="title">Meine Details</title>
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">-->
		<script language="javascript" src="pages/BrowserCheck.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
			function toggleChangePasswordTable()
			{
				var elem = document.getElementById('MainContent_ChangePasswordTable');
				if (elem.style.display == 'none')
				{
					elem.style.display = 'block';
				}
				else
				{
					elem.style.display = 'none';
				}
			}
		</script>
			<h1 id="pageTitle" runat="server">Event-Site des SoUndSo - Meine Details</h1>
			<table style="TABLE-LAYOUT: fixed;" width="100%">
				<col width="300">
				<col>
				<tr>
					<td style="FONT-WEIGHT: bold" colSpan="2">Informationen zum angemeldeten Benutzer</td>
				</tr>
				<tr>
					<td>Name:</td>
					<td><%=userName%></td>
				</tr>
				<tr>
					<td>Email:</td>
					<td><%=userEmail%></td>
				</tr>
				<tr>
					<td>Mitglied in folgenden Berechtigungs-Rollen:</td>
					<td><%=userRoles%></td>
				</tr>
				<tr id="SmsCreditRow" runat="server">
					<td>SMS Kredit:</td>
					<td><%=userSmsCredit%></td>
				</tr>
			</table>
			<table style="TABLE-LAYOUT: fixed; MARGIN-TOP: 10px; border: 1px solid black; background-color: #eeeeee;" width="100%">
				<col width="300">
				<col>
				<tr>
					<td style="FONT-WEIGHT: bold" colSpan="2">Allgemeine Einstellungen</td>
				</tr>
				<tr>
					<td>Name:</td>
					<td><asp:textbox id="Name" runat="server" Width="260px"></asp:textbox></td>
				</tr>
				<tr>
					<td>Email:</td>
					<td><asp:textbox id="Email" runat="server" Width="260px"></asp:textbox></td>
				</tr>
				<tr>
					<td title="Bitte Handy Nummer im Format +41XXXXXXXXXX angeben">Handy Nummer:</td>
					<td title="Bitte Handy Nummer im Format +41XXXXXXXXXX angeben"><asp:textbox id="MobilePhone" runat="server" Width="260px"></asp:textbox></td>
				</tr>
			</table>
			<div style="margin-top:10px;" runat="server" id="ChangePasswordToggle"><a href="javascript:toggleChangePasswordTable();">Zugangsdaten ändern</a></div>
			<table style="display:none; TABLE-LAYOUT: fixed; MARGIN-TOP: 10px; border: 1px solid black; background-color: #eeeeee;" width="100%" runat="server" id="ChangePasswordTable">
				<tr style="height: 2px;"><td style="width: 300px;height: 2px;"></td><td></td></tr>
				<tr>
					<td style="FONT-WEIGHT: bold" colSpan="2">Zugangsdaten ändern</td>
				</tr>
				<tr>
					<td style="width:300px;">Login:</td>
					<td><asp:textbox id="newLogin" runat="server" TextMode="SingleLine" Width="260px"></asp:textbox></td>
				</tr>
				<tr>
					<td>Neues Passwort:</td>
					<td><asp:textbox id="newPassword" runat="server" TextMode="Password" Width="260px"></asp:textbox></td>
				</tr>
				<tr>
					<td>Neues Passwort wiederholen:</td>
					<td><asp:textbox id="newPasswordRepeat" runat="server" TextMode="Password" Width="260px"></asp:textbox></td>
				</tr>
				<tr>
					<td></td>
					<td><asp:Button ID="ChangePasswordButton" runat="server" Text="Speichern" /></td>
				</tr>
			</table>
			<table style="TABLE-LAYOUT: fixed; MARGIN-TOP: 10px; border: 1px solid black; background-color: #eeeeee;" width="100%" id="GlobalSmsSettings" runat="server">
				<tr style="height: 2px;"><td style="width: 300px;height: 2px;"></td><td></td></tr>
				<tr>
					<td style="FONT-WEIGHT: bold" colSpan="2">Globale SMS Einstellungen</td>
				</tr>
				<tr id="LiftMgmtSettings" runat="server">
					<td>SMS für Mitfahrten Management:</td>
					<td><asp:checkbox id="LiftMgmtSmsOn" runat="server" Text="Ein"></asp:checkbox></td>
				</tr>
				<tr id="EventMgmtSettings" runat="server">
					<td title="Wenn diese Einstellung auf 'Aus' ist, werden keine SMS Benachrichtigungen verschickt - unabhängig was für Einstellungen pro Kategorie konfiguriert sind.">SMS für Anlass Benachrichtigungen:</td>
					<td title="Wenn diese Einstellung auf 'Aus' ist, werden keine SMS Benachrichtigungen verschickt - unabhängig was für Einstellungen pro Kategorie konfiguriert sind."><asp:checkbox id="EventMgmtSmsOn" runat="server" Text="Ein"></asp:checkbox> <span class="labelDescription">(Benachrichtigung bei Anlässen / Eintragungen - diese Einstellung gewinnt vor den Einstellungen pro Kategorie)</span></td>
				</tr>
			</table>
			<asp:datalist id="EventCategoryList" runat="server" CssClass="MyDetailsCategSetBox">
				<HeaderStyle Font-Bold="True" CssClass="MyDetailsCategBoxHeader"></HeaderStyle>
				<HeaderTemplate>
					Einstellungen pro Kategorie
				</HeaderTemplate>
				<FooterStyle Wrap="False" HorizontalAlign="Right" Height="45px" VerticalAlign="Bottom" CssClass="MyDetailsCategBoxFooter"></FooterStyle>
				<FooterTemplate>
					<INPUT type="reset" value="Zurücksetzen" id="reset">
					<asp:Button id="SaveButton" runat="server" Text="Speichern" CommandName="Save"></asp:Button>
				</FooterTemplate>
				<ItemStyle CssClass="MyDetailsCategSetItemStyle"></ItemStyle>
				<ItemTemplate>
				</ItemTemplate>
			</asp:datalist>
			<br>
			<div>Erläuterungen zu den Einstellungen pro Kategorie:</div>
			<div style="FONT-SIZE: 12px" id="NotifyByMailDesc" runat="server">- E-Mail Benachrichtigung: Gibt an, ob E-Mail Benachrichtigungen verschickt werden sollen oder nicht.</div>
			<div style="FONT-SIZE: 12px" id="NotifyBySmsDesc" runat="server">- SMS Benachrichtigung: Gibt an, ob SMS Benachrichtigungen verschickt werden sollen oder nicht.</div>
			<div style="FONT-SIZE: 12px" id="NotifSubscriptionDesc" runat="server">- SMS Abonnierungsfunktion verwenden: Wenn eingeschaltet, werden die Benutzer nur beim Erstellen bzw. Ändern von Anlässen per SMS benachrichtigt. Um weitere Notifizierungen zu erhalten, muss der Benutzer den spezifischen Anlass abonnieren. Wenn ausgeschaltet, werden immer alle SMS Benachrichtigungen verschickt.</div>
			<div style="FONT-SIZE: 12px" id="AutoNotifSubscriptionDesc" runat="server">- Automatische Abonnierung: Wenn eine ganze positive Zahl angegeben ist, werden neue Anlässe in derjenigen Kategorie automatisch abonniert und es werden dadurch die gegebene Anzahl SMS verschickt. Wenn das Feld leer ist, ist die automatische Abonnierung ausgeschaltet.</div>

			<asp:Button id="LogoutButton" style="DISPLAY: none" runat="server" Text="logout"></asp:Button>
</asp:Content>