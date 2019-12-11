using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Platform.Data
{
    public interface IObjectPool
    {
        object TakeObject();

        void PutObject(object item);
    }

    public interface IObjectPool<T> : IObjectPool
    {
        ObjectPoolDescriptor<T> Descriptor { get; }

        bool TryTake(out T item);

        T Take();

        void Put(T item);
    }
}
