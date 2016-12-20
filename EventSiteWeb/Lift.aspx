<%@ Page language="c#" Codebehind="Lift.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.Lift" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Route definieren</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
		<base target="_self">
		<script language="javascript" src="pages/BrowserCheck.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
		function fClose()
		{
			if(!is.ie)
			{
				try
				{
					opener.document.getElementById('MainContent_refreshEventButton').click();
					//opener.__doPostBack('refreshEventButton','');
				}
				catch(ex)
				{
					alert('Fehler beim Aktualisieren des Browserfensters. Bitte manuell aktualisieren!\n' +
						'Fehlermeldung: ' + ex);
				}
			}
			window.close();
		}

		function fOnSubmit()
		{
			//alert('onsubmit');
			document.body.style.cursor = 'wait';
			document.getElementById('SaveButton').style.cursor = 'wait';
			document.getElementById('CloseButton').style.cursor = 'wait';
			document.getElementById('CloseButton').disabled = true;
			window.setTimeout('document.getElementById(\'SaveButton\').disabled = true', 100);
		}
		</script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server" onsubmit="fOnSubmit();">
			<h1 id="pageTitle" runat="server">Mitfahrt&nbsp;definieren</h1>
			<table style="TABLE-LAYOUT: fixed; WIDTH: 100%" border="0">
				<colgroup>
					<col width="160">
					<col>
				</colgroup>
				<tr>
					<td colSpan="2">
						<div style="MARGIN-BOTTOM: 10px"><asp:checkbox id="TakesLift" runat="server" AutoPostBack="True" Text="Ich möchte bei jemandem Mitfahren"></asp:checkbox></div>
					</td>
				</tr>
				<tr>
					<td style="HEIGHT: 18px" colSpan="2"><STRONG>Mitfahrgelegenheiten:</STRONG></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:dropdownlist id="LiftChoice" runat="server" AutoPostBack="True"></asp:dropdownlist>
						<asp:RequiredFieldValidator id="LiftChoiceValitator" runat="server" EnableClientScript="False" ControlToValidate="LiftChoice"
							ErrorMessage="Bitte eine Mitfahrgelegenheit wählen.">*</asp:RequiredFieldValidator></td>
				<TR>
					<TD colSpan="2" height="30">Die Abfahrtszeit am Startpunkt ist um&nbsp;<span id="JourneyStartTime" style="BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 2px; BORDER-TOP: black 1px solid; PADDING-LEFT: 2px; BORDER-LEFT: black 1px solid; BORDER-BOTTOM: black 1px solid"
							runat="server">&nbsp;</span>
					</TD>
				</TR>
				<tr>
					<td style="HEIGHT: 18px" colSpan="2"><STRONG>Routenpunkt wählen:</STRONG></td>
				</tr>
				<tr>
					<td colSpan="2">(An diesem Punkt wird der Mitfahrer abgeholt)<br>
						<asp:dropdownlist id="JourneyStationChoice" runat="server" style="MARGIN-TOP: 5px"></asp:dropdownlist>
						<asp:RequiredFieldValidator id="JourneyStationChoiceValidator" runat="server" EnableClientScript="False" ControlToValidate="JourneyStationChoice"
							ErrorMessage="Bitte einen Routenpunkt auswählen.">*</asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td style="HEIGHT: 18px" colSpan="2"><STRONG>Bemerkung (max. 160 Zeichen):</STRONG></td>
				</tr>
				<tr>
					<td colSpan="2">
						<asp:TextBox id="Comment" runat="server" TextMode="MultiLine" Height="72px"></asp:TextBox>
						<asp:CustomValidator id="CommentValidator" runat="server" EnableClientScript="False" ControlToValidate="Comment"
							ErrorMessage="Bemerkung darf maximal 160 Zeichen enthalten.">*</asp:CustomValidator>(Diese 
						kurze Bemerkung wird der Benachrichtigung an den Fahrer angehängt)</td>
				</tr>
				<tr>
					<td colSpan="2"></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:validationsummary id="ValSummary" runat="server"></asp:validationsummary></td>
					</TD></tr>
				<tr>
					<td style="PADDING-TOP: 20px" colSpan="2"><button id="CloseButton" style="MARGIN-LEFT: 4px; WIDTH: 80px;" onclick="fClose();" type="button">Schliessen</button>
						<asp:button id="SaveButton" runat="server" Text="Speichern"></asp:button><br>
						<br>
						Der Fahrer wird in folgenden Fällen automatisch benachrichtigt:
						<li>wenn eine neue Mitfahrt erstellt wird</li>
						<li>wenn der Routenpunkt einer bestehenden Mitfahrt geändert wird</li>
						<li>wenn eine Mitfahrt gelöscht wird</li>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
