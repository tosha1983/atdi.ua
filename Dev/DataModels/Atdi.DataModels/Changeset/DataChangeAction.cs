using Atdi.DataModels.DataConstraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the action with contains into the changeset of the web query
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DataChangeAction
    {
        // <summary>
        /// The ID of the action
        /// </summary>
        [DataMember]
        public Guid ActionId { get; set; }

        /// <summary>
        /// The type of the action
        /// </summary>
        [DataMember]
        public ActionType Type { get; set; }

        [DataMember]
        public DataSetColumn[] Columns { get; set; }

        [DataMember]
        public Filter Filter { get; set; }

        [DataMember]
        public DataRowType RowType { get; set; }

        [DataMember]
        public ObjectDataRow ObjectRow { get; set; }

        [DataMember]
        public StringDataRow StringRow { get; set; }

        [DataMember]
        public TypedDataRow TypedRow { get; set; }


        public static explicit operator Action(DataChangeAction dataChangeAction)
        {
            if (dataChangeAction == null)
            {
                return null;
            }

            if (dataChangeAction.Type == ActionType.Create)
            {
                CreationAction action = null;

                switch (dataChangeAction.RowType)
                {
                    case DataRowType.TypedCell:
                        action = new TypedRowCreationAction
                        {
                            Row = dataChangeAction.TypedRow
                        };
                        break;
                    case DataRowType.StringCell:
                        action = new StringRowCreationAction
                        {
                            Row = dataChangeAction.StringRow
                        };
                        break;
                    case DataRowType.ObjectCell:
                        action = new ObjectRowCreationAction
                        {
                            Row = dataChangeAction.ObjectRow
                        };
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported the data row type with name '{dataChangeAction.RowType}'");
                }

                action.Id = dataChangeAction.ActionId;
                action.Type = dataChangeAction.Type;
                action.Columns = dataChangeAction.Columns;
                action.RowType = dataChangeAction.RowType;

                return action;
            }

            if (dataChangeAction.Type == ActionType.Update)
            {
                UpdationAction action = null;

                switch (dataChangeAction.RowType)
                {
                    case DataRowType.TypedCell:
                        action = new TypedRowUpdationAction
                        {
                            Row = dataChangeAction.TypedRow
                        };
                        break;
                    case DataRowType.StringCell:
                        action = new StringRowUpdationAction
                        {
                            Row = dataChangeAction.StringRow
                        };
                        break;
                    case DataRowType.ObjectCell:
                        action = new ObjectRowUpdationAction
                        {
                            Row = dataChangeAction.ObjectRow
                        };
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported the data row type with name '{dataChangeAction.RowType}'");
                }

                action.Id = dataChangeAction.ActionId;
                action.Type = dataChangeAction.Type;
                action.Columns = dataChangeAction.Columns;
                action.RowType = dataChangeAction.RowType;
                action.Condition = (Condition)dataChangeAction.Filter;
                return action;
            }

            if (dataChangeAction.Type == ActionType.Delete)
            {
                var action = new DeletionAction
                {
                    Type = ActionType.Delete,
                    Id = dataChangeAction.ActionId,
                    Condition = (Condition)dataChangeAction.Filter
                };

                return action;
            }

            throw new InvalidOperationException($"Unsupported the data change action type with name '{dataChangeAction.Type}'");
        }
    }
}
