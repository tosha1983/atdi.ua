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
        public static IFormatProvider CultureEnUs = new System.Globalization.CultureInfo("en-US");
        private readonly XWEBQUERYATTRIBUTES[] _AttributesValue;
        private readonly XWEBCONSTRAINT[] _ConstraintsValue;
        private readonly Dictionary<string, DataType> _hashSet;
        private readonly XWEBQUERY _QueryValue;
        private const string _typeConditionGenerationCondition = "Data generation condition";
        private const string _typeConditionValidation = "Validation";
        private const string _momentOfUseAlways = "Always";
        private const string _momentOfUseCreate = "Create";
        private const string _momentOfUseUpdate = "Update";
        private const string _momentOfUseDelete = "Delete";



        public string IdentUserField { get; private set; }
        public string TableName { get; private set; }
        public QueryMetadata Metadata { get; private set; }
        public IrpColumn[] IrpDescrColumns { get; private set; }
        public Dictionary<string,IrpColumn> IrpDictionary { get; private set; }
        public QueryTokenDescriptor QueryTokenDescriptor { get; private set; }

        public QueryDescriptor(XWEBQUERY QueryValue, XWEBCONSTRAINT[] ConstraintsValue, XWEBQUERYATTRIBUTES[] AttributesValue, IrpDescriptor irpdescription, QueryTokenDescriptor queryTokenDescriptor)
        {
            this._hashSet = new Dictionary<string, DataType>();
            this.IrpDictionary = new Dictionary<string, IrpColumn>();
            this._QueryValue = QueryValue;
            this._ConstraintsValue = ConstraintsValue;
            this._AttributesValue = AttributesValue;
            this.QueryTokenDescriptor = queryTokenDescriptor;

            this.TableName = irpdescription.TableName;
            this.IdentUserField = QueryValue.IDENTUSER;
            this.IrpDescrColumns = irpdescription.irpColumns.ToArray();

            var checkColumnMetaData = this.IrpDescrColumns.Select(t => t.columnMeta).ToArray();
            for (int i = 0; i < checkColumnMetaData.Length; i++)
            {
                var column = checkColumnMetaData[i];
                var listAttributes = AttributesValue.ToList();
                if (listAttributes != null)
                {
                    var findAttribute = listAttributes.Find(t => t.PATH == column.Name);
                    if (findAttribute != null)
                    {
                        column.Readonly = findAttribute.READONLY==1? true : false;
                        column.NotChangeableByAdd = findAttribute.NOTCHANGEADD  == 1 ? true : false;
                        column.NotChangeableByEdit  = findAttribute.NOTCHANGEEDIT  == 1 ? true : false;
                    }
                }
            }


            this.Metadata = new QueryMetadata
            {
                Columns = checkColumnMetaData,
                Name = QueryValue.NAME,
                Code = QueryValue.CODE,
                Token = queryTokenDescriptor.Token,
                Description = QueryValue.COMMENTS,
                Title = QueryValue.NAME,
                PrimaryKey = irpdescription.PrimaryKey,
                UI = new QueryUIMetadata()
                {
                    AddFormColumns = QueryValue.ADDCOLUMNS!=null ? QueryValue.ADDCOLUMNS.Split(new char[] { ';', ',' }) : null,
                    EditFormColumns = QueryValue.EDITCOLUMNS != null ? QueryValue.EDITCOLUMNS.Split(new char[] { ';', ',' }) : null,
                    TableColumns = QueryValue.TABLECOLUMNS != null ? QueryValue.TABLECOLUMNS.Split(new char[] { ';', ',' }) : null,
                    ViewFormColumns = QueryValue.VIEWCOLUMNS  != null ? QueryValue.VIEWCOLUMNS.Split(new char[] { ';', ',' }) : null
                }
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

       
        public void GetAllColumnValuesFromCondition(Condition condition, ref List<ColumnValue> listColumnValues)
        {
            if (condition is ConditionExpression)
            {
                var operandLeft = (condition as Atdi.DataModels.DataConstraint.ConditionExpression).LeftOperand;
                var operandRight = (condition as Atdi.DataModels.DataConstraint.ConditionExpression).RightOperand;
                if (operandLeft.Type == OperandType.Column)
                {
                    var columnOperand = (operandLeft as ColumnOperand);
                    if (operandRight is ShortValueOperand)
                    {
                        var colValue = new ShortColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as ShortValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is UnsignedShortValueOperand)
                    {
                        var colValue = new UnsignedShortColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as UnsignedShortValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is UnsignedLongValueOperand)
                    {
                        var colValue = new UnsignedLongColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as UnsignedLongValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is UnsignedIntegerValueOperand)
                    {
                        var colValue = new UnsignedIntegerColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as UnsignedIntegerValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is DateValueOperand)
                    {
                        var colValue = new DateColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as DateValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is DateTimeValueOperand)
                    {
                        var colValue = new DateTimeColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as DateTimeValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is DecimalValueOperand)
                    {
                        var colValue = new DecimalColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as DecimalValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is DoubleValueOperand)
                    {
                        var colValue = new DoubleColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as DoubleValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is FloatValueOperand)
                    {
                        var colValue = new FloatColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as FloatValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is IntegerValueOperand)
                    {
                        var colValue = new IntegerColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as IntegerValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                    else if (operandRight is StringValueOperand)
                    {
                        var colValue = new StringColumnValue()
                        {
                            Name = columnOperand.ColumnName,
                            Source = columnOperand.Source,
                            Value = (operandRight as StringValueOperand).Value
                        };
                        listColumnValues.Add(colValue);
                    }
                }
              
            }
            else if (condition is ComplexCondition)
            {
                if (((condition as ComplexCondition).Conditions != null) && (((condition as ComplexCondition).Conditions.Length > 0)))
                {
                    for (int i = 0; i < ((condition as ComplexCondition).Conditions).Length; i++)
                    {
                        if (((condition as ComplexCondition).Conditions)[i] is Atdi.DataModels.DataConstraint.ConditionExpression)
                        {
                            GetAllColumnValuesFromCondition(((condition as ComplexCondition).Conditions)[i] as Atdi.DataModels.DataConstraint.ConditionExpression, ref listColumnValues);
                        }
                        else if (((condition as ComplexCondition).Conditions)[i] is Atdi.DataModels.DataConstraint.ComplexCondition)
                        {
                            GetAllColumnValuesFromCondition(((condition as ComplexCondition).Conditions)[i], ref listColumnValues);
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

        public void CheckValidation(XWEBCONSTRAINT cntr, ColumnValue columnValue, ConditionOperator conditionOperator, ActionType actionType, string typeCondition)
        {
            string errorMessage = "";
            bool validationChecked = true;
            if (columnValue != null)
            {
                if (cntr.MIN != null)
                {
                    if (columnValue is IntegerColumnValue)
                    {
                        var value = (columnValue as IntegerColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as IntegerColumnValue).Value = (int?)cntr.MIN;
                                value = (columnValue as IntegerColumnValue).Value;
                            }
                            else
                            {
                                if ((columnValue as IntegerColumnValue).Value == (int?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (int?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }

                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (int?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }

                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (int?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (int?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (int?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as IntegerColumnValue).Value = int.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as IntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (int?)cntr.MIN) && (value <= (int?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (int?)cntr.MIN) && (value <= (int?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }

                    }
                    else if (columnValue is DoubleColumnValue)
                    {
                        var value = (columnValue as DoubleColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as DoubleColumnValue).Value = (double?)cntr.MIN;
                                value = (columnValue as DoubleColumnValue).Value;
                            }
                            else
                            {
                                if (value == (double?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (double?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (double?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (double?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (double?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (double?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DoubleColumnValue).Value = double.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DoubleColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (double?)cntr.MIN) && (value <= (double?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (double?)cntr.MIN) && (value <= (double?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else if (columnValue is FloatColumnValue)
                    {
                        var value = (columnValue as FloatColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as FloatColumnValue).Value = (float?)cntr.MIN;
                                value = (columnValue as FloatColumnValue).Value;
                            }
                            else
                            {
                                if (value == (float?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (float?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (float?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (float?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (float?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (float?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }

                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as FloatColumnValue).Value = float.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as FloatColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (float?)cntr.MIN) && (value <= (float?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (float?)cntr.MIN) && (value <= (float?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }

                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else if (columnValue is ShortColumnValue)
                    {
                        var value = (columnValue as ShortColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as ShortColumnValue).Value = (short?)cntr.MIN;
                                value = (columnValue as ShortColumnValue).Value;
                            }
                            else
                            {
                                if (value == (short?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (short?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (short?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (short?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (short?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (short?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as ShortColumnValue).Value = short.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as ShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (short?)cntr.MIN) && (value <= (short?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (short?)cntr.MIN) && (value <= (short?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else if (columnValue is UnsignedShortColumnValue)
                    {

                        var value = (columnValue as UnsignedShortColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as UnsignedShortColumnValue).Value = (ushort?)cntr.MIN;
                                value = (columnValue as UnsignedShortColumnValue).Value;
                            }
                            else
                            {
                                if (value == (ushort?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }

                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (ushort?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (ushort?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (ushort?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (ushort?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (ushort?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedShortColumnValue).Value = ushort.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedShortColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (ushort?)cntr.MIN) && (value <= (ushort?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (ushort?)cntr.MIN) && (value <= (ushort?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else if (columnValue is UnsignedIntegerColumnValue)
                    {
                        var value = (columnValue as UnsignedIntegerColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as UnsignedIntegerColumnValue).Value = (uint?)cntr.MIN;
                                value = (columnValue as UnsignedIntegerColumnValue).Value;
                            }
                            else
                            {
                                if (value == (uint?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (uint?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (uint?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (uint?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (uint?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (uint?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedIntegerColumnValue).Value = uint.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedIntegerColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (uint?)cntr.MIN) && (value <= (uint?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (uint?)cntr.MIN) && (value <= (uint?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else if (columnValue is UnsignedLongColumnValue)
                    {
                        var value = (columnValue as UnsignedLongColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as UnsignedLongColumnValue).Value = (ulong?)cntr.MIN;
                                value = (columnValue as UnsignedLongColumnValue).Value;
                            }
                            else
                            {
                                if (value == (ulong?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (ulong?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (ulong?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (ulong?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (ulong?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (ulong?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as UnsignedLongColumnValue).Value = ulong.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as UnsignedLongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (ulong?)cntr.MIN) && (value <= (ulong?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (ulong?)cntr.MIN) && (value <= (ulong?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else if (columnValue is LongColumnValue)
                    {
                        var value = (columnValue as LongColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as LongColumnValue).Value = (long?)cntr.MIN;
                                value = (columnValue as LongColumnValue).Value;
                            }
                            else
                            {
                                if (value == (long?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (long?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (long?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (long?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (long?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (long?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as LongColumnValue).Value = long.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as LongColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (long?)cntr.MIN) && (value <= (long?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (long?)cntr.MIN) && (value <= (long?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else if (columnValue is DecimalColumnValue)
                    {
                        var value = (columnValue as DecimalColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as DecimalColumnValue).Value = (decimal?)cntr.MIN;
                                value = (columnValue as DecimalColumnValue).Value;
                            }
                            else
                            {
                                if (value == (decimal?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (decimal?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (decimal?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (decimal?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (decimal?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (decimal?)cntr.MIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DecimalColumnValue).Value = decimal.Parse(cntr.DEFAULTVALUE);
                                    value = (columnValue as DecimalColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (decimal?)cntr.MIN) && (value <= (decimal?)cntr.MAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (decimal?)cntr.MIN) && (value <= (decimal?)cntr.MAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                    }


                    if (validationChecked == false)
                    {
                        errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                    }
                }
                if (cntr.DATEVALUEMIN != null)
                {
                    if (columnValue is DateTimeColumnValue)
                    {
                        var value = (columnValue as DateTimeColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                (columnValue as DateTimeColumnValue).Value = cntr.DATEVALUEMIN;
                                value = (columnValue as DateTimeColumnValue).Value;
                            }
                            else
                            {
                                if (value == (DateTime?)cntr.DATEVALUEMIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value >= (DateTime?)cntr.DATEVALUEMIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.GreaterThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value > (DateTime?)cntr.DATEVALUEMIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value <= (DateTime?)cntr.DATEVALUEMIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.LessThan)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value < (DateTime?)cntr.DATEVALUEMIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != (DateTime?)cntr.DATEVALUEMIN)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.Between) || (conditionOperator == ConditionOperator.NotBetween))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as DateTimeColumnValue).Value = DateTime.ParseExact(cntr.DEFAULTVALUE, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                    value = (columnValue as DateTimeColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.Between)
                                {
                                    if ((value >= (DateTime?)cntr.DATEVALUEMIN) && (value <= (DateTime?)cntr.DATEVALUEMAX))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotBetween)
                                {
                                    if (((value >= (DateTime?)cntr.DATEVALUEMIN) && (value <= (DateTime?)cntr.DATEVALUEMAX)) == false)
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                    }


                    if (validationChecked == false)
                    {
                        errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                    }
                }
                if (cntr.STRVALUE != null)
                {
                    if (columnValue is StringColumnValue)
                    {
                        var value = (columnValue as StringColumnValue).Value;
                        if (conditionOperator == ConditionOperator.Equal)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as StringColumnValue).Value = cntr.STRVALUE;
                                    value = (columnValue as StringColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == cntr.STRVALUE)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotEqual)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as StringColumnValue).Value = cntr.DEFAULTVALUE;
                                    value = (columnValue as StringColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != cntr.STRVALUE)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.IsNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as StringColumnValue).Value = cntr.DEFAULTVALUE;
                                    value = (columnValue as StringColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value == null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }

                        }
                        else if (conditionOperator == ConditionOperator.IsNotNull)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as StringColumnValue).Value = cntr.DEFAULTVALUE;
                                    value = (columnValue as StringColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.Like)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as StringColumnValue).Value = cntr.DEFAULTVALUE;
                                    value = (columnValue as StringColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (cntr.STRVALUE.Contains(value))
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if (conditionOperator == ConditionOperator.NotLike)
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as StringColumnValue).Value = cntr.DEFAULTVALUE;
                                    value = (columnValue as StringColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (!cntr.STRVALUE.Contains(value))
                                {
                                    validationChecked = true;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                        }
                        else if ((conditionOperator == ConditionOperator.In) || (conditionOperator == ConditionOperator.NotIn))
                        {
                            if ((actionType == ActionType.Create) && (typeCondition == _typeConditionGenerationCondition))
                            {
                                if (!string.IsNullOrEmpty(cntr.DEFAULTVALUE))
                                {
                                    (columnValue as StringColumnValue).Value = cntr.DEFAULTVALUE;
                                    value = (columnValue as StringColumnValue).Value;
                                }
                                else
                                {
                                    validationChecked = false;
                                }
                            }
                            else
                            {
                                if (conditionOperator == ConditionOperator.In)
                                {
                                    if (cntr.STRVALUE.Contains(value))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                                else if (conditionOperator == ConditionOperator.NotIn)
                                {
                                    if (!cntr.STRVALUE.Contains(value))
                                    {
                                        validationChecked = true;
                                    }
                                    else
                                    {
                                        validationChecked = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException(Exceptions.HandlerOperationNotImplemented);
                    }
                    if (validationChecked == false)
                    {
                        errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                    }
                }
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    throw new Exception(errorMessage);
                }
            }
        }


        public ComplexCondition GetComplexCondition(XWEBCONSTRAINT constraint, string NameFldLon)
        {
            ComplexCondition condition = null;
            if (constraint.MIN != null)
            {
                condition = new ComplexCondition()
                {
                    Operator = LogicalOperator.And,
                    Conditions = new Condition[]
                     {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), constraint.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new DoubleValueOperand(){
                                             Value = constraint.MIN
                                             }
                                             }
                     }
                };
            }
            if (constraint.DATEVALUEMIN != null)
            {
                condition = new ComplexCondition()
                {
                    Operator = LogicalOperator.And,
                    Conditions = new Condition[]
                     {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), constraint.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new DateTimeValueOperand(){
                                             Value =  constraint.DATEVALUEMIN
                                             }
                                             }
                     }
                };
            }
            if (constraint.STRVALUE != null)
            {
                condition = new ComplexCondition()
                {
                    Operator = LogicalOperator.And,
                    Conditions = new Condition[]
                     {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), constraint.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new StringValueOperand(){
                                             Value = constraint.STRVALUE
                                             }
                                             }
                     }
                };
            }
            return condition;
        }

        public bool PrapareValidationConditions(UserTokenData tokenData, ColumnValue[] columnValues, Atdi.DataModels.Action actionType = null)
        {
            bool validationChecked = false;
            string errorMessage = "";
            foreach (XWEBCONSTRAINT cntr in _ConstraintsValue)
            {
                string NameFldLon = "";
                if (cntr.TYPECONDITION == _typeConditionValidation)
                {
                    for (int i = 0; i < Metadata.Columns.Length; i++)
                    {
                        var columnValue = columnValues.ToList().Find(z => z.Name == cntr.PATH);
                        if (columnValue != null)
                        {
                            if ((Metadata.Columns[i].Name == cntr.PATH) && (columnValue.Name == cntr.PATH))
                            {
                                if (((actionType.Type == ActionType.Create) && ((cntr.MOMENTOFUSE == _momentOfUseAlways) || (cntr.MOMENTOFUSE == _momentOfUseCreate)))
                                      || ((actionType.Type == ActionType.Update) && ((cntr.MOMENTOFUSE == _momentOfUseAlways) || (cntr.MOMENTOFUSE == _momentOfUseUpdate)))
                                      || ((actionType.Type == ActionType.Delete) && ((cntr.MOMENTOFUSE == _momentOfUseAlways) || (cntr.MOMENTOFUSE == _momentOfUseDelete))))
                                {

                                    NameFldLon = Metadata.Columns[i].Name;
                                    if (!string.IsNullOrEmpty(NameFldLon))
                                    {
                                        if (columnValues != null)
                                        {
                                            var findColumnValue = columnValues.ToList().Find(z => z.Name == cntr.PATH);
                                            if (findColumnValue != null)
                                            {
                                                CheckValidation(cntr, findColumnValue, (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION), actionType.Type, cntr.TYPECONDITION);
                                            }
                                        }

                                        var condition = new ComplexCondition();
                                        if ((cntr.OPERCONDITION == ConditionOperator.Between.ToString()) || (cntr.OPERCONDITION == ConditionOperator.NotBetween.ToString()))
                                        {
                                            if ((cntr.MIN != null) && (cntr.MAX != null))
                                            {
                                                if (columnValue is IntegerColumnValue)
                                                {
                                                    var value = (columnValue as IntegerColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is DoubleColumnValue)
                                                {
                                                    var value = (columnValue as DoubleColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is FloatColumnValue)
                                                {
                                                    var value = (columnValue as FloatColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is ShortColumnValue)
                                                {
                                                    var value = (columnValue as ShortColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is UnsignedShortColumnValue)
                                                {
                                                    var value = (columnValue as UnsignedShortColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is UnsignedIntegerColumnValue)
                                                {
                                                    var value = (columnValue as UnsignedIntegerColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is UnsignedLongColumnValue)
                                                {
                                                    var value = (columnValue as UnsignedLongColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is ShortColumnValue)
                                                {
                                                    var value = (columnValue as ShortColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is LongColumnValue)
                                                {
                                                    var value = (columnValue as LongColumnValue).Value;
                                                    if ((value >= cntr.MIN) && (value <= cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }
                                                else if (columnValue is DecimalColumnValue)
                                                {
                                                    var value = (columnValue as DecimalColumnValue).Value;
                                                    if ((value >= (decimal?)cntr.MIN) && (value <= (decimal?)cntr.MAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }

                                                if (validationChecked == false)
                                                {
                                                    errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                                                }
                                            }
                                            if ((cntr.DATEVALUEMIN != null) && (cntr.DATEVALUEMAX != null))
                                            {
                                                if (columnValue is DateTimeColumnValue)
                                                {
                                                    var value = (columnValue as DateTimeColumnValue).Value;
                                                    if ((value >= cntr.DATEVALUEMIN) && (value <= cntr.DATEVALUEMAX))
                                                    {
                                                        validationChecked = true;
                                                    }
                                                }

                                                if (validationChecked == false)
                                                {
                                                    errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                                                }
                                            }
                                            if ((cntr.STRVALUE != null) && (cntr.STRVALUETO != null))
                                            {
                                                if (validationChecked == false)
                                                {
                                                    errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                                                    throw new NotImplementedException(Exceptions.OperationBetweenNotSupportedfoString);
                                                }
                                            }
                                        }
                                        else if ((cntr.OPERCONDITION == ConditionOperator.In.ToString()) || (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                        {
                                            if (cntr.STRVALUE != null)
                                            {

                                                if (Metadata.Columns[i].Type == DataType.String)
                                                {
                                                    string inValues = cntr.STRVALUE;
                                                    string[] inValuesString = null;
                                                    if (!string.IsNullOrEmpty(inValues))
                                                    {
                                                        string[] wrd = inValues.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                        if (wrd != null)
                                                        {
                                                            inValuesString = wrd;

                                                        }
                                                    }

                                                    if (columnValue is StringColumnValue)
                                                    {
                                                        var value = (columnValue as StringColumnValue).Value;
                                                        if ((inValuesString.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesString.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }

                                                    }

                                                    if (validationChecked == false)
                                                    {
                                                        errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                                                    }

                                                }
                                                else if ((Metadata.Columns[i].Type == DataType.Integer) || (Metadata.Columns[i].Type == DataType.Short) || (Metadata.Columns[i].Type == DataType.Long) || (Metadata.Columns[i].Type == DataType.Float) || (Metadata.Columns[i].Type == DataType.Double) || (Metadata.Columns[i].Type == DataType.Decimal))
                                                {
                                                    string inValues = cntr.STRVALUE;
                                                    double?[] inValuesDouble = null;
                                                    if (!string.IsNullOrEmpty(inValues))
                                                    {
                                                        string[] wrd = inValues.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                        if (wrd != null)
                                                        {
                                                            inValuesDouble = new double?[wrd.Length];
                                                            for (int j = 0; j < wrd.Length; j++)
                                                            {
                                                                inValuesDouble[j] = double.Parse(wrd[j]);
                                                            }
                                                        }
                                                    }

                                                    if (columnValue is IntegerColumnValue)
                                                    {
                                                        var value = (columnValue as IntegerColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is DoubleColumnValue)
                                                    {
                                                        var value = (columnValue as DoubleColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is FloatColumnValue)
                                                    {
                                                        var value = (columnValue as FloatColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is ShortColumnValue)
                                                    {
                                                        var value = (columnValue as ShortColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is UnsignedShortColumnValue)
                                                    {
                                                        var value = (columnValue as UnsignedShortColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is UnsignedIntegerColumnValue)
                                                    {
                                                        var value = (columnValue as UnsignedIntegerColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is UnsignedLongColumnValue)
                                                    {
                                                        var value = (columnValue as UnsignedLongColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is ShortColumnValue)
                                                    {
                                                        var value = (columnValue as ShortColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is LongColumnValue)
                                                    {
                                                        var value = (columnValue as LongColumnValue).Value;
                                                        if ((inValuesDouble.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }
                                                    else if (columnValue is DecimalColumnValue)
                                                    {
                                                        var value = (columnValue as DecimalColumnValue).Value;
                                                        if ((inValuesDouble.Contains((double?)value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDouble.Contains((double?)value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                    }

                                                    if (validationChecked == false)
                                                    {
                                                        errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                                                    }

                                                }
                                                else if ((Metadata.Columns[i].Type == DataType.Date) || (Metadata.Columns[i].Type == DataType.DateTime))
                                                {
                                                    string inValues = cntr.STRVALUE;
                                                    DateTime?[] inValuesDateTime = null;
                                                    if (!string.IsNullOrEmpty(inValues))
                                                    {
                                                        string[] wrd = inValues.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                        if (wrd != null)
                                                        {
                                                            inValuesDateTime = new DateTime?[wrd.Length];
                                                            for (int j = 0; j < wrd.Length; j++)
                                                            {
                                                                inValuesDateTime[j] = DateTime.ParseExact(wrd[j], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                                            }
                                                        }
                                                    }
                                                    if (columnValue is DateTimeColumnValue)
                                                    {
                                                        var value = (columnValue as DateTimeColumnValue).Value;
                                                        if ((inValuesDateTime.Contains(value)) && (cntr.OPERCONDITION == ConditionOperator.In.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }
                                                        else if ((inValuesDateTime.Contains(value) == false) && (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                                        {
                                                            validationChecked = true;
                                                        }

                                                    }

                                                    if (validationChecked == false)
                                                    {
                                                        errorMessage = string.Format(cntr.MESSAGENOTVALID, cntr.PATH);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            CheckValidation(cntr, columnValue, (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION), actionType.Type, cntr.TYPECONDITION);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

         if (!string.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);
            return validationChecked;
        }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="tokenData"></param>
            /// <returns></returns>
        public Condition[] GetConditions(UserTokenData tokenData, ColumnValue[] columnValues = null, Atdi.DataModels.Action actionType = null)
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
                          Value = tokenData.UserId
                        }
                    };
                    List_Expressions.Add(condition);
                }
            }

            foreach (XWEBCONSTRAINT cntr in _ConstraintsValue)
            {
                string NameFldLon = "";
                if (cntr.TYPECONDITION == _typeConditionGenerationCondition)
                {
                    for (int i = 0; i < Metadata.Columns.Length; i++)
                    {
                        if (Metadata.Columns[i].Name == cntr.PATH)
                        {
                            NameFldLon = Metadata.Columns[i].Name;
                            if (!string.IsNullOrEmpty(NameFldLon))
                            {
                                if (columnValues != null)
                                {
                                    var findColumnValue = columnValues.ToList().Find(z => z.Name == cntr.PATH);
                                    if (findColumnValue != null)
                                    {
                                        CheckValidation(cntr, findColumnValue, (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION), actionType.Type, cntr.TYPECONDITION);
                                    }
                                }
                                var condition = new ComplexCondition();
                                if ((cntr.OPERCONDITION == ConditionOperator.Between.ToString()) || (cntr.OPERCONDITION == ConditionOperator.NotBetween.ToString()))
                                {
                                    if ((cntr.MIN != null) && (cntr.MAX != null))
                                    {
                                        condition = new ComplexCondition()
                                        {
                                            Operator = LogicalOperator.And,
                                            Conditions = new Condition[]
                                             {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new DoubleValuesOperand(){
                                             Values = new double?[] {  cntr.MIN, cntr.MAX}
                                             }
                                          }
                                          }
                                        };
                                    }
                                    if ((cntr.DATEVALUEMIN != null) && (cntr.DATEVALUEMAX != null))
                                    {
                                        condition = new ComplexCondition()
                                        {
                                            Operator = LogicalOperator.And,
                                            Conditions = new Condition[]
                                             {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new DateTimeValuesOperand(){
                                             Values = new DateTime?[] { cntr.DATEVALUEMIN, cntr.DATEVALUEMAX }
                                             }
                                             }
                                             }
                                        };
                                    }
                                    if ((cntr.STRVALUE != null) && (cntr.STRVALUETO != null))
                                    {
                                        condition = new ComplexCondition()
                                        {
                                            Operator = LogicalOperator.And,
                                            Conditions = new Condition[]
                                             {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new StringValuesOperand(){
                                             Values = new String[] { cntr.STRVALUE, cntr.STRVALUETO }
                                             }
                                             }
                                             }
                                        };
                                    }
                                }
                                else if ((cntr.OPERCONDITION == ConditionOperator.In.ToString()) || (cntr.OPERCONDITION == ConditionOperator.NotIn.ToString()))
                                {
                                    if (cntr.STRVALUE != null)
                                    {

                                        if (Metadata.Columns[i].Type == DataType.String)
                                        {
                                            string inValues = cntr.STRVALUE;
                                            string[] inValuesString = null;
                                            if (!string.IsNullOrEmpty(inValues))
                                            {
                                                string[] wrd = inValues.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                if (wrd != null)
                                                {
                                                    inValuesString = wrd;
                                                }
                                            }

                                            condition = new ComplexCondition()
                                            {
                                                Operator = LogicalOperator.And,
                                                Conditions = new Condition[]
                                                 {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator =  (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new StringValuesOperand(){
                                             Values = inValuesString
                                             }
                                             }
                                                 }
                                            };
                                        }
                                        else if ((Metadata.Columns[i].Type == DataType.Integer) || (Metadata.Columns[i].Type == DataType.Short) || (Metadata.Columns[i].Type == DataType.Long) || (Metadata.Columns[i].Type == DataType.Float) || (Metadata.Columns[i].Type == DataType.Double) || (Metadata.Columns[i].Type == DataType.Decimal))
                                        {
                                            string inValues = cntr.STRVALUE;
                                            double?[] inValuesDouble = null;
                                            if (!string.IsNullOrEmpty(inValues))
                                            {
                                                string[] wrd = inValues.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                if (wrd != null)
                                                {
                                                    inValuesDouble = new double?[wrd.Length];
                                                    for (int j = 0; j < wrd.Length; j++)
                                                    {
                                                        inValuesDouble[j] = double.Parse(wrd[j]);
                                                    }
                                                }
                                            }
                                            condition = new ComplexCondition()
                                            {
                                                Operator = LogicalOperator.And,
                                                Conditions = new Condition[]
                                                 {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                             ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new DoubleValuesOperand(){
                                             Values =  inValuesDouble
                                             }
                                             }
                                                 }
                                            };
                                        }
                                        else if ((Metadata.Columns[i].Type == DataType.Date) || (Metadata.Columns[i].Type == DataType.DateTime))
                                        {
                                            string inValues = cntr.STRVALUE;
                                            DateTime?[] inValuesDateTime = null;
                                            if (!string.IsNullOrEmpty(inValues))
                                            {
                                                string[] wrd = inValues.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                if (wrd != null)
                                                {
                                                    inValuesDateTime = new DateTime?[wrd.Length];
                                                    for (int j = 0; j < wrd.Length; j++)
                                                    {
                                                        inValuesDateTime[j] = DateTime.ParseExact(wrd[j], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                                    }
                                                }
                                            }
                                            condition = new ComplexCondition()
                                            {
                                                Operator = LogicalOperator.And,
                                                Conditions = new Condition[]
                                                 {
                                             new ConditionExpression(){
                                             LeftOperand = new ColumnOperand() {
                                              ColumnName =  NameFldLon
                                             },
                                             Operator = (ConditionOperator)Enum.Parse(typeof(ConditionOperator), cntr.OPERCONDITION),
                                             Type = ConditionType.Expression,
                                             RightOperand = new DateTimeValuesOperand(){
                                             Values =  inValuesDateTime
                                             }
                                             }
                                             }
                                            };
                                        }
                                    }
                                }
                                else
                                {
                                    condition = GetComplexCondition(cntr, NameFldLon);
                                }
                                List_Expressions.Add(condition);
                            }
                        }
                    }
                }
            }
            return List_Expressions.ToArray();
        }
    }
}
        
          
