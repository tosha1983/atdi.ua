using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform
{
    public interface IStatistics
    {
        IStatisticEntry<T> Entry<T>(IStatisticEntryKey<T> entryKey);

        void Set<T>(IStatisticEntryKey<T> entryKey, T newValue);

        IStatisticCounter Counter(IStatisticCounterKey entryKey);

        void Reset(IStatisticCounterKey entryKey);

        void Increment(IStatisticCounterKey entryKey);

        void Decrement(IStatisticCounterKey entryKey);

        void Change(IStatisticCounterKey entryKey, long newValue);

        IStatisticEntry[] GetEntries();

        IStatisticCounter[] GetCounters();

        IStatisticEntryKey[] GetEntryKeys();

        IStatisticCounterKey[] GetCounterKeys();

    }

    public static class STS
    {
        public static IStatisticEntryKey<string> DefineEntryKeyAsString(string name)
        {
            return new StatisticEntryKey<string>(name);
        }
        public static IStatisticEntryKey<int> DefineEntryKeyAsInteger(string name)
        {
            return new StatisticEntryKey<int>(name);
        }

        public static IStatisticEntryKey<long> DefineEntryKeyAsLong(string name)
        {
            return new StatisticEntryKey<long>(name);
        }

        public static IStatisticCounterKey DefineCounterKey(string name)
        {
            return new StatisticCounterKey(name);
        }
        public static IStatisticCounterKey DefineCounterKey(string name, float scale)
        {
            return new StatisticCounterKey(name, scale);
        }

        public static class OS
        {
            public static class Version
            {
                public static readonly IStatisticEntryKey<string> Name = STS.DefineEntryKeyAsString("OS.Version.Name");
                public static readonly IStatisticEntryKey<string> Number = STS.DefineEntryKeyAsString("OS.Version.Number");
                public static readonly IStatisticEntryKey<string> ServicePack = STS.DefineEntryKeyAsString("OS.Version.ServicePack");
                public static readonly IStatisticEntryKey<string> Platform = STS.DefineEntryKeyAsString("OS.Version.Platform");
            }

            public static readonly IStatisticEntryKey<string> Is64Bit = STS.DefineEntryKeyAsString("OS.Is64Bit");
        }

        public static class Host
        {
            public static readonly IStatisticEntryKey<string> Name = STS.DefineEntryKeyAsString("OS.Host.Name");
            public static class CPU
            {
                public static readonly IStatisticEntryKey<int> Cores = STS.DefineEntryKeyAsInteger("OS.Host.CPU.Cores");
            }
        }
        public static class Process
        {
            public static readonly IStatisticEntryKey<string> Is64Bit = STS.DefineEntryKeyAsString("OS.Process.Is64Bit");
            public static readonly IStatisticEntryKey<string> UserName = STS.DefineEntryKeyAsString("OS.Process.UserName");
            public static readonly IStatisticEntryKey<string> Directory = STS.DefineEntryKeyAsString("OS.Process.Directory");
        }

        public static class Counter
        {

        }
    }
}
