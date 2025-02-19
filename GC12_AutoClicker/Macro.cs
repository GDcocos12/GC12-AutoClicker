using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace GC12_AutoClicker
{
    public class Macro
    {
        public string Name { get; set; }
        public ObservableCollection<MacroAction> Actions { get; set; } = new ObservableCollection<MacroAction>();
    }

    public class MacroAction
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int ClickDuration { get; set; }
        public int Delay { get; set; }
        public int Repetitions { get; set; } = 1;
    }
}
