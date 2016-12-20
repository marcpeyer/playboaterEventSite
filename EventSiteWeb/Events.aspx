<%@ Page Title="Anlass Neu / Bearbeiten" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" validateRequest="false" EnableEventValidation="false" CodeBehind="Events.aspx.cs" Inherits="kcm.ch.EventSite.Web.Events" %>
<%@ Register TagPrefix="dpick" TagName="AJAXDatePickerControl" Src="modules/AJAXDatePickerControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
		<script language="javascript" src="pages/BrowserCheck.js" type="text/javascript"></script>
		<script language="javascript" src="pages/EventSiteCommons.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
		function showLocationMgr(mandatorId)
		{
			showEventSiteModalWindow('Locations.aspx?mid=' + mandatorId, 'EventSite_Locations', 400, 800, 'LocationMgrButton');
		}
		</script>
			<h1 id="pageTitle" runat="server">Anlass: "EventTitle"</h1>
			<asp:panel id="pnlEvent" style="BORDER-RIGHT: 1px solid; PADDING-RIGHT: 3px; BORDER-TOP: 1px solid; PADDING-LEFT: 3px; MARGIN-BOTTOM: 10px; PADDING-BOTTOM: 3px; BORDER-LEFT: 1px solid; PADDING-TOP: 3px; BORDER-BOTTOM: 1px solid"
				runat="server" BackColor="#EEEEEE">
				<TABLE style="TABLE-LAYOUT: fixed; MARGIN-RIGHT: 10px" cellSpacing="0" cellPadding="0"
					border="0">
					<TR id="EventCategoryRow" runat="server">
						<TD width="25%">
							<asp:Label id="lblEventCategory" runat="server">Kategorie:</asp:Label></TD>
						<TD width="75%">
							<asp:DropDownList id="EventCategories" style="VERTICAL-ALIGN: middle" runat="server" AutoPostBack="True"></asp:DropDownList><asp:Label id="lblCategoryDesc" Font-Size="9" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<TD>
							<asp:Label id="lblEvent" runat="server">Bestehender Anlass auswählen:</asp:Label></TD>
						<TD>
							<asp:DropDownList id="EventList" style="VERTICAL-ALIGN: middle" runat="server" AutoPostBack="True"></asp:DropDownList>
							<span title="Neuer Anlass erstellen" style="margin-right: 6px;">Neuer Anlass:
							<asp:ImageButton id="NewEventButton" style="VERTICAL-ALIGN: middle" runat="server" ToolTip="Neuer Anlass erstellen"
								AlternateText="Neuer Anlass erstellen" ImageUrl="images/icons/new.gif" CausesValidation="False"></asp:ImageButton></span>
							<span title="Anlass kopieren und daraus neuen erstellen">Anlass kopieren:
							<asp:ImageButton id="CopyEventButton" style="VERTICAL-ALIGN: middle" runat="server" ToolTip="Anlass kopieren und daraus neuen erstellen"
								AlternateText="Anlass kopieren und daraus neuen erstellen" ImageUrl="images/icons/copy.gif" CausesValidation="False"></asp:ImageButton><asp:Image id="CopyEventInactive" title="Anlass kopieren" style="VERTICAL-ALIGN: middle" runat="server"
								AlternateText="Anlass kopieren" ImageUrl="images/icons/copy_inactive.gif"></asp:Image></span></TD>
					</TR>
				</TABLE>
			</asp:panel>
			<TABLE id="Table1" style="MARGIN-BOTTOM: 10px" borderColor="gray" cellSpacing="0" cellPadding="2"
				width="100%" bgColor="#eeeeee" border="1">
				<TR>
					<TD width="25%">Titel:
						<asp:requiredfieldvalidator id="reqTitle" runat="server" ErrorMessage="Titel darf nicht leer sein!" ControlToValidate="TitleTextBox"
							EnableClientScript="False">*</asp:requiredfieldvalidator></TD>
					<TD width="25%"><asp:textbox id="TitleTextBox" runat="server" Maxlength="50" Width="100%"></asp:textbox></TD>
					<TD width="25%">Ort:&nbsp;<asp:requiredfieldvalidator id="reqLocation" runat="server" ErrorMessage="Ort darf nicht leer sein!" ControlToValidate="LocationDropDown"
							EnableClientScript="False">*</asp:requiredfieldvalidator>&nbsp;<asp:hyperlink id="LocationMgrLink" runat="server">[Orte bearbeiten]</asp:hyperlink></TD>
					<TD width="25%"><asp:dropdownlist id="LocationDropDown" runat="server" Width="100%"></asp:dropdownlist></TD>
				</TR>
				<TR>
					<TD>Startdatum, -zeit:</TD>
					<TD><dpick:ajaxdatepickercontrol id="StartDate" runat="server"></dpick:ajaxdatepickercontrol></TD>
					<TD>Dauer:</TD>
					<TD><asp:textbox id="DurationTextBox" runat="server" Maxlength="50" Width="100%"></asp:textbox></TD>
				</TR>
				<TR id="EventUrlRow" runat="server">
					<TD>Web Adresse:</TD>
					<TD><asp:textbox id="UrlTextBox" runat="server" Width="100%">http://</asp:textbox></TD>
					<td colSpan="2">&nbsp;</td>
				</TR>
				<TR id="MinMaxSubscriptionsRow" runat="server">
					<TD>Min. Anz. Anmeldungen:</TD>
					<TD><asp:textbox id="MinSubscriptionsTextBox" runat="server" Width="100%"></asp:textbox></TD>
					<TD>Max. Anz. Anmeldungen:</TD>
					<TD><asp:textbox id="MaxSubscriptionsTextBox" runat="server" Width="100%"></asp:textbox></TD>
				</TR>
				<TR>
					<TD>Beschreibung:</TD>
					<TD colSpan="3"><asp:textbox id="DescriptionTextBox" runat="server" Width="100%" TextMode="MultiLine" Rows="8"></asp:textbox></TD>
				</TR>
			</TABLE>
			<div id="NotifyChangeArea" style="MARGIN-BOTTOM: 10px" runat="server">
				<div style="COLOR: red">Achtung das ist eine Änderung eines bestehenden Anlasses! 
					Um diesen<br>
					Anlass als Vorlage für einen neuen Anlass zu verwenden, klicke auf den 
					"Kopieren" Knopf.</div>
				Benutzer über diese Änderung benachrichtigen?
				<asp:radiobuttonlist id="NotifyChange" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:radiobuttonlist><asp:requiredfieldvalidator id="reqNotify" runat="server" ErrorMessage="Bitte angeben ob Benachrichtigt werden soll!"
					ControlToValidate="NotifyChange" EnableClientScript="False">*</asp:requiredfieldvalidator></div>
			<asp:button id="ResetButton" runat="server" CausesValidation="False" Text="Zurücksetzen"></asp:button>&nbsp;<asp:button id="SaveButton" runat="server" Text="Speichern"></asp:button><asp:button id="LocationMgrButton" style="DISPLAY: none" runat="server" CausesValidation="False"
				Text="Orte Bearbeiten"></asp:button>
			<asp:validationsummary id="validationSummary" runat="server" EnableClientScript="False"></asp:validationsummary>
</asp:Content>