using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API = Atdi.DataModels.EntityOrm.Api;


namespace Atdi.WebApiServices.EntityOrm.Controllers.DTO
{
    public class UnitMetadata : API.IUnitMetadata
    {
        public UnitMetadata(IUnitMetadata unitMetadata)
        {
            if (unitMetadata == null)
            {
                throw new ArgumentNullException(nameof(unitMetadata));
            }

            this.Name = unitMetadata.Name;
            this.Dimension = unitMetadata.Dimension;
            this.Category = unitMetadata.Category;
        }

        public string Name { get; }

        public string Dimension { get; }

        public string Category { get; }
    }
}
