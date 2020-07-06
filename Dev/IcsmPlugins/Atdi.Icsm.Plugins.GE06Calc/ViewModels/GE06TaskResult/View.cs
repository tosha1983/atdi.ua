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
//using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult.Queries;
using Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult.Adapters;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Data;
using System.Windows;
using ICSM;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using Atdi.DataModels.Sdrn.CalcServer.Entities.Tasks;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06TaskResult
{
    [ViewXaml("GE06TaskResult.xaml")]
    [ViewCaption("GE06: Task result")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private long _resultId;

        public AllotmentOrAssignmentDataAdapter AllotmentOrAssignments { get; set; }
        public ContourDataAdapter Contours { get; set; }
        public AffectedADMDataAdapter AffectedADMs { get; set; }
        
        public View(
            AllotmentOrAssignmentDataAdapter allotmentOrAssignmentDataAdapter,
            ContourDataAdapter contourDataAdapter,
            AffectedADMDataAdapter affectedADMDataAdapter,
            IObjectReader objectReader,
            ICommandDispatcher commandDispatcher,
            ViewStarter starter,
            IEventBus eventBus,
            ILogger logger)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;

            this.AllotmentOrAssignments = allotmentOrAssignmentDataAdapter;
            this.Contours = contourDataAdapter;
            this.AffectedADMs = affectedADMDataAdapter;
        }

        public long ResultId
        {
            get => this._resultId;
            set => this.Set(ref this._resultId, value, () => { this.OnChangedResultId(value); });
        }
        private void OnChangedResultId(long resultId)
        {
            this.AllotmentOrAssignments.ResultId = resultId;
            this.AllotmentOrAssignments.Refresh();
            this.Contours.ResultId = resultId;
            this.Contours.Refresh();
            this.AffectedADMs.ResultId = resultId;
            this.AffectedADMs.Refresh();
        }
        public override void Dispose()
        {
        }
    }
}
