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
using Atdi.WpfControls.EntityOrm.Controls;
using Atdi.Icsm.Plugins.GE06Calc.Environment;
using FRM = System.Windows.Forms;
using System.IO;
using System.Globalization;

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
        private IList _currentAllotmentOrAssignments;
        private IList _currentContours;
        private MapDrawingData _currentMapData;

        public AllotmentOrAssignmentDataAdapter AllotmentOrAssignments { get; set; }
        public ContourDataAdapter Contours { get; set; }
        public AffectedADMDataAdapter AffectedADMs { get; set; }
        public ViewCommand ExportToHTZCommand { get; set; }

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

            this.ExportToHTZCommand = new ViewCommand(this.OnExportToHTZCommand);
            this.AllotmentOrAssignments = allotmentOrAssignmentDataAdapter;
            this.Contours = contourDataAdapter;
            this.AffectedADMs = affectedADMDataAdapter;
        }

        public long ResultId
        {
            get => this._resultId;
            set => this.Set(ref this._resultId, value, () => { this.OnChangedResultId(value); });
        }
        public MapDrawingData CurrentMapData
        {
            get => this._currentMapData;
            set => this.Set(ref this._currentMapData, value);
        }
        public IList CurrentAllotmentOrAssignments
        {
            get => this._currentAllotmentOrAssignments;
            set
            {
                this._currentAllotmentOrAssignments = value;
                RedrawMap();
            }
        }
        public IList CurrentContours
        {
            get => this._currentContours;
            set
            {
                this._currentContours = value;
                RedrawMap();
            }
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
        private void OnExportToHTZCommand(object parameter)
        {
            try
            {
                if (this._currentContours != null)
                {
                    FRM.SaveFileDialog sfd = new FRM.SaveFileDialog() { Filter = "CSV (*.csv)|*.csv", FileName = $"HTZ_{this.ResultId.ToString()}.csv" };
                    if (sfd.ShowDialog() == FRM.DialogResult.OK)
                    {
                        if (File.Exists(sfd.FileName))
                        {
                            try
                            {
                                File.Delete(sfd.FileName);
                            }
                            catch (IOException ex)
                            {
                                _starter.ShowException(Exceptions.GE06Client, new Exception("It wasn't possible to write the data to the disk." + ex.Message));
                            }
                        }
                        var output = new List<string>();
                        output.Add("#,X or longitude,Y or latitude,Coord. code,Azimuth deg,Info 1,Info 2,Envelop dbuV/m");
                        long i = 0;
                        string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                        foreach (ContourModel item in this._currentContours)
                        {
                            foreach (var point in item.CountoursPoints)
                            {
                                output.Add($"{++i},{point.Lon_DEC.ToString().Replace(sep, ".")},{point.Lat_DEC.ToString().Replace(sep, ".")},4DEC,,,,{point.FS.ToString()}");
                            }
                        }
                        System.IO.File.WriteAllLines(sfd.FileName, output.ToArray(), System.Text.Encoding.UTF8);
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Exceptions.GE06Client, e);
            }
        }
        private void RedrawMap()
        {
            var data = new MapDrawingData();
            var polygons = new List<MapDrawingDataPolygon>();
            var points = new List<MapDrawingDataPoint>();

            if (this._currentAllotmentOrAssignments != null)
            {
                foreach (AllotmentOrAssignmentModel item in this._currentAllotmentOrAssignments)
                {
                    if (item.TypeTable == "Assignment")
                    {
                        if (item.Longitude_DEC.HasValue && item.Latitude_DEC.HasValue)
                        {
                            points.Add(MapsDrawingHelper.MakeDrawingPointForSensor(item.Longitude_DEC.Value, item.Latitude_DEC.Value, item.Name));
                        }
                    }
                    if (item.CountoursPoints != null && item.CountoursPoints.Length > 0)
                    {
                        var polygonPoints = new List<Location>();

                        item.CountoursPoints.ToList().ForEach(countourPoint =>
                        {
                            polygonPoints.Add(new Location() { Lat = countourPoint.Lat_DEC, Lon = countourPoint.Lon_DEC });
                        });

                        polygons.Add(new MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
                    }

                }
            }

            if (this._currentContours != null)
            {
                foreach (ContourModel item in this._currentContours)
                {
                    if (item.CountoursPoints != null && item.CountoursPoints.Length > 0)
                    {
                        //var polygonPoints = new List<Location>();

                        item.CountoursPoints.ToList().ForEach(countourPoint =>
                        {
                            string tooltip = $"Longitude = {countourPoint.Lon_DEC.ToString()}\nLatitude = {countourPoint.Lat_DEC.ToString()}\nFS = {countourPoint.FS}\nDistance = {countourPoint.Distance}\nHeight = {countourPoint.Height}\nStatus = {countourPoint.PointType.ToString()}";

                            //polygonPoints.Add(new Location() { Lat = countourPoint.Lat_DEC, Lon = countourPoint.Lon_DEC });
                            if (countourPoint.PointType == DataModels.Sdrn.CalcServer.Internal.Iterations.PointType.Affected)
                                points.Add(MapsDrawingHelper.MakeDrawingPointForCountourAffected(countourPoint.Lon_DEC, countourPoint.Lat_DEC, tooltip));
                            else
                                points.Add(MapsDrawingHelper.MakeDrawingPointForCountour(countourPoint.Lon_DEC, countourPoint.Lat_DEC, tooltip));
                        });

                        //polygons.Add(new MapDrawingDataPolygon() { Points = polygonPoints.ToArray(), Color = System.Windows.Media.Colors.Red, Fill = System.Windows.Media.Colors.Red });
                    }
                }
            }

            data.Polygons = polygons.ToArray();
            data.Points = points.ToArray();

            this.CurrentMapData = data;
        }
        public override void Dispose()
        {
        }
    }
}
