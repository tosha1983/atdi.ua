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

namespace Atdi.AppServices.WebQuery
{
    public sealed class QueryDescriptor
    {
        public string IdentUserField { get; set; }
        public string TableName { get; set; }
        public QueryMetadata Metadata { get; set; }
        private  XWEBQUERY _QueryValue { get; set; }
        private XWEBCONSTRAINT[] _ConstraintsValue { get; set; }
        private Dictionary<string,DataType> _hashSet;

        public QueryDescriptor(XWEBQUERY QueryValue, XWEBCONSTRAINT[] ConstraintsValue, IrpDescriptor irpdescription)
        {
            _hashSet = new Dictionary<string, DataType>();
            _QueryValue = QueryValue;
            _ConstraintsValue = ConstraintsValue;
            Metadata = new QueryMetadata();
            Metadata.Columns = irpdescription.columnMetaData;
            IdentUserField = QueryValue.IDENTUSER;
            TableName = irpdescription.TableName;
            Metadata.Name = QueryValue.NAME;
            Metadata.Code = QueryValue.CODE;
            Metadata.Token = new QueryToken();
            Metadata.Token.Id = QueryValue.ID;
            Metadata.Token.Stamp = Guid.NewGuid().ToByteArray();
            Metadata.Token.Version = "1.0";
            Metadata.Description = QueryValue.COMMENTS;
            Metadata.Title = QueryValue.NAME;

            var listColumns = Metadata.Columns.ToList();
            for (int i = 0; i < listColumns.Count(); i++)   {
                if (!_hashSet.ContainsKey(listColumns[i].Name))   _hashSet.Add(listColumns[i].Name, listColumns[i].Type);
            }
        }


        public bool HasColumn(string nameColumn)
        {
            if (this._hashSet.ContainsKey(nameColumn)) return true;
            else
            {
                return false;
            }
        }

        public string ValidateColumns(string[] columns)
        {
            string warnings = "";
            if (columns != null)
            {
                for (int i = 0; i < columns.Count(); i++)
                {
                   if (HasColumn(columns[i]) == false) warnings +="'" + columns[i]+"',";
                }
            }
            return warnings;
        }

        public string ValidateColumns(DataModels.DataConstraint.Condition[] conditions)
        {
            string warnings = "";
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Count(); i++)
                {
                    if (conditions[i] is Atdi.DataModels.DataConstraint.ConditionExpression)
                    {
                        DataModels.DataConstraint.Operand operand = (conditions[i] as Atdi.DataModels.DataConstraint.ConditionExpression).LeftOperand;
                        if (operand is Atdi.DataModels.DataConstraint.ColumnOperand)
                        {
                            string column = (operand as Atdi.DataModels.DataConstraint.ColumnOperand).ColumnName;
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
                    for (int i = 0; i < Metadata.Columns.Count(); i++) {
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
                        for (int i = 0; i < Metadata.Columns.Count(); i++) {
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
                    for (int i = 0; i < Metadata.Columns.Count(); i++) {
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
        
          
