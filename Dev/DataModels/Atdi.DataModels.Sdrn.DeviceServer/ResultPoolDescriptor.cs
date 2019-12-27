using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.DeviceServer
{
    public interface IResultPoolDescriptor<T>
    {
        string Key { get; }
        int MinSize { get; }
        int MaxSize { get; }
        Type ResultType { get; }
        Func<T> Factory { get; }
    }

    public class ResultPoolDescriptor<T>: IResultPoolDescriptor<T>
        where T : ICommandResultPart
    {
        public static readonly string DefaultKey = "default";

        public ResultPoolDescriptor()
        {
            this.ResultType = typeof(T);
        }

        public string Key { get; set; } = ResultPoolDescriptor<T>.DefaultKey;
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public Type ResultType { get; }
        public Func<T> Factory {get; set; }
    }
}
