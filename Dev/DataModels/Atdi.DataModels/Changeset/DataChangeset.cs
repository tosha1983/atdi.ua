using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels
{
    /// <summary>
    /// Represents the changeset of the web query
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public class DataChangeset
    {
        /// <summary>
        /// The ID of the changeset
        /// </summary>
        [DataMember]
        public Guid ChangesetId { get; set; }

        /// <summary>
        /// The action set is included changeset
        /// </summary>
        [DataMember]
        public DataChangeAction[] Actions { get; set; }

        public static explicit operator Changeset(DataChangeset dataChangeset)
        {
            if (dataChangeset == null)
            {
                return null;
            }

            var changeset = new Changeset
            {
                Id = dataChangeset.ChangesetId,
                Actions = dataChangeset.Actions?.Select(a => (Action)a).ToArray()
            };

            return changeset;
        }
    }
}
