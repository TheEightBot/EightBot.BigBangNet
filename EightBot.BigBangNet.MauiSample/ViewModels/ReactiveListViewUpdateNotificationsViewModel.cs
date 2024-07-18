using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DynamicData;
using EightBot.BigBang.ViewModel;
using FluentValidation;
using ReactiveUI;
using ReactiveUI.Legacy;

namespace EightBot.BigBang.Sample.ViewModels
{
    public class ReactiveListViewUpdateNotificationsViewModel : ViewModelBase
    {
        public ReactiveListViewUpdateNotificationsViewModel()
        {
        }

        public override string Title => "ListView Notifications";

        SerialDisposable _processingDisposable = new SerialDisposable();

        SourceList<SampleViewModel> _sampleListSource;

        ReadOnlyObservableCollection<SampleViewModel> _sampleList;

        [DataMember]
        public ReadOnlyObservableCollection<SampleViewModel> SampleList
        {
            get { return _sampleList; }
            private set { this.RaiseAndSetIfChanged(ref _sampleList, value); }
        }

        ReactiveCommand<Unit, Unit> _startMonitoring;

        [DataMember]
        public ReactiveCommand<Unit, Unit> StartMonitoring
        {
            get { return _startMonitoring; }
            set { this.RaiseAndSetIfChanged(ref _startMonitoring, value); }
        }

        ReactiveCommand<Unit, Unit> _stopMonitoring;

        [DataMember]
        public ReactiveCommand<Unit, Unit> StopMonitoring
        {
            get { return _stopMonitoring; }
            set { this.RaiseAndSetIfChanged(ref _stopMonitoring, value); }
        }

        protected override void Initialize()
        {
            base.Initialize();

            _sampleListSource = new SourceList<SampleViewModel>();

            _sampleListSource.AddRange(
                Enumerable
                    .Range(0, 3)
                .Select(x => {
                    return new SampleViewModel {
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

            StartMonitoring = ReactiveCommand
			    .Create(() =>
			    {
                    var rngesus = new Random(Guid.NewGuid().GetHashCode());

                    Observable
                        .Generate(
                            string.Empty,
                            x => true,
                            x => $"Generated_{Guid.NewGuid().ToString("N")}",
                            x => x,
                            x => TimeSpan.FromMilliseconds(rngesus.Next(1, 1000)))
                        .Select(x => {
                            return new SampleViewModel
                            {
                                StringProperty = x
                            };
                        })
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(x => {
                            _sampleListSource.Add(x);
                        })
                        .DisposeWith(_processingDisposable);
                })
			    .DisposeWith(ViewModelBindings);

            StopMonitoring = ReactiveCommand
                .Create(() =>
                {
                    _processingDisposable?.Dispose();
                })
                .DisposeWith(ViewModelBindings);
        }
    }
}

