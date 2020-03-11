using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMapSector_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMapSector: IMapSector_PK
	{

        IMap MAP { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        string SectorName { get; set; }

		int AxisXIndex { get; set; }

        int AxisYIndex { get; set; }

		int AxisXNumber { get; set; }

		int AxisYNumber { get; set; }

		int UpperLeftX { get; set; }

		int UpperLeftY { get; set; }

		int UpperRightX { get; set; }

		int UpperRightY { get; set; }

		int LowerLeftX { get; set; }

		int LowerLeftY { get; set; }

		int LowerRightX { get; set; }

		int LowerRightY { get; set; }

		int ContentSize { get; set; }

		string ContentType { get; set; }

		string ContentEncoding { get; set; }

		byte[] Content { get; set; }
	}
}
