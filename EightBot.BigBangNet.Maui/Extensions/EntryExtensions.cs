using System;
using Xamarin.Forms;
using EightBot.BigBang.XamForms.AttachedProperties;

namespace EightBot.BigBang.XamForms
{
    public static class EntryExtensions
    {
        public static void SetNextControl(this Entry entry, VisualElement visualElement){
            EntryKeyboardReturnKeyTypeProperty.SetKeyboardReturnKeyType(entry, EntryKeyboardReturnType.Next);
            EntryKeyboardReturnKeyTypeProperty.SetNextVisualElement(entry, visualElement);
        }

        public static void ReturnKeyType(this Entry entry, EntryKeyboardReturnType returnType)
        {
            EntryKeyboardReturnKeyTypeProperty.SetKeyboardReturnKeyType(entry, returnType);
        }
    }
}
