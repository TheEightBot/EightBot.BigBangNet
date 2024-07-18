using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using EightBot.BigBang.ViewModel;
using EightBot.BigBang.Maui;
using EightBot.BigBang.Maui.Pages;
using FluentValidation;
using ReactiveUI;
using Microsoft.Maui;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    [PreCache]
    public class AddressEdit : EditModalBase<AddressViewModel>
    {
        private Grid cityStateZipGrid;
        private StackLayout mainStack;

        public AddressEdit(AddressViewModel viewModel)
            : base(viewModel)
        {
            ViewModel = viewModel;
        }

        public override string DeleteConfirmationMessage => $"Are you sure you want to delete the location?";

        public override string DeleteConfirmationTitle => "Delete Location?";

        protected override void BindControls()
        {
            base.BindControls();

            //this.Bind(ViewModel, x => x.Title, c => c.Title)
            //    .DisposeWith(ControlBindings);

            //this.WhenAnyObservable(page => page.IsAppearing)
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .SubscribeAsync(async _ =>
            //    {
            //        ViewModel.PageInitialized.ExecuteIfCan().FireAndForget();
            //    })
            //    .DisposeWith(ControlBindings);
        }

        protected override void SetupUserInterface()
        {
            base.SetupUserInterface();

            mainStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
            };

            cityStateZipGrid = new Grid
            {
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = GridLength.Auto },
                },
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                },
                ColumnSpacing = 0,
            };

            mainStack.Children.Add(cityStateZipGrid);

            PrimaryScrollView.Content = mainStack;
        }
    }

    public abstract class EditModalBase<TViewModel> : ContentPageBase<TViewModel>
            where TViewModel : EditModalViewModelBase<TViewModel>
    {
        private readonly Button deleteButton = new Button
        {
            Text = " Delete ",
            BackgroundColor = Colors.Transparent
        };

        private readonly Grid moreControlsContainer = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                },
        };

        private readonly ToolbarItem viewMore = new ToolbarItem { Text = "More" };

        private ToolbarItem cancelButton;
        private Label loadingLabel;
        private ToolbarItem saveButton;

        protected EditModalBase(TViewModel viewModel)
        {
            SetupToolbarOptions(viewModel);

            ViewModel = viewModel;
        }

        public abstract string DeleteConfirmationMessage { get; }

        public abstract string DeleteConfirmationTitle { get; }

        public ScrollView PrimaryScrollView { get; set; }

        public bool ShouldPopExtraNavigationPageOnDelete { get; set; }

        protected override void BindControls()
        {
            //this.OneWayBind(ViewModel, vm => vm.Title, c => c.Title)
            //    .DisposeWith(ControlBindings);

            //this.BindCommand(ViewModel, vm => vm.TrySave, page => page.saveButton)
            //    .DisposeWith(ControlBindings);

            //this.BindCommand(ViewModel, vm => vm.TryPromptToDelete, page => page.deleteButton)
            //    .DisposeWith(ControlBindings);

            this.BindCommand(ViewModel, vm => vm.TryCancel, page => page.cancelButton);

            this.WhenAnyObservable(page => page.ViewModel.TryCancel)
                .NavigatePopPopupPage()
                .DisposeWith(ControlBindings);

            //this.WhenAnyObservable(page => page.ViewModel.TryPromptToDelete)
            //   .ObserveOn(RxApp.MainThreadScheduler)
            //   .SubscribeAsync(async _ =>
            //   {
            //       var continueToDelete = await DisplayAlert(DeleteConfirmationTitle, DeleteConfirmationMessage, "Yes", "No").ConfigureAwait(true);
            //       if (continueToDelete)
            //       {
            //           var deleteSuccessfull = await ViewModel.TryDelete.ExecuteIfCan(null).ConfigureAwait(true);
            //           if (deleteSuccessfull)
            //           {
            //               await Navigation.PopModalAsync().ConfigureAwait(true);

            //               // CJVW: This does not seem possible from within here (Unless we pass in the stack and page that should be removed). The logic needs to be on the calling page.
            //               //       It should listen for this delete action and act accordingly
            //               if (ShouldPopExtraNavigationPageOnDelete)
            //               {
            //                   var stackCount = Navigation.NavigationStack.Count;
            //                   if (stackCount > 1)
            //                   {
            //                       await Navigation.PopAsync().ConfigureAwait(true);
            //                   }
            //               }
            //           }
            //       }
            //   })
            //   .DisposeWith(ControlBindings);

            //this.WhenAnyObservable(page => page.ViewModel.TrySave)
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(saveSuccessful =>
            //    {
            //        if (saveSuccessful)
            //        {
            //            _ = Navigation.PopModalAsync();
            //        }
            //    })
            //    .DisposeWith(ControlBindings);

            //Observable
            //    .CombineLatest(
            //        this.WhenAnyObservable(page => page.ViewModel.TrySave.IsExecuting).StartWith(false),
            //        this.WhenAnyObservable(page => page.ViewModel.TryDelete.IsExecuting).StartWith(false),
            //        (isSaving, isDeleting) => isSaving || isDeleting)
            //    .DistinctUntilChanged()
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Subscribe(isWorking =>
            //    {
            //    })
            //    .DisposeWith(ControlBindings);
        }

        protected override void SetupUserInterface()
        {
            var loadingGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                },
            };

            loadingLabel = new Label
            {
                Text = "Working...",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            loadingGrid.Children.Add(loadingLabel);
            Grid.SetColumn(loadingLabel, 1);

            cancelButton = new ToolbarItem
            {
                Text = "Cancel",
            };

            saveButton = new ToolbarItem
            {
                Text = "Save",
            };

            PrimaryScrollView = new ScrollView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            Content = PrimaryScrollView;
        }

        private void SetupToolbarOptions(TViewModel viewModel)
        {
            ToolbarItems.Add(cancelButton);

            if (viewModel.IsEditable)
            {
                ToolbarItems.Add(saveButton);
            }

            if (viewModel.IsDeletable)
            {
                moreControlsContainer.Add(deleteButton, 1, 0);
            }
        }
    }

    public class AddressViewModel : EditModalViewModelBase<AddressViewModel>
    {
        public AddressViewModel()
        {
        }

        public ReactiveCommand<object, Unit> PageInitialized { get; private set; }

        public override string Title => "Address";

        public override AbstractValidator<AddressViewModel> Validator => null;

        public override Task<bool> ExecuteTryDeleteAsync()
        {
            return Task.FromResult(true);
        }

        public override Task<bool> ExecuteTrySaveAsync()
        {
            return Task.FromResult(true);
        }

        protected override void RegisterObservables()
        {
            base.RegisterObservables();

            //PageInitialized = ReactiveCommand
            //    .CreateFromTask<object, Unit>(
            //        async _ =>
            //        {
            //            return await Task.FromResult(Unit.Default).ConfigureAwait(true);
            //        },
            //        this.WhenAnyObservable(vm => vm.PageInitialized.IsExecuting).ValueIsFalse().StartWith(true))
            //    .DisposeWith(ViewModelBindings);
        }
    }


    public abstract class EditModalViewModelBase<T> : ValidationViewModelBase<T>
        where T : class
    {
        private bool _isDeletable;
        private bool _isSaveable;

        private ReactiveCommand<object, Unit> _tryCancel;
        private ReactiveCommand<object, bool> _tryDelete;
        private ReactiveCommand<object, Unit> _tryPromptToDelete;
        private ReactiveCommand<object, bool> _trySave;

        protected EditModalViewModelBase()
        {
            IsDeletable = true;
            IsEditable = true;
        }

        public bool IsDeletable
        {
            get => _isDeletable;
            set => this.RaiseAndSetIfChanged(ref _isDeletable, value);
        }

        public bool IsEditable
        {
            get => _isSaveable;
            set => this.RaiseAndSetIfChanged(ref _isSaveable, value);
        }

        public ReactiveCommand<object, Unit> TryCancel
        {
            get => _tryCancel;
            set => this.RaiseAndSetIfChanged(ref _tryCancel, value);
        }

        public ReactiveCommand<object, bool> TryDelete
        {
            get => _tryDelete;
            set => this.RaiseAndSetIfChanged(ref _tryDelete, value);
        }

        public ReactiveCommand<object, Unit> TryPromptToDelete
        {
            get => _tryPromptToDelete;
            set => this.RaiseAndSetIfChanged(ref _tryPromptToDelete, value);
        }

        public ReactiveCommand<object, bool> TrySave
        {
            get => _trySave;
            set => this.RaiseAndSetIfChanged(ref _trySave, value);
        }

        public abstract Task<bool> ExecuteTryDeleteAsync();

        public abstract Task<bool> ExecuteTrySaveAsync();

        protected override void RegisterObservables()
        {
            TryCancel =
                ReactiveCommand
                    .CreateFromObservable<object, Unit>(_ => Observables.UnitDefault)
                    .DisposeWith(ViewModelBindings);

            //TryCancel
            //    .ThrownExceptions
            //    .Subscribe(
            //        x =>
            //        {
            //            System.Diagnostics.Debug.WriteLine(x);
            //        })
            //    .DisposeWith(ViewModelBindings);

            TrySave = ReactiveCommand.CreateFromTask<object, bool>(
                async _ =>
                {
                    return await ExecuteTrySaveAsync().ConfigureAwait(true);
                });

            //TryPromptToDelete = ReactiveCommand.CreateFromTask<object, Unit>(
            //    _ => Task.FromResult(Unit.Default),
            //    this.WhenAnyObservable(vm => vm.TryPromptToDelete.IsExecuting).ValueIsFalse().StartWith(true)).DisposeWith(ViewModelBindings);

            //TryDelete = ReactiveCommand.CreateFromTask<object, bool>(
            //   async _ =>
            //   {
            //       return await ExecuteTryDeleteAsync().ConfigureAwait(true);
            //   },
            //   this.WhenAnyObservable(vm => vm.TryPromptToDelete.IsExecuting).CombineLatest(this.WhenAnyObservable(vm => vm.TryDelete.IsExecuting), (bool1, bool2) => !(bool1 || bool2)))
            //   .DisposeWith(ViewModelBindings);
        }
    }
}
