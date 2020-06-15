using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters
{
    public class RotatorPositionParameter
    {
        public RotatorPositionMode Mode;//Get = пришлет только текущее значение, SetAndGet = установит и будит слать результат как настроенно ниже
        public float Azimuth_dg;//азимут относительно севера по часовой стрелке в градусах
        public float AzimuthStep_dg;//шаг перестройки за раз, если равно 0 то непрерывно без остановки
        public int AzimuthTimeStep_ms;//время задержки после перестройки на один шаг в мс
        public int AzimuthSpeed;// скорость движения по азимуту, передается согласно пропертисов адаптера, если скорость одна то игнорируется

        public float Polarization_dg;//угол относительно вертикальной оси в градусах, по часовой стрелке, касается антенны 
                                     //установленной на СЕВЕР, антенна установленная на ЮГ соответственно 
                                     //будит вращаться против часовой стрелки
        public float PolarizationStep_dg;//шаг перестройки за раз, если равно 0 то непрерывно без остановки (а если 
                                         //другая ось с шагом то сначала установится эта а потом та что с шагом)
        public int PolarizationTimeStep_ms;//время задержки после перестройки на один шаг в мс
        public int PolarizationSpeed;// скорость движения по углу поляризации, передается согласно пропертисов адаптера, если скорость одна то игнорируется

        public float Elevation_dg;// Угол наклона в грабусах относительно вертикальной оси т.е. 
                               //вертикально в верх равно 0 гр горизотально 90 гр может быть доступно например от -20 до 100
        public float ElevationStep_dg;//шаг перестройки за раз, если равно 0 то непрерывно без остановки
        public int ElevationTimeStep_ms;//время задержки после перестройки на один шаг в мс
        public int ElevationSpeed;// скорость движения по углу поляризации, передается согласно пропертисов адаптера, если скорость одна то игнорируется

        public bool PublicResultAfterSet;//если True то публиковать резульат после фактической установки в углы, 
                                         //если  False то во время поворота текущее публиковать и после установки тоже
        
    }
}
