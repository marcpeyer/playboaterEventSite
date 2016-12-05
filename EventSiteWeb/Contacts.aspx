<%@ Register TagPrefix="es" TagName="Navigation" Src="modules/Navigation.ascx" %>
<%@ Register TagPrefix="es" TagName="ContactControl" Src="modules/ContactControl.ascx" %>
<%@ Page language="c#" Codebehind="Contacts.aspx.cs" AutoEventWireup="false" Inherits="kcm.ch.EventSite.Web.Contacts" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title runat="server" id="title">Kontakt-Administration</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">
		<script language="javascript" src="pages/BrowserCheck.js" type="text/javascript"></script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<es:Navigation runat="server" id="PageNavigation"></es:Navigation>
			<hr>
			<h1 id="pageTitle" runat="server">Event-Site des SoUndSo - Kontakt-Administration</h1>
			<asp:datagrid id="dgrContacts" runat="server" AutoGenerateColumns="False" Width="750px" BorderColor="#999999"
				BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="3" GridLines="Vertical"
				ShowFooter="True">
				<FooterStyle Font-Size="X-Small" Wrap="False" ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
				<ItemStyle Font-Size="X-Small" Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Size="X-Small" Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Editieren">
						<HeaderStyle Width="84px"></HeaderStyle>
						<ItemTemplate>
							<asp:LinkButton id="dgrEditLink" runat="server" Text="Edit" CommandName="Edit" CausesValidation="false" ForeColor="black" Visible='<%# ModifyAllowed(Convert.ToInt32(DataBinder.Eval(Container, "DataItem.ContactId"))) %>'>
							</asp:LinkButton>
						</ItemTemplate>
						<FooterTemplate>
							<asp:LinkButton id="dgrInsertLink" runat="server" Text="Insert" CommandName="Insert" CausesValidation="false" ForeColor="black"></asp:LinkButton>
						</FooterTemplate>
						<EditItemTemplate>
							<asp:LinkButton id="dgrSaveLink" runat="server" Text="Save" CommandName="Update" CausesValidation="false" ForeColor="black"></asp:LinkButton>&nbsp;
							<asp:LinkButton id="dgrCancelLink" runat="server" Text="Cancel" CommandName="Cancel" CausesValidation="false" ForeColor="black"></asp:LinkButton>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="L&#246;schen">
						<ItemTemplate>
							<asp:ImageButton id="dgrDeleteButton" runat="server" CommandName="Delete" CausesValidation="false" ImageUrl="images/icons/delete.gif" AlternateText="Diesen Eintrag löschen" Visible='<%# ModifyAllowed(Convert.ToInt32(DataBinder.Eval(Container, "DataItem.ContactId"))) %>'>
							</asp:ImageButton>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn Visible="False" HeaderText="ContactId">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ContactId") %>' ID="ContactIdLabel">
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ContactId") %>' ID="ContactIdLabel">
							</asp:Label>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Name">
						<ItemTemplate>
							<es:ContactControl runat="server" Contact='<%# DataBinder.Eval(Container, "DataItem") %>'>
							</es:ContactControl>
						</ItemTemplate>
						<FooterTemplate>
							<asp:TextBox id="newName" style="width: 120px;" runat="server" Text=""></asp:TextBox>
						</FooterTemplate>
						<EditItemTemplate>
							<asp:TextBox runat="server" style="width: 120px;" Text='<%# DataBinder.Eval(Container, "DataItem.Name") %>' ID="txtName">
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="E-Mail">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Email") %>'>
							</asp:Label>
						</ItemTemplate>
						<FooterTemplate>
							<asp:TextBox id="newMail" style="width: 160px;" runat="server" Text=""></asp:TextBox>
						</FooterTemplate>
						<EditItemTemplate>
							<asp:TextBox runat="server" style="width: 160px;" Text='<%# DataBinder.Eval(Container, "DataItem.Email") %>' ID="txtEmail">
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Mobile Phone">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MobilePhone") %>'>
							</asp:Label>
						</ItemTemplate>
						<FooterTemplate>
							<asp:TextBox id="newMobile" style="width: 120px;" runat="server" Text=""></asp:TextBox>
						</FooterTemplate>
						<EditItemTemplate>
							<asp:TextBox runat="server" style="width: 120px;" Text='<%# DataBinder.Eval(Container, "DataItem.MobilePhone") %>' ID="txtMobilePhone">
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="SMS Kredit">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%# ((int)DataBinder.Eval(Container, "DataItem.SmsPurchased")) - ((int)DataBinder.Eval(Container, "DataItem.SmsLog")) %>'>
							</asp:Label>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:Label runat="server" Text='<%# ((int)DataBinder.Eval(Container, "DataItem.SmsPurchased")) - ((int)DataBinder.Eval(Container, "DataItem.SmsLog")) %>' ID="Label1">
							</asp:Label>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			<h4 style="MARGIN-BOTTOM: 6px">Erläuterungen:</h4>
			<div style="FONT-SIZE: 12px" id="NameDesc" runat="server">- Name: Der Name, welcher überall (in den Benachrichtigungen, auf der Weboberfläche) verwendet wird.</div>
			<div style="FONT-SIZE: 12px" id="MailDesc" runat="server">- E-Mail: Auf diese Email-Adresse werden Benachrichtigungen verschickt (falls eingeschaltet).</div>
			<div style="FONT-SIZE: 12px" id="MobileDesc" runat="server">- Mobile Phone: Auf diese Nummer werden SMS Benachrichtigungen verschickt (falls eingeschaltet).</div>
<%--			<div style="FONT-SIZE: 12px" id="NotifyBySmsDesc" runat="server">- Notifizierung als SMS: Gibt an, ob SMS Benachrichtigungen verschickt werden sollen.</div>
			<div style="FONT-SIZE: 12px" id="NotifyByMailDesc" runat="server">- Notifizierung als E-Mail: Gibt an, ob E-Mail Benachrichtigungen verschickt werden sollen.</div>
			<div style="FONT-SIZE: 12px" id="NotifSubscriptionDesc" runat="server">- Notifizierungsabonnierung: Wenn eingeschaltet, werden die Benutzer nur beim Erstellen bzw. Ändern von Anlässen benachrichtigt. Um weitere Notifizierungen zu erhalten, muss der Benutzer den Anlass abonnieren. Wenn ausgeschaltet, werden immer alle Benachrichtigungen verschickt.</div>
--%>			
			<div style="FONT-SIZE: 12px" id="SmsCreditDesc" runat="server">- SMS Kredit: Gibt an, wieviele SMS Nachrichten der Benutzer noch empfangen kann bevor sein Kredit aufgebraucht ist.</div>
			<asp:Button id="LogoutButton" style="DISPLAY: none" runat="server" Text="logout"></asp:Button>
			<asp:button id="DeleteContact" style="DISPLAY: none" runat="server" Text="delete"></asp:button><INPUT id="hdnDeleteConfiremd" type="hidden" value="false" runat="server">
		</form>
	</body>
</HTML>
