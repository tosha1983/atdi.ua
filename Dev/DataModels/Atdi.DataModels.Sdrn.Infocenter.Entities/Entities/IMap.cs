using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.Infocenter.Entities
{
    [EntityPrimaryKeyAttribute]
    public interface IMap_PK
    {
        long Id { get; set; }
    }

    [Entity]
    public interface IMap: IMap_PK
	{

        byte StatusCode { get; set; }

        string StatusName { get; set; }

        string StatusNote { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        byte TypeCode { get; set; }

		string TypeName { get; set; }

		string Projection { get; set; }

		string StepUnit { get; set; }

		string StepDataType { get; set; }

		byte StepDataSize { get; set; }

		int AxisXNumber { get; set; }

		int AxisXStep { get; set; }

		int AxisYNumber { get; set; }

		int AxisYStep { get; set; }

		int UpperLeftX { get; set; }

		int UpperLeftY { get; set; }

		int UpperRightX { get; set; }

		int UpperRightY { get; set; }

		int LowerLeftX { get; set; }

		int LowerLeftY { get; set; }

		int LowerRightX { get; set; }

		int LowerRightY { get; set; }

		int ContentSize { get; set; }

		string ContentSource { get; set; }

		int? FileSize { get; set; }

		string FileName { get; set; }

		string MapName { get; set; }

		string MapNote { get; set; }


		int SectorsCount { get; set; }

		int SectorsXCount { get; set; }
		int SectorsYCount { get; set; }

		
	}

    public enum MapStatusCode
    {
		/// <summary>
		/// создана
		/// </summary>
		Created = 0,
		/// <summary>
		/// модифицируется - расскладка оп секторам
		/// </summary>
		Modifying = 1,
		/// <summary>
		/// карта доступна
		/// </summary>
		Available = 2,
		/// <summary>
		/// карта временно заблокирована
		/// </summary>
		Locked = 3,
		/// <summary>
		/// Карта не актуальна
		/// </summary>
		Archived = 4
	}
}
