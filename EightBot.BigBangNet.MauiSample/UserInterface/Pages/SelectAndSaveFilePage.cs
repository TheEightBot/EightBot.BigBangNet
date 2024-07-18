using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using EightBot.BigBang.Interfaces;
using EightBot.BigBang.ViewModel;
using EightBot.BigBang.XamForms.Pages;
using FluentValidation;
using ReactiveUI;
using Splat;
using Xamarin.Forms;
using System.Reactive.Disposables;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class SelectAndSaveFilePage : ContentPageBase<SelectAndSaveFilePage.SelectAndSaveFile>, IActivatableView
    {
        Label _fileInfo;

        public SelectAndSaveFilePage()
        {
            ViewModel = new SelectAndSaveFile();
        }

        protected override void BindControls()
        {
            this.OneWayBind(ViewModel, vm => vm.FileInfo, ui => ui._fileInfo.Text)
                .DisposeWith(ControlBindings);

            this.WhenAnyObservable(x => x.Activated)
                .Take(1)
                .Delay(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .InvokeCommand(this, x => x.ViewModel.TrySelectFile)
                .DisposeWith(ControlBindings);
        }

        protected override void SetupUserInterface()
        {
            _fileInfo = new Label { 
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            this.Content = _fileInfo;
        }

        public class SelectAndSaveFile : ViewModelBase
        {
            public override string Title => null;

            ReactiveCommand<Unit, Unit> _trySelectFile;

            [DataMember]
            public ReactiveCommand<Unit, Unit> TrySelectFile
            {
                get { return _trySelectFile; }
                set { this.RaiseAndSetIfChanged(ref _trySelectFile, value); }
            }

            string _fileInfo;

            [DataMember]
            public string FileInfo
            {
                get { return _fileInfo; }
                set { this.RaiseAndSetIfChanged(ref _fileInfo, value); }
            }

            protected override void RegisterObservables()
            {
                TrySelectFile = 
                    ReactiveCommand
                        .CreateFromTask(async _ =>
                        {
                            var fileSelection = Locator.Current.GetService<IFileSelection>();

                            var selectedFile = await fileSelection.SelectFileAsync();

                            var saveResult = await fileSelection.SaveFileAsync(selectedFile.FileName, selectedFile.Data);

                            FileInfo = $"File Name: {selectedFile.FileName}{Environment.NewLine}File Length: {selectedFile?.Data?.Length ?? 0}{Environment.NewLine}Save Result: {saveResult}";
                        })
                        .DisposeWith(ViewModelBindings);
            }
        }
    }
}
