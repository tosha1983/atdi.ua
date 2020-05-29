using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System.Collections.Specialized;
using System.Collections;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext.Queries;
using Atdi.Platform.Cqrs;
using Atdi.Platform.Events;
using System.Windows;
using VM = Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ClientContext
{
    [ViewXaml("ClientContext.xaml")]
    [ViewCaption("Calc Server Client: Client Context")]
    public class View : ViewBase
    {
        private readonly IObjectReader _objectReader;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewStarter _starter;
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;
        private long _projectId;
        private long? _contextId;
        private byte _mode; /* 0 - Add, 1 - Edit */

        private ClientContextModel _currentClientContextCard;

        public ViewCommand SaveCommand { get; set; }

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

            this.CurrentClientContextCard = new ClientContextModel();
            this.SaveCommand = new ViewCommand(this.OnSaveCommand);
        }
        public long ProjectId
        {
            get => this._projectId;
            set => this.Set(ref this._projectId, value);
        }
        public long? ContextId
        {
            get => this._contextId;
            set => this.Set(ref this._contextId, value, () => { ReloadContext(); Mode = 1; });
        }
        public byte Mode
        {
            get => this._mode;
            set => this.Set(ref this._mode, value);
        }
        public ClientContextModel CurrentClientContextCard
        {
            get => this._currentClientContextCard;
            set => this.Set(ref this._currentClientContextCard, value);
        }
        private void ReloadContext()
        {
            this.CurrentClientContextCard = _objectReader.Read<ClientContextModel>().By(new GetClientContextById { Id = this.ContextId.Value });
        }
        private void OnSaveCommand(object parameter)
        {
            try
            {
                if (this.Mode == 1)
                {
                    var contextModifier = new Modifiers.EditClientContext
                    {
                        Id = this.ContextId.GetValueOrDefault(),
                        Name = CurrentClientContextCard.Name,
                        Note = CurrentClientContextCard.Note,
                        TypeCode = CurrentClientContextCard.TypeCode
                    };

                    _commandDispatcher.Send(contextModifier);
                }
                else
                {
                    var contextModifier = new Modifiers.CreateClientContext
                    {
                        ProjectId = this.ProjectId,
                        Name = CurrentClientContextCard.Name,
                        Note = CurrentClientContextCard.Note,
                        TypeCode = CurrentClientContextCard.TypeCode,
                        OwnerId = Guid.NewGuid()
                    };

                    _commandDispatcher.Send(contextModifier);
                }
                _starter.Stop(this);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        public override void Dispose()
        {
        }
    }
}
