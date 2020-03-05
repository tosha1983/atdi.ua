using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    public enum DataSourceType
    {
        Database = 0,
        Json,
        Xml,
        Csv,
        Excel,
        ClrObject
    }

    public enum StoreContentType
    {
        Binary = 0,
        Json,
        Xml
    }

    public enum DataSourceObject
    {
        Table = 0,
        Query,
        View,
        File,
        Service
    }

    public enum FieldSourceType
    {
        Column = 0,
        Reference,
        Extension,
        Relation,
        //Expression,
        //All
    }

    public enum RelationJoinType
    {
        Inner,
        Outer
    }
    public enum EntityType
    {
        /// <summary>
        /// Обычная сущность
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Абстрактная сущность, используетсят олько для организации агрегативного наследования, т.е. только струткруа без отражение на источник данных
        /// </summary>
        Abstract,

        /// <summary>
        /// Сущность является расширением существующей
        /// Первичный ключ соотвествует первичному ключу расшираемой сущности
        /// Поля первичного ключа расшираемой сущности входят в состав сущности расширения 
        /// </summary>
        Extension,

        /// <summary>
        /// Сущность содержит предопределенные значения, опциональный набор данных.
        /// </summary>
        Predefined,

        /// <summary>
        /// Прототипное наследование (по умолчанию) - Базовая таблица - Таблица наследуемой сущности - Общий первичный ключ - отношение один к одному
        /// </summary>
        Prototype,

        /// <summary>
        /// Ролевое наследование - Базовая таблица - таблицы ролевых сущностей - Синхронизация значений для пересикающиеся по именам поля среди всех таблиц Общий первичный ключ - отношение один к одному
        /// </summary>
        Role,

        /// <summary>
        /// Простое наследование - Базовой таблицы нет или она не используется - Таблица наследуемой сущности получает все поля базовой сущности, включая первичный ключ - нет физических отношений так как одна таблица.
        /// </summary>
        Simple
    }

    //public enum InheritanceType
    //{
    //    /// <summary>
    //    /// Прототипное наследование (по умолчанию) - Базовая таблица - Таблица наследуемой сущности - Общий первичный ключ - отношение один к одному
    //    /// </summary>
    //    None = 0,
    //    /// <summary>
    //    /// Прототипное наследование (по умолчанию) - Базовая таблица - Таблица наследуемой сущности - Общий первичный ключ - отношение один к одному
    //    /// </summary>
    //    Prototype,
    //    /// <summary>
    //    /// Ролевое наследование - Базовая таблица - таблицы ролевых сущностей - Синхронизация значений для пересикающиеся по именам поля среди всех таблиц Общий первичный ключ - отношение один к одному
    //    /// </summary>
    //    Role,
    //    /// <summary>
    //    /// Простое наследование - Базовой таблицы нет или она не используется - Таблица наследуемой сущности получает все поля базовой сущности, включая первичный ключ - нет физических отношений так как одна таблица.
    //    /// </summary>
    //    Simple
    //}

    public enum PrimaryKeyMappedMatchWith
    {
        /// <summary>
        /// мапинг на значение
        /// </summary>
        Value = 0,
        /// <summary>
        /// мапиен на поле сущности
        /// </summary>
        Field,
        /// <summary>
        /// мапинг на название
        /// </summary>
        SourceName
    }

    public enum DataSourceVarType
    {
        UNDEFINED = 0,
        BOOL,
        BIT,
        BYTE,
        BYTES,
        BLOB,
        INT08,
        INT16,
        INT32,
        INT64,
        NCHAR,
        NVARCHAR,
        NTEXT,
        CHAR,
        VARCHAR,
        TEXT,
        TIME,
        DATE,
        DATETIME,
        DATETIMEOFFSET,
        MONEY,
        FLOAT,
        DOUBLE,
        DECIMAL,
        GUID,
        XML,
        JSON,
        CHARS,
        NCHARS,
        CLRTYPE
    }
}
