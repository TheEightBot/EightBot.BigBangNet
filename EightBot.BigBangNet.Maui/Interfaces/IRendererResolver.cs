using System;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui.Interfaces
{
    public interface IRendererResolver
    {
        object GetRenderer(VisualElement element);

        bool HasRenderer(VisualElement element);

        object GetCellRenderer(BindableObject element);

        bool HasCellRenderer(BindableObject element);
    }
}

