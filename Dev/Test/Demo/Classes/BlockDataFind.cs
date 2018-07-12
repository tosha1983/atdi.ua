using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace OnlinePortal
{
    /// <summary>
    /// 
    /// </summary>
    public class BlockDataFind
    {
        public object Value { get; set; }
        public Type type_ { get; set; }
        public TypeRadioButton typeRadio_ { get; set; }
        public string NameField { get; set; }
        public string CaptionField { get; set; }
        public string TableName { get; set; }

        public BlockDataFind()
        {
            Value = new object();
            typeRadio_ = TypeRadioButton.Unknown;
        }
    }
}