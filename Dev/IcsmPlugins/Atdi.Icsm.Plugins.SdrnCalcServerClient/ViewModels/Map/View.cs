using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Windows;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map
{
    [ViewXaml("Map.xaml")]
    [ViewCaption("Calc Server Client: Creating layers")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        private long _projectId;

        private MapModel _currentMapCard;

        private IEventHandlerToken<Events.OnCreatedMap> _onCreatedMapToken;

        public ViewCommand MapAddCommand { get; set; }

        public View(IObjectReader objectReader,
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

            this.CurrentMapCard = new MapModel();
            this.MapAddCommand = new ViewCommand(this.OnMapAddCommand);
        }
        public long ProjectId
        {
            get => this._projectId;
            set => this.Set(ref this._projectId, value);
        }
        public MapModel CurrentMapCard
        {
            get => this._currentMapCard;
            set => this.Set(ref this._currentMapCard, value);
        }
        private void OnMapAddCommand(object parameter)
        {
            try
            {
                _onCreatedMapToken = _eventBus.Subscribe<Events.OnCreatedMap>(this.OnCreatedMapHandle);

                var mapModifier = new Modifiers.CreateMap
                {
                    ProjectId = this.ProjectId,
                    MapName = CurrentMapCard.MapName,
                    MapNote = CurrentMapCard.MapNote,
                    StepUnit = "M",
                    OwnerId = Guid.NewGuid(),
                    OwnerAxisXNumber = CurrentMapCard.OwnerAxisXNumber.GetValueOrDefault(),
                    OwnerAxisXStep = CurrentMapCard.OwnerAxisXStep.GetValueOrDefault(),
                    OwnerAxisYNumber = CurrentMapCard.OwnerAxisYNumber.GetValueOrDefault(),
                    OwnerAxisYStep = CurrentMapCard.OwnerAxisYStep.GetValueOrDefault(),
                    OwnerUpperLeftX = CurrentMapCard.OwnerUpperLeftX.GetValueOrDefault(),
                    OwnerUpperLeftY = CurrentMapCard.OwnerUpperLeftX.GetValueOrDefault()
                };

                _commandDispatcher.Send(mapModifier);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnCreatedMapHandle(Events.OnCreatedMap data)
        {
            
        }
        public override void Dispose()
        {
            _onCreatedMapToken?.Dispose();
            _onCreatedMapToken = null;
        }
    }
}
