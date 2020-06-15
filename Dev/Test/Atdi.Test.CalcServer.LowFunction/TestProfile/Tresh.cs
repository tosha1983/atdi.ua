using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.CalcServer.LowFunction
{

    // при условии что IResMeas.TypeMeasurements (плохо что данное поле не енумка) == Monitoring Station
    public struct MesurementsResults
    {
        int SDRNId; // идетификаторы (Res_Meas_ID) IResMeas.Id
        DateTime date; //(из таблици Res_Meas) IResMeas.TimeMeas
        string SensoreName; //ISensor.Name (доступ через IResMeas.ISubTaskSensor)
        string SensoreTitle; //ISensor.Title (доступ через IResMeas.ISubTaskSensor)
        string[] Standards; // до 20 элементов поле до 20 знаков -> IResMeasStation.Standard набор различных Standard 
        string[] CountByStandard; // до 20 элементов количество со стандартом IResMeasStation.Standard синхронно со строчкой выше
        int CountGSID; // общее количество записей IResMeasStation ссылающиеся на данный IResMeas
        double FreqMin_MHz; // Минимальное значение IResMeasStation.Frequency
        double FreqMax_MHz; // Максимальное значение IResMeasStation.Frequency
        DriveTestsResult[] DriveTestsResults;
    }
    public struct DriveTestsResult
    {
        string GSID;
        double Freq_MHz;
        string Standard;
        PointFS[] Points;
    }
    public struct PointFS
    {
        double Lon_DEC;
        double Lat_DEC;
        int Height_m;
        float FieldStrength_dBmkVm;
        float Level_dBm;
    }
}
