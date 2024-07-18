using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;

namespace EightBot.BigBang.ViewModel
{
    public class GroupedViewModelBase<TViewModel> : ViewModelBase
        where TViewModel : ViewModelBase
    {
        [Reactive]
        public IList<TViewModel> Items { get; set; }

        [Reactive]
        public string GroupTitle { get; set; }

        protected ObservableAsPropertyHelper<string> _title;
        public override string Title { get { return _title.Value; } }

        protected override void Initialize()
        {
            base.Initialize();

            Items = Items ?? new List<TViewModel>();
        }

        protected override void RegisterObservables()
        {
            this.WhenAnyValue(vm => vm.GroupTitle)
                .ToProperty(this, vm => vm.Title, out _title);
        }
    }
}
