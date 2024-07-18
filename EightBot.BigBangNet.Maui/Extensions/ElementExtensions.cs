using System;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms
{
	public static class ElementExtensions
	{
        public static Page ParentPage(this Element element) {
            if (element == null || element.Parent == null)
                return null;

            var hasParent = false;

            var parentElement = element.Parent;
            
            while (hasParent == false) {
                if (parentElement.Parent == null)
                    break;

                hasParent = parentElement is Page;

                if (!hasParent)
                    parentElement = parentElement.Parent;
            }

            return hasParent && parentElement != null ? parentElement as Page : null;
        }
	}
}

