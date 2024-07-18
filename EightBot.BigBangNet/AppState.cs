using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace EightBot.BigBang
{
    [DataContract]
    public class AppState
    {
        [DataMember]
        public Guid SavedGuid { get; set; }

        public AppState()
        {
            SavedGuid = Guid.NewGuid();
        }
    }
}
