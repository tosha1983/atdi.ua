using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atdi.DataModels.DataConstraint;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public static class CheckCondition
    {
        public static void Check(Condition condition, ref bool isCorrectCondition)
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
                    return;
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
                            Check(((condition as ComplexCondition).Conditions)[i] as ConditionExpression, ref isCorrectCondition);
                        }
                        else if (((condition as ComplexCondition).Conditions)[i] is ComplexCondition)
                        {
                            Check(((condition as ComplexCondition).Conditions)[i], ref isCorrectCondition);
                        }
                    }
                }
                else
                {
                    isCorrectCondition = false;
                    return;
                }
            }
        }
    }
}
