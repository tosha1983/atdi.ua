using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using LitvaPortal;
using Utils;
using OnlinePortal.Utils;

namespace OnlinePortal
{
    /// <summary>
    /// 
    /// </summary>
    public class ManageBlocks
    {
        public List<FindSimpleBlock> Arr_Block { get; set; }
        public string TypeContainer_ { get; set; }


        public ManageBlocks(string TypeContainer)
        {
            Arr_Block = new List<FindSimpleBlock>();
            TypeContainer_ = TypeContainer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void FillArrBlocks(List<ClassIRPObject> obj, string Prefix, bool isIncludeChecker, UserControl cntr, string SectionT)
        {
            if (obj != null)
            {

                List<ClassIRPObject> f_info = obj;
                ClassIRPObject rp_f = new ClassIRPObject();

                if (!string.IsNullOrEmpty(TypeContainer_))
                {
                    rp_f = f_info.Find(r => r.Setting_param.NAME == TypeContainer_ && r.StatusObject.ToString() == SectionT);
                }

                if (rp_f != null)
                {
                    List<Type> tp_lst = new List<Type>();
                    List<string> Active_List_FLD = new List<string>();
                    for (int i = 0; i < rp_f.FLD.Count(); i++)
                    {
                        Active_List_FLD.Add(rp_f.FLD[i]);
                    }

                    tp_lst = rp_f.FLD_TYPE;

                    if (tp_lst.Count > 0)
                    {

                        for (int i = 0; i < rp_f.FLD.Count(); i++)
                        {
                            bool isTxt = true;
                            if (isIncludeChecker)
                            {
                                if ((rp_f.FLD[i] == "ID") || (rp_f.Setting_param.Ident_User == rp_f.FLD[i]))
                                {
                                    continue;
                                }
                            }

                            if (FindControls.FindAnyControl(cntr, "Lbl_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString()) == null)
                            {

                                FindSimpleBlock f_block_simple = new FindSimpleBlock();
                                f_block_simple.Lbl.ID = "Lbl_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                f_block_simple.Lbl.Text = " " + rp_f.CAPTION_FLD[i].ToString() + "  ";
                                f_block_simple.NameField = rp_f.FLD[i].ToString();
                                f_block_simple.Lbl.Font.Bold = true;
                                f_block_simple.isTxt = isTxt;


                                if (rp_f.FLD[i] == "PATH")
                                {
                                    f_block_simple.LnkControl.ID = "LnkControl_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                    f_block_simple.LnkControl.Width = new Unit("100%");
                                }
                                else
                                {
                                    f_block_simple.LnkControl = null;
                                    if (isTxt)
                                    {
                                        f_block_simple.Txt.ID = "Txt_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                        f_block_simple.Txt.Width = new Unit("60%");
                                        f_block_simple.Txt.ValidationGroup = "FindGroup";
                                        f_block_simple.Txt.ValidateRequestMode = ValidateRequestMode.Enabled;
                                    }
                                    else
                                    {
                                        f_block_simple.Combo.ID = "Combo_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                        f_block_simple.Combo.Width = new Unit("100%");
                                        f_block_simple.Combo.ValidationGroup = "FindGroup";
                                        f_block_simple.Combo.Items.Add("TEST"); f_block_simple.Combo.Items.Add("PNV_819L");
                                        f_block_simple.Combo.ValidateRequestMode = ValidateRequestMode.Enabled;
                                    }
                                }




                                if (rp_f.FLD[i] != "PATH")
                                {
                                    f_block_simple.RegularExpr = new RegularExpressionValidator();
                                    f_block_simple.RegularExpr.ID = isTxt ? ("regex" + f_block_simple.Txt.ID) : ("regex" + f_block_simple.Combo.ID);
                                    f_block_simple.RegularExpr.ForeColor = System.Drawing.Color.Red;
                                    f_block_simple.RegularExpr.ErrorMessage = "Warning! Expression error!";
                                    f_block_simple.RegularExpr.ControlToValidate = isTxt ? f_block_simple.Txt.ID.ToString() : f_block_simple.Combo.ID.ToString();
                                    f_block_simple.RegularExpr.ValidationGroup = "FindGroup";
                                    f_block_simple.type_ = tp_lst[i];

                                }
                                else
                                {
                                    f_block_simple.RegularExpr = null;
                                }

                                Type tp_ = tp_lst[i];
                                bool Stat_ = true;

                                if (f_block_simple.Txt.ID != null)
                                {
                                    f_block_simple.Range.ControlToValidate = f_block_simple.Txt.ID.ToString();
                                }
                                else if (f_block_simple.Combo.ID != null)
                                {
                                    f_block_simple.Range.ControlToValidate = f_block_simple.Combo.ID.ToString();
                                }
                                else f_block_simple.Range = null;

                                if (((rp_f.CAPTION_FLD[i].Contains("Ilguma")) || (rp_f.CAPTION_FLD[i].Contains("Platuma")) || (rp_f.CAPTION_FLD[i].Contains("Spindulys"))))// && ((rp_f.StatusObject == TypeStatus.ISV) || (rp_f.StatusObject == TypeStatus.HCC)))
                                {
                                    if (rp_f.CAPTION_FLD[i] == "Spindulys")
                                    {
                                        if (f_block_simple.Txt.ID != null)
                                        {
                                            f_block_simple.Range.ID = "Range_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Range.Type = ValidationDataType.Double;
                                            f_block_simple.Range.MinimumValue = "0";
                                            f_block_simple.Range.MaximumValue = "50";
                                            f_block_simple.Range.ForeColor = System.Drawing.Color.Red;
                                            f_block_simple.Range.ErrorMessage = "> 50";
                                            f_block_simple.Range.ValidateRequestMode = ValidateRequestMode.Enabled;
                                            f_block_simple.Range.ValidationGroup = "FindGroup";
                                            f_block_simple.Range.EnableClientScript = false;
                                            f_block_simple.Range.SetFocusOnError = true;
                                            f_block_simple.Range.Display = ValidatorDisplay.Dynamic;
                                            f_block_simple.Txt.Attributes.Add("onkeyup", "javascript: cmtChanged50(this);");

                                        }
                                    }
                                    if (rp_f.FLD[i] != "PATH") f_block_simple.RegularExpr.ErrorMessage = "";
                                }

                                if (Stat_)
                                {
                                    switch (tp_.ToString())
                                    {
                                        case "System.Double":

                                            f_block_simple.Txt.TextMode = TextBoxMode.Search;
                                            if (rp_f.FLD[i] != "PATH") f_block_simple.RegularExpr.ValidationExpression = @"^[0-9-+.,]*$";


                                            f_block_simple.Less.ID = "LessRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Less.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Less.Text = "<";

                                            f_block_simple.More.ID = "MoreRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.More.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.More.Text = ">";

                                            f_block_simple.Equally.ID = "EquallyRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Equally.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Equally.Text = "=";


                                            break;
                                        case "System.Int32":
                                            f_block_simple.Txt.TextMode = TextBoxMode.Search;
                                            if (rp_f.FLD[i] != "PATH") f_block_simple.RegularExpr.ValidationExpression = @"^[0-9-+.,]*$";

                                            f_block_simple.Less.ID = "LessRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Less.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Less.Text = "<  ";

                                            f_block_simple.More.ID = "MoreRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.More.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.More.Text = ">  ";

                                            f_block_simple.Equally.ID = "EquallyRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Equally.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Equally.Text = "=  ";

                                            break;

                                        case "System.Boolean":
                                            f_block_simple.Txt.TextMode = TextBoxMode.Search;
                                            if (rp_f.FLD[i] != "PATH") f_block_simple.RegularExpr.ValidationExpression = "[0-1]";

                                            f_block_simple.Less = null;
                                            f_block_simple.More = null;
                                            f_block_simple.Equally = null;

                                            break;

                                        case "System.String":
                                            if (isTxt)
                                                f_block_simple.Txt.TextMode = TextBoxMode.Search;
                                            if (rp_f.FLD[i] != "PATH") f_block_simple.RegularExpr.ValidationExpression = @"(^[а-яА-ЯёЁa-zA-Z0-9]+$)";
                                            f_block_simple.Less = null;
                                            f_block_simple.More = null;
                                            f_block_simple.Equally = null;

                                            break;

                                        case "System.DateTime":
                                            f_block_simple.Txt.TextMode = TextBoxMode.DateTime;
                                            if (rp_f.FLD[i] != "PATH") f_block_simple.RegularExpr.ValidationExpression = @"(^[а-яА-ЯёЁa-zA-Z0-9]+$)";


                                            f_block_simple.Less.ID = "LessRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Less.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Less.Text = "<  ";

                                            f_block_simple.More.ID = "MoreRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.More.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.More.Text = ">  ";

                                            f_block_simple.Equally.ID = "EquallyRadio_" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Equally.GroupName = "Radio" + Prefix + rp_f.FLD[i] + rp_f.StatusObject.ToString();
                                            f_block_simple.Equally.Text = "=  ";


                                            //f_block_simple.Txt.Attributes.Add("placeholder", "mm.dd.yyyy");
                                            f_block_simple.Txt.Attributes.Add("placeholder", "YYYY-MM-DD");
                                            f_block_simple.Extender = new MaskedEditExtender();
                                            f_block_simple.Extender.ID = "extender" + f_block_simple.Txt.ID;
                                            f_block_simple.Extender.TargetControlID = f_block_simple.Txt.ID;
                                            f_block_simple.Extender.Mask = "9999-99-99";
                                            f_block_simple.Extender.MaskType = MaskedEditType.None;
                                            f_block_simple.Extender.ClearMaskOnLostFocus = false;



                                            break;

                                    }
                                }
                                else
                                {
                                    f_block_simple.Less = null;
                                    f_block_simple.More = null;
                                    f_block_simple.Equally = null;
                                }




                                FindSimpleBlock fnd = Arr_Block.Find(rt => rt.Lbl.ID == f_block_simple.Lbl.ID);
                                if (fnd == null)
                                {
                                    Arr_Block.Add(f_block_simple);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}