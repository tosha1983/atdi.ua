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

        public QueryTokenDescriptor QueryTokenDescriptor { get; private set; }

        public QueryDescriptor(XWEBQUERY QueryValue, XWEBCONSTRAINT[] ConstraintsValue, IrpDescriptor irpdescription, QueryTokenDescriptor queryTokenDescriptor)
        {
            this._hashSet = new Dictionary<string, DataType>();
            this._QueryValue = QueryValue;
            this._ConstraintsValue = ConstraintsValue;
            this.QueryTokenDescriptor = queryTokenDescriptor;

            this.TableName = irpdescription.TableName;
            this.IdentUserField = QueryValue.IDENTUSER;
            this.Metadata = new QueryMetadata
            {
                Columns = irpdescription.columnMetaData,
                Name = QueryValue.NAME,
                Code = QueryValue.CODE,
                Token = queryTokenDescriptor.Token,
                Description = QueryValue.COMMENTS,
                Title = QueryValue.NAME
            };

            var listColumns = this.Metadata.Columns;
            for (int i = 0; i < listColumns.Length; i++)
            {
                var column = listColumns[i];
                if (!_hashSet.ContainsKey(column.Name))
                    _hashSet.Add(column.Name, column.Type);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasColumn(string nameColumn)
        {
            return this._hashSet.ContainsKey(nameColumn); 
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
        public string ValidateColumns(string[] columns)
        {
            string warnings = "";
            if (columns != null)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                   if (HasColumn(columns[i]) == false) warnings +="'" + columns[i]+"',";
                }
            }
            return warnings;
        }

        public string ValidateConditions(Condition[] conditions)
        {
            string warnings = "";
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] is ConditionExpression)
                    {
                        var operand = (conditions[i] as Atdi.DataModels.DataConstraint.ConditionExpression).LeftOperand;
                        if (operand is ColumnOperand)
                        {
                            string column = (operand as ColumnOperand).ColumnName;
                            if ( HasColumn(column)==false) warnings += "'"+column + "',";
                        }
                    }
                }
            }
            return warnings;
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
                                    var condition = new ConditionExpression(){
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName =  NameFldLon
                                },
                                        Operator = ConditionOperator.GreaterEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand(){
                                            Value = cntr.MIN
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon 
                                        },
                                        Operator = ConditionOperator.LessEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
                                            Value = cntr.MAX
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.INCLUDE == 0) {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName =  NameFldLon 
                                        },
                                        Operator = ConditionOperator.LessThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
                                            Value = cntr.MIN
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression(){
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName =  NameFldLon 
                                        },
                                        Operator = ConditionOperator.GreaterThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
                                            Value = cntr.MAX
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
                                    if ((cntr.STRVALUE.EndsWith(" * ")) || (cntr.STRVALUE.StartsWith("*"))) {
                                        if (cntr.INCLUDE == 1) {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName = NameFldLon 
                                                },
                                                Operator = ConditionOperator.Like,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.STRVALUE
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
                                                    Value = cntr.STRVALUE
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
                                if (cntr.INCLUDE == 1) {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Value = cntr.DATEVALUEMIN
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Value = cntr.DATEVALUEMAX
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.INCLUDE == 0)  {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand()  {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Value = cntr.DATEVALUEMIN
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand()
                                        {
                                            Value = cntr.DATEVALUEMAX
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
        
          
