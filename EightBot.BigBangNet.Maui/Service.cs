using System;
using EightBot.BigBang.XamForms.Interfaces;
using Xamarin.Forms;
namespace EightBot.BigBang.XamForms
{
    public static class Service
    {
        static IRendererResolver _rendererResolver;
        
        public static IRendererResolver RendererResolver
        {
            get {
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
