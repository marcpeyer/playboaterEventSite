<%@ Page language="c#" Codebehind="Journey.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.Journey" %>
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
			<h1 id="pageTitle" runat="server">Route&nbsp;definieren</h1>
			<table style="TABLE-LAYOUT: fixed; WIDTH: 100%" border="0">
				<colgroup>
					<col width="160">
					<col>
				</colgroup>
				<tr>
					<td colSpan="2">
						<div style="MARGIN-BOTTOM: 10px"><asp:checkbox id="IsDriving" runat="server" Text="Ich fahre selber zu diesem Anlass" AutoPostBack="True"></asp:checkbox></div>
						<div style="MARGIN-BOTTOM: 20px">Mit mir können zusätzlich maximal&nbsp;<asp:textbox id="NumLifts" runat="server" MaxLength="2" Width="20">0</asp:textbox>
							<asp:rangevalidator id="NumLiftsValidator" runat="server" Display="Dynamic" Type="Integer" MinimumValue="0"
								MaximumValue="20" ErrorMessage="Bitte eine Zahl zwischen 0 und 20 eingeben!" ControlToValidate="NumLifts"
								EnableClientScript="False">*</asp:rangevalidator>&nbsp;Personen mitfahren.</div>
					</td>
				</tr>
				<tr>
					<td colSpan="2"><STRONG>In der Vergangenheit definierte Routen:</STRONG></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:dropdownlist id="PastJourneys" runat="server"></asp:dropdownlist><asp:linkbutton id="ApplyPastJourney" runat="server" CausesValidation="False">Als Vorlage übernehmen</asp:linkbutton></td>
				<tr>
					<td colSpan="2"><STRONG>Routenpunkte festlegen (an diesen Punkten können sich Mitfahrer 
							eintragen):</STRONG></td>
				</tr>
				<tr>
					<td colSpan="2">
						<asp:DataGrid id="JourneyStations" runat="server" AutoGenerateColumns="False" BorderColor="#999999"
							BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="3" GridLines="Vertical"
							style="FLOAT: left">
							<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="#DCDCDC"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn HeaderText="Routenpunkt">
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Station") %>' ID="Station">
										</asp:Label>
										<%--um
								<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.StationTime") --%>
										<%--' ID="StationTime">
								</asp:Label>--%>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Station") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Bearbeiten">
									<ItemTemplate>
										<asp:ImageButton runat="server" AlternateText="Diesen Routenpunkt nach oben verschieben" ImageUrl="images/icons/arrow_up.gif"
											CausesValidation="false" CommandName="MoveUp"></asp:ImageButton>
										<asp:ImageButton runat="server" AlternateText="Diesen Routenpunkt nach unten verschieben" ImageUrl="images/icons/arrow_down.gif"
											CausesValidation="false" CommandName="MoveDown"></asp:ImageButton>
										<asp:ImageButton runat="server" AlternateText="Diesen Routenpunkt entfernen" ImageUrl="images/icons/delete.gif"
											CausesValidation="false" CommandName="Delete"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
						</asp:DataGrid>
						<asp:CustomValidator id="JourneyStationsValidator" runat="server" ErrorMessage="Es muss mindestens ein Routenpunkt definiert werden!"
							EnableClientScript="False">*</asp:CustomValidator>
					</td>
				</tr>
				<tr>
					<td>Routenpunkt hinzufügen:</td>
					<td><asp:textbox id="NewJourneyStation" runat="server"></asp:textbox>
						<%--ungefähre Zeit:&nbsp;<asp:textbox id="NewJourneyStationTime" runat="server"></asp:textbox>--%>
						<asp:button id="AddJourneyStation" runat="server" Text="Routenpunkt einfügen" CausesValidation="False"></asp:button>
					</td>
				</tr>
				<tr>
					<td>
						Abfahrtszeit festlegen:</td>
					<td><asp:TextBox id="JourneyStartTime" runat="server"></asp:TextBox>
						<asp:RequiredFieldValidator id="JourneyStartTimeValidator" runat="server" ControlToValidate="JourneyStartTime"
							ErrorMessage="Die Abfahrtszeit muss ausgefüllt sein!" EnableClientScript="False">*</asp:RequiredFieldValidator></td>
				</tr>
				<tr>
					<td colSpan="2"></td>
				</tr>
				<tr>
					<td colSpan="2"><asp:validationsummary id="ValSummary" runat="server"></asp:validationsummary></td>
					</TD></tr>
				<tr>
					<td style="PADDING-TOP: 20px" colSpan="2"><button id="CloseButton" style="MARGIN-LEFT: 4px; WIDTH: 80px; HEIGHT: 24px" onclick="fClose();" type="button">Schliessen</button>
						<asp:button id="SaveButton" runat="server" Text="Speichern"></asp:button></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
