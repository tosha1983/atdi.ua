﻿using System;
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
        private readonly XWebConstraint[] _valueConstraints;
        private readonly QueryGroup _valueGroup;
        private readonly XWebQuery _valueQuery;
        public QueryMetadata Metadata { get; set; }
       


        public QueryDescriptor(QueryGroup valueGroup, XWebConstraint[] valueConstraints, XWebQuery valueQuery)
        {
            _valueGroup = valueGroup;
            _valueConstraints = valueConstraints;
            _valueQuery = valueQuery;
        }

        public bool CheckActionRight(UserTokenData tokenData, ActionType actionType)
        {
            bool Ret = false;
            if (_valueGroup != null)
            {
                if ((_valueGroup.CanCreateAndModify == 1) && (actionType == ActionType.Create))
                    Ret = true;
                if ((_valueGroup.CanCreateAndModify == 1) && (actionType == ActionType.Update))
                    Ret = true;
                if ((_valueGroup.CanDelete == 1) && (actionType == ActionType.Delete))
                    Ret = true;
            }
            else Ret = false;
            return Ret;
        }

        public Condition[] GetConditions(UserTokenData tokenData, ActionType actionType)
        {
            List<Condition> List_Expressions = new List<Condition>();
            if (_valueQuery != null) {
                if (!string.IsNullOrEmpty(_valueQuery.IdentUser)) {
                    var condition = new ConditionExpression() {
                        LeftOperand = new ColumnOperand() {
                             ColumnName = _valueQuery.IdentUser
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

            foreach (XWebConstraint cntr in _valueConstraints) {
                if ((cntr.Min != null) || (cntr.Max != null)) {
                    string NameFldLon = "";
                    for (int i = 0; i < Metadata.Columns.Count(); i++) {
                        if (Metadata.Columns[i].Description == cntr.Path) {
                            NameFldLon = Metadata.Columns[i].Description;
                            if (!string.IsNullOrEmpty(NameFldLon)) {
                                if (cntr.Include == 1) {
                                    var condition = new ConditionExpression(){
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand(){
                                            Value = cntr.Min
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
                                            Value = cntr.Max
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.Include == 0) {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
                                            Value = cntr.Min
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                    condition = new ConditionExpression(){
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DoubleValueOperand() {
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
                    if (!string.IsNullOrEmpty(cntr.StrValue)) {
                        string NameFldLon = "";
                        for (int i = 0; i < Metadata.Columns.Count(); i++) {
                            if (Metadata.Columns[i].Description == cntr.Path) {
                                NameFldLon = Metadata.Columns[i].Description;
                                if (!string.IsNullOrEmpty(NameFldLon)) {
                                    if ((cntr.StrValue.EndsWith("*")) || (cntr.StrValue.StartsWith("*"))) {
                                        if (cntr.Include == 1) {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName = NameFldLon
                                                },
                                                Operator = ConditionOperator.Like,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.StrValue
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                        else  {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName = NameFldLon
                                                },
                                                Operator = ConditionOperator.NotLike,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.StrValue
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                    }
                                    else {
                                        if (cntr.Include == 1) {
                                            var condition = new ConditionExpression() {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName = NameFldLon
                                                },
                                                Operator = ConditionOperator.Equal,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
                                                    Value = cntr.StrValue
                                                }
                                            };
                                            List_Expressions.Add(condition);
                                        }
                                        else {
                                            var condition = new ConditionExpression()  {
                                                LeftOperand = new ColumnOperand() {
                                                    ColumnName = NameFldLon
                                                },
                                                Operator = ConditionOperator.NotEqual,
                                                Type = ConditionType.Expression,
                                                RightOperand = new StringValueOperand() {
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
                if ((cntr.DateValueMin != null) || (cntr.DateValueMin != null)) {
                    string NameFldLon = "";
                    for (int i = 0; i < Metadata.Columns.Count(); i++) {
                        if (Metadata.Columns[i].Description == cntr.Path) {
                            NameFldLon = Metadata.Columns[i].Description;
                            if (!string.IsNullOrEmpty(NameFldLon)) {
                                if (cntr.Include == 1) {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand() {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.GreaterEqual,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Value = cntr.DateValueMin
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
                                            Value = cntr.DateValueMax
                                        }
                                    };
                                    List_Expressions.Add(condition);
                                }
                                if (cntr.Include == 0)  {
                                    var condition = new ConditionExpression() {
                                        LeftOperand = new ColumnOperand()  {
                                            ColumnName = NameFldLon
                                        },
                                        Operator = ConditionOperator.LessThan,
                                        Type = ConditionType.Expression,
                                        RightOperand = new DateTimeValueOperand() {
                                            Value = cntr.DateValueMin
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
        
          
