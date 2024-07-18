using System;
using EightBot.BigBang.Maui.Interfaces;
using Microsoft.Maui;
namespace EightBot.BigBang.Maui
{
    public static class Service
    {
        static IRendererResolver _rendererResolver;

        public static IRendererResolver RendererResolver
        {
            get
            {
                if (_rendererResolver != null)
                    return _rendererResolver;

                _rendererResolver = DependencyService.Get<Interfaces.IRendererResolver>();

                if (_rendererResolver == null)
                    throw new NullReferenceException("The renderer resolver was not initialized properly");

                return _rendererResolver;
            }
        }
    }
}
