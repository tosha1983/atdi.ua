using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels;
using Atdi.DataModels.DataConstraint;
using Atdi.DataModels.WebQuery;


namespace Atdi.AppServices.WebQuery
{
    public sealed class QueryDescriptor
    {
        public static int NullI = 2147483647;
        public static double NullD = 1E-99;
        public static DateTime NullT = new DateTime(1, 1, 1, 0, 0, 0);

        private readonly XWebConstraint[] _value_constraints;
        private readonly QueryGroup _value_group;
        private readonly XWebQuery _value_query;

        public QueryMetadata Metadata { get; set; }
       


        public QueryDescriptor(QueryGroup value_group, XWebConstraint[] value_constraints, XWebQuery value_query)
        {
            _value_group = value_group;
            _value_constraints = value_constraints;
            _value_query = value_query;
        }

        public bool CheckActionRight(UserTokenData tokenData, ActionType actionType)
        {
            if (_value_group != null)
            {
                if ((_value_group.Cust_Chb1 == 1) && (actionType == ActionType.Create))
                    return true;
                else if ((_value_group.Cust_Chb1 == 1) && (actionType == ActionType.Update))
                    return true;
                else if ((_value_group.Cust_Chb1 == 2) && (actionType == ActionType.Delete))
                    return true;
                else return false;
            }
            else return false;
        }

        public Condition[] GetConditions(UserTokenData tokenData, ActionType actionType)
        {
            List<Condition> List_Expressions = new List<Condition>();
            if (_value_query != null) {
                if (_value_query.IdentUser!="") {
                    var condition = new ConditionExpression() {
                        LeftOperand = new StringValueOperand() {
                            Type = OperandType.Column,
                            DataType = DataType.String,
                            Value = _value_query.IdentUser
                        },
                        Operator = ConditionOperator.Equal,
                        Type = ConditionType.Expression,
                        RightOperand = new IntegerValueOperand() {
                            Type = OperandType.Value,
                            DataType = DataType.Double,
                            Value = tokenData.Id
                        }
                    };
                    List_Expressions.Add(condition);
                }
            }

            foreach (XWebConstraint cntr in _value_constraints) {
                if ((cntr.Min != NullD) || (cntr.Max != NullD)) {
                    string NameFldLon = "";
                    for (int i = 0; i < Metadata.Columns.Count(); i++) {
                        if (Metadata.Columns[i].Name == cntr.Path) {
                            NameFldLon = Metadata.Columns[i].Name;
                            if (!string.IsNullOrEmpty(NameFldLon)) {
                                if (cntr.Include == 1) {
                                    var condition = new ConditionExpression(){
                                        LeftOperand = new StringValueOperand() {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand(){
                                            Type = OperandType.Value,
                                            DataType = DataType.Double,
                                            Value = cntr.Min
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression() {
                                        LeftOperand = new StringValueOperand() {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
                                            Type = OperandType.Value,
                                            DataType = DataType.Double,
                                            Value = cntr.Max
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.Include == 0) {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new StringValueOperand() {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
                                            Type = OperandType.Value,
                                            DataType = DataType.Double,
                                            Value = cntr.Min
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression(){
                                        LeftOperand = new StringValueOperand() {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
                                            Type = OperandType.Value,
                                            DataType = DataType.Double,
                                            Value = cntr.Max
                                        }
                                    };
                                    List_Expressions.Add(condition);

                                }
                            }
                        }
                    }
                }
                else if (cntr.StrValue != null){
                    if (cntr.StrValue != "") {
                        string NameFldLon = "";
                        for (int i = 0; i < Metadata.Columns.Count(); i++) {
                            if (Metadata.Columns[i].Name == cntr.Path) {
                                NameFldLon = Metadata.Columns[i].Name;
                                if (!string.IsNullOrEmpty(NameFldLon)) {
                                    if ((cntr.StrValue.EndsWith("*")) || (cntr.StrValue.StartsWith("*"))) {
                                        if (cntr.Include == 1) {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new StringValueOperand() {
                                                    Type = OperandType.Column,
                                                    DataType = DataType.String,
                                                    Value = NameFldLon
                                                },
                                                Operator = ConditionOperator.Like,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Type = OperandType.Value,
                                                    DataType = DataType.String,
                                                    Value = cntr.StrValue
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                        else  {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new StringValueOperand() {
                                                    Type = OperandType.Column,
                                                    DataType = DataType.String,
                                                    Value = NameFldLon
                                                },
                                                Operator = ConditionOperator.NotLike,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Type = OperandType.Value,
                                                    DataType = DataType.String,
                                                    Value = cntr.StrValue
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                    }
                                    else {
                                        if (cntr.Include == 1) {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new StringValueOperand() {
                                                    Type = OperandType.Column,
                                                    DataType = DataType.String,
                                                    Value = NameFldLon
                                                },
                                                Operator = ConditionOperator.Equal,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Type = OperandType.Value,
                                                    DataType = DataType.String,
                                                    Value = cntr.StrValue
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                        else {
                                            var condition = new ConditionExpression()  {
                                                LeftOperand = new StringValueOperand() {
                                                    Type = OperandType.Column,
                                                    DataType = DataType.String,
                                                    Value = NameFldLon
                                                },
                                                Operator = ConditionOperator.NotEqual,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Type = OperandType.Value,
                                                    DataType = DataType.String,
                                                    Value = cntr.StrValue
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
                if ((cntr.DateValueMin != NullT) || (cntr.DateValueMin != NullT)) {
                    string NameFldLon = "";
                    for (int i = 0; i < Metadata.Columns.Count(); i++) {
                        if (Metadata.Columns[i].Name == cntr.Path) {
                            NameFldLon = Metadata.Columns[i].Name;
                            if (!string.IsNullOrEmpty(NameFldLon)) {
                                if (cntr.Include == 1) {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new StringValueOperand() {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Type = OperandType.Value,
                                            DataType = DataType.DateTime,
                                            Value = cntr.DateValueMin
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression() {
                                        LeftOperand = new StringValueOperand() {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Type = OperandType.Value,
                                            DataType = DataType.DateTime,
                                            Value = cntr.DateValueMax
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.Include == 0)  {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new StringValueOperand()  {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Type = OperandType.Value,
                                            DataType = DataType.DateTime,
                                            Value = cntr.DateValueMin
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression() {
                                        LeftOperand = new StringValueOperand() {
                                            Type = OperandType.Column,
                                            DataType = DataType.String,
                                            Value = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand()
                                        {
                                            Type = OperandType.Value,
                                            DataType = DataType.DateTime,
                                            Value = cntr.DateValueMax
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
        
          
