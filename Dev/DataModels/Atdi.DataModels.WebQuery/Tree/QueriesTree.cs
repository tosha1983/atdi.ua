﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Atdi.DataModels.WebQuery
{
    /// <summary>
    /// Represents the tree of the queries
    /// </summary>
    [DataContract(Namespace = Specification.Namespace)]
    public class QueriesTree
    {
        /// <summary>
        /// The nodes of the tree
        /// </summary>
        [DataMember]
        public QueriesTreeNode[] Nodes { get; set; }

    }
}