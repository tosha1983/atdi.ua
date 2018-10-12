using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public interface IEntityMetadata
    {
        /// <summary>
        /// Имя сущности
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Отображаемое название сущности
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Описание сущности и ее назначение
        /// </summary>
        string Desc { get; }

        /// <summary>
        /// Тип сущности
        /// </summary>
        EntityType Type { get; }

        /// <summary>
        /// Наследуемая сущность
        
        /// </summary>
        IEntityMetadata BaseEntity { get; }

        /// <summary>
        /// Вид наследования
        /// Планируются следующие видынаследования
        /// 1. Прототипное наследование (по умолчанию) - Базовая таблица - Таблица наследуемой сущности - Общий первичный ключ - отношение один к одному
        /// 2. Ролевое наследование - Базовая таблица - таблицы ролевых сущностей - Синхронизация значений для пересикающиеся по именам поля среди всех таблиц Общий первичный ключ - отношение один к одному
        /// 3. Простое наследование - Базовой таблицы нет или она не используется - Таблица наследуемой сущности получает все поля базовой сущности, включая первичный ключ - нет физических отношений так как одна таблица.
        /// </summary>
        InheritanceType? Inheritance { get; }

        /// <summary>
        /// Расширяемая сущность
        /// </summary>
        IEntityMetadata ExtendEntity { get; }

        /// <summary>
        /// Описание природы источника данных
        /// </summary>
        IDataSourceMetadata DataSource { get; }

        /// <summary>
        ///  Описание первичного ключа сущности
        /// </summary>
        IPrimaryKeyMetadata PrimaryKey { get; }

        /// <summary>
        /// Набор полей сущности
        /// </summary>
        IReadOnlyDictionary<string, IFieldMetadata> Fields { get; }

    }
}
