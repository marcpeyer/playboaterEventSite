<%@ Page Title="Über" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs" Inherits="kcm.ch.EventSite.Web.Default" %>
<%@ Register TagPrefix="es" TagName="Navigation" Src="modules/Navigation.ascx" %>
<%@ Register TagPrefix="es" TagName="ContactControl" Src="modules/ContactControl.ascx" %>
<%@ Register TagPrefix="es" TagName="PopupControl" Src="modules/PopupControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
		<script language="javascript" src="pages/BrowserCheck.js" type="text/javascript"></script>
		<script language="javascript" src="pages/EventSiteCommons.js" type="text/javascript"></script>
		<script language="javascript" type="text/javascript">
		var cellInnerHtmlBefore;
		var popupField = null;
		var mid = '<%= BLL.Mandator.MandatorId %>';
		var popupVisible = false;

		function HidePopups()
		{
			if(!popupVisible)
			{
				if(popupField != null)
				{
					popupField.innerHTML = cellInnerHtmlBefore;
				}
				popupField = null;
			}
		}
		
		function DisplayAjaxPopup(evt, sid, sourceElem, mode)
		{
			//search the parent table cell element
			while(sourceElem.nodeName != 'TD')
			{
				sourceElem = sourceElem.parentNode;
			}

			//hide existing popups
			HidePopups();

			//set choicefield with the current table cell elem
			popupField = sourceElem;
			
			//store html source before
			cellInnerHtmlBefore = sourceElem.innerHTML;

			//detect mouse position
			var top = 0;
			var left = 0;
			if(!is.ie)
			{
				top = evt.pageY;
				left = evt.pageX;
			}
			else
			{
				top = window.event.clientY + document.body.scrollTop;
				left = window.event.clientX + document.body.scrollLeft;
			}

			//ajax postback to retrieve html source
			var htmlSource;
			switch(mode)
			{
				case 'lift':
					htmlSource = EventSiteBL.GetLiftMgtChoiceHtml(mid, sid, left, top);
					break;
				case 'edit':
					htmlSource = EventSiteBL.GetSubscriptionChangeHtml(mid, sid, left, top);
				break;
				default:
					alert('Unsupported popup mode given! Can\'t complete this action.');
					return;
			}

//			htmlSource.value = '<div style="background-color: white; border: 1px solid red; position: absolute; top: ' + top + 'px; left:' + left + 'px;">test2</div>';
			
			//render the choicemenu
			if(htmlSource.error == null)
			{
				//append choice menu to cell
				sourceElem.innerHTML = cellInnerHtmlBefore + htmlSource.value;
				
				popupVisible = true;
				window.setTimeout('resetPopupVisible()', 300);
			}
			else
			{
				alert(htmlSource.error);
			}
		}
		
		function fSaveSubscriptionChange(mid, sid, stfields, timefid, commfid)
		{
			var stid = -1;
			for(var i = 0; i < stfields.length; i++)
			{
				var field = document.getElementById(stfields[i]);
				if(field.checked)
				{
					stid = field.value;
				}
			}
			if(stid == -1)
			{
				alert('Error. No selected stage found!');
				HidePopups();
				return;
			}
			
			var result;
			result = EventSiteBL.ApplySubscriptionChange(mid, sid, stid, document.getElementById(commfid).value, document.getElementById(timefid).value);
			
			if(result.error == null)
			{
				if(result.value)
				{
					//alert('Änderungen erfolgreich gespeichert.');
				}
				else
				{
					alert('Fehler beim Speichern des Eintrages!');
				}
			}
			else
			{
				alert(result.error);
			}
		
			HidePopups();
			
			document.getElementById('MainContent_refreshEventButton').click();
		}
		
		function resetPopupVisible()
		{
			popupVisible = false;
		}
		
		</script>
			<es:Navigation id="PageNavigation" runat="server"></es:Navigation>
			<hr>
			<h1 id="pageTitle" runat="server">Event-Site des SoUndSo</h1>
			<div id="pnlEventContainer" style="BORDER-RIGHT: blue 0px solid; BORDER-TOP: blue 0px solid; MARGIN-BOTTOM: 10px; BORDER-LEFT: blue 0px solid; BORDER-BOTTOM: blue 0px solid">
				<asp:panel id="pnlEvent" style="BORDER-RIGHT: red 0px solid; BORDER-TOP: red 0px solid; FLOAT: left; MARGIN-BOTTOM: auto; BORDER-LEFT: red 0px solid; MARGIN-RIGHT: 5px; BORDER-BOTTOM: red 0px solid"
					runat="server">
					<DIV id="eventChooser" style="BORDER-RIGHT: black 1px solid; PADDING-RIGHT: 3px; BORDER-TOP: black 1px solid; PADDING-LEFT: 3px; PADDING-BOTTOM: 3px; BORDER-LEFT: black 1px solid; PADDING-TOP: 3px; BORDER-BOTTOM: black 1px solid; WHITE-SPACE: nowrap; BACKGROUND-COLOR: #eeeeee">
						<asp:Label id="lblEvent" runat="server" Font-Size="X-Small">Anlass:</asp:Label>
						<asp:DropDownList id="events" style="VERTICAL-ALIGN: middle" runat="server" AutoPostBack="True"></asp:DropDownList>
						<asp:ListBox id="eventList" runat="server" AutoPostBack="True" Rows="8"></asp:ListBox>
						<asp:ImageButton id="refreshEventButton" style="VERTICAL-ALIGN: middle" runat="server" CausesValidation="False"
							ImageUrl="images/icons/refresh.gif" ToolTip="Aktualisieren" AlternateText="Aktualisieren"></asp:ImageButton>
						<asp:ImageButton id="showOldEventsButton" style="VERTICAL-ALIGN: middle" runat="server" CausesValidation="False"
							ImageUrl="images/icons/show_old.gif" ToolTip="Vergangene Anlässe auch anzeigen" AlternateText="Vergangene Anlässe auch anzeigen"></asp:ImageButton>
						<es:PopupControl id="EventCategoryFilterPopup" runat="server" PopupIconSrc="images/icons/filter_off.gif" PopupText="Set Category Filter"></es:PopupControl>
						<!--<asp:HyperLink id="eventDetailsLink" runat="server">Anlassdetails anzeigen</asp:HyperLink>-->
					</DIV>
					<DIV id="notifSubscription" runat="server">
						<asp:Label id="lblNotifSubscription" runat="server" Font-Size="X-Small">SMS Abonnierung:</asp:Label><BR>
						<asp:Label id="lblNofifSubscriptionDisabled" Font-Size="X-Small" Visible="False" Runat="server">Du hast die Abonnierungsfunktion<br>zur Zeit ausgeschaltet</asp:Label>
						<asp:CheckBox id="chkNotifSubscription" Font-Size="X-Small" Runat="server" Text="Benachrichtigungen ein / aus"></asp:CheckBox><BR>
						<asp:TextBox id="txtNumSmsNotifications" Runat="server" MaxLength="2" Width="22" style="width:22px;"></asp:TextBox><LABEL id="numSmsNotificationsLabel" for="txtNumSmsNotifications"
							runat="server">&nbsp;Anzahl Benachrichtigungen (leer = unbeschränkt)</LABEL>
					</DIV>
				</asp:panel>
				<asp:Panel ID="pnlEventDetails" Runat="server">
					<DIV id="eventDetails">
					<TABLE style="width:100%;" cellSpacing="0" cellPadding="0">
						<tr><td id="eventDetailsCell">
						<TABLE id="eventDetailsTable" borderColor="red" cellSpacing="0" cellPadding="2"
							width="100%" border="0">
							<TR id="EventDetails_EventCategory" runat="server">
								<TD style="FONT-SIZE: 12px" vAlign="top">Kategorie:</TD>
								<TD style="FONT-SIZE: 12px" colSpan="3">
									<asp:label id="EventCategoryLabel" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD style="FONT-SIZE: 12px" vAlign="top">Titel:</TD>
								<TD style="FONT-SIZE: 12px" vAlign="top">
									<asp:label id="TitleLabel" runat="server"></asp:label></TD>
								<TD style="FONT-SIZE: 12px" vAlign="top">Ort:&nbsp;</TD>
								<TD style="FONT-SIZE: 12px; WHITE-SPACE: normal">
									<asp:label id="LocationLabel" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD style="FONT-SIZE: 12px">Datum, -zeit:</TD>
								<TD style="FONT-SIZE: 12px">
									<asp:label id="StartDateLabel" runat="server"></asp:label></TD>
								<TD style="FONT-SIZE: 12px">Dauer:</TD>
								<TD style="FONT-SIZE: 12px">
									<asp:label id="DurationLabel" runat="server"></asp:label></TD>
							</TR>
							<TR id="EventDetails_MinMaxSubscriptions" runat="server">
								<TD style="FONT-SIZE: 12px">Min. Anmeldungen:</TD>
								<TD style="FONT-SIZE: 12px">
									<asp:label id="MinSubscriptionsLabel" runat="server"></asp:label></TD>
								<TD style="FONT-SIZE: 12px">Max. Anmeldungen:</TD>
								<TD style="FONT-SIZE: 12px">
									<asp:label id="MaxSubscriptionsLabel" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD id="EventDetails_EventUrlLabel" style="FONT-SIZE: 12px" runat="server">Web 
									Adresse:</TD>
								<TD id="EventDetails_EventUrlControl" style="FONT-SIZE: 12px" runat="server">
									<asp:hyperlink id="UrlLink" runat="server"></asp:hyperlink>&nbsp;</TD>
								<TD style="FONT-SIZE: 12px">Ersteller:</TD>
								<TD style="FONT-SIZE: 12px">
									<es:ContactControl id="EventCreator" runat="server"></es:ContactControl></TD>
							</TR>
							<TR>
								<TD style="FONT-SIZE: 12px" vAlign="top">Beschreibung:</TD>
								<TD style="FONT-SIZE: 12px; WHITE-SPACE: normal" colSpan="3">
									<asp:label id="DescriptionLabel" runat="server"></asp:label></TD>
							</TR>
						</TABLE>
						</td></tr>
					</TABLE>
					</DIV>
				</asp:Panel>
			</div>
			<table style="MARGIN-BOTTOM: 15px; WIDTH: 100%" border="0">
				<tr>
					<td style="BORDER-RIGHT: 2px inset; PADDING-RIGHT: 5px; BORDER-TOP: 2px inset; PADDING-LEFT: 5px; PADDING-BOTTOM: 5px; VERTICAL-ALIGN: top; BORDER-LEFT: 2px inset; PADDING-TOP: 5px; BORDER-BOTTOM: 2px inset"><asp:panel id="pnlSubscriptions" runat="server">
							<asp:Label id="lblSubscriptions" runat="server" Font-Size="X-Small">Zur Zeit sind für diesen Anlass folgende Einträge vorhanden:</asp:Label>
							<asp:DataGrid id="dgrSubscriptions" style="MARGIN-TOP: 5px" runat="server" Width="100%" AutoGenerateColumns="False"
								BorderColor="#999999" BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="2">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle Font-Size="X-Small" ForeColor="Black" BackColor="White"></AlternatingItemStyle>
								<ItemStyle Font-Size="X-Small" ForeColor="Black" BackColor="White" VerticalAlign="Top"></ItemStyle>
								<HeaderStyle Font-Size="X-Small" Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn Visible="False" HeaderText="SubscriptionId">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SubscriptionId") %>' ID="SubscriptionIdLabel">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SubscriptionId") %>' ID="Label1">
											</asp:Label>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Edit">
										<HeaderStyle Width="25px"></HeaderStyle>
										<ItemTemplate>
										    <span runat="server" id="subscrEditContainer" Visible='<%# ModifyAllowed(Convert.ToInt32(DataBinder.Eval(Container, "DataItem.Contact.ContactId"))) %>'>
    										    <img src="images/icons/edit.png" style="border-width: 0px; cursor: pointer;" onclick="DisplayAjaxPopup(event, <%# DataBinder.Eval(Container, "DataItem.SubscriptionId") %>, this, 'edit')" />
										    </span>
											<%-- <asp:ImageButton id=ImageButton1 runat="server" Visible='<%# ModifyAllowed(Convert.ToInt32(DataBinder.Eval(Container, "DataItem.Contact.ContactId"))) && Convert.ToBoolean(DataBinder.Eval(Container, "DataItem.IsDeletable")) %>' AlternateText="Diesen Eintrag löschen" ImageUrl="images/icons/delete.gif" CausesValidation="false" CommandName="Delete">
											</asp:ImageButton> --%>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Datum">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Event.StartDate", "{0: dd.MM.yyyy}") %>' ID="Label2" NAME="Label2">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Event.StartDate", "{0: dd.MM.yyyy}") %>' ID="Textbox1" NAME="Textbox1">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Zeit">
										<ItemTemplate>
											<asp:Label runat="server" ID="SubscriptionTimeLabel" Text='<%# DataBinder.Eval(Container, "DataItem.Event.StartDate", "{0: HH:mm}") %>'>
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Event.StartDate", "{0: HH:mm}") %>' ID="Textbox2" NAME="Textbox2">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Name">
										<ItemStyle Wrap="False"></ItemStyle>
										<ItemTemplate>
											<es:ContactControl runat="server" Contact='<%# DataBinder.Eval(Container, "DataItem.Contact") %>'>
											</es:ContactControl>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Contact.Name") %>' ID="Textbox3" NAME="Textbox3">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Status">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SubscriptionStateText") %>' ID="Label4" NAME="Label4">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.SubscriptionStateText") %>' ID="Textbox4" NAME="Textbox4">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Anlass Ort">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Event.Location.LocationText") %>' ID="Label5" NAME="Label5">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Event.Location.LocationText") %>' ID="Textbox5" NAME="Textbox5">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="Beschreibung">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Event.Location.LocationDescription") %>' ID="Label6" NAME="Label6">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Event.Location.LocationDescription") %>' ID="Textbox6" NAME="Textbox6">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Kommentar">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# HttpUtility.HtmlEncode(DataBinder.Eval(Container, "DataItem.Comment").ToString()).Replace("\r\n", "\n").Replace("\n", "<br/>") %>' ID="Label7" NAME="Label7">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Comment") %>' ID="Textbox7" NAME="Textbox7">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderImageUrl="images/icons/car_undefined.gif" HeaderText="Mitfahrten">
										<ItemTemplate>
											<img src='images/icons/car_<%# DataBinder.Eval(Container, "DataItem.SubscriptionLiftState") %>.gif' style='border-width: 0px; cursor: pointer;' onClick='DisplayAjaxPopup(event, <%# DataBinder.Eval(Container, "DataItem.SubscriptionId") %>, this, "lift")' />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
							</asp:DataGrid>
							<DIV id="legendDiv" style="MARGIN-TOP: 3px" runat="server">Legende:<BR>
								<IMG src="images/icons/car_undefined.gif"> = undefiniert<BR>
								<IMG src="images/icons/car_drives.gif"> = fährt selbst und hat noch freie 
								Plätze<BR>
								<IMG src="images/icons/car_full.gif"> = fährt selbst und hat keine freien 
								Plätze mehr<BR>
								<IMG src="images/icons/car_lift.gif"> = fährt bei jemandem anderen mit
							</DIV>
						</asp:panel><asp:panel id="pnlAddContact" runat="server" Visible="False">
							<asp:Label id="lblAddContact" Font-Size="X-Small" Runat="server">Für die angegebene E-Mail Adresse wurde noch kein<BR>Kontakt erfasst. Bitte Angaben zum Kontakt ausfüllen.</asp:Label>
							<BR>
							<TABLE>
								<TR>
									<TD>
										<asp:Label id="lblName" runat="server" Font-Size="X-Small">Name:</asp:Label></TD>
									<TD>
										<asp:TextBox id="txtName" runat="server"></asp:TextBox>
										<asp:RequiredFieldValidator id="reqAddContactName" runat="server" Font-Size="X-Small" ErrorMessage="Bitte Namen angeben!"
											ControlToValidate="txtName"></asp:RequiredFieldValidator></TD>
								</TR>
								<TR>
									<TD>
										<asp:Label id="lblEmail" runat="server" Font-Size="X-Small">E-Mail:</asp:Label></TD>
									<TD>
										<asp:TextBox id="txtEmail" runat="server" BackColor="InactiveBorder" Enabled="False"></asp:TextBox>
										<asp:CheckBox id="chkNotifyByEmail" runat="server" Font-Size="X-Small" Text="Notifizierung als E-Mail"
											Checked="True"></asp:CheckBox></TD>
								</TR>
								<TR>
									<TD>
										<asp:Label id="lblMobile" runat="server" Font-Size="X-Small">Handy:</asp:Label></TD>
									<TD>
										<asp:TextBox id="txtMobile" runat="server"></asp:TextBox>
										<asp:CheckBox id="chkNotifyBySms" runat="server" Font-Size="X-Small" Text="Notofizierung als SMS"></asp:CheckBox></TD>
								</TR>
								<TR>
									<TD><%--<asp:Button id="AddContact" style="MARGIN-TOP: 5px" runat="server" Text="Speichern"></asp:Button>--%>
										<asp:Button id="AddContact" runat="server" Text="Kontakt erstellen"></asp:Button></TD>
									<TD></TD>
								</TR>
							</TABLE>
						</asp:panel></td>
					<td id="AddSubscriptionCell" style="PADDING-RIGHT: 5px; PADDING-LEFT: 5px; PADDING-BOTTOM: 5px; VERTICAL-ALIGN: top; WIDTH: 220px; PADDING-TOP: 5px"
						runat="server"><asp:panel id="pnlAddSubscription" runat="server">
							<asp:Label id="lblSubscript" runat="server" Font-Size="X-Small">Eintragen:</asp:Label>
							<asp:RadioButtonList id="subscriptionStates" runat="server" Font-Size="X-Small"></asp:RadioButtonList>
							<HR>
							<DIV id="subscriptionMailContainer" runat="server">
								<asp:Label id="lblAddSubscriptionEmail" runat="server" Font-Size="X-Small">E-Mail:</asp:Label><BR>
								<A onclick="this.nextSibling.nextSibling.style.display = (this.nextSibling.nextSibling.style.display=='none'?'block':'none');"
									href="#">Erläuterung ein/ausblenden</A>
								<DIV style="DISPLAY: none; FONT-SIZE: 12px; WHITE-SPACE: normal">Da du Manager oder 
									Administrator bist kannst du hier für jemand anderes Eintragen. Falls das Feld 
									leer gelassen wird, wird der angemeldete Benutzer verwendet.</DIV>
								<BR>
								<asp:TextBox id="email" runat="server"></asp:TextBox></DIV>
							<asp:Label id="lblSubscriptionTime" runat="server" Font-Size="X-Small">Zeit (falls anders als im Anlass):</asp:Label>
							<BR>
							<asp:TextBox id="subscriptionTime" runat="server" MaxLength="20"></asp:TextBox>
							<BR>
							<%--<asp:Label id=lblMaxNotifications runat="server" Font-Size="X-Small">Max.&nbsp;Anzahl&nbsp;SMS-<br>Notifizierungen&nbsp;für<br>diesen&nbsp;Anlass&nbsp;(leer&nbsp;=&nbsp;unbeschränkt):</asp:Label><BR>
			<asp:TextBox id=maxNotifications runat="server" MaxLength="3"></asp:TextBox><BR><BR>--%>
							<asp:Label id="lblComment" runat="server" Font-Size="X-Small">Kommentar:</asp:Label>
							<BR>
							<asp:TextBox id="txtComment" runat="server" Rows="4" TextMode="MultiLine"></asp:TextBox>
							<BR>
							<%--<asp:Button id="AddSubscription" style="MARGIN-TOP: 5px" runat="server" Text="Eintragen"></asp:Button>--%>
							<asp:Button id="AddSubscription" runat="server" Text="Eintragen"></asp:Button>
						</asp:panel></td>
				</tr>
			</table>
			<asp:Panel id="pnlFeature" runat="server" cssClass="FeatureBox"></asp:Panel>
			<!--<a style="FONT-SIZE: 14px" href="http://www.bwg.admin.ch/lhg/SMS.xml" Target="_blank">
				Aktuelle Pegel- und Abflussdaten</a>-->
			<h4 style="MARGIN-BOTTOM: 6px">Erläuterungen:</h4>
			<div style="FONT-SIZE: 12px"><asp:literal id="helptext" Runat="server"></asp:literal></div>
			<asp:checkbox id="chkCategoryFilterOn" style="DISPLAY: none" runat="server" AutoPostBack="True"></asp:checkbox>
			<asp:label id="lblSmsBalanceNotifier" Font-Size="Small" Visible="False" Runat="server" ForeColor="Red"></asp:label>
			<script language="javascript" type="text/javascript">
<!--
				//set to minimum for the scrollwith to mimimize
				function fResetEventDetailSize()
				{
					var container = document.getElementById('pnlEventContainer');
					var source = document.getElementById('MainContent_pnlEvent');
					var target = document.getElementById('MainContent_pnlEventDetails');
					target.style.width = (document.body.clientWidth - source.clientWidth - 35) + 'px';
					container.style.width = (document.body.clientWidth - 20) + 'px';
					//alert('reseted');
					window.setTimeout('fSetEventDetailSize()', 5);
				}
				
				function fSetEventDetailSize()
				{
					var container = document.getElementById('pnlEventContainer');
					var target = document.getElementById('MainContent_pnlEventDetails');
					var source = document.getElementById('MainContent_pnlEvent');
					
					var gap;

					if(!is.ie)
					{
						if(is.chrome)
						{
							target.style.display = 'inline-block';
							gap = 27;
						}
						else
						{
							gap = 24;
						}
					}
					else
					{
						gap = 38;
					}

					target.style.width = (document.body.scrollWidth - source.clientWidth - gap) + 'px';
					container.style.width = (document.body.scrollWidth - 20) + 'px';
					//alert(window.innerWidth);
					//alert(window.outerWidth);
					//alert(document.body.clientWidth);
					//alert('offset' + document.body.offsetWidth);
					//alert((document.body.clientWidth - source.clientWidth - gap));
					
				}
				
				if(!is.ie)
				{
					try
					{
						//document.getElementById('eventChooser').style.paddingTop = '6px';
					}
					catch(e)
					{};
				}

				document.body.onclick = function () { window.setTimeout('HidePopups()', 15); };

				window.theForm.onsubmit = function () {
					//alert('onsubmit');
					document.body.style.cursor = 'wait';
					try {
						document.getElementById('MainContent_AddSubscription').style.cursor = 'wait';
						window.setTimeout('document.getElementById(\'AddSubscription\').disabled = true', 100);
					}
					catch (e)
					{ }
					try {
						document.getElementById('MainContent_AddContact').style.cursor = 'wait';
						window.setTimeout('document.getElementById(\'AddContact\').disabled = true', 100);
					}
					catch (e)
					{ }
				}
//-->
			</script>
</asp:Content>