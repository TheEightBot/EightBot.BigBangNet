using System;
using System.Linq;
using System.Reactive;
using System.Runtime.Serialization;
using EightBot.BigBang.ViewModel;
using FluentValidation;
using ReactiveUI;
using System.Reactive.Disposables;
using ReactiveUI.Legacy;
using DynamicData;
using System.Collections.ObjectModel;
using EightBot.BigBang.Sample.Models;

namespace EightBot.BigBang.Sample.ViewModels
{
    [PreCache]
    public class ReactiveListViewPinnedViewModel : ViewModelBase
    {
        public ReactiveListViewPinnedViewModel()
        {
        }

        public override string Title => "Sample";

        SourceList<SampleModel> _sampleListSource;

        ReadOnlyObservableCollection<SampleModel> _sampleList;

        [DataMember]
        public ReadOnlyObservableCollection<SampleModel> SampleList
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

            _sampleListSource = new SourceList<SampleModel>();

            _sampleListSource.AddRange(
                Enumerable
                    .Range(0, 100)
                .Select(x =>
                {
                    return new SampleModel
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

