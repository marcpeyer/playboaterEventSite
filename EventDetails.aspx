<%@ Page language="c#" Codebehind="EventDetails.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.EventDetails" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title id="title" runat="server">"EventTitle"</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<h1 id="pageTitle" runat="server">Anlass: "EventTitle"</h1>
			<TABLE id="Table1" style="MARGIN-BOTTOM: 10px" borderColor="gray" cellSpacing="0" cellPadding="2"
				width="100%" border="1">
				<TR>
					<TD>Titel:</TD>
					<TD><asp:label id="TitleLabel" runat="server">Label</asp:label></TD>
					<TD>Ort:&nbsp;</TD>
					<TD><asp:label id="LocationLabel" runat="server">Label</asp:label></TD>
				</TR>
				<TR>
					<TD>Startdatum, -zeit:</TD>
					<TD><asp:label id="StartDateLabel" runat="server">Label</asp:label></TD>
					<TD>Dauer:</TD>
					<TD><asp:label id="DurationLabel" runat="server">Label</asp:label></TD>
				</TR>
				<TR>
					<TD>Web Adresse:</TD>
					<TD><asp:hyperlink id="UrlLink" runat="server">HyperLink</asp:hyperlink>&nbsp;</TD>
					<td colSpan="2">&nbsp;</td>
				</TR>
				<TR>
					<TD>Min. Anz. Anmeldungen:</TD>
					<TD><asp:label id="MinSubscriptionsLabel" runat="server">Label</asp:label></TD>
					<TD>Max. Anz. Anmeldungen:</TD>
					<TD><asp:label id="MaxSubscriptionsLabel" runat="server">Label</asp:label></TD>
				</TR>
				<TR>
					<TD>Beschreibung:</TD>
					<TD colSpan="3"><asp:label id="DescriptionLabel" runat="server">Label</asp:label></TD>
				</TR>
			</TABLE>
			<INPUT onclick="window.close();" type="button" value="Schliessen">
		</form>
	</body>
</HTML>
