using System;
using System.Collections.Generic;
using Xamarin.Forms;
using EightBot.BigBang.XamForms;
using EightBot.BigBang.XamForms.Pages;
using EightBot.BigBang.ViewModel;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using ReactiveUI.Fody.Helpers;
using System.Linq;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public enum ItemsTestEnum
    {
        One,
        Two,
        Three,
    }

    [PreCache]
    public class PickersTest : ContentPageBase<PickersTestVM>
    {


        public PickersTest()
        {
            this.ViewModel = new PickersTestVM();
        }

        Picker _firstPicker;

        Picker _secondPicker;

        Picker _thirdPicker;

        Button _clearSelectedItem;

        protected override void SetupUserInterface()
        {
            var stackLayout = new StackLayout { };
        
            _firstPicker = new Picker();

            stackLayout.Children.Add(_firstPicker);

            _secondPicker = new Picker();

            stackLayout.Children.Add(_secondPicker);

            _thirdPicker = new Picker();

            stackLayout.Children.Add(_thirdPicker);

            _clearSelectedItem =
                new Button
                {
                    Text = "Clear Selections",
                };

            stackLayout.Children.Add(_clearSelectedItem);

            this.Content = stackLayout;            
        }

        protected override void BindControls()
        {
            _firstPicker
                .Bind(
                    this.WhenAnyValue(x => x.ViewModel.PickerObjects),
                    x =>
                    {
                        this.ViewModel.SelectedPickerObject = x;
                        System.Diagnostics.Debug.WriteLine($"Selected: {x?.PropertyOne}");
                    },
                    x =>
                    {
                        return this.ViewModel?.SelectedPickerObject?.PropertyOne == x.PropertyOne;
                    },
                    x => x.PropertyOne,
                    this.WhenAnyValue(x => x.ViewModel.SelectedPickerObject))
                .DisposeWith(ControlBindings);

            _secondPicker
                .Bind(
                    this.WhenAnyValue(x => x.ViewModel.Categories),
                    x => ViewModel.SelectedCategory = x?.Id ?? 0,
                    x => x.Id == ViewModel.SelectedCategory,
                    x => x.Name)
                .DisposeWith(ControlBindings);

            _thirdPicker
                .Bind(
                    this.WhenAnyValue(x => x.ViewModel.TestItems),
                    x =>
                    {
                        ViewModel.SelectedTestItem = x;
                    },
                    x => x == ViewModel.SelectedTestItem,
                    x => x.ToString())
                .DisposeWith(ControlBindings);
        }

        protected override void OnAppearing()
        {
            _clearSelectedItem.Clicked -= ClearSelectedItem_Clicked;
            _clearSelectedItem.Clicked += ClearSelectedItem_Clicked;

            //base.OnAppearing();

            //await Task.Delay(5000);

            //this.ViewModel.PickerObjects =
            //    new ObservableCollection<PickerTestObject>(
            //        new[]
            //        {
            //            new PickerTestObject{ PropertyOne = "Test a" },
            //            new PickerTestObject{ PropertyOne = "Test b" },
            //            new PickerTestObject{ PropertyOne = "Test c" }
            //        });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _clearSelectedItem.Clicked -= ClearSelectedItem_Clicked;
        }

        private void ClearSelectedItem_Clicked(object sender, EventArgs e)
        {
            this.ViewModel.SelectedCategory = 0;
            this.ViewModel.SelectedPickerObject = null;
        }
    }
    
    public class PickersTestVM : ViewModelBase
    {
        ObservableCollection<PickerTestObject> _pickerObjects;

        public ObservableCollection<PickerTestObject> PickerObjects
        {
            get => _pickerObjects;
            set => this.RaiseAndSetIfChanged(ref _pickerObjects, value);
        }

        [Reactive]
        public PickerTestObject SelectedPickerObject { get; set; }

        [Reactive]
        public int SelectedCategory { get; set; }

        [Reactive]
        public IEnumerable<ItemsTestEnum> TestItems { get; set; }

        [Reactive]
        public ItemsTestEnum SelectedTestItem { get; set; }

        [Reactive]
        public IEnumerable<CategoryModel> Categories { get; set; }

        PickerTestObject _lateAddObject;

        protected override void Initialize()
        {
            base.Initialize();

            _lateAddObject = new PickerTestObject { PropertyOne = "Test 4" };

            SelectedPickerObject = _lateAddObject;

            PickerObjects = 
                new ObservableCollection<PickerTestObject>();

            var categories =
                new[]
                {
                    new CategoryModel { Id = 1, ExternalId = "100", IsProduceCategory = true, Name = "Name 1" },
                    new CategoryModel { Id = 2, ExternalId = "200", IsProduceCategory = true, Name = "Name Bad" },
                    new CategoryModel { Id = 3, ExternalId = "300", IsProduceCategory = true, Name = "Name Bad" },
                    new CategoryModel { Id = 4, ExternalId = "400", IsProduceCategory = true, Name = "Name 4" },
                };

            SelectedCategory = categories.FirstOrDefault().Id;

            Categories = categories;

            SelectedTestItem = ItemsTestEnum.One;

            TestItems = Enum.GetValues(typeof(ItemsTestEnum)).Cast<ItemsTestEnum>().ToList();
        }

        protected override void RegisterObservables()
        {
            Observable
                .Interval(TimeSpan.FromMilliseconds(50))
                .Take(50)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(i => PickerObjects.Add(new PickerTestObject { PropertyOne = $"Test {i + 10}" }))
                .Subscribe()
                .DisposeWith(ViewModelBindings);

            Observable
                .Interval(TimeSpan.FromMilliseconds(500))
                .Take(1)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(i => PickerObjects.Add(_lateAddObject))
                .Subscribe()
                .DisposeWith(ViewModelBindings);

            this.WhenAnyValue(x => x.SelectedCategory)
                .Do(x => System.Diagnostics.Debug.WriteLine($"Selected: {x}"))
                .Subscribe()
                .DisposeWith(ViewModelBindings);
        }
    }
    
    public class PickerTestObject
    {
        public string PropertyOne { get; set; }
        
        public string PropertyTwo { get; set; }
    }

    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
        public bool IsProduceCategory { get; set; }
    }

}
