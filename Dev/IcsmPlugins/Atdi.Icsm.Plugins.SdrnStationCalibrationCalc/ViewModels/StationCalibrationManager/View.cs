using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.ProjectManager.Queries;
using Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager.Adapters;

using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Windows.Forms;
using System.Windows;

namespace Atdi.Icsm.Plugins.SdrnStationCalibrationCalc.ViewModels.StationCalibrationManager
{

	[ViewXaml("StationCalibrationManager.xaml")]
	[ViewCaption("Station Calibration calc client")]
	public class View : ViewBase
    {
        private DateTime? _dateStartLoadDriveTest;
        private DateTime? _dateStopLoadDriveTest;


        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        private ResMeasModel _currentResMeas;
        public ResMeasDataAdapter ResMeasData { get; set; }


        public ViewCommand StartStationCalibrationCommand { get; set; }
        public ViewCommand LoadDriveTestsCommand { get; set; }





        public View(
            ResMeasDataAdapter  resMeasDataAdapter,
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


            this.StartStationCalibrationCommand = new ViewCommand(this.OnStartStationCalibrationCommand);
            this.LoadDriveTestsCommand = new ViewCommand(this.OnLoadDriveTestsCommand);
            this.ResMeasData = resMeasDataAdapter;

            ReloadData();

        }

        public DateTime? DateStartLoadDriveTest
        {
            get => this._dateStartLoadDriveTest;
            set => this.Set(ref this._dateStartLoadDriveTest, value);
        }
        public DateTime? DateStopLoadDriveTest
        {
            get => this._dateStopLoadDriveTest;
            set => this.Set(ref this._dateStopLoadDriveTest, value);
        }

        public ResMeasModel CurrentResMeas
        {
            get => this._currentResMeas;
            set => this.Set(ref this._currentResMeas, value, () => { this.OnChangedCurrentResMeas(value); });
        }


        private void OnChangedCurrentResMeas(ResMeasModel resMeas)
        {

        }


        public ResMeasModel ReadResMeas(long id)
        {
            var resMeas = _objectReader
                .Read<ResMeasModel>()
                .By(new GetResMeasById() 
                {
                     Id = id
                });
            return resMeas;
        }

        private void ReloadData()
        {
            this.DateStartLoadDriveTest = DateTime.Now.AddDays(-30);
            this.DateStopLoadDriveTest = DateTime.Now;
        }



		public override void Dispose()
		{

        }


        private void OnLoadDriveTestsCommand(object parameter)
        {
            try
            {
                this.ResMeasData.StartDateTime = this.DateStartLoadDriveTest.Value;
                this.ResMeasData.StopDateTime = this.DateStopLoadDriveTest.Value;
                this.ResMeasData.Refresh();
            }
            catch (Exception e)
            {

            }
        }



        private void OnStartStationCalibrationCommand(object parameter)
        {
            try
            {

            }
            catch (Exception e)
            {

            }
        }
    }
}
