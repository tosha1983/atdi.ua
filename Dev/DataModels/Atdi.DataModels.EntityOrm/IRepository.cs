using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm
{
    public interface IRepository<T,I> : IDisposable
        where T : class
    {
        void RemoveOldObjects();
        T[] LoadObjectsWithRestrict(ref List<string> listRunTask); // получение объектов с ограничениями
        T[] LoadAllObjects(); // получение всех объектов
        T LoadObject(I id); // получение одного объекта по id
        I Create(T item); // создание объекта
        bool Update(T item); // обновление объекта
        bool Delete(I id); // удаление объекта по id
    }

}
