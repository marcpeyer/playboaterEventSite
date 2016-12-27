<%@ Page Title="Über" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="About.aspx.cs" Inherits="kcm.ch.EventSite.Web.About" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<!--		<title runat="server" id="title">About</title>
		<LINK href="pages/EventSite.css" type="text/css" rel="styleSheet">-->
			<h1 id="pageTitle" runat="server">Event-Site des SoUndSo</h1>
			<table id="appInfoTable">
				<tr>
					<td>Titel dieser Webapplikation:</td>
					<td>Event Site</td>
				</tr>
				<tr>
					<td>Version:</td>
					<td><%=version%></td>
				</tr>
				<tr>
					<td>Build Datum:</td>
					<td><%=buildDate%></td>
				</tr>
				<tr>
					<td>Assembly:</td>
					<td><%=assemblyName%></td>
				</tr>
				<tr>
					<td>Aktueller Mandant:</td>
					<td><%=mandatorName%></td>
				</tr>
				<tr id="MandatorSmsCreditRow" runat="server">
					<td>SMS Kredit des Mandanten:</td>
					<td><%=mandatorSmsCredit%></td>
				</tr>
				<tr>
					<td>Seitentitel dieses Mandanten:</td>
					<td><%=mandatorSiteTitle%></td>
				</tr>
				<tr>
					<td>Programmiert durch:</td>
					<td><a href="mailto:webmaster@playboater.ch">playboater.ch</a></td>
				</tr>
			</table>
			<h2>Bekannte Bugs (Fehler):</h2>
			<ul style="color: red;">
				<li>Die Anlass SMS mit Kommentaren werden als 2 SMS verrechnet (wahrsch. 1-2 Zeichen zu lang)</li>
				<li>Das Logging der Two-Way SMS ist falsch</li>
				<li>Im SMS Popup auf der Kontakt Seite können keine Zeilenumbrüche eingegeben werden</li>
				<!--<span style="color: black;">keine Bekannt</span>-->
			</ul>
			<h2>Version History:</h2>
			<h3>Ideen für künftige Versionen:</h3>
			<ul>
				<li>Per SMS soll ein Anlass, für den man sich interessiert, abonniert werden können</li>
				<li>Optionaler Konfigurationsparameter für die Custom Authentication für eine IsDeleted Spalte einführen</li>
				<li>Berücksichtigung der Parameter "Min. Anzahl Anmeldungen" und "Max. Anzahl Anmeldungen"<!-- ACHTUNG: nur Subscriptions mit IsDeletable = 0 zählen! --></li>
				<li>Evtl: beim Erstellen von Anl&auml;ssen soll im KCM Homepage Kalender ein neuer Eintrag erstellt werden</li>
			</ul>
			<h3>Version 5.0.6206:</h3>
			<ul>
				<li>Upgrade auf .NET Framework 4.6.1</li>
				<li>Startseite zur Mandantenwahl eingebaut</li>
				<li>Konsolidierung und Zusammenführung aller Mandanten</li>
				<li>Migration nach EventSite.ch</li>
				<li>Redesign mit Master Pages inkl. Burger-Menu für mobile Devices</li>
			</ul>
			<h3>Version 4.1.4464:</h3>
			<ul>
				<li>Neu wird beim Konfiguration laden erst versucht eine Mandanten spezifische Konfiguration zu laden.</li>
			</ul>
			<h3>Version 4.1.4461:</h3>
			<ul>
				<li>Bugfix: Bei einem fehlerhaften Kontakt konnten alle weiteren Benachrichtigungen nicht mehr versendent werden.</li>
				<li>Bugfix: Auf Kontakteinstellungen wurde Handy-Nummer überprüft, was überflüssig ist, und zu Fehlern führen kann.</li>
			</ul>
			<h3>Version 4.1.4442:</h3>
			<ul>
				<li>Bugfix: Die Legende des Mitfahrtgelegenheiten Managements wurde auch angezeigt, wenn diese ausgeschaltet war</li>
			</ul>
			<h3>Version 4.1.4384:</h3>
			<ul>
				<li>Bugfix: Die Version vom Common Assembly entspricht neu der des Web Assemblys</li>
				<li>Authentifizierungs-Einstellungen neu pro Mandant anstatt global</li>
				<li>Neu kann ein interner authentifizierungs Mechanismus verwendet werden</li>
			</ul>
			<h3>Version 4.0.3720:</h3>
			<ul>
				<li>Remotelogin Bug fixed (Application Root URL eingeführt)</li>
			</ul>
			<h3>Version 4.0.3348:</h3>
			<ul>
				<li>Remotelogin Bug fixed (Anmeldung aus KCM Mitgliederseiten funktionierte nicht mehr)</li>
			</ul>
			<h3>Version 4.0.3341:</h3>
			<ul>
				<li>Das Two-Way SMS Messaging kann per Konfiguration global ein- und ausgeschaltet werden</li>
				<li>Auf einem Kontakt kann in der Datenbank das Two-Way SMS Messaging ein-/ausgeschaltet werden</li>
			</ul>
			<h3>Version 4.0.3335:</h3>
			<ul>
				<li>Two-Way SMS Messaging eingebaut, damit per SMS angemeldet und konfiguriert werden kann</li>
				<li>Neuer Konfigurationsparameter unter "Meine Details": "SMS für Event Management", mitdem die Anlass / Anmelde Benachrichtigungs-SMS global über alle Kategorien ein- bzw. ausgeschaltet werden kann.</li>
				<li>Konfiguration in externes Config File ausgelagert</li>
				<li>Logging verbessert</li>
				<li>Viele diverse Verbesserungen / Refactoring</li>
			</ul>
			<h3>Version 3.5.3275:</h3>
			<ul>
				<li>Status kann nicht mehr von 'komme' nach 'komme nicht' geändert werden, wenn die Anzahl Anmeldungen zwischen den konfigurierten kritischen Werten liegt</li>
				<li>Benachrichtigungs Applikation &uuml;berarbeitet</li>
				<li>DB Access Layer &uuml;berarbeitet</li>
				<li>Kleine Text Anpassungen bei den SMS Benachrichtigungen</li>
			</ul>
			<h3>Version 3.5.3247:</h3>
			<ul>
				<li>Auf einer Eintragung kann neu Status, Kommentar und Zeit ge&auml;ndert werden</li>
				<li>Benachrichtigungen werden neu in einer externen Applikation abgearbeitet</li>
				<li>Benachrichtigung wenn kein SMS Kredit mehr vorhanden ist, wird nicht mehr mehrfach geschickt</li>
				<li>Authentication Cookie Handling verbessert</li>
				<li>Logging erweitert</li>
				<li>Viele weitere stabilit&auml;ts und performance Verbesserungen</li>
			</ul>
			<h3>Version 3.0.2991:</h3>
			<ul>
				<li>Layout &Auml;nderungen für .NET Framework 2.0</li>
				<li>Logging erweitert</li>
			</ul>
			<h3>Version 3.0.2942:</h3>
			<ul>
				<li>Vorbereitungen für Two-Way SMS Messaging eingebaut (EventSiteRemoting).</li>
				<li>BUGFIX: Fehler im BusinessLayer Dispose behoben.</li>
			</ul>
			<h3>Version 3.0.2859:</h3>
			<ul>
				<li>Performance Verbesserungen: Das Erstellen von Anl&auml;ssen bzw. Anmelden an Anl&auml;ssen dauert neu noch einen Bruchteil der Zeit wie zuvor.</li>
			</ul>
			<h3>Version 3.0.2825:</h3>
			<ul>
				<li>BUGFIX: Auf Anmeldungen welche per Email Link get&auml;tigt wurden, wurde die Uhrzeit nicht angezeigt</li>
			</ul>
			<h3>Version 3.0.2820:</h3>
			<ul>
				<li>In den Benachrichtigungs Emails wird neu ein direkt Link mitgeschickt, um sich mit einem Klick für einen Anlass anzumelden.</li>
			</ul>
			<h3>Version 3.0.2782:</h3>
			<ul>
				<li>Die Benutzer können eine Auto-SMS-Abonnierung pro Kategorie konfigurieren. D.h. neue Anlässe werden automatisch für eine gewisse Anzahl SMS abonniert</li>
				<li>BUGFIX: Bei Postbacks auf der Home Seite wurde der ausgewählte Status gelöscht (bsp. anderer Anlass auswählen, aktualisieren...)</li>
			</ul>
			<h3>Version 3.0.2741:</h3>
			<ul>
				<li>Beschriftung der Buttons für Anlass erstellen / kopieren</li>
				<li>BUGFIX: Die Benachrichtigungen beim Ändern von Anlässen wurden fälschlicherweise auch verschickt, wenn das ausgeschaltet war</li>
				<li>BUGFIX: Home Seite im Mozilla / Firefox: Design bei Fehler im Feature völlig verzerrt</li>
			</ul>
			<h3>Version 3.0.2734:</h3>
			<ul>
				<li>BUGFIX: Die Email Benachrichtigung für Anmeldungen funktionierte nicht</li>
				<li>BUGFIX: Die Uhrzeit von Anlässen wurde nicht gespeichert</li>
			</ul>
			<h3>Version 3.0.2732:</h3>
			<ul>
				<li>BUGFIX: Beim erstellen eines Anlasses trat ein Fehler auf. Als Folge funktionierte die Event Site nicht mehr</li>
				<li>In der Navigation wurde &quot;Neuer Anlass erstellen&quot; entfernt (war nicht klar in welcher Kategorie dieser erstellt werden sollte)</li>
			</ul>
			<h3>Version 3.0.2728:</h3>
			<ul>
				<li>BUGFIX: Wenn die Email Adresse eines anderen Kontaktes (nicht die des eigenen) geändert wurde, musste man sich fälschlicherweise neu anmelden</li>
				<li>BUGFIX: Wenn ein bestehender Anlass als Vorlage für einen neuen übernommen wird, wird die Beschreibung nun auch kopiert</li>
				<li>BUGFIX: Überprüfung für doppelte Einträge an einem Datum am selben Ort war fehlerhaft</li>
				<li>Neu: Anlass Kategorie - die Anlässe und Orte werden neu in Kategorien aufgeteilt</li>
				<li>Neu: Auf einem Kontakt können die Kategorien konfiguriert werden, für welche Benachrichtigungen gesendet werden sollen oder nicht</li>
				<li>Neu: Eine Anlass Kategorie kann Gratis oder Kostenpflichtig sein in Bezug auf SMS Benachrichtigung bei Anlass Erzeugung / Änderung</li>
				<li>Neu: Beim Ein-/Austragen einer Mitfahrt wird der Fahrer auch per SMS benachrichtigt, falls der Mitfahrer keine SMS Kredite besitzt</li> 
				<li>Neu: Farbliche Darstellung beim Speichern der Anlass-Abonnierung (gelb=speichern, grau=speichern abgeschlossen)</li>
				<li>Neu: E-Mail Benachrichtigung an die Benutzer, wenn der SMS Kredit zur Neige geht</li>
				<li>SMTP Server neu auch als SSL konfigurierbar</li>
				<li>Diverse Performance- und Architekturverbesserungen</li>
			</ul>			
			<h3>Version 2.3.2424:</h3>
			<ul>
				<li>Neuer Menu-Punkt "neuer Anlass" welcher auf die Seite "Anlässe bearbeiten" wechselt und dort einen neuen leeren Anlass erzeugt</li>
			</ul>			
			<h3>Version 2.3.2418:</h3>
			<ul>
				<li>BUGFIX: Das Mitfahrten-Management Auswahlmenu sollte nun immer funktionieren</li>
			</ul>			
			<h3>Version 2.3.2355:</h3>
			<ul>
				<li>BUGFIX: Wenn das Mitfahrgelegenheiten-Management ausgeschaltet ist, wird neu auch deren Legende ausgeblendet</li>
				<li>Neuer Konfigurationsparameter "SMS verwenden" implementiert</li>
				<li>Neuer Konfigurationsparameter "Anmeldungen verwenden" implementiert</li>
				<li>Neuer Konfigurationsparameter "Anlässe als Liste darstellen" implementiert</li>
				<li>Neuer Konfigurationsparameter "Web Adresse für Anlass verwenden" implementiert</li>
				<li>Neuer Konfigurationsparameter "Min / Max Anz. Anmeldungen für Anlass verwenden" implementiert</li>
				<li>Ausblenden aller Felder/Spalten/Controls in Zusammenhang mit Benachrichtigung, wenn alle Benachrichtigungsfunktionen ausgeschaltet sind</li>
			</ul>
			<h3>Version 2.3.2330:</h3>
			<ul>
				<li>Neu gibt es ein RemoteLogin. Dieses ermöglicht das direkte einloggen von aussen her mit der Überprüfung eines Hashs (siehe Mitgliederbereich auf der KCM Homepage).</li>
				<li>Neu gibt es einen "Exit" Link um das Fenster zu schliessen ohne sich abzumelden</li>
			</ul>
			<h3>Version 2.3.2323:</h3>
			<ul>
				<li>Wenn sich jemand für eine Mitfahrt einträgt, wird das SMS neu nicht mehr dem Fahrer sondern dem Mitfahrer belastet</li>
				<li>Wenn ein Fahrer seine Route ändert, wird das SMS neu nicht mehr dem Mitfahrer sondern dem Fahrer belastet (sofern schon Mitfahrer vorhanden sind)</li>
				<li>Eine Schnittstelle für Zusatzfunktionen pro Mandant (wie z.B. Pegelanzeige bei "KCM Trainings") wurde implementiert</li>
			</ul>
			<h3>Version 2.3.2315:</h3>
			<ul>
				<li>Das Übernehmen eines bestehenden Anlasses als Vorlage für neue wurde vereinfacht (Neue Knöpfe in der Anlass Bearbeitungsmaske)</li>
				<li>Nach dem Speichern eines Anlasses wird er automatisch auf der Homeseite angezeigt</li>
			</ul>
			<h3>Version 2.3.2314:</h3>
			<ul>
				<li>BUGFIX: Wenn ein Anlass kopiert wurde, wurde in den Mails der Anlass-Ersteller des original Anlasses angezeigt</li>
				<li>BUGFIX: Bei den vergangenen Anlässen wurde der Anlass-Ersteller teilweise nicht angezeigt</li>
				<li>Neue Funktion: Angemeldete Benutzer können anderen Benutzern SMS versenden</li>
				<li>Beim Kopieren eines Anlasses wird neu die Uhrzeit auch kopiert</li>
				<li>Neu wird auf den Anlass-Details beim Ort als Tooltip die Beschreibung angezeigt (auch im Email)</li>
				<li>Neu wird der Anlass Ersteller im Benachrichtigungs-Mail angezeigt</li>
				<li>Benachrichtigungstexte ein wenig angepasst</li>
			</ul>
			<h3>Version 2.3.2302:</h3>
			<ul>
				<li>BUGFIX: Wenn ein Benutzer auf einem Mandanten eingeloggt war, kann er alle anderen Mandanten ohne neu anzumelden aufrufen, auch wenn er nicht berechtigt war. Dies führte zu Fehlern</li>
				<li>BUGFIX: Die Startzeit in Benachrichtigungen für Anlass-Anmeldungen war immer die des Anlasses anstatt die vom Benutzer angegebene</li>
				<li>BUGFIX: Beim speichern einer Route bzw. einer Mitfahrt, wurde fälschlicherweise eine Benachrichtigung wegen Änderung der Anmeldung verschickt</li>
				<li>Neue Funktion: Benachrichtigungs-Abonnierung pro Anlass</li>
				<li>Neue Eigenschaft für Kontakte "Notifizierungsabonnierung": Wenn eingeschaltet, werden die Benutzer nur beim Erstellen bzw. Ändern von Anlässen benachrichtigt. Um weitere Notifizierungen zu erhalten, muss der Benutzer den Anlass abonnieren. Wenn ausgeschaltet, werden immer alle Benachrichtigungen verschickt.</li>
				<li>Anlass Ersteller wird neu in den Anlass Details angezeigt</li>
				<li>Die Handynummer wird auf dem Icon hinter den Namen angezeigt</li>
				<li>Anlässe werden neu bis um Mitternacht unter aktuelle Anlässe angezeigt</li>
				<li>Icons anstatt Links für 'Aktualisieren' und 'Vergangene Anlässe anzeigen'</li>
				<li>Anlass Details neu nicht mehr als Popup in neuem Fenster sondern in Hauptseite integriert</li>
				<li>Benutzer, welche für min. einen Mandanten registriert sind, werden neu beim Versuch sich an einem Mandanten einzuloggen, bei dem sie nicht registriert sind, entsprechend informiert</li>
				<li>Meldungen in Dialogboxen des Mitfahrten-Managements angepasst</li>
			</ul>
			<h3>Version 2.2.2273:</h3>
			<ul>
				<li>Berechtigungskonzept eingebaut: Es können Logindaten eines Fremdsystems angekoppelt werden<br>
				Ein angemeldeter Benutzer kann eine oder mehrere der folgenden Berechtigungsrollen haben:<br>
				<b>Reader</b>: Hat nur die Rechte um zu lesen (kann sich nicht für Anlässe eintragen und auch keine generieren)<br>
				<b>User</b>: Hat nur die Rechte um sich selbst für Anlässe einzutragen und um sich selbst in der Kontakt-Administration zu editieren (kann keine Anlässe generieren)<br>
				<b>EventCreator</b>: Hat nur die Rechte um Anlässe zu generieren und zu ändern (kann sich nicht für Anlässe eintragen)<br>
				<b>Manager</b>: Hat die Rechte sich und auch andere für Anlässe einzutragen sowie auch alle Accounts in der Kontakt-Administration zu bearbeiten<br>
				<b>Administrator</b>: Hat Zugriff auf alle Funktionen.<br>
				Die Rollen in denen der angemeldete Benutzer ist, werden unter "Über EventSite" aufgelistet.</li>
				<li>Wenn eine Route geändert wird, erscheint neu auch der Name des Fahrers im Benachrichtigungs-Mail bzw. -SMS</li>
				<li>Wenn ein neuer Anlass erstellt wird, wird der Anlass-Ersteller abgespeichert</li>
			</ul>
			<h3>Version 2.1.2260:</h3>
			<ul>
				<li>BUGFIX: Build Datum im "Über" war nicht korrekt</li>
				<li>BUGFIX: Mitfahrgelegenheiten Management bei vergangenen Anlässen ausblenden</li>
				<li>BUGFIX: Doppelte Einträge der Routenpunkte bei der Mitfahrdefinition --> Speicherknopf wird neu nach dem klicken deaktiviert</li>
				<li>BUGFIX: Position des Mitfahr-Kontext-Menus, wenn Scrollbar nicht ganz links ist, korrigiert</li>
				<li>BUGFIX: Teilweise entstanden Fehler, da der ViewState und somit die ganze Seite viel zu gross war --> ViewState verkleinert - Fehler hoffenlich weg</li>
				<li>Neue Versionsnummer-Struktur</li>
				<li>Verschiedene kleine Verbesserungen bei Speicherfunktionen</li>
				<li>Beim Löschen von Anmeldungen wird neu zuerst noch nachgefragt, ob man wirklich löschen will</li>
				<li>Performance wurde verbessert</li>
			</ul>
			<h3>Version 2.1.1:</h3>
			<ul>
				<li>BUGFIX: NullReferenceException beim Löschen von Anmeldungen (nicht klar reproduzierbar --> evtl. gelöst)</li>
				<li>BUGFIX: Überprüfung ob Ort ausgefüllt ist bei "neuer Anlass" / "Anlass bearbeiten" eingebaut</li>
				<li>BUGFIX: Berücksichtigung der Konfigurations-Einstellung ob Mitfahrgelegenheiten Management eingeschaltet ist</li>
				<li>BUGFIX: Position des Mitfahr-Kontext-Menus, wenn Scrollbar nicht zuoberst ist, korrigiert</li>
				<li>Neu ist konfigurierbar, ob bei Einträgen mit Status, welcher gelöscht werden kann (z.B. "komme nicht"-Einträge), benachrichtigt werden soll</li>
				<li>Beim Löschen einer Anmeldung Sicherheitsabfrage wenn Mitfahrt vorhanden</li>
			</ul>
			<h3>Version 2.1.0:</h3>
			<ul>
				<li>Mitfahrgelegenheiten Management eingebaut</li>
				<li>In der Kontaktadministration wird der persönliche SMS Kredit pro Kontakt angezeigt</li>
				<li>Neue Funktion: 'Aktualisieren' aktualisiert alle Einträge für den gewählten Anlass</li>
				<li>Bei Anlass-Auswahllisten wird neu auch das Datum angezeigt</li>
				<li>Unterschied zwischen Anlass bearbeiten und neuer Anlass erstellen wurde klarer gemacht</li>
			</ul>
			<h3>Version 2.0.4:</h3>
			<ul>
				<li>BUGFIX: Gross-Klein-Schreibung beim Entfernen von Email-Adressen wird f&auml;lschlicherweise ber&uuml;cksichtigt</li>
				<li>Neue Funktion: &quot;Anlass kopieren&quot;</li>
				<li>Anzeige wenn der totale SMS Kredit langsam zur Neige geht</li>
				<li>Zwei neue Felder zur Angabe ob bei Anlass-Änderung benachrichtigt werden soll</li>
				<li>Neu gibts die Möglichkeit, dass sich Personen aus den Email-Adressen für Benachrichtigungen auf dem Mandanten entfernen können (Link im Benachrichtigungs-Mail)</li>
			</ul>
			<h3>Version 2.0.3:</h3>
			<ul>
				<li>BUGFIX: Löschen von Kontakten im Edit-Modus funktioniert nicht</li>
				<li>BUGFIX: In Mozilla-Browsern (FireFox, Netscape...) funktioniert der Kalender zur Datums-Auswahl nicht</li>
				<li>BUGFIX: Schlechte Validierung im Datumsfeld unter &quot;Anl&auml;sse bearbeiten&quot; (wenn Feld leer ist schlägt Validierung fehl)</li>
				<li>BUGFIX: Zeilenumbrüche in der Eventbeschreibung darstellen</li>
				<li>Location in Event-Benachrichtigungs-Mail</li>
				<li>Anmelde-Status in Benachrichtigungs-SMS und -Email</li>
				<li>Web Adresse im Event Benachrichtigungs-Mail und Min- / Maxteilnehmer wenn 0 dann &quot;&quot;</li>
				<li>Vergangene Anl&auml;sse per Link einblendbar, aber keine &Auml;nderungen möglich</li>
				<li>Anzeige des Build Datums im &quot;&Uuml;ber Event Site&quot;</li>
			</ul>
			<h3>Version 2.0.2:</h3>
			<ul>
				<li>Auf der Startseite werden vergangene Anl&auml;sse herausgefiltert</li>
				<li>Beim Erstellen eines Kontaktes wird die Email-Adresse aus den Email-Adressen für Benachrichtigungen auf dem Mandanten entfernt</li>
			</ul>
			<h3>Version 2.0.1:</h3>
			<ul>
				<li>Mandantenfähig, d.h. für verschiedenste Event-Typen anwendbar</li>
				<li>Auf einem Mandanten können u.a. Email-Adressen für die Benachrichtigung konfiguriert werden</li>
				<li>Zuerst wird ein Event mit all seinen Parametern erstellt</li>
				<li>Mehrere neue Parameter für einen Event</li>
				<li>Auf die erstellten Anl&auml;sse können Anmeldungen erfolgen</li>
				<li>Beim Anmelden kann angegeben werden, wieviele SMS Benachrichtigungen für diesen Event noch geschickt werden sollen</li>
				<li>Benachrichtigungen bei neuen Anl&auml;ssen, beim Ändern von Anl&auml;ssen, bei neuen Anmeldungen, beim Ändern und beim Löschen von Anmeldungen (alles konfigurierbar pro Mandanten)</li>
				<li>Die Liste der Orte kann dynamisch angepasst werden.</li>
				<li>Gelöschte Kontakte können wiederhergestellt werden</li>
				<li>Es gibt verschiedene Anmelde-Stati pro Mandant</li>
				<li>Je nach Anmelde Status können Anmeldungen auf jeden Fall wieder gelöscht werden, oder nur bis zu einer konfigurierbaren Limite von Anmeldungen</li>
				<li>Das Empfangen von Email Benachrichtigungen kann pro Kontakt ausgeschaltet werden</li>
				<li>Auf den Kontakten ist ein SMS Kredit gespeichert</li>
				<li>Das Versenden von SMS Nachrichten wird pro Kontakt gelogged und falls der Kredit aufgebraucht ist, wird per E-Mail benachrichtigt</li>
			</ul>
			<h3>Version 1.1.0:</h3>
			<ul>
				<li>Eintragen von Trainings auch in der Zukunft</li>
				<li>Das Empfangen von SMS Benachrichtigungen kann pro Kontakt ausgeschaltet werden</li>
			</ul>
			<h3>Version 1.0.0:</h3>
			<ul>
				<li>Eintragen von Trainings am aktuellen Tag</li>
				<li>Fest bestimmte Liste mit Orten</li>
				<li>SMS Benachrichtigungen bei neuen Einträgen</li>
			</ul>
</asp:Content>