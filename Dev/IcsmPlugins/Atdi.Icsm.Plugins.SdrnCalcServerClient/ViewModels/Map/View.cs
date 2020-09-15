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
using System.Windows.Forms;
using Atdi.DataModels.Sdrn.CalcServer.Entities;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map
{
    [ViewXaml("Map.xaml", WindowState = FormWindowState.Normal, Width = 530, Height = 490)]
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

            _onCreatedMapToken = _eventBus.Subscribe<Events.OnCreatedMap>(this.OnCreatedMapHandle);
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
                    OwnerUpperLeftY = CurrentMapCard.OwnerUpperLeftY.GetValueOrDefault()
                };

                _commandDispatcher.Send(mapModifier);
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        private void OnCreatedMapHandle(Events.OnCreatedMap data)
        {
            var mapModifier = new Modifiers.ChangeStateMap
            {
                Id = data.MapId,
                StatusCode = (byte)ProjectMapStatusCode.Pending
            };

            _commandDispatcher.Send(mapModifier);
            _starter.Stop(this);
        }
        public override void Dispose()
        {
            _onCreatedMapToken?.Dispose();
            _onCreatedMapToken = null;
        }
    }
}
