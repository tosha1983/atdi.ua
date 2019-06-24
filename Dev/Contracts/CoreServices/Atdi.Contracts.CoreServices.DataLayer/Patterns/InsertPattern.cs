using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer.Patterns
{
    // выражение запроса
    public class QueryExpression
    {

    }
    public class SelectExpression : QueryExpression
    {

    }
    public class InsertExpression : QueryExpression
    {
        /// <summary>
        /// Описатель выражения
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Целевой объект через который необходимо иницировать создание записи
        /// </summary>
        public EngineObject Target { get; set; }

        public SetValueExpression[] Values { get; set; } 
    }

    public class SetValueExpression
    {
        /// <summary>
        /// Определитель поля владельца
        /// </summary>
        public DataEngineMember Property { get; set; }

        /// <summary>
        /// Выражение определения значения поля
        /// </summary>
        public ValueSourceExpression Expression { get; set; }
    }

    /// <summary>
    /// Выражение определяющей значение 
    /// </summary>
    public class ValueSourceExpression
    {
        public static ConstantValueExpression CreateBy(object value)
        {
            var constValue = new ConstantValueExpression
            {
                Value = value
            };

            return constValue;
        }
        public static ReferenceValueExpression CreateBy(EngineMember member)
        {
            var refValue = new ReferenceValueExpression
            {
                Value = member
            };

            return refValue;
        }
    }
    public class ValueSourceExpression<TValue> : ValueSourceExpression
    {
        public TValue Value { get; set; }
    }

    public class ConstantValueExpression : ValueSourceExpression<object>
    {
    }

    public class ReferenceValueExpression : ValueSourceExpression<EngineMember>
    {
    }

    /// <summary>
    /// Выражение источника данных
    /// Им может быть объект хранилища или запрос
    /// </summary>
    public class SourceQueryExpression : QueryExpression
    {

    }

    /// <summary>
    /// Источник данных к которому можно применить операцию выборки, это может быть:
    /// - выражение запроса
    /// - таблица
    /// - вьюха
    /// </summary>
    public class EngineSource
    {
        public string Alias { get; set; }

        public static EngineObjectEngineSource CreateBy(EngineObject source, string alias)
        {
            var result = new EngineObjectEngineSource
            {
                Alias = alias,
                Source = source
            };

            return result;
        }
        public static SelectExpressionEngineSource CreateBy(SelectExpression source, string alias)
        {
            var result = new SelectExpressionEngineSource
            {
                Alias = alias,
                Source = source
            };

            return result;
        }
    }

    /// <summary>
    /// Источник данных как выражение запроса
    /// </summary>
    public class SelectExpressionEngineSource : EngineSource
    {
        public SelectExpression Source { get; set; }
    }

    /// <summary>
    /// Источник данных как объект хранилища
    /// </summary>
    public class EngineObjectEngineSource : EngineSource
    {
        public EngineObject Source { get; set; }
    }


    public enum EngineObjectKind
    {
        Table = 0,
        View,
        Function,
        StoredProc
    }
    /// <summary>
    /// Объект хранилища: таблицы, запрос, функция, хранимая прцедура
    /// </summary>
    public class EngineObject 
    {
        public EngineObject(EngineObjectKind kind)
        {
            this.Kind = kind;
        }
        public EngineObjectKind Kind { get; }

        public string Schema { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// Объект хранилища как таблица
    /// </summary>
    public class EngineTable : EngineObject
    {
        public EngineTable() 
            : base(EngineObjectKind.Table)
        {
        }

        public EngineTablePrimaryKey PrimaryKey { get; set; }
    }


    

    /// <summary>
    /// Объект хранилища как вью
    /// </summary>
    public class EngineView : EngineObject
    {
        public EngineView()
            : base(EngineObjectKind.View)
        {
        }
    }

    public class EngineMember
    {
        public EngineSource Owner { get; set; }
    }

    public class EngineTablePrimaryKey : EngineMember
    {
        public PrimaryKeyField[] Fields { get; set; }
    }

    public class NamedEngineMember : EngineMember
    {
        public string Name { get; set; }
    }

    public class DataEngineMember : NamedEngineMember
    {
        public DataType DataType { get; set; }
    }

    public class  PrimaryKeyField : DataEngineMember
    {
        /// <summary>
        /// Признак того что поле подлежит автоматической генерации на сервере
        /// </summary>
        public bool Generated { get; set; }
    }

    public class ValueEngineMember : DataEngineMember
    {
        object Value { get; set; }
    }

    


    /// <summary>
    /// Создание записи о сущности. Возможны сценарии
    /// 1. Сущность одиночка, создание записи  в одном объекте, возврат первичного ключа
    /// 2. Сущность наслденик базовой, передаем цепочку целевых объектов, начиная с базового
    /// 3. Сущность имеет расширение, передается цепочка целевых объектов, начина или с базового или с основного
    /// </summary>
    public class InsertPattern : EngineQueryPattern
    {
        /// <summary>
        /// Последовательность выражений для создания связанных записей.
        /// Построитель гарантирует корреткный порядок следованмй зависимых выражений
        /// Связи не уровне первичных ключей
        /// Пример:
        /// INSERT INTO PARENT_TABLE (FIELD1, FIELD2, FIELD3)
        /// VALUES (@FIELD1, @FIELD2, @FIELD3);
        /// SELECT @PK = @@IDENTITY();
        /// INSERT INTO CHILD_TABLE (FIELD1, @PARENT_ID, FIELD2, FIELD3)
        /// VALUES (@FIELD1_1, @PK, @FIELD1_2, @FIELD1_3)
        /// SELECT @CHILD_1_PK = @@IDENTITY();
        /// INSERT INTO CHILD_TABLE (FIELD1, @PARENT_ID, FIELD2, FIELD3)
        /// VALUES (@FIELD2_1, @PK, @FIELD2_2, @FIELD2_3)
        /// SELECT @CHILD_2_PK = @@IDENTITY();
        /// 
        /// В случаи заказа результата (параметры или ридера) возврат всех айди
        /// 
        /// SELECT @PK, @CHILD_1_PK, @CHILD_2_PK, ...., @YYYTABLE_XX_PK
        /// </summary>
        public InsertExpression[] Expressions { get; set; }


    }
}
