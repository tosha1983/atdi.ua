using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the action with contains into the changeset of the web query
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    [KnownType(typeof(CreationAction))]
    [KnownType(typeof(TypedRowCreationAction))]
    [KnownType(typeof(StringRowCreationAction))]
    [KnownType(typeof(ObjectRowCreationAction))]
    [KnownType(typeof(UpdationAction))]
    [KnownType(typeof(TypedRowUpdationAction))]
    [KnownType(typeof(StringRowUpdationAction))]
    [KnownType(typeof(ObjectRowUpdationAction))]
    [KnownType(typeof(DeleteionAction))]
    public class Action
    {
        /// <summary>
        /// The ID of the action
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The type of the action
        /// </summary>
        [DataMember]
        public ActionType Type { get; set; }

    }
}
