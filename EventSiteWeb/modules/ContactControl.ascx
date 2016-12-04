<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ContactControl.ascx.cs" Inherits="kcm.ch.EventSite.Web.modules.ContactControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:HyperLink id="NameLink" runat="server">HyperLink</asp:HyperLink>
<asp:Image id="MobileIcon" runat="server" ImageUrl="../images/icons/mobile_only.gif" style="VERTICAL-ALIGN: middle"></asp:Image>
<asp:Panel id="pnlSendSms" runat="server" CssClass="dragableBox smsBox" EnableViewState="False" Wrap="False" style="DISPLAY: none;">
	<asp:TextBox id="SmsBody" runat="server" EnableViewState="False" TextMode="MultiLine"></asp:TextBox>
	<TABLE border="0">
		<TR>
			<TD>
				<asp:HyperLink id="sendLink" runat="server">senden</asp:HyperLink>&nbsp;
				<asp:HyperLink id="closeLink" runat="server">schliessen</asp:HyperLink></TD>
			<TD align="right">
				<asp:Label id="CharCounter" runat="server">160/1</asp:Label></TD>
		</TR>
	</TABLE>
</asp:Panel>
