<%@ Page language="c#" Codebehind="Locations.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.Locations" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Orte bearbeiten</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
		<base target="_self">
		<script language="javascript" type="text/javascript" src="pages/BrowserCheck.js"></script>
		<script language="javascript" type="text/javascript">
		function fClose()
		{
			if(!is.ie)
			{
				try
				{
					opener.document.getElementById('LocationMgrButton').click();
				}
				catch(ex)
				{
					alert('Fehler beim Aktualisieren des Browserfensters. Bitte manuell aktualisieren!');
				}
			}
			window.close();
		}
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<h1 id="pageTitle" runat="server">Orte bearbeiten</h1>
			<table border="0" style="TABLE-LAYOUT: fixed; WIDTH: 100%">
				<colgroup>
					<col width="160">
					<col>
				</colgroup>
				<tr>
					<td colspan="2">Kategorie:</td>
				</tr>
				<tr>
					<td colspan="2" vAlign="top">
						<asp:DropDownList id="EventCategories" runat="server" AutoPostBack="True"></asp:DropDownList></td>
				</tr>
				<tr>
					<td colspan="2">Bestehende Orte:</td>
				</tr>
				<tr>
					<td colspan="2" vAlign="top"><asp:ListBox id="ExistingLocations" runat="server" Width="208px" AutoPostBack="True"></asp:ListBox></td>
				</tr>
				<tr>
					<td colspan="2">Orte bearbeiten:</td>
				</tr>
				<tr>
					<td title="Bezeichnung des Ortes. Dies wird in E-Mail und auf der Webseite verwendet zur Angabe des Ortes eines Anlasses.">Ort: <span class="labelDescription">(wird in E-Mails und<br/>auf der Webseite verwendet)</span>
						<asp:RequiredFieldValidator id="reqLocationText" runat="server" ErrorMessage="Ort" ControlToValidate="LocationTextBox">*</asp:RequiredFieldValidator></td>
					<td valign="top"><asp:TextBox id="LocationTextBox" runat="server" Width="100%" MaxLength="50"></asp:TextBox></td>
				</tr>
				<tr>
					<td title="Möglichst kurze Version des Ortes, da diese in SMS Benachrichtigungen verwendet wird.">Kurzbezeichnung: <span class="labelDescription">(wird<br/>in SMS verwendet)</span>
						<asp:RequiredFieldValidator id="reqLocationShort" runat="server" ErrorMessage="Kurzbezeichnung" ControlToValidate="LocationShortTextBox">*</asp:RequiredFieldValidator></td>
					<td valign="top"><asp:TextBox id="LocationShortTextBox" runat="server" Width="100%" MaxLength="15"></asp:TextBox></td>
				</tr>
				<tr>
					<td title="Detaillierte Beschreibung des Ortes. Wird auf der Webseite und in Emails als Tooltip verwendet wenn der Mauszeiger auf den Ort zeigt.">Beschreibung: <span class="labelDescription">(wird auf<br/>der Webseite und in Emails<br/>als Tooltip verwendet)</span></td>
					<td valign="top"><asp:TextBox id="LocationDescription" runat="server" Width="100%" MaxLength="200"></asp:TextBox></td>
				</tr>
				<tr>
					<td colspan="2" style="PADDING-TOP: 20px">
						<asp:ValidationSummary id="valSummary" runat="server" HeaderText="Nicht alle notwendigen Felder wurden ausgefüllt."></asp:ValidationSummary>
						<asp:Button id="RenameLocation" runat="server" ToolTip="Hiermit wird der oben gewählte Ort mit den Texten in den Feldern überschrieben." Text="Änderungen an gewähltem Ort speichern"
							Width="256px"></asp:Button>
						<asp:Button id="DeleteLocation" runat="server" ToolTip="Hiermit wird der oben gewählte Ort gelöscht." Text="Gewählter Ort löschen" Width="144px" CausesValidation="False"></asp:Button>
						<asp:Button id="CreateLocation" runat="server" ToolTip="Hiermit wird ein neuer Ort eingefügt mit den Texten in den Feldern." Text="Neuer Ort speichern" Width="130px"></asp:Button>
						<button onclick="fClose();" style="MARGIN-LEFT: 4px; WIDTH: 80px; HEIGHT: 24px" type="button">
							Schliessen</button>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
