using System;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using EightBot.BigBang.ViewModel;
using FluentValidation;
using ReactiveUI;
using System.Reactive.Disposables;
using DynamicData;
using System.Collections.ObjectModel;

namespace EightBot.BigBang.Sample.ViewModels
{
    public class ReactiveListViewContentHooksViewModel : ViewModelBase
    {
        public ReactiveListViewContentHooksViewModel()
        {
        }

        public override string Title => "Sample";

        SourceList<SampleViewModel> _sampleListSource;

        ReadOnlyObservableCollection<SampleViewModel> _sampleList;

        [DataMember]
        public ReadOnlyObservableCollection<SampleViewModel> SampleList
        {
            get { return _sampleList; }
            private set { this.RaiseAndSetIfChanged(ref _sampleList, value); }
        }

        ReactiveCommand<Unit, Unit> _selectionUnitSample;

        [DataMember]
        public ReactiveCommand<Unit, Unit> SelectionUnitSample
        {
            get { return _selectionUnitSample; }
            set { this.RaiseAndSetIfChanged(ref _selectionUnitSample, value); }
        }

        protected override void Initialize()
        {
            base.Initialize();

            _sampleListSource = new SourceList<SampleViewModel>();

            _sampleListSource.AddRange(
                Enumerable
                    .Range(0, 100)
                .Select(x =>
                {
                    return new SampleViewModel
                    {
                        StringProperty = Guid.NewGuid().ToString("N")
                    };
                })
            );

            _sampleListSource
                .Connect()
                .Bind(out var sampleList)
                .Subscribe();

            SampleList = sampleList;
        }

        protected override void RegisterObservables()
        {
            SelectionUnitSample = ReactiveCommand
                .Create(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Selection");

                })
                .DisposeWith(ViewModelBindings);
        }
    }
}

