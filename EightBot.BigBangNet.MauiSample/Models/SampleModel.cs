using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EightBot.BigBang.Sample.Models
{
    public class SampleModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private string _stringProperty;
        public string StringProperty
        {
            get { return _stringProperty; }
            set { _stringProperty = value; OnPropertyChanged(); }
        }

        private int _intProperty;
        public int IntProperty
        {
            get { return _intProperty; }
            set { _intProperty = value; OnPropertyChanged(); }
        }

        private bool _boolProperty;
        public bool BoolProperty
        {
            get { return _boolProperty; }
            set { _boolProperty = value; OnPropertyChanged(); }
        }

        public SampleModel()
        {
        }
    }
}
