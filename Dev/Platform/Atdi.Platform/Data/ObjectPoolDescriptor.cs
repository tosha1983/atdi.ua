using System;

namespace Atdi.Platform.Data
{
    public class ObjectPoolDescriptor
    {
        public ObjectPoolDescriptor(Type type)
        {
            this.Type = type;
        }

        public string Key;
        public int MinSize;
        public int MaxSize;
        public readonly Type Type;

        public override string ToString()
        {
            return $"Key='{Key}', MinSize={MinSize.ToString()}, MaxSize={MaxSize.ToString()}, Type='{Type.FullName}'";
        }
    }

    public sealed class ObjectPoolDescriptor<T> : ObjectPoolDescriptor
    {
        public ObjectPoolDescriptor()
            : base(typeof(T))
        {
        }

        public Func<T> Factory;
    }
}