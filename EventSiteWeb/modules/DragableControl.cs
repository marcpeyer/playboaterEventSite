using System;
using System.Web.UI;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web.modules
{
	/// <summary>
	/// Summary description for DragableControl.
	/// </summary>
	public class DragableControl : UserControl
	{
		protected override void OnLoad(EventArgs e)
		{
			((PageBase)base.Page).RegisterStartupScriptIfNeeded("DragableControlScripts", string.Format(pbHelpers.JavaScriptString, GetDragMoveScript()));

			base.OnLoad (e);
		}

		private string GetDragMoveScript()
		{
			string script = @"
	var dragApproved = false;
	var dragElemCssClass = 'dragableBox';
	var dragElem;
	var mouseX, mouseY;
	var tempDragElemLeft, tempDragElemTop;

	function dragSet(e)
	{
		try
		{
			var eventSource = is.ie ? event.srcElement : e.target;
			
			var topElement = is.ie ? 'BODY' : 'html';
			if(eventSource.tagName.toUpperCase() != 'TEXTAREA' && eventSource.tagName != 'A')
			{
				while (eventSource.tagName != topElement && eventSource.className.substring(0, dragElemCssClass.length) != dragElemCssClass)
				{
					eventSource = is.ie ? eventSource.parentElement : eventSource.parentNode;
				}
				
				if(eventSource.className.substring(0, dragElemCssClass.length) == dragElemCssClass)
				{
					dragApproved = true;
					dragElem = eventSource;
					tempDragElemLeft = parseInt(dragElem.style.left + 0);
					tempDragElemTop = parseInt(dragElem.style.top + 0);
					mouseX = is.ie ? event.clientX : e.clientX;
					mouseY = is.ie ? event.clientY : e.clientY;
					document.onmousemove = dragMove;
					return false;
				}
			}
		}
		catch(e)
		{}
	}

	function dragMove(e)
	{
		if(dragApproved)
		{
			dragElem.style.left = is.ie ? tempDragElemLeft + event.clientX - mouseX + 'px' : tempDragElemLeft + e.clientX - mouseX + 'px';
			dragElem.style.top = is.ie ? tempDragElemTop + event.clientY - mouseY + 'px' : tempDragElemTop + e.clientY - mouseY + 'px';
			return false;
		}
	}

	document.onmousedown = dragSet;
	document.onmouseup = new Function('dragApproved = false');";
			return script;
		}
	}
}
