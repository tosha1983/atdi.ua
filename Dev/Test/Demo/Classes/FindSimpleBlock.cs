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
    public class FindSimpleBlock
    {
        public Label Lbl { get; set; }
        public TextBox Txt { get; set; }
        public DropDownList Combo { get; set; }
        public bool isTxt { get; set; }
        public HyperLink LnkControl { get; set; }
        public MaskedEditExtender Extender { get; set; }
        public RegularExpressionValidator RegularExpr { get; set; }
        public RadioButton Less { get; set; }
        public RadioButton More { get; set; }
        public RadioButton Equally { get; set; }
        public Type type_ { get; set; }
        public string NameField { get; set; }
        public RangeValidator Range { get; set; }


        public FindSimpleBlock()
        {
            isTxt = true;
            Lbl = new Label();
            Txt = new TextBox();
            Combo = new DropDownList();
            LnkControl = new HyperLink();
            RegularExpr = new RegularExpressionValidator();
            Extender = null;
            Less = new RadioButton();
            More = new RadioButton();
            Equally = new RadioButton();
            Range = new RangeValidator();
        }
    }
}