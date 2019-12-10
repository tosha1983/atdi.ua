using System;

namespace Atdi.Platform.Data
{
    public class ObjectPoolDescriptor<T>
    {
        public ObjectPoolDescriptor()
        {
            this.Type = typeof(T);
        }
        public string Key;
        public int MinSize;
        public int MaxSize;
        public readonly Type Type;
        public Func<T> Factory;

        public override string ToString()
        {
            return $"Key='{Key}', MinSize={MinSize.ToString()}, MaxSize={MaxSize.ToString()}, Type='{Type.FullName}'";
        }
    }
}