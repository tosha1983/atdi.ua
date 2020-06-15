using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Results
{
    [Serializable]
    public class RotatorPositionResult : CommandResultPartBase
    {
        public RotatorPositionResult(ulong partIndex, CommandResultStatus status)
                 : base(partIndex, status)
        { }
        public RotatorPositionResult()
            : base()
        { }

        public float Azimuth_dg;//азимут относительно севера по часовой стрелке в градусах от 0 до 360

        public float Polarization_dg;//угол относительно вертикальной оси в градусах, по часовой стрелке, касается антенны 
                                     //установленной на СЕВЕР, антенна установленная на ЮГ соответственно 
                                     //будит вращаться против часовой стрелки

        public float Elevation_dg;// Угол наклона в грабусах относительно вертикальной оси т.е. 
                                  //вертикально в верх равно 0 гр горизотально 90 гр может быть 
                                  //доступно например от -20 до 100
        public bool Established;//True значит ротатор повернулся в установленное значение.
        // False значит что ротатор начал движение или движется.

    }
}
