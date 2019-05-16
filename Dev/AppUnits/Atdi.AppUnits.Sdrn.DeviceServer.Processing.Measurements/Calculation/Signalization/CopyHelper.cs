using Atdi.Contracts.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Parameters;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using System.Reflection;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements
{
    public static class CopyHelper
    {
        private static readonly Type arrayType = typeof(Array);

        private static readonly MethodInfo memberwiseClone = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        private static void MakeArrayRowDeepCopy(Dictionary<object, object> state,
            Array array, int[] indices, int rank)
        {
            int next_rank = rank + 1;
            int upperBound = array.GetUpperBound(rank);

            while (indices[rank] <= upperBound)
            {
                object value = array.GetValue(indices);
                if (!ReferenceEquals(value, null))
                    array.SetValue(CreateDeepCopyInternal(state, value), indices);

                if (next_rank < array.Rank)
                    MakeArrayRowDeepCopy(state, array, indices, next_rank);

                indices[rank] += 1;
            }
            indices[rank] = array.GetLowerBound(rank);
        }

        private static Array CreateArrayDeepCopy(Dictionary<object, object> state, Array array)
        {
            Array result = (Array)array.Clone();
            int[] indices = new int[result.Rank];
            for (int rank = 0; rank < indices.Length; ++rank)
                indices[rank] = result.GetLowerBound(rank);
            MakeArrayRowDeepCopy(state, result, indices, 0);
            return result;
        }

        private static object CreateDeepCopyInternal(Dictionary<object, object> state,
            object o)
        {
            object exist_object;
            if (state.TryGetValue(o, out exist_object))
                return exist_object;

            if (o is Array)
            {
                object arrayCopy = CreateArrayDeepCopy(state, (Array)o);
                state[o] = arrayCopy;
                return arrayCopy;
            }
            else if (o is string)
            {
                object stringCopy = string.Copy((string)o);
                state[o] = stringCopy;
                return stringCopy;
            }
            else
            {
                Type o_type = o.GetType();
                if (o_type.IsPrimitive)
                    return o;
                object copy = memberwiseClone.Invoke(o, null);
                state[o] = copy;
                foreach (FieldInfo f in o_type.GetFields(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    object original = f.GetValue(o);
                    if (!ReferenceEquals(original, null))
                        f.SetValue(copy, CreateDeepCopyInternal(state, original));
                }
                return copy;
            }
        }

        public static T CreateDeepCopy<T>(T o)
        {
            object input = o;
            if (ReferenceEquals(o, null))
                return o;
            return (T)CreateDeepCopyInternal(new Dictionary<object, object>(), input);
        }
    }
}
