using System;
using Microsoft.Maui;
using EightBot.BigBang.Maui.AttachedProperties;

namespace EightBot.BigBang.Maui
{
    public static class EntryExtensions
    {
        public static void SetNextControl(this Entry entry, VisualElement visualElement)
        {
            EntryKeyboardReturnKeyTypeProperty.SetKeyboardReturnKeyType(entry, EntryKeyboardReturnType.Next);
            EntryKeyboardReturnKeyTypeProperty.SetNextVisualElement(entry, visualElement);
        }

        public static void ReturnKeyType(this Entry entry, EntryKeyboardReturnType returnType)
        {
            EntryKeyboardReturnKeyTypeProperty.SetKeyboardReturnKeyType(entry, returnType);
        }
    }
}
