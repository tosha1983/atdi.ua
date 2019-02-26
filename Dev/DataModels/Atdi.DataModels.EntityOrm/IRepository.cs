using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atdi.DataModels.EntityOrm
{
    public interface IRepository<T> : IDisposable
         where T : class
    {
        T[] LoadAllObjects(); // получение всех объектов
        T LoadObject(int id); // получение одного объекта по id
        int? Create(T item); // создание объекта
        bool Update(T item); // обновление объекта
        bool Delete(int id); // удаление объекта по id
    }

}
