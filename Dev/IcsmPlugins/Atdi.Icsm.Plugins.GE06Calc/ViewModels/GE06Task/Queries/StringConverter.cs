using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.CalcServer.Entities;
using Atdi.Platform.Cqrs;
using Atdi.DataModels.Sdrn.DeepServices.GN06;
using ICSM;
using System.Globalization;

namespace Atdi.Icsm.Plugins.GE06Calc.ViewModels.GE06Task.Queries
{
    public static class StringConverter
    {
        public static ActionType ConvertToActionType(string value)
        {
            if (value.Equals("Add", StringComparison.OrdinalIgnoreCase))
                return ActionType.Add;
            else
                return ActionType.Modify;
        }
        public static PlanEntryType ConvertToPlanEntryType(string value)
        {
            if (value.Equals("SingleAssignment", StringComparison.OrdinalIgnoreCase))
                return PlanEntryType.SingleAssignment;
            else if (value.Equals("SinglSFNeAssignment", StringComparison.OrdinalIgnoreCase))
                return PlanEntryType.SFN;
            else if (value.Equals("Allotment", StringComparison.OrdinalIgnoreCase))
                return PlanEntryType.Allotment;
            else if (value.Equals("AllotmentWithLinkedAssignmentAndSfn", StringComparison.OrdinalIgnoreCase))
                return PlanEntryType.AllotmentWithLinkedAssignmentAndSfn;
            else if (value.Equals("AllotmentWithSingleLinkedAssignmentAndNoSfn", StringComparison.OrdinalIgnoreCase))
                return PlanEntryType.AllotmentWithSingleLinkedAssignmentAndNoSfn;
            else
                return PlanEntryType.Unknown;
        }
        public static PlanEntryType ConvertToPlanEntryType(int value)
        {
            if (value != IM.NullI)
            {
                if (value == 1)
                    return PlanEntryType.SingleAssignment;
                else if (value == 2)
                    return PlanEntryType.SFN;
                else if (value == 3)
                    return PlanEntryType.Allotment;
                else if (value == 4)
                    return PlanEntryType.AllotmentWithLinkedAssignmentAndSfn;
                else if (value == 5)
                    return PlanEntryType.AllotmentWithSingleLinkedAssignmentAndNoSfn;
                else if (value == 1)
                    return PlanEntryType.SingleAssignment;
                else
                    return PlanEntryType.Unknown;
            }
            else 
                return PlanEntryType.Unknown;
        }
        public static AssignmentCodeType ConvertToAssignmentCodeType(string value)
        {
            if (value.Equals("S", StringComparison.OrdinalIgnoreCase))
                return AssignmentCodeType.S;
            else if (value.Equals("C", StringComparison.OrdinalIgnoreCase))
                return AssignmentCodeType.C;
            else if (value.Equals("L", StringComparison.OrdinalIgnoreCase))
                return AssignmentCodeType.L;
            else
                return AssignmentCodeType.U;
        }
        public static RefNetworkConfigType ConvertToRefNetworkConfigType(string value)
        {
            if (value.Equals("RPC1", StringComparison.OrdinalIgnoreCase))
                return RefNetworkConfigType.RPC1;
            else if (value.Equals("RPC2", StringComparison.OrdinalIgnoreCase))
                return RefNetworkConfigType.RPC2;
            else if (value.Equals("RPC3", StringComparison.OrdinalIgnoreCase))
                return RefNetworkConfigType.RPC3;
            else if (value.Equals("RPC4", StringComparison.OrdinalIgnoreCase))
                return RefNetworkConfigType.RPC4;
            else if (value.Equals("RPC5", StringComparison.OrdinalIgnoreCase))
                return RefNetworkConfigType.RPC5;
            else
                return RefNetworkConfigType.Unknown;
        }
        public static PolarType ConvertToPolarType(string value)
        {
            if (value.Equals("V", StringComparison.OrdinalIgnoreCase))
                return PolarType.V;
            else if (value.Equals("M", StringComparison.OrdinalIgnoreCase))
                return PolarType.M;
            else
                return PolarType.H;
        }
        public static SpectrumMaskType ConvertToSpectrumMaskType(string value)
        {
            if (value.Equals("S", StringComparison.OrdinalIgnoreCase))
                return SpectrumMaskType.S;
            else
                return SpectrumMaskType.N;
        }
        public static RefNetworkType ConvertToRefNetworkType(string value)
        {
            if (value.Equals("RN2", StringComparison.OrdinalIgnoreCase))
                return RefNetworkType.RN2;
            else if (value.Equals("RN3", StringComparison.OrdinalIgnoreCase))
                return RefNetworkType.RN3;
            else if (value.Equals("RN4", StringComparison.OrdinalIgnoreCase))
                return RefNetworkType.RN4;
            else if (value.Equals("RN5", StringComparison.OrdinalIgnoreCase))
                return RefNetworkType.RN5;
            else if (value.Equals("RN6", StringComparison.OrdinalIgnoreCase))
                return RefNetworkType.RN6;
            else
                return RefNetworkType.RN1;
        }
        public static SystemVariationType ConvertToSystemVariationType(string value)
        {
            if (value.Equals("A1", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.A1;
            else if (value.Equals("A2", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.A2;
            else if (value.Equals("A3", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.A3;
            else if (value.Equals("A5", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.A5;
            else if (value.Equals("A7", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.A7;
            else if (value.Equals("B1", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.B1;
            else if (value.Equals("B2", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.B2;
            else if (value.Equals("B3", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.B3;
            else if (value.Equals("B5", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.B5;
            else if (value.Equals("B7", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.B7;
            else if (value.Equals("C1", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.C1;
            else if (value.Equals("C2", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.C2;
            else if (value.Equals("C3", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.C3;
            else if (value.Equals("C5", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.C5;
            else if (value.Equals("C7", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.C7;
            else if (value.Equals("D1", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.D1;
            else if (value.Equals("D2", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.D2;
            else if (value.Equals("D3", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.D3;
            else if (value.Equals("D5", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.D5;
            else if (value.Equals("D7", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.D7;
            else if (value.Equals("E1", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.E1;
            else if (value.Equals("E2", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.E2;
            else if (value.Equals("E3", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.E3;
            else if (value.Equals("E5", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.E5;
            else if (value.Equals("E7", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.E7;
            else if (value.Equals("F1", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.F1;
            else if (value.Equals("F2", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.F2;
            else if (value.Equals("F3", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.F3;
            else if (value.Equals("F5", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.F5;
            else if (value.Equals("F7", StringComparison.OrdinalIgnoreCase))
                return SystemVariationType.F7;
            else
                return SystemVariationType.Unknown;
        }
        public static RxModeType ConvertToRxModeType(string value)
        {
            if (value.Equals("FX", StringComparison.OrdinalIgnoreCase))
                return RxModeType.FX;
            else if (value.Equals("PO", StringComparison.OrdinalIgnoreCase))
                return RxModeType.PO;
            else if (value.Equals("PI", StringComparison.OrdinalIgnoreCase))
                return RxModeType.PI;
            else if (value.Equals("MO", StringComparison.OrdinalIgnoreCase))
                return RxModeType.MO;
            else
                return RxModeType.Unknown;
        }
        public static AntennaDirectionType ConvertToAntennaDirectionType(string value)
        {
            if (value.Equals("D", StringComparison.OrdinalIgnoreCase))
                return AntennaDirectionType.D;
            else
                return AntennaDirectionType.ND;
        }
        public static short[] ConvertToEffHeight(string value)
        {
            var splitVariants = new char[] { ',', ';', ' ' };
            var values = new List<short>();
            foreach (var item in value.Split(splitVariants))
            {
                if (short.TryParse(item, out short val))
                    values.Add(val);
            }
            return values.ToArray();
        }
        public static float[] ConvertToDiagr(string value)
        {
            string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var valueA = value.Replace("VECTOR 10 ", "").Replace(".", sep);
            var values = new List<float>();
            foreach (var item in valueA.Split(' '))
            {
                if (float.TryParse(item, out float val))
                    values.Add(val);
            }
            return values.ToArray();
        }
        public static short[] ConvertToContourId(string value)
        {
            var splitVariants = new char[] { ',', ';', ' ' };
            var values = new List<short>();
            foreach (var item in value.Split(splitVariants))
            {
                if (short.TryParse(item, out short val))
                    values.Add(val);
            }
            return values.ToArray();
        }

    }
}
