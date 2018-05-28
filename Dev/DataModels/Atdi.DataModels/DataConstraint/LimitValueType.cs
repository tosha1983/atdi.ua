using System.Runtime.Serialization;

namespace Atdi.DataModels.DataConstraint
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = CommonSpecification.Namespace)]
    public enum LimitValueType
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Records,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Percent
    }
}