using GetStoreApp.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetStoreApp.Models
{
    public class StatusBarState
    {
        public StateImageMode StateImageMode { get; set; }

        public string StateInfoText { get; set; }

        public bool StatePrRingVisValue { get; set; }

        public bool StatePrRingActValue { get; set; }
    }
}
