using Atdi.Icsm.Plugins.Core;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Api.EntityOrm.WebClient;
using CS_ES = Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager;

namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.ViewModels.ProjectManager.Adapters
{
    public sealed class ClientContextDataAdapter : EntityDataAdapter<CS_ES.IClientContext, ClientContextModel>
    {
        public ClientContextDataAdapter(CalcServerDataLayer dataLayer, ILogger logger) : base(dataLayer.Origin, logger)
        {
        }
        public long ProjectId;
        protected override void PrepareQuery(IReadQuery<CS_ES.IClientContext> query)
        {
            query.Select(
                c => c.Id,
                c => c.Name,
                c => c.Note,
                c => c.CreatedDate,
                c => c.TypeName,
                c => c.StatusName,
                c => c.StatusNote,
                c => c.OwnerInstance)
            .Filter(f => f.PROJECT.Id, ProjectId)
            .OrderByDesc(o => o.Id);
        }
        protected override ClientContextModel ReadData(IDataReader<CS_ES.IClientContext> reader, int index)
        {
            return new ClientContextModel
            {
                Id = reader.GetValue(c => c.Id),
                Name = reader.GetValue(c => c.Name),
                Note = reader.GetValue(c => c.Note),
                CreatedDate = reader.GetValue(c => c.CreatedDate),
                TypeName = reader.GetValue(c => c.TypeName),
                StatusName = reader.GetValue(c => c.StatusName),
                StatusNote = reader.GetValue(c => c.StatusNote),
                OwnerInstance = reader.GetValue(c => c.OwnerInstance)
            };
        }
    }
}
