using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms
{
    public abstract class StyleBase : ReactiveObject
    {
        protected Application CurrentApplication;

        private readonly ConcurrentDictionary<string, Style> _cachedStyles = new ConcurrentDictionary<string, Style>();

        public void ApplyStylesToApplication(Application app = null)
        {
            CurrentApplication = app ?? Application.Current;

            if (CurrentApplication.Resources == null)
            {
                CurrentApplication.Resources = new ResourceDictionary();
            }

            RegisterStyles(CurrentApplication);
        }

        protected abstract void RegisterStyles(Application app);

        protected Style GetStyle(Func<Style> styleCreator, [CallerMemberName]string name = null)
        {
            if (name == null)
            {
                return default(Style);
            }

            if (!_cachedStyles.ContainsKey(name))
            {
                _cachedStyles[name] = styleCreator.Invoke();
            }

            return _cachedStyles[name];
        }
    }
}
