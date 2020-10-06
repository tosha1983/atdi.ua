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
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.LegacyServices.Icsm;
using Atdi.AppUnits.Icsm.CoverageEstimation.Localization;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public class CreateConditionForMobStation2
    {
        private CodeOperatorAndStatusConfig _dataConfig { get; set; }
        private ILogger _logger { get; set; }
        private readonly IDataLayer<IcsmDataOrm> _dataLayer;
        private string _provincesConfigName { get; set; }
        public CreateConditionForMobStation2(IDataLayer<IcsmDataOrm> dataLayer, CodeOperatorAndStatusConfig dataConfig, string provincesConfigName, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._dataConfig = dataConfig;
            this._logger = logger;
            this._provincesConfigName = provincesConfigName;
        }

        public bool CheckField(string fld_check, string tableName)
        {
            bool isSuccess = false;
            try
            {
                var QueryNext = _dataLayer.Builder
                .From(tableName)
                .Where("ID", -1)
                .Select(fld_check);

                var scope = this._dataLayer.CreateScope<IcsmDataContext>();
                isSuccess = scope.Executor.Fetch(QueryNext, reader =>
                {
                    return true;
                });
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public Condition GetCondition()
        {
            var complexCondition = new ComplexCondition();
            try
            {
                var provs = this._provincesConfigName.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                complexCondition.Operator = LogicalOperator.And;

                var complexConditionCodeOperatorConfig = new ComplexCondition();
                complexConditionCodeOperatorConfig.Operator = LogicalOperator.And;
                var lsitCodeOperatorConfigOr = new List<ConditionExpression>();


                var complexConditionprovincesConfig = new ComplexCondition();
                complexConditionprovincesConfig.Operator = LogicalOperator.Or;
                var lsitConditionprovincesConfigOr = new List<ConditionExpression>();

                var complexConditionFreqs = new ComplexCondition();
                complexConditionFreqs.Operator = LogicalOperator.Or;


                var lsitConditionExpressionsAnd = new List<ConditionExpression>();
                var codeOperatorAndStatusesConfig = this._dataConfig;
                if (codeOperatorAndStatusesConfig.FreqConfig != null)
                {
                    if (!string.IsNullOrEmpty(codeOperatorAndStatusesConfig.FreqConfig.Values))
                    {
                        if (codeOperatorAndStatusesConfig.FreqConfig.Values.Contains(";"))
                        {
                            var wrds = codeOperatorAndStatusesConfig.FreqConfig.Values.Split(new char[] { '\n', '\t', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries);
                            double[] freqs = null;
                            if ((wrds != null) && (wrds.Length > 0))
                            {
                                freqs = new double[wrds.Length];
                                for (int j = 0; j < wrds.Length; j++)
                                {
                                    freqs[j] = Convert.ToDouble(wrds[j]);
                                }
                            }

                            if (freqs != null)
                            {
                                var listcomplexConditionFreq = new List<ComplexCondition>();

                                for (int j = 0; j < freqs.Length; j++)
                                {
                                    var complexConditionFreq = new ComplexCondition();
                                    complexConditionFreq.Operator = LogicalOperator.And;

                                    var lsitConditionFreq = new List<ConditionExpression>();

                                    lsitConditionFreq.Add(
                                    new ConditionExpression()
                                    {
                                        LeftOperand = new ColumnOperand { ColumnName = "AssignedFrequencies.TX_FREQ" },
                                        Operator = ConditionOperator.Equal,
                                        RightOperand = new DoubleValueOperand { Value = freqs[j] }
                                    });

                                    complexConditionFreq.Conditions = lsitConditionFreq.ToArray();
                                    listcomplexConditionFreq.Add(complexConditionFreq);
                                }
                                complexConditionFreqs.Conditions = listcomplexConditionFreq.ToArray();
                            }
                        }
                        else if (codeOperatorAndStatusesConfig.FreqConfig.Values.Contains("-"))
                        {
                            var wrds = codeOperatorAndStatusesConfig.FreqConfig.Values.Split(new char[] { '\n', '\t', '\r', '-' }, StringSplitOptions.RemoveEmptyEntries);
                            double?[] freqs = null;
                            if ((wrds != null) && (wrds.Length == 2))
                            {
                                freqs = new double?[wrds.Length];
                                for (int j = 0; j < wrds.Length; j++)
                                {
                                    freqs[j] = Convert.ToDouble(wrds[j]);
                                }
                            }

                            if (freqs != null)
                            {
                                var listcomplexConditionFreq = new List<ComplexCondition>();

                                var complexConditionFreq = new ComplexCondition();
                                complexConditionFreq.Operator = LogicalOperator.And;

                                var lsitConditionFreq = new List<ConditionExpression>();

                                lsitConditionFreq.Add(
                                new ConditionExpression()
                                {
                                    LeftOperand = new ColumnOperand { ColumnName = "AssignedFrequencies.TX_FREQ" },
                                    Operator = ConditionOperator.Between,
                                    RightOperand = new DoubleValuesOperand { Values = freqs }
                                });

                                complexConditionFreq.Conditions = lsitConditionFreq.ToArray();
                                listcomplexConditionFreq.Add(complexConditionFreq);

                                complexConditionFreqs.Conditions = listcomplexConditionFreq.ToArray();
                            }
                        }
                    }

                    if (codeOperatorAndStatusesConfig.FreqConfig.provincesConfig != null)
                    {
                        var tempLsitConditionprovincesConfigOr = new List<Condition>();
                        bool isContinue = false;
                        for (int v = 0; v < codeOperatorAndStatusesConfig.FreqConfig.provincesConfig.Length; v++)
                        {
                            lsitCodeOperatorConfigOr = new List<ConditionExpression>();
                            var complexConditionOwnerAndProvince = new ComplexCondition();
                            complexConditionOwnerAndProvince.Operator = LogicalOperator.And;

                            var codeOperatorConfig = codeOperatorAndStatusesConfig.FreqConfig.provincesConfig[v];
                            var tempConditionOwnerAndProvince = new List<Condition>();
                            if (codeOperatorConfig != null)
                            {

                                var complexConditionProvinces = new ComplexCondition();
                                complexConditionProvinces.Operator = LogicalOperator.And;

                                var tempProvs = codeOperatorConfig.Name.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                if ((tempProvs != null) && (tempProvs.Length > 0))
                                {
                                    var complexCodes = new ComplexCondition();
                                    complexCodes.Operator = LogicalOperator.Or;

                                    var complexProvinces = new ComplexCondition();
                                    complexProvinces.Operator = LogicalOperator.Or;


                                    for (int s = 0; s < tempProvs.Length; s++)
                                    {

                                        if (!provs.Contains(tempProvs[s]))
                                        {
                                            isContinue = true;
                                            continue;
                                        }
                                        isContinue = false;
                                     
                                        lsitCodeOperatorConfigOr.Add(
                                                        new ConditionExpression()
                                                        {
                                                            LeftOperand = new ColumnOperand { ColumnName = "Position.PROVINCE" },
                                                            Operator = ConditionOperator.Equal,
                                                            RightOperand = new StringValueOperand { Value = tempProvs[s] }
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
                                            if ((lsitConditionprovincesConfigOr != null) && (lsitConditionprovincesConfigOr.Count > 0))
                                            {
                                                complexCodes.Conditions = lsitConditionprovincesConfigOr.ToArray();
                                            }
                                        }
                                    }

                                    if ((lsitCodeOperatorConfigOr != null) && (lsitCodeOperatorConfigOr.Count > 0))
                                    {
                                        complexProvinces.Conditions = lsitCodeOperatorConfigOr.ToArray();
                                    }

                                    if (isContinue == false)
                                    {
                                        if ((complexCodes.Conditions != null) && (complexCodes.Conditions.Length > 0))
                                        {
                                            tempConditionOwnerAndProvince.Add(complexCodes);
                                        }

                                        if ((complexProvinces.Conditions != null) && (complexProvinces.Conditions.Length > 0))
                                        {
                                            tempConditionOwnerAndProvince.Add(complexProvinces);
                                        }

                                        complexConditionOwnerAndProvince.Conditions = tempConditionOwnerAndProvince.ToArray();
                                        tempLsitConditionprovincesConfigOr.Add(complexConditionOwnerAndProvince);
                                        complexConditionCodeOperatorConfig.Conditions = tempLsitConditionprovincesConfigOr.ToArray();
                                        complexConditionprovincesConfig = new ComplexCondition();
                                    }
                                }
                            }
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

                if ((codeOperatorAndStatusesConfig.MaskFieldName != null) && (codeOperatorAndStatusesConfig.MaskPattern != null))
                {
                    if (CheckField(codeOperatorAndStatusesConfig.MaskFieldName, "MOB_STATION2"))
                    {
                        lsitConditionExpressionsAnd.Add(
                         new ConditionExpression()
                         {
                             LeftOperand = new ColumnOperand { ColumnName = codeOperatorAndStatusesConfig.MaskFieldName },
                             Operator = ConditionOperator.Like,
                             RightOperand = new StringValueOperand { Value = codeOperatorAndStatusesConfig.MaskPattern }
                         });
                    }
                    else
                    {
                        this._logger.Warning(Contexts.CalcCoverages, (EventText)$"Value Mask.FieldName ='{codeOperatorAndStatusesConfig.MaskFieldName}' not found in table 'MOB_STATION2'");
                    }
                }

                var listConditions = new List<Condition>();
                listConditions.AddRange(lsitConditionExpressionsAnd);
                if ((complexConditionCodeOperatorConfig.Conditions != null) && (complexConditionCodeOperatorConfig.Conditions.Length > 0))
                {
                    listConditions.Add(complexConditionCodeOperatorConfig);
                }
                if ((complexConditionFreqs.Conditions != null) && (complexConditionFreqs.Conditions.Length>0))
                {
                    listConditions.Add(complexConditionFreqs);
                }

                bool isCorrectCondition = false;
                var outListConditions = new List<Condition>();
                for (int u = 0; u < listConditions.Count; u++)
                {
                    CheckCondition.Check(listConditions[u], ref isCorrectCondition);
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
