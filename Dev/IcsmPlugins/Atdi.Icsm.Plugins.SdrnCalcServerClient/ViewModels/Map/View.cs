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
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map.Adapters;
using MP = Atdi.WpfControls.EntityOrm.Controls;
using Atdi.WpfControls.EntityOrm.Controls;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.Gis;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.Map
{
    [ViewXaml("Map.xaml", WindowState = FormWindowState.Maximized)] //, Width = 530, Height = 490)]
    [ViewCaption("Calc Server Client: Creating layers")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        private readonly ITransformation _transformation;
        private long _projectId;

        private MapModel _currentMapCard;
        private MapInfoModel _currentMap;
        private IList _currentMaps;
        private MP.MapDrawingData _currentMapData;

        private IEventHandlerToken<Events.OnCreatedMap> _onCreatedMapToken;

        public MapDataAdapter Maps { get; set; }

        public ViewCommand MapAddCommand { get; set; }

        public View(MapDataAdapter mapDataAdapter,
                    IObjectReader objectReader,
                    ICommandDispatcher commandDispatcher,
                    ViewStarter starter,
                    IEventBus eventBus,
                    ILogger logger,
                    ITransformation transformation)
        {
            _objectReader = objectReader;
            _commandDispatcher = commandDispatcher;
            _starter = starter;
            _eventBus = eventBus;
            _logger = logger;
            _transformation = transformation;

            this.Maps = mapDataAdapter;
            this.CurrentMapCard = new MapModel();
            this.MapAddCommand = new ViewCommand(this.OnMapAddCommand);

            _onCreatedMapToken = _eventBus.Subscribe<Events.OnCreatedMap>(this.OnCreatedMapHandle);
            ReloadMaps();
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
        public MapInfoModel CurrentMap
        {
            get => this._currentMap;
            set
            {
                this._currentMap = value;

                if (value != null)
                {
                    var file = value.FileName.Split('.');
                    if (file.Length == 2)
                    {
                        var card = (MapModel)this._currentMapCard.Clone();
                        card.MapName = file[0];
                        CurrentMapCard = card;
                    }
                }
            }
        }
        public IList CurrentMaps
        {
            get => this._currentMaps;
            set
            {
                this._currentMaps = value;
                RedrawMap();
            }
        }
        public MP.MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        private void ReloadMaps()
        {
            this.Maps.Refresh();
            this.CurrentMap = null;
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
        private void RedrawMap()
        {
            try
            {
                var data = new MP.MapDrawingData();
                var points = new List<MP.MapDrawingDataPoint>();
                var polygons = new List<MP.MapDrawingDataPolygon>();

                if (this._currentMaps != null)
                {
                    foreach (MapInfoModel model in this._currentMaps)
                    {
                        var polygonPoints = new List<Location>();

                        var coordinateUpperLeft = this._transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = model.UpperLeftX, Y = model.UpperLeftY }, this._transformation.ConvertProjectionToCode(model.Projection));
                        var coordinateUpperRight = this._transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = model.UpperRightX, Y = model.UpperRightY }, this._transformation.ConvertProjectionToCode(model.Projection));
                        var coordinateLowerRight = this._transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = model.LowerRightX, Y = model.LowerRightY }, this._transformation.ConvertProjectionToCode(model.Projection));
                        var coordinateLowerLeft = this._transformation.ConvertCoordinateToWgs84(new EpsgCoordinate() { X = model.LowerLeftX, Y = model.LowerLeftY }, this._transformation.ConvertProjectionToCode(model.Projection));
                        polygonPoints.Add(new Location() { Lat = coordinateUpperLeft.Latitude, Lon = coordinateUpperLeft.Longitude });
                        polygonPoints.Add(new Location() { Lat = coordinateUpperRight.Latitude, Lon = coordinateUpperRight.Longitude });
                        polygonPoints.Add(new Location() { Lat = coordinateLowerRight.Latitude, Lon = coordinateLowerRight.Longitude });
                        polygonPoints.Add(new Location() { Lat = coordinateLowerLeft.Latitude, Lon = coordinateLowerLeft.Longitude });
                        polygons.Add(new MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
                    }
                }

                data.Polygons = polygons.ToArray();
                data.Points = points.ToArray();
                this.CurrentMapData = data;
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.CalcServerClient, e);
            }
        }
        public override void Dispose()
        {
            _onCreatedMapToken?.Dispose();
            _onCreatedMapToken = null;
        }
    }
}
