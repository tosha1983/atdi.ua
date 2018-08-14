using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;
using Atdi.Contracts.LegacyServices.Icsm;
using System.Runtime.CompilerServices;

namespace Atdi.AppServices.WebQuery
{
    public sealed class QueryDescriptor
    {
        private readonly XWEBCONSTRAINT[] _ConstraintsValue;
        private readonly Dictionary<string, DataType> _hashSet;
        private readonly XWEBQUERY _QueryValue;
       


        public string IdentUserField { get; private set; }

        public string TableName { get; private set; }

        public QueryMetadata Metadata { get; private set; }
        public IrpColumn[] IrpDescrColumns { get; private set; }
        public Dictionary<string,IrpColumn> IrpDictionary { get; private set; }

        public QueryTokenDescriptor QueryTokenDescriptor { get; private set; }

        public QueryDescriptor(XWEBQUERY QueryValue, XWEBCONSTRAINT[] ConstraintsValue, IrpDescriptor irpdescription, QueryTokenDescriptor queryTokenDescriptor)
        {
            this._hashSet = new Dictionary<string, DataType>();
            this.IrpDictionary = new Dictionary<string, IrpColumn>();
            this._QueryValue = QueryValue;
            this._ConstraintsValue = ConstraintsValue;
            this.QueryTokenDescriptor = queryTokenDescriptor;

            this.TableName = irpdescription.TableName;
            this.IdentUserField = QueryValue.IDENTUSER;
            this.IrpDescrColumns = irpdescription.irpColumns.ToArray();

            this.Metadata = new QueryMetadata
            {
                Columns = this.IrpDescrColumns.Select(t => t.columnMeta).ToArray(),
                Name = QueryValue.NAME,
                Code = QueryValue.CODE,
                Token = queryTokenDescriptor.Token,
                Description = QueryValue.COMMENTS,
                Title = QueryValue.NAME,
                PrimaryKey = irpdescription.PrimaryKey
            };

            

            var listColumns = this.Metadata.Columns;
            for (int i = 0; i < listColumns.Length; i++)
            {
                var column = listColumns[i];
                if (!_hashSet.ContainsKey(column.Name))
                    _hashSet.Add(column.Name, column.Type);
                if (!IrpDictionary.ContainsKey(column.Name))
                {
                    IrpColumn col = irpdescription.irpColumns.Find(t => t.columnMeta.Name == column.Name);
                    if (col != null)
                    {
                        IrpDictionary.Add(column.Name, col);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataSetColumn MakeDataSetColumn(string name)
        {

            return new DataSetColumn
            {
                Name = name,
                Type = this.GetColumnType(name)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasColumn(string nameColumn)
        {
            return this._hashSet.ContainsKey(nameColumn); 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataType GetColumnType(string name)
        {
            return this._hashSet[name];
        }

      
        public void VerifyAccessToAction(ActionType type)
        {
            var result = false;
            switch (type)
            {
                case ActionType.Create:
                    result = this.QueryTokenDescriptor.GroupDescriptor.Group.CanCreate;
                    break;
                case ActionType.Update:
                    result = this.QueryTokenDescriptor.GroupDescriptor.Group.CanModify;
                    break;
                case ActionType.Delete:
                    result = this.QueryTokenDescriptor.GroupDescriptor.Group.CanDelete;
                    break;
                default:
                    result = false;
                    break;
            }
            if (!result)
            {
                //throw new InvalidOperationException(Exceptions.AccessToActionDenied.With(type));
            } 
        }
        public void CheckColumns(string[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (HasColumn(columns[i]) == false)
                {
                    var badCollumns = new List<string>();
                    for (int j = 0; j < columns.Length; j++)
                    {
                        if (HasColumn(columns[i]) == false)
                        {
                            if (!badCollumns.Contains(columns[i]))
                                badCollumns.Add(columns[i]);
                        }
                    }
                    var message = string.Join(", ", badCollumns);
                    throw new InvalidOperationException($"Not found columns with name's {message}'");
                }
            }
        }

        public void CheckColumns(DataSetColumn[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (HasColumn(columns[i].Name) == false)
                {
                    var badCollumns = new List<string>();
                    for (int j = 0; j < columns.Length; j++)
                    {
                        if (HasColumn(columns[i].Name) == false)
                        {
                            if (!badCollumns.Contains(columns[i].Name))
                                badCollumns.Add(columns[i].Name);
                        }
                    }
                    var message = string.Join(", ", badCollumns);
                    throw new InvalidOperationException($"Not found columns with name's {message}'");
                }
            }
        }

        public void CheckColumns(OrderExpression[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (HasColumn(columns[i].ColumnName) == false)
                {
                    var badCollumns = new List<string>();
                    for (int j = 0; j < columns.Length; j++)
                    {
                        if (HasColumn(columns[i].ColumnName) == false)
                        {
                            if (!badCollumns.Contains(columns[i].ColumnName))
                                badCollumns.Add(columns[i].ColumnName);
                        }
                    }
                    var message = string.Join(", ", badCollumns);
                    throw new InvalidOperationException($"Not found columns with name's {message}'");
                }
            }
        }

        public void CheckCondition(Condition condition)
        {
            if (condition is ConditionExpression)
            {
                var operand = (condition as Atdi.DataModels.DataConstraint.ConditionExpression).LeftOperand;
                if (operand is ColumnOperand)
                {
                    if (HasColumn((operand as ColumnOperand).ColumnName) == false)
                    {
                        var message = string.Join(", ", (operand as ColumnOperand).ColumnName);
                        throw new InvalidOperationException($"Not found columns with name's {message}'");
                    }
                }
            }
            else if (condition is ComplexCondition)
            {
               if (((condition as ComplexCondition).Conditions!=null) && (((condition as ComplexCondition).Conditions.Length>0)))
                {
                    for ( int i=0; i< ((condition as ComplexCondition).Conditions).Length; i++)
                    {
                       if (((condition as ComplexCondition).Conditions)[i] is Atdi.DataModels.DataConstraint.ConditionExpression)
                        {
                            CheckCondition(((condition as ComplexCondition).Conditions)[i] as Atdi.DataModels.DataConstraint.ConditionExpression);
                        }
                        else if (((condition as ComplexCondition).Conditions)[i] is Atdi.DataModels.DataConstraint.ComplexCondition) 
                        {
                            CheckCondition(((condition as ComplexCondition).Conditions)[i]);
                        }
                    }
                }

            }
        }

      
      

        public string[] PreperedColumnsForFetching(string[] columns)
        {
            var result = new string[columns.Length];
            for (int i=0; i< columns.Length; i++)
            {
                var metadat = IrpDictionary[columns[i]];
                if (metadat.TypeColumn== IrpColumnEnum.StandardColumn)
                {
                    result[i] = columns[i];
                }
                else if (metadat.TypeColumn == IrpColumnEnum.Expression)
                {
                    result[i] = metadat.Expr;
                }
                else
                {
                    throw new InvalidOperationException($"The type was unsupported");
                }
            }
            return result;
        }
       


        public Condition[] GetConditions(UserTokenData tokenData)
        {
            List<Condition> List_Expressions = new List<Condition>();
            if (_QueryValue != null) {
                
                if (!string.IsNullOrEmpty(_QueryValue.IDENTUSER)) {
                    string IdentUser =  _QueryValue.IDENTUSER;
                    var condition = new ConditionExpression() {
                        LeftOperand = new ColumnOperand() {
                            ColumnName = IdentUser
                        },
                        Operator = ConditionOperator.Equal,
                        Type = ConditionType.Expression,
                        RightOperand = new IntegerValueOperand() {
                          Value = tokenData.Id
                        }
                    };
                    List_Expressions.Add(condition);
                }
            }

            foreach (XWEBCONSTRAINT cntr in _ConstraintsValue) {
                if ((cntr.MIN != null) || (cntr.MAX != null)) {
                    string NameFldLon = "";
                    for (int i = 0; i < Metadata.Columns.Length; i++) {
                        if (Metadata.Columns[i].Name == cntr.PATH) {
                            NameFldLon = Metadata.Columns[i].Name;
                            if (!string.IsNullOrEmpty(NameFldLon)) {
                                if (cntr.INCLUDE == 1) {
                                    var condition = new ComplexCondition()
                                    {
                                        Operator = LogicalOperator.And,
                                        Conditions = new Condition[]
                                         {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = ConditionOperator.GreaterEqual,
                                             Type = ConditionType.Expression,
                                             RightOperand = new DoubleValueOperand(){
                                             Value = cntr.MIN
                                             }
                                          },
                                             new ConditionExpression() {
                                            LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                            },
                                            Operator = ConditionOperator.LessEqual,
                                            Type = ConditionType.Expression,
                                            RightOperand = new DoubleValueOperand() {
                                            Value = cntr.MAX
                                        }
                                    }

                                         }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.INCLUDE == 0) {

                                    var condition = new ComplexCondition()
                                    {
                                        Operator = LogicalOperator.Or,
                                        Conditions = new Condition[]
                                         {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = ConditionOperator.LessThan,
                                             Type = ConditionType.Expression,
                                             RightOperand = new DoubleValueOperand(){
                                             Value = cntr.MIN
                                             }
                                          },
                                             new ConditionExpression() {
                                            LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                            },
                                            Operator = ConditionOperator.GreaterThan,
                                            Type = ConditionType.Expression,
                                            RightOperand = new DoubleValueOperand() {
                                            Value = cntr.MAX
                                        }
                                    }

                                    }
                                    };
                                    List_Expressions.Add(condition);

                                }
                            }
                        }
                    }
                }
                else if (cntr.STRVALUE != null){
                    if (!string.IsNullOrEmpty(cntr.STRVALUE)) {
                        string NameFldLon = "";
                        for (int i = 0; i < Metadata.Columns.Length; i++) {
                            if (Metadata.Columns[i].Name == cntr.PATH) {
                                NameFldLon = Metadata.Columns[i].Name;
                                if (!string.IsNullOrEmpty(NameFldLon)) {
                                    if ((cntr.STRVALUE.EndsWith("*")) || (cntr.STRVALUE.StartsWith("*"))) {
                                        if (cntr.INCLUDE == 1) {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName = NameFldLon 
                                                },
                                                Operator = ConditionOperator.Like,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.STRVALUE.Replace("*","%")
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                        else  {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName =  NameFldLon 
                                                },
                                                Operator = ConditionOperator.NotLike,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.STRVALUE.Replace("*", "%")
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                    }
                                    else {
                                        if (cntr.INCLUDE == 1) {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName =  NameFldLon 
                                                },
                                                Operator = ConditionOperator.Equal,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.STRVALUE
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                        else {
                                            var condition = new ConditionExpression()  {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName =  NameFldLon 
                                                },
                                                Operator = ConditionOperator.NotEqual,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.STRVALUE
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if ((cntr.DATEVALUEMIN != null) || (cntr.DATEVALUEMAX != null)) {
                    string NameFldLon = "";
                    for (int i = 0; i < Metadata.Columns.Length; i++) {
                        if (Metadata.Columns[i].Name == cntr.PATH) {
                            NameFldLon = Metadata.Columns[i].Name;
                            if (!string.IsNullOrEmpty(NameFldLon)) {
                                if (cntr.INCLUDE == 1)
                                {
                                    var condition = new ComplexCondition()
                                    {
                                        Operator = LogicalOperator.And,
                                        Conditions = new Condition[]
                                         {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = ConditionOperator.GreaterEqual,
                                             Type = ConditionType.Expression,
                                             RightOperand = new DateTimeValueOperand(){
                                             Value = cntr.DATEVALUEMIN
                                             }
                                            },
                                             new ConditionExpression() {
                                            LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                            },
                                            Operator = ConditionOperator.LessEqual,
                                            Type = ConditionType.Expression,
                                            RightOperand = new DateTimeValueOperand() {
                                            Value = cntr.DATEVALUEMAX
                                            }
                                          }

                                         }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.INCLUDE == 0)
                                {
                                    var condition = new ComplexCondition()
                                    {
                                        Operator = LogicalOperator.Or,
                                        Conditions = new Condition[]
                                         {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = ConditionOperator.LessThan,
                                             Type = ConditionType.Expression,
                                             RightOperand = new DateTimeValueOperand(){
                                             Value = cntr.DATEVALUEMIN
                                             }
                                          },
                                             new ConditionExpression() {
                                            LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                            },
                                            Operator = ConditionOperator.GreaterThan,
                                            Type = ConditionType.Expression,
                                            RightOperand = new DateTimeValueOperand() {
                                            Value = cntr.DATEVALUEMAX
                                        }
                                    }

                                    }
                                    };
                                    List_Expressions.Add(condition);

                                }
                            }
                        }
                    }
                }
            }
            return List_Expressions.ToArray();
        }
    }
}
        
          
