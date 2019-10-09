using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.DataModels.CoverageCalculation;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using Atdi.WebQuery.CoverageCalculation;

namespace Atdi.WebQuery.CoverageCalculation
{
    public class CreateCondition
    {
        private CodeOperatorAndStatusConfig _dataConfig { get; set; }
        private ILogger _logger { get; set; }
        public CreateCondition(CodeOperatorAndStatusConfig dataConfig, ILogger logger)
        {
            this._dataConfig = dataConfig;
            this._logger = logger;
        }

        public void CheckCondition(Condition condition, ref bool isCorrectCondition)
        {
            if (condition is ConditionExpression)
            {
                if ((condition as ConditionExpression) != null)
                {
                    isCorrectCondition = true;
                }
                else
                {
                    isCorrectCondition = false;
                }
            }
            else if (condition is ComplexCondition)
            {
                if (((condition as ComplexCondition).Conditions != null) && (((condition as ComplexCondition).Conditions.Length > 0)))
                {
                    for (int i = 0; i < ((condition as ComplexCondition).Conditions).Length; i++)
                    {
                        if (((condition as ComplexCondition).Conditions)[i] is ConditionExpression)
                        {
                            CheckCondition(((condition as ComplexCondition).Conditions)[i] as ConditionExpression, ref isCorrectCondition);
                        }
                        else if (((condition as ComplexCondition).Conditions)[i] is ComplexCondition)
                        {
                            CheckCondition(((condition as ComplexCondition).Conditions)[i], ref isCorrectCondition);
                        }
                    }
                }
                else
                {
                    isCorrectCondition = false;
                }
            }
        }

        public Condition GetCondition()
        {
            var complexCondition = new ComplexCondition();
            try
            {
                complexCondition.Operator = LogicalOperator.And;

                var complexConditionCodeOperatorConfig = new ComplexCondition();
                complexConditionCodeOperatorConfig.Operator = LogicalOperator.Or;
                var lsitCodeOperatorConfigOr = new List<ConditionExpression>();


                var complexConditionprovincesConfig = new ComplexCondition();
                complexConditionprovincesConfig.Operator = LogicalOperator.Or;
                var lsitConditionprovincesConfigOr = new List<ConditionExpression>();


                var lsitConditionExpressionsAnd = new List<ConditionExpression>();

                var codeOperatorAndStatusesConfig = this._dataConfig;

                if (codeOperatorAndStatusesConfig.StandardConfig != null)
                {
                    if (!string.IsNullOrEmpty(codeOperatorAndStatusesConfig.StandardConfig.Name))
                    {
                        lsitConditionExpressionsAnd.Add(
                        new ConditionExpression()
                        {
                            LeftOperand = new ColumnOperand { ColumnName = "STANDARD" },
                            Operator = ConditionOperator.Equal,
                            RightOperand = new StringValueOperand { Value = codeOperatorAndStatusesConfig.StandardConfig.Name }
                        });
                    }

                    if (codeOperatorAndStatusesConfig.StandardConfig.provincesConfig != null)
                    {
                        var tempLsitConditionprovincesConfigOr = new List<Condition>();
                        for (int v = 0; v < codeOperatorAndStatusesConfig.StandardConfig.provincesConfig.Length; v++)
                        {
                            lsitCodeOperatorConfigOr = new List<ConditionExpression>();
                            var complexConditionOwnerAndProvince = new ComplexCondition();
                            var codeOperatorConfig = codeOperatorAndStatusesConfig.StandardConfig.provincesConfig[v];
                            var tempConditionOwnerAndProvince = new List<Condition>();
                            if (codeOperatorConfig != null)
                            {
                                complexConditionOwnerAndProvince.Operator = LogicalOperator.And;
                                lsitCodeOperatorConfigOr.Add(
                                                new ConditionExpression()
                                                {
                                                    LeftOperand = new ColumnOperand { ColumnName = "Position.PROVINCE" },
                                                    Operator = ConditionOperator.Equal,
                                                    RightOperand = new StringValueOperand { Value = codeOperatorConfig.Name }
                                                });




                                if (codeOperatorConfig.CodeOperatorConfig != null)
                                {
                                    lsitConditionprovincesConfigOr = new List<ConditionExpression>();

                                    for (int p = 0; p < codeOperatorConfig.CodeOperatorConfig.Length; p++)
                                    {
                                        var provincesConfig = codeOperatorConfig.CodeOperatorConfig[p];

                                        if (provincesConfig != null)
                                        {

                                            lsitConditionprovincesConfigOr.Add(
                                                new ConditionExpression()
                                                {
                                                    LeftOperand = new ColumnOperand { ColumnName = "Owner.CODE" },
                                                    Operator = ConditionOperator.Equal,
                                                    RightOperand = new StringValueOperand { Value = provincesConfig.Code }
                                                });
                                        }
                                    }
                                    complexConditionprovincesConfig.Conditions = lsitConditionprovincesConfigOr.ToArray();
                                    tempConditionOwnerAndProvince.Add(complexConditionprovincesConfig);
                                }
                            }

                            tempConditionOwnerAndProvince.AddRange(lsitCodeOperatorConfigOr);
                            complexConditionOwnerAndProvince.Conditions = tempConditionOwnerAndProvince.ToArray();
                            tempLsitConditionprovincesConfigOr.Add(complexConditionOwnerAndProvince);
                            complexConditionCodeOperatorConfig.Conditions = tempLsitConditionprovincesConfigOr.ToArray();
                            complexConditionprovincesConfig = new ComplexCondition();
                        }
                    }
                }

                if (codeOperatorAndStatusesConfig.Status != null)
                {
                    lsitConditionExpressionsAnd.Add(
                     new ConditionExpression()
                     {
                         LeftOperand = new ColumnOperand { ColumnName = "STATUS" },
                         Operator = ConditionOperator.Equal,
                         RightOperand = new StringValueOperand { Value = codeOperatorAndStatusesConfig.Status }
                     });
                }

                var listConditions = new List<Condition>();
                listConditions.AddRange(lsitConditionExpressionsAnd);
                listConditions.Add(complexConditionCodeOperatorConfig);

                bool isCorrectCondition = false;
                var outListConditions = new List<Condition>();
                for (int u = 0; u < listConditions.Count; u++)
                {
                    CheckCondition(listConditions[u], ref isCorrectCondition);
                    if (isCorrectCondition == true)
                    {
                        outListConditions.Add(listConditions[u]);
                    }
                }
                complexCondition.Conditions = outListConditions.ToArray();
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return complexCondition;
        }
    }
}
