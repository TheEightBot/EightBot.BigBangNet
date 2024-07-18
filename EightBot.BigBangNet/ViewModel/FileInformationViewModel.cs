using System;
using System.Runtime.Serialization;
using ReactiveUI;
using System.IO;
using ReactiveUI.Fody.Helpers;

namespace EightBot.BigBang.ViewModel
{
    public class FileInformationViewModel : ViewModelBase
    {
        public enum StatusState
        {
            Unknown = 0,
            Successful = 1,
            FileNotFound = 2,
            FileEmpty = 3,
            NoFileSelected = 4,
            FileTooBig = 5,
            OtherException = 999
        }


        [Reactive]
        public string FileName { get; set; }

        [Reactive]
        public byte[] Data { get; set; }

        [Reactive]
        public long FileLength { get; set; }

        [Reactive]
        public Stream StreamData { get; set; }

        [Reactive]
        public StatusState Status { get; set; }

        protected override void RegisterObservables()
        {
        }
    }
}
