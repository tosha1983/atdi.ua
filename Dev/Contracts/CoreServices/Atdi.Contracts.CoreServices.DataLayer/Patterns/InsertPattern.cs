using Atdi.DataModels;
using CS = Atdi.DataModels.DataConstraint;
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

    public class UpdateExpression : QueryExpression
    {
        public UpdateExpression()
            : base(QueryExpressionKind.Update)
        {
        }

        public EngineObject Target { get; set; }

        public SetValueExpression[] Values { get; set; }

        public JoinExpression[] Joins { get; set; }

        public ConditionExpression Condition { get; set; }
    }

    public class DeleteExpression : QueryExpression
    {
        public DeleteExpression()
            : base(QueryExpressionKind.Delete)
        {
        }

        public EngineObject Target { get; set; }

        public JoinExpression[] Joins { get; set; }

        public ConditionExpression Condition { get; set; }
    }

    public class SelectExpression : QueryExpression
    {
        public SelectExpression()
            : base(QueryExpressionKind.Select)
        {
        }

        /// <summary>
        /// Целевой объект через который необходимо иницировать выборку
        /// </summary>
        public ColumnExpression[] Columns { get; set; }

        public TargetObject From { get; set; }

        public JoinExpression[] Joins { get; set; }

        public ConditionExpression Condition { get; set; }

        public SortExpression[] Sorting { get; set; }

        public bool Distinct { get; set; }

        public CS.DataLimit Limit { get; set; }


		public long OffsetRows { get; set; }

    }

    public enum JoinOperationType
    {
        Inner,
        Left,
        Right,
        Full,
        Cross
    }

    public class JoinExpression
    {
        public JoinOperationType Operation { get; set; }

        public TargetObject Target { get; set; }

        public ConditionExpression Condition { get; set; }
    }

    public enum ConditionKind
    {
        Complex,
        OneOperand,
        TwoOperand,
        More
    }

    public class ConditionExpression
    {
        public ConditionExpression(ConditionKind kind)
        {
            this.Kind = kind;
        }
        public ConditionKind Kind { get; }

        public static ConditionLogicalOperator LogicalOperator(CS.LogicalOperator logicalOperator)
        {
            switch (logicalOperator)
            {
                case CS.LogicalOperator.And:
                    return ConditionLogicalOperator.And;
                case CS.LogicalOperator.Or:
                    return ConditionLogicalOperator.Or;
                default:
                    throw new InvalidOperationException($"Unsupported logical operator '{logicalOperator}'");
            }
        }
    }

    public enum ConditionLogicalOperator
    {
        /// <summary>
        /// 
        /// </summary>
        And,
        /// <summary>
        /// 
        /// </summary>
        Or
    }

    public class ComplexConditionExpression : ConditionExpression
    {
        public ComplexConditionExpression()
            : base(ConditionKind.Complex)
        {
        }

        public ConditionLogicalOperator Operator { get; set; }

        public ConditionExpression[] Conditions { get; set; }
    }

    /// <summary>
    /// Describes the type of comparison for two values (or expressions) in a condition expression
    /// </summary>

    public enum OneOperandOperator
    {
        /// <summary>
        /// The value is null.
        /// </summary>
        IsNull,
        /// <summary>
        /// The value is not null. 
        /// </summary>
        IsNotNull
    }

    public enum TwoOperandOperator
    {
        /// <summary>
        /// The values are compared for equality.
        /// </summary>
        Equal,
        /// <summary>
        /// The value is greater than or equal to the compared value.
        /// </summary>
        GreaterEqual,
        /// <summary>
        /// The value is greater than the compared value.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// The value is less than or equal to the compared value.
        /// </summary>
        LessEqual,
        /// <summary>
        /// The value is less than the compared value.
        /// </summary>
        LessThan,
        /// <summary>
        /// The two values are not equal.
        /// </summary>
        NotEqual,
        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        Like,
        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        NotLike,
        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        BeginWith,
        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        EndWith,
        /// <summary>
        /// The character string is matched to the specified pattern.
        /// </summary>
        Contains,
        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        NotBeginWith,
        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        NotEndWith,
        /// <summary>
        /// The character string does not match the specified pattern.
        /// </summary>
        NotContains
    }

    public enum MoreOperandsOperator
    {
        
        /// <summary>
        /// The value exists in a list of values.
        /// </summary>
        In,
        /// <summary>
        /// The value does not exist in a list of values.
        /// </summary>
        NotIn,
        /// <summary>
        /// The value is between two values.
        /// </summary>
        Between,
        /// <summary>
        /// The value is not between two values.
        /// </summary>
        NotBetween
        
    }

    public class OneOperandConditionExpression : ConditionExpression
    {
        public OneOperandConditionExpression() 
            : base(ConditionKind.OneOperand)
        {
        }
        public OneOperandOperator Operator { get; set; }

        public OperandExpression Operand { get; set; }
    }

    public class TwoOperandConditionExpression : ConditionExpression
    {
        public TwoOperandConditionExpression()
            : base(ConditionKind.TwoOperand)
        {
        }
        public TwoOperandOperator Operator { get; set; }

        public OperandExpression LeftOperand { get; set; }

        public OperandExpression RightOperand { get; set; }
    }

    public class MoreOperandsConditionExpression : ConditionExpression
    {
        public MoreOperandsConditionExpression()
            : base(ConditionKind.More)
        {
        }
        public MoreOperandsOperator Operator { get; set; }

        public OperandExpression Test { get; set; }

        public OperandExpression[] Operands { get; set; }
    }

    public enum OperandKind
    {
        Member,
        Value
    }

    public class OperandExpression
    {
        public OperandExpression(OperandKind kind)
        {
            Kind = kind;
        }
        public OperandKind Kind { get; }
    }

    public class MemberOperandExpression : OperandExpression
    {
        public MemberOperandExpression() 
            : base(OperandKind.Member)
        {
        }

        public DataEngineMember Member { get; set; }
    }

    public class ValueOperandExpression : OperandExpression
    {
        public ValueOperandExpression()
            : base(OperandKind.Value)
        {
        }

        public ValueExpression Expression { get; set; }
    }

    public enum ColumnExpressionKind
    {
        Member,
        Expression
    }

    public class ColumnExpression
    {
        public ColumnExpression(ColumnExpressionKind kind)
        {
            this.Kind = kind;
        }

        public ColumnExpressionKind Kind { get; }
    }

    public class MemberColumnExpression : ColumnExpression
    {
        public MemberColumnExpression() 
            : base(ColumnExpressionKind.Member)
        {
        }

        public DataEngineMember Member { get; set; }
    }

    public class ExpressionColumnExpression : ColumnExpression
    {
        public ExpressionColumnExpression()
            : base(ColumnExpressionKind.Expression)
        {
        }
        ExpressionClause Expression { get; set; }
    }

    public class ExpressionClause
    {

    }

    public enum SortingDirection
    {
        Ascending, 
        Descending
    }
    public class SortExpression
    {
        public SortingDirection Direction { get; set; }

        public NamedEngineMember Member { get; set; }
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

        public static ConstantValueExpression CreateBy(object value, DataType dataType)
        {
            var constValue = new ConstantValueExpression(dataType)
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

    public class TypedValueExpression : ValueExpression
    {
        public TypedValueExpression(DataType dataType, ValueExpressionKind kind)
            : base(kind)
        {
            this.DataType = dataType;
        }

        public DataType DataType { get;  }
    }

    public class ConstantValueExpression : TypedValueExpression
    {
        public ConstantValueExpression(DataType dataType) 
            : base(dataType, ValueExpressionKind.Constant)
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

    public class GeneratedValueExpression : TypedValueExpression
    {
        public GeneratedValueExpression(DataType dataType)
            : base(dataType, ValueExpressionKind.Generated)
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
        StoredProc,
        Service
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

    public class EngineService : EngineObject
    {
        public EngineService()
            : base(TargetObjectKind.Service)
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
