<%@ Register TagPrefix="es" TagName="Navigation" Src="modules/Navigation.ascx" %>
<%@ Page language="c#" Codebehind="Help.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.Help" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title runat="server" id="title">Help</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<es:Navigation runat="server" id="PageNavigation"></es:Navigation>
			<hr>
			<h1 id="pageTitle" runat="server">Event-Site des SoUndSo</h1>
		</form>
	</body>
</HTML>
