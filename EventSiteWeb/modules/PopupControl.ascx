<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PopupControl.ascx.cs" Inherits="kcm.ch.EventSite.Web.modules.PopupControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:HyperLink id="PopupLink" runat="server">HyperLink</asp:HyperLink>
<asp:Image id="PopupIcon" runat="server" ImageUrl="../images/icons/mobile_only.gif" style="VERTICAL-ALIGN: middle"></asp:Image>
<asp:Panel id="pnlPopup" runat="server" CssClass="dragableBox popupBox" style="display: none;" EnableViewState="False" Wrap="False">
	<DIV id="PopupBody" runat="server"></DIV>
	<asp:HyperLink id="saveLink" runat="server">speichern</asp:HyperLink>&nbsp;
	<asp:HyperLink id="closeLink" runat="server">schliessen</asp:HyperLink>
</asp:Panel>
