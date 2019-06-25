using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Contracts.CoreServices.DataLayer.Patterns
{
    public enum QueryExpressionKind
    {
        Insert,
        Select,
        Update,
        Delete
    }

    // выражение запроса
    public abstract class QueryExpression
    {
        public QueryExpression(QueryExpressionKind kind)
        {
            this.Kind = kind;
        }

        public QueryExpressionKind Kind { get; }
    }

    public class SelectExpression : QueryExpression
    {
        public SelectExpression() 
            : base(QueryExpressionKind.Select)
        {
        }
    }

    public class InsertExpression : QueryExpression
    {
        public InsertExpression() 
            : base(QueryExpressionKind.Insert)
        {
        }

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
        public ValueExpression Expression { get; set; }
    }


    public enum ValueExpressionKind
    {
        Constant,
        Reference,
        Generated
    }
    /// <summary>
    /// Выражение определяющей значение 
    /// </summary>
    public abstract class ValueExpression
    {
        public ValueExpression(ValueExpressionKind kind)
        {
            this.Kind = kind;
        }

        public ValueExpressionKind Kind { get; }

        public static ConstantValueExpression CreateBy(object value)
        {
            var constValue = new ConstantValueExpression
            {
                Value = value
            };

            return constValue;
        }

        public static ReferenceValueExpression CreateBy(NamedEngineMember member)
        {
            var refValue = new ReferenceValueExpression
            {
                Member = member
            };

            return refValue;
        }
    }


    public class ConstantValueExpression : ValueExpression
    {
        public ConstantValueExpression() 
            : base(ValueExpressionKind.Constant)
        {
        }

        public object Value { get; set; }

        public override string ToString()
        {
            return $"Kind = '{Kind}', ValueType = [{this.Value?.GetType().Name}]";
        }
    }

    public class ReferenceValueExpression : ValueExpression
    {
        public ReferenceValueExpression() 
            : base(ValueExpressionKind.Reference)
        {
        }

        public NamedEngineMember Member { get; set; }

        public override string ToString()
        {
            return $"Kind = '{Kind}', Member = [{this.Member}]";
        }
    }

    public enum GeneratedValueOperation
    {
        SetDefault,
        SetNext
    }
    public class GeneratedValueExpression : ValueExpression
    {
        public GeneratedValueExpression()
            : base(ValueExpressionKind.Generated)
        {
        }

        public GeneratedValueOperation Operation { get; set; }

        public override string ToString()
        {
            return $"Kind = '{Kind}', Operation = '{Operation}'";
        }
    }


    public enum TargetObjectKind
    {
        Expression,
        Table,
        View,
        Function,
        StoredProc
    }
    /// <summary>
    /// Целевой объект к которому можно применить операцию выборки, изменения, вставки, удаления, это может быть:
    /// - выражение запроса (подзапрос)
    /// - таблица
    /// - вьюха
    /// </summary>
    public abstract class TargetObject
    {
        public TargetObject(TargetObjectKind kind)
        {
            this.Kind = kind;
        }

        public TargetObjectKind Kind { get; }

        public string Alias { get; set; }
    }

    /// <summary>
    /// Источник данных как выражение запроса
    /// </summary>
    public class ExpressionObject : TargetObject
    {
        public ExpressionObject() 
            : base(TargetObjectKind.Expression)
        {
        }

        public QueryExpression Expression { get; set; }

        public override string ToString()
        {
            return $"Kind = '{Kind}', Expression = '{this.Expression.Kind}'";
        }
    }

 
    /// <summary>
    /// Объект хранилища: таблицы, запрос, функция, хранимая прцедура
    /// </summary>
    public abstract class EngineObject : TargetObject
    {
        public EngineObject(TargetObjectKind kind) 
            : base(kind)
        {
        }

        public string Schema { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"Kind = '{Kind}', Schema = '{Schema}', Name = '{Name}', Alias = '{Alias}'";
        }
    }

    /// <summary>
    /// Объект хранилища как таблица
    /// </summary>
    public class EngineTable : EngineObject
    {
        public EngineTable() 
            : base(TargetObjectKind.Table)
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
            : base(TargetObjectKind.View)
        {
        }
    }

    public class EngineFunction : EngineObject
    {
        public EngineFunction()
            : base(TargetObjectKind.Function)
        {
        }
    }

    public class EngineStoredProc : EngineObject
    {
        public EngineStoredProc()
            : base(TargetObjectKind.StoredProc)
        {
        }
    }


    public class EngineMember
    {
        public TargetObject Owner { get; set; }
    }

    public class EngineTablePrimaryKey : EngineMember
    {
        public PrimaryKeyField[] Fields { get; set; }
    }

    public class NamedEngineMember : EngineMember
    {
        public string Name { get; set; }

        public string Property { get; set; }

        public override string ToString()
        {
            return $"Name = '{Name}', Property = '{Property}', Owner = [{this.Owner}]";
        }
    }

    public class DataEngineMember : NamedEngineMember
    {
        public DataType DataType { get; set; }

        public override string ToString()
        {
            return $"Name = '{this.Name}', DataType = '{this.DataType}', Owner = [{this.Owner}]";
        }
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


        public NamedEngineMember[] PrimaryKeyFields { get; set; }
    }
}
