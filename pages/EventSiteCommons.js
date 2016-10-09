
function showEventSiteModalWindow(url, windowName, height, width, callbackIdToClick)
{
	if(is.ie)
	{
		window.showModalDialog(url, '', 'dialogHeight:' + height + 'px; dialogWidth:' + width + 'px;');
		if(callbackIdToClick != null)
		{
			document.getElementById(callbackIdToClick).click();
		}
	}
	else
	{
		window.open(url, windowName, 'height=' + height + ',width=' + width + ',toolbar=no,menubar=no,location=no,dependent=yes');
	}
}
