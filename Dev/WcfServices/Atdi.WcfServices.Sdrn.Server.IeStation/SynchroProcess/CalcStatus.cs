using System.Collections.Generic;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;


namespace Atdi.WcfServices.Sdrn.Server.IeStation
{
    public class CalcStatus
    {

        /// <summary>
        /// Вычисление статуса для записей таблицы RefSpectrum
        /// </summary>
        /// <param name="permissionCancelDate"></param>
        /// <param name="permissionStop"></param>
        /// <param name="permissionStart"></param>
        /// <param name="DocNum"></param>
        /// <param name="testStartDate"></param>
        /// <param name="testStopDate"></param>
        /// <param name="dateMeasFromRefSpectrum"></param>
        /// <returns></returns>
        public static string CalcStatusMeasForRefSpectrum(DateTime? permissionCancelDate, DateTime? permissionStop, DateTime? permissionStart, string DocNum, DateTime? testStartDate, DateTime? testStopDate, DateTime dateMeasFromRefSpectrum)
        {
            var StatusMeas = "";
            DateTime? Start = null;
            DateTime? Stop = null;
            if ((permissionCancelDate == null) && (permissionStop != null) && (permissionStart != null))
            {
                Start = permissionStart;
                Stop = permissionStop;
            }
            else if ((permissionCancelDate != null) && (permissionStart != null))
            {
                Start = permissionStart;
                Stop = permissionCancelDate;
            }
            else if (!string.IsNullOrEmpty(DocNum) && (testStartDate != null) && (testStopDate != null))
            {
                Start = testStartDate;
                Stop = testStopDate;
            }

            if ((Start != null) && (Stop != null))
            {
                if ((Start.Value <= dateMeasFromRefSpectrum && Stop.Value >= dateMeasFromRefSpectrum) == true)
                {
                    StatusMeas = "U";
                }
                else
                {
                    StatusMeas = "I";
                }
            }
            else
            {
                StatusMeas = "I";
            }
            return StatusMeas;
        }

        /// <summary>
        /// Вычисление статуса для записей таблицы StationExtended
        /// </summary>
        /// <param name="isEmitingContain"></param>
        /// <param name="permissionCancelDate"></param>
        /// <param name="permissionStop"></param>
        /// <param name="permissionStart"></param>
        /// <param name="docNum"></param>
        /// <param name="testStartDate"></param>
        /// <param name="testStopDate"></param>
        /// <param name="dateMeasFromRefSpectrum"></param>
        /// <returns></returns>
        public static string CalcStatusMeasForStationExtended(bool isEmitingContain, DateTime? permissionCancelDate, DateTime? permissionStop, DateTime? permissionStart, string docNum, DateTime? testStartDate, DateTime? testStopDate, DateTime dateMeasFromRefSpectrum)
        {
            var StatusMeas = "";

            if ((permissionCancelDate == null) && (permissionStop != null) && (permissionStart != null))
            {
                if ((permissionStart <= dateMeasFromRefSpectrum) && (permissionStop >= dateMeasFromRefSpectrum) == true)
                {
                    if (isEmitingContain==false)
                    {
                        StatusMeas = "U";
                    }
                    else
                    {
                        StatusMeas = "A";
                    }
                }
                else
                {
                    StatusMeas = "I";
                }
            }
            else if ((permissionCancelDate != null) && (permissionStart != null))
            {
                if ((permissionStart <= dateMeasFromRefSpectrum) && (permissionCancelDate >= dateMeasFromRefSpectrum) == true)
                {
                    if (isEmitingContain == false)
                    {
                        StatusMeas = "U";
                    }
                    else
                    {
                        StatusMeas = "A";
                    }
                }
                else
                {
                    StatusMeas = "I";
                }
            }
            else if (!string.IsNullOrEmpty(docNum) && (testStartDate != null) && (testStopDate != null))
            {
                if ((testStartDate <= dateMeasFromRefSpectrum) && (testStopDate >= dateMeasFromRefSpectrum) == true)
                {
                    if (isEmitingContain == false)
                    {
                        StatusMeas = "U";
                    }
                    else
                    {
                        StatusMeas = "T";
                    }
                }
                else
                {
                    StatusMeas = "I";
                }
            }
            else
            {
                StatusMeas = "I";
            }
            return StatusMeas;
        }

        /// <summary>
        /// Вычисление даты начала для периода, который будет использоваться при поиске  эмитингов в БД
        /// </summary>
        /// <param name="statusMeas"></param>
        /// <param name="permissionCancelDate"></param>
        /// <param name="permissionStop"></param>
        /// <param name="permissionStart"></param>
        /// <param name="docNum"></param>
        /// <param name="testStartDate"></param>
        /// <param name="testStopDate"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public static  DateTime? CalcDateStart(string statusMeas, DateTime? permissionCancelDate, DateTime? permissionStop, DateTime? permissionStart, string docNum, DateTime? testStartDate, DateTime? testStopDate, DateTime startDate)
        {
            DateTime? startDateVal = null;
            if (statusMeas == "U")
            {
                if ((permissionCancelDate == null) && (permissionStop != null) && (permissionStart != null))
                {
                    if (permissionStart.Value > startDate)
                    {
                        startDateVal = permissionStart.Value;
                    }
                    else
                    {
                        startDateVal = startDate;
                    }
                }
                else if ((permissionCancelDate != null) && (permissionStart != null))
                {
                    if (permissionStart.Value > startDate)
                    {
                        startDateVal = permissionStart.Value;
                    }
                    else
                    {
                        startDateVal = startDate;
                    }
                }
                else if (!string.IsNullOrEmpty(docNum) && (testStartDate != null) && (testStopDate != null))
                {
                    if (testStartDate.Value > startDate)
                    {
                        startDateVal = testStartDate.Value;
                    }
                    else
                    {
                        startDateVal = startDate;
                    }
                }
            }
            if (statusMeas == "I")
            {
                if ((permissionCancelDate == null) && (permissionStop != null) && (permissionStart != null))
                {
                    if (permissionStop.Value > startDate)
                    {
                        startDateVal = permissionStop.Value;
                    }
                    else
                    {
                        startDateVal = startDate;
                    }
                }
                else if ((permissionCancelDate != null) && (permissionStart != null))
                {
                    if (permissionCancelDate.Value > startDate)
                    {
                        startDateVal = permissionCancelDate.Value;
                    }
                    else
                    {
                        startDateVal = startDate;
                    }
                }
                else if (!string.IsNullOrEmpty(docNum) && (testStartDate != null) && (testStopDate != null))
                {
                    if (testStopDate.Value > startDate)
                    {
                        startDateVal = testStopDate.Value;
                    }
                    else
                    {
                        startDateVal = startDate;
                    }
                }
                else
                {
                    startDateVal = startDate;
                }
            }
            return startDateVal;
        }

    }
}


