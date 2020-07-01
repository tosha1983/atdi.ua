using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using VM = Atdi.Icsm.Plugins.GE06Calc.ViewModels;
//using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries;
//using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Adapters;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Data;
using System.Windows;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult
{
    [ViewXaml("GE06TaskResult.xaml")]
    [ViewCaption("GE06: Task result")]
    public class View : ViewBase
    {
        public override void Dispose()
        {
        }
    }
}
