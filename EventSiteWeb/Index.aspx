<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Index.aspx.cs" Inherits="kcm.ch.EventSite.Web.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <title>Playboater's EventSite - Mandanten-Übersicht</title>
    <div>
    <h1>Bitte einen Mandanten wählen</h1>
    </div>
    <asp:Repeater ID="MandatorList" runat="server">
			<ItemTemplate><div><a href="Default.aspx?mid=<%# Eval("MandatorId") %>" target="_self"><%# Eval("MandatorName") %></a></div></ItemTemplate>
		</asp:Repeater>
</asp:Content>
