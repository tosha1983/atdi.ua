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

        public static IStatisticEntryKey<DateTime> DefineEntryKeyAsDatetTime(string name)
        {
            return new StatisticEntryKey<DateTime>(name);
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
            public static readonly IStatisticEntryKey<string> Name = STS.DefineEntryKeyAsString("Host.Name");
            public static class CPU
            {
                public static readonly IStatisticEntryKey<int> Cores = STS.DefineEntryKeyAsInteger("Host.CPU.Cores");
            }
        }
        public static class Process
        {
            public static readonly IStatisticEntryKey<string> Name = STS.DefineEntryKeyAsString("Process.Name");
            public static readonly IStatisticEntryKey<int> ID = STS.DefineEntryKeyAsInteger("Process.ID");
            public static readonly IStatisticEntryKey<DateTime> StartTime = STS.DefineEntryKeyAsDatetTime("Process.StartTime");

            public static readonly IStatisticEntryKey<string> Is64Bit = STS.DefineEntryKeyAsString("Process.Is64Bit");
            public static readonly IStatisticEntryKey<string> UserName = STS.DefineEntryKeyAsString("Process.UserName");
            public static readonly IStatisticEntryKey<string> Directory = STS.DefineEntryKeyAsString("Process.Directory");
            public static readonly IStatisticEntryKey<string> CommandLine = STS.DefineEntryKeyAsString("Process.CommandLine");
            public static class Memory
            {
                public static readonly IStatisticEntryKey<long> PeakVirtual = STS.DefineEntryKeyAsLong("Process.Memory.Peak.Virtual");
                public static readonly IStatisticEntryKey<long> PeakWorkingSet = STS.DefineEntryKeyAsLong("Process.Memory.Peak.WorkingSet");
                public static readonly IStatisticEntryKey<long> PeakPaged = STS.DefineEntryKeyAsLong("Process.Memory.Peak.Paged");
            }

        }

        public static class Counter
        {
            public static class Process
            {
                public static readonly IStatisticCounterKey Threads = STS.DefineCounterKey("Process.Threads");
                public static readonly IStatisticCounterKey ThreadsLogical = STS.DefineCounterKey("Process.Threads.Logical");
                public static readonly IStatisticCounterKey ThreadsPhysical = STS.DefineCounterKey("Process.Threads.Physical");
                public static readonly IStatisticCounterKey ThreadsQueue = STS.DefineCounterKey("Process.Threads.Queue");
                public static readonly IStatisticCounterKey Handles = STS.DefineCounterKey("Process.Handles");
            }
            public static class Memory
            {
                public static readonly IStatisticCounterKey Paged = STS.DefineCounterKey("Process.Memory.Current.Paged");
                public static readonly IStatisticCounterKey NonpagedSystem = STS.DefineCounterKey("Process.Memory.Current.NonpagedSystem");
                public static readonly IStatisticCounterKey PagedSystem = STS.DefineCounterKey("Process.Memory.Current.PagedSystem");

                public static readonly IStatisticCounterKey Private = STS.DefineCounterKey("Process.Memory.Current.Private");
                public static readonly IStatisticCounterKey WorkingSet = STS.DefineCounterKey("Process.Memory.Current.WorkingSet");
                public static readonly IStatisticCounterKey Virtual = STS.DefineCounterKey("Process.Memory.Current.Virtual");
                public static class GC
                {
                    public static readonly IStatisticCounterKey Private = STS.DefineCounterKey("Process.Memory.GC.Private");
                    public static readonly IStatisticCounterKey Gen0 = STS.DefineCounterKey("Process.Memory.GC.Gen0.Count");
                    public static readonly IStatisticCounterKey Gen1 = STS.DefineCounterKey("Process.Memory.GC.Gen1.Count");
                    public static readonly IStatisticCounterKey Gen2 = STS.DefineCounterKey("Process.Memory.GC.Gen2.Count");
                    public static readonly IStatisticCounterKey Gen0Heap = STS.DefineCounterKey("Process.Memory.GC.Gen0.HeapSize");
                    public static readonly IStatisticCounterKey Gen1Heap = STS.DefineCounterKey("Process.Memory.GC.Gen1.HeapSize");
                    public static readonly IStatisticCounterKey Gen2Heap = STS.DefineCounterKey("Process.Memory.GC.Gen2.HeapSize");
                    public static readonly IStatisticCounterKey LOHeap = STS.DefineCounterKey("Process.Memory.GC.LargeObject.HeapSize");
                }
            }
            public static class Network
            {

            }
        }
    }
}
