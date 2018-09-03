using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OnlinePortal.Utils;
using LitvaPortal;
using DAL;
using AjaxControlToolkit;

namespace OnlinePortal
{
    public partial class ContainerFinder : System.Web.UI.UserControl
    {

        public List<List<RecordPtrDB>> Res_Lst { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Init(bool isRecreated)
        {
            if (Session["AuthUser"] != null)
            {
                string TypeContainer = "";
                string SectionT = "";
                if (Request.QueryString["SectionT"] != null) {
                   SectionT = Request.QueryString["SectionT"].ToString();
                }
                if (Request.QueryString["TypeContainer"] != null)
                {
                    TypeContainer = Request.QueryString["TypeContainer"].ToString();
                }

                string NTypeContainer = "";
                List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
                List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];

                if (Session["Irp"] != null)
                {
                    GenerateStructFinder(Session["Irp"], string.IsNullOrEmpty(NTypeContainer) ? TypeContainer : NTypeContainer, "Finder", isRecreated, SectionT);
                }

                if (Page.IsPostBack)
                {
                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Init(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void GenerateStructFinder(object obj, string TypeContainer, string Prefix, bool isRecreated, string SectT)
        {
          
            if (isRecreated)
            {
                if (PanelFinder1==null)   PanelFinder1 = new Panel();
                PanelFinder1.HorizontalAlign = HorizontalAlign.Left;
                ManageBlocks tr = new ManageBlocks(TypeContainer);
                tr.FillArrBlocks((List<ClassIRPObject>)Session["Irp"], Prefix, false, this, SectT);

                Table findet_tabl = new Table();
                TableRow tr_x_gl = new TableRow();
                List<TableCell> cell_arr = new List<TableCell>();
                List<TableRow> row_arr = new List<TableRow>();

                string IdentUser = "";
                int countColumnVisible = 0;
                List<ClassIRPObject> f_info = (List<ClassIRPObject>)Session["Irp"];
                if (!string.IsNullOrEmpty(TypeContainer))
                {
                    ClassIRPObject Ir_ = f_info.Find(r => r.Setting_param.NAME == TypeContainer && r.StatusObject.ToString() == SectT);
                    if (Ir_ != null)
                    {
                        countColumnVisible = Ir_.Setting_param.MAX_COLUMNS;
                        for (int y = 0; y < Ir_.FLD.Count; y++)
                        {
                            if (Ir_.FLD[y] == Ir_.Setting_param.Ident_User)
                            {
                                IdentUser = Ir_.CAPTION_FLD[y];
                                break;
                            }
                        }
                    }
                }


                int idx_t_block = 1;
                int cntAllVisCol = 0;
                for (int i = 0; i < tr.Arr_Block.Count(); i++)
                {
                    if (tr.Arr_Block[i].Lbl.Text.Trim() == "ID")
                        continue;

                    if ((tr.Arr_Block[i].Lbl.Text.Trim() == IdentUser) && (IdentUser != ""))
                        continue;

                    if (i > countColumnVisible - 1) continue;

                    Panel pn = new Panel();
                    pn.ID = tr.Arr_Block[i].Lbl.Text + "Pn" + i.ToString();
                    pn.HorizontalAlign = HorizontalAlign.Left;
                    pn.Attributes["style"] = "padding: 5px; margin 5px;";


                    Table tabl = new Table();
                    TableRow tr_x = new TableRow();
                    TableCell cell1 = new TableCell();
                    cell1.Controls.Add(tr.Arr_Block[i].Lbl);
                    TableCell cell2 = new TableCell(); cell2.Width = new Unit("100%");
                    if (tr.Arr_Block[i].LnkControl != null) 
                    { cell2.Controls.Add(tr.Arr_Block[i].LnkControl); }
                    else {
                        if (tr.Arr_Block[i].isTxt)
                        {
                            if (tr.Arr_Block[i].Txt.ID.Length > 0) cell2.Controls.Add(tr.Arr_Block[i].Txt);
                        }
                        else
                        {
                            if (tr.Arr_Block[i].Combo.ID.Length > 0) cell2.Controls.Add(tr.Arr_Block[i].Combo);
                        }
                    }


                    TableCell cell4 = new TableCell();
                    if (tr.Arr_Block[i].Less != null) { cell4.Controls.Add(tr.Arr_Block[i].Less); } else { cell4 = null; }
                    TableCell cell5 = new TableCell();
                    if (tr.Arr_Block[i].More != null) { cell5.Controls.Add(tr.Arr_Block[i].More); } else { cell5 = null; }
                    TableCell cell6 = new TableCell();
                    if (tr.Arr_Block[i].Equally != null) { cell6.Controls.Add(tr.Arr_Block[i].Equally); } else { cell6 = null; }

                    tr_x.Controls.Add(cell1); tr_x.Controls.Add(cell2); //tr_x.Controls.Add(cell3);
                    if (cell4 != null) tr_x.Controls.Add(cell4);
                    if (cell5 != null) tr_x.Controls.Add(cell5);
                    if (cell6 != null) tr_x.Controls.Add(cell6);
                    tabl.Controls.Add(tr_x);

                    pn.Controls.Add(tabl);
                    if (tr.Arr_Block[i].Extender != null) pn.Controls.Add(tr.Arr_Block[i].Extender);
                    if (idx_t_block == 1)
                    {
                        cell_arr = new List<TableCell>();
                        tr_x_gl = new TableRow();
                    }


                    if (((idx_t_block % 3) == 0) || (i == tr.Arr_Block.Count() - 1) || (i == (countColumnVisible - 1)))
                    {
                        TableCell cx_ = new TableCell();
                        cx_.BorderStyle = BorderStyle.Dotted;
                        cx_.BorderWidth = 1;
                        cx_.Controls.Add(pn);
                        cell_arr.Add(cx_);

                        foreach (TableCell item in cell_arr)
                        {
                            item.Style.Add("-webkit-border-radius", "10px");
                            item.Style.Add("-moz-border-radius", "10px");
                            item.Style.Add("border-radius", "10px");
                            tr_x_gl.Controls.Add(item);
                        }

                        row_arr.Add(tr_x_gl);
                        idx_t_block = 1;
                    }
                    else
                    {
                        TableCell cx_ = new TableCell();
                        cx_.BorderStyle = BorderStyle.Dotted;
                        cx_.BorderWidth = 1;
                        cx_.Controls.Add(pn);
                        cx_.Style.Add("-webkit-border-radius", "10px");
                        cx_.Style.Add("-moz-border-radius", "10px");
                        cx_.Style.Add("border-radius", "10px");
                        cell_arr.Add(cx_);
                        idx_t_block++;
                    }

                    cntAllVisCol++;
                }

                findet_tabl.BorderWidth = 1;
                findet_tabl.BorderStyle = BorderStyle.Dotted;
                findet_tabl.Style.Add("-webkit-border-radius", "10px");
                findet_tabl.Style.Add("-moz-border-radius", "10px");
                findet_tabl.Style.Add("border-radius", "10px");


                for (int itx = 0; itx < row_arr.Count(); itx++)
                {
                    row_arr[itx].BorderStyle = BorderStyle.Dotted;
                    row_arr[itx].BorderWidth = 1;
                    findet_tabl.Controls.Add(row_arr[itx]);
                }
                findet_tabl.ValidateRequestMode = System.Web.UI.ValidateRequestMode.Enabled;
                PanelFinder1.ValidateRequestMode = System.Web.UI.ValidateRequestMode.Enabled;
                PanelFinder1.Controls.Add(findet_tabl);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<BlockDataFind>  GetFieldFromFormFinder(UserControl cntr, object obj, string TypeContainer_, string Prefix, bool isIncludeChecker, int ReadWrite, object Curr_ID, bool IsCreateNew, string SectionT)
        {
            object Ret_Val = new object();
            ClassIRPObject Ir_ = new ClassIRPObject();
            List<object> Lst_Ob = new List<object>();
            List<BlockDataFind> LstBlockFndVal = new List<BlockDataFind>();
            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            try
            {

                List<ClassIRPObject> f_info = (List<ClassIRPObject>)obj;
                if (!string.IsNullOrEmpty(TypeContainer_))
                {
                    Ir_ = f_info.Find(r => r.Setting_param.NAME == TypeContainer_ && r.StatusObject.ToString() == SectionT);
                }
                int indx_ID_Val=-1;
                for (int gx=0; gx<Ir_.FLD.Count();gx++)
                {
                    if (Ir_.FLD[gx] == "ID")
                    {
                        indx_ID_Val = gx;
                        break;
                    }
                }

                    if (Ir_ != null)
                    {
                        List<object> Lst_Obj = new List<object>();
                        List<Type> tp_lst = new List<Type>(); 
                        for (int i = 0; i < Ir_.Val_Arr.Count(); i++)
                        {
                            if (Ir_.Val_Arr[i][indx_ID_Val] != null) 
                            {
                                if (Curr_ID != null)
                                {
                                   int Val_iNT1 = -1;
                                   int Val_iNT2=-1;
                                   int.TryParse(Curr_ID.ToString(), out Val_iNT1);
                                   int.TryParse(Ir_.Val_Arr[i][indx_ID_Val].ToString(), out Val_iNT2);

                                   if (Val_iNT1 == Val_iNT2)
                                    {
                                        for (int j = 0; j < Ir_.Val_Arr[i].Count(); j++)
                                        {
                                            if ((Ir_.Val_Arr[i][j].ToString() != ConnectDB.NullD.ToString()) && (Ir_.Val_Arr[i][j].ToString() != ConnectDB.NullI.ToString()))
                                            {
                                                Lst_Obj.Add(Ir_.Val_Arr[i][j]);
                                            }
                                            else Lst_Obj.Add("");
                                        }
                                        break;
                                    }
                                   else if (Val_iNT1 == -1)
                                    {
                                        for (int j = 0; j < Ir_.Val_Arr[i].Count(); j++)
                                        {
                                            if ((Ir_.Val_Arr[i][j].ToString() != ConnectDB.NullD.ToString()) && (Ir_.Val_Arr[i][j].ToString() != ConnectDB.NullI.ToString()))
                                            {
                                                Lst_Obj.Add(Ir_.Val_Arr[i][j]);
                                            }
                                            else Lst_Obj.Add("");
                                        }
                                        break;
                                    }
                                }
                               
                            }
                            
                            
                        }

                        List<string> Active_List_FLD = new List<string>();
                        for (int i = 0; i < Ir_.FLD.Count(); i++) {
                            Active_List_FLD.Add(Ir_.FLD[i]);
                        }

                        tp_lst = Ir_.FLD_TYPE;
                        for (int i = 0; i < Ir_.FLD.Count(); i++)
                        {
                            if (isIncludeChecker) {
                                if ((Ir_.FLD[i]=="ID") || (Ir_.FLD[i] == Ir_.Setting_param.Ident_User)) {
                                    continue;
                                }
                            }
                            if (((HyperLink)FindControls.FindAnyControl(cntr, "LnkControl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())) != null)
                            {

                               if (Ir_.FLD[i] == "PATH")
                                {
                                    string FileN = System.IO.Path.GetFileName(Lst_Obj[i].ToString());
                                    ((HyperLink)FindControls.FindAnyControl(cntr, "LnkControl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).NavigateUrl = string.Format("~/HandlerGetFiles.ashx?&name={0}", Lst_Obj[i].ToString());
                                    ((HyperLink)FindControls.FindAnyControl(cntr, "LnkControl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Text = FileN;
                                }

                                if ((Ir_.TABLE_NAME == "DOCFILES") && (Prefix!="Finder"))
                                {
                                    string FileN = System.IO.Path.GetFileName(Lst_Obj[i].ToString());
                                    ((HyperLink)FindControls.FindAnyControl(cntr, "LnkControl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Text = FileN.ToString();

                                   
                                        if (!Lst_Obj[i].ToString().Contains("http"))
                                        {
                                            ((HyperLink)FindControls.FindAnyControl(cntr, "LnkControl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).NavigateUrl = string.Format("~/HandlerGetFiles.ashx?&name={0}", Lst_Obj[i].ToString());
                                            
                                        }
                                        else
                                        {
                                            ((HyperLink)FindControls.FindAnyControl(cntr, "LnkControl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).NavigateUrl = Lst_Obj[i].ToString();
                                        }
                                }
                            }
                            else
                            {
                                ManageBlocks bm = new ManageBlocks(TypeContainer_);
                                bool isTxt =  true;
                                string tNameComponent = "";
                                if (isTxt)
                                    tNameComponent  = "Txt_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString();
                                else
                                    tNameComponent = "Combo_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString();

                                bool isFindCom = false;
                                if (isTxt)
                                {
                                    if (((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)) != null)
                                    {
                                        isFindCom = true;
                                        BlockDataFind bl_f = new BlockDataFind();

                                        if (((RadioButton)FindControls.FindAnyControl(cntr, "LessRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())) != null)
                                        {
                                            if (((RadioButton)FindControls.FindAnyControl(cntr, "LessRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Checked) bl_f.typeRadio_ = TypeRadioButton.Less;
                                        }

                                        if (((RadioButton)FindControls.FindAnyControl(cntr, "MoreRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())) != null)
                                        {
                                            if (((RadioButton)FindControls.FindAnyControl(cntr, "MoreRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Checked) bl_f.typeRadio_ = TypeRadioButton.More;
                                        }

                                        if (((RadioButton)FindControls.FindAnyControl(cntr, "EquallyRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())) != null)
                                        {
                                            if (((RadioButton)FindControls.FindAnyControl(cntr, "EquallyRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Checked) bl_f.typeRadio_ = TypeRadioButton.Equally;
                                        }


                                        string Val_ = ((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text;

                                        if (ReadWrite == 0)
                                        {
                                            Lst_Ob.Add(((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text);
                                        }
                                        else
                                        {
                                            if (Ir_.FLD[i] != "ID")
                                            {
                                                if (tp_lst[i] != typeof(DateTime))
                                                {
                                                    ((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text = Lst_Obj[i].ToString();
                                                }
                                                else
                                                {
                                                    DateTime Vl_T;
                                                    if (DateTime.TryParse(Lst_Obj[i].ToString(), out Vl_T))
                                                    {
                                                        ((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text = Vl_T.ToString("yyyy-MM-dd");
                                                    }
                                                }


                                            }
                                            else
                                            {
                                               
                                                ((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Visible = false;
                                                ((TextBox)FindControls.FindAnyControl(cntr, "Lbl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Visible = false;
                                            }
                                        }

                                        bl_f.NameField = Ir_.FLD[i];
                                        bl_f.CaptionField = Ir_.CAPTION_FLD[i];
                                        bl_f.TableName = Ir_.TABLE_NAME;
                                        bl_f.type_ = tp_lst[i];
                                        Type tp_ = tp_lst[i];

                                        if (Ir_.Setting_param.IS_SQL_REQUEST)
                                        {
                                            tp_lst = Ir_.FLD_TYPE;
                                            bl_f.type_ = Ir_.FLD_TYPE[i];
                                            tp_ = Ir_.FLD_TYPE[i];
                                        }
                                        switch (tp_.ToString())
                                        {
                                            case "System.Double":
                                                double Vl_D;
                                                Val_ = Val_.Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                if (double.TryParse(Val_, out Vl_D)) { bl_f.Value = Vl_D; } else { bl_f.Value = ""; }
                                                break;
                                            case "System.Int32":
                                                Val_ = Val_.Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                int Vl_I;
                                                if (int.TryParse(Val_, out Vl_I)) { bl_f.Value = Vl_I; } else { bl_f.Value = ""; }
                                                break;
                                            case "System.String":
                                                bl_f.Value = Val_;
                                                break;
                                            case "System.DateTime":
                                                DateTime Vl_T;
                                                if (DateTime.TryParse(Val_, out Vl_T)) { bl_f.Value = Vl_T; } else { bl_f.Value = ""; }
                                                break;
                                            default:
                                                bl_f.Value = Val_;
                                                break;
                                        }

                                        LstBlockFndVal.Add(bl_f);
                                    }
                                }
                                else
                                {
                                    if (((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)) != null)
                                    {
                                        isFindCom = true;

                                        BlockDataFind bl_f = new BlockDataFind();

                                        if (((RadioButton)FindControls.FindAnyControl(cntr, "LessRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())) != null)
                                        {
                                            if (((RadioButton)FindControls.FindAnyControl(cntr, "LessRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Checked) bl_f.typeRadio_ = TypeRadioButton.Less;
                                        }

                                        if (((RadioButton)FindControls.FindAnyControl(cntr, "MoreRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())) != null)
                                        {
                                            if (((RadioButton)FindControls.FindAnyControl(cntr, "MoreRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Checked) bl_f.typeRadio_ = TypeRadioButton.More;
                                        }

                                        if (((RadioButton)FindControls.FindAnyControl(cntr, "EquallyRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())) != null)
                                        {
                                            if (((RadioButton)FindControls.FindAnyControl(cntr, "EquallyRadio_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Checked) bl_f.typeRadio_ = TypeRadioButton.Equally;
                                        }


                                        string Val_ = ((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text;

                                        if (ReadWrite == 0)
                                        {
                                            Lst_Ob.Add(((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text);
                                        }
                                        else
                                        {
                                            if (Ir_.FLD[i] != "ID")
                                            {
                                                if (tp_lst[i] != typeof(DateTime))
                                                {
                                                    ((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text = Lst_Obj[i].ToString();
                                                }
                                                else
                                                {
                                                    DateTime Vl_T;
                                                    if (DateTime.TryParse(Lst_Obj[i].ToString(), out Vl_T))
                                                    {
                                                        ((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text = Vl_T.ToString("yyyy-MM-dd");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                ((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Visible = false;
                                                ((TextBox)FindControls.FindAnyControl(cntr, "Lbl_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString())).Visible = false;
                                            }
                                        }

                                        bl_f.NameField = Ir_.FLD[i];
                                        bl_f.CaptionField = Ir_.CAPTION_FLD[i];
                                        bl_f.TableName = Ir_.TABLE_NAME;
                                        bl_f.type_ = tp_lst[i];
                                        Type tp_ = tp_lst[i];

                                        if (Ir_.Setting_param.IS_SQL_REQUEST)
                                        {
                                            tp_lst = Ir_.FLD_TYPE;
                                            bl_f.type_ = Ir_.FLD_TYPE[i];
                                            tp_ = Ir_.FLD_TYPE[i];
                                        }

                                        switch (tp_.ToString())
                                        {
                                            case "System.Double":
                                                double Vl_D;
                                                Val_ = Val_.Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                if (double.TryParse(Val_, out Vl_D)) { bl_f.Value = Vl_D; } else { bl_f.Value = ""; }
                                                break;
                                            case "System.Int32":
                                                Val_ = Val_.Replace(".", decimal_sep).Replace(",", decimal_sep);
                                                int Vl_I;
                                                if (int.TryParse(Val_, out Vl_I)) { bl_f.Value = Vl_I; } else { bl_f.Value = ""; }
                                                break;
                                            case "System.String":
                                                bl_f.Value = Val_;
                                                break;
                                            case "System.DateTime":
                                                DateTime Vl_T;
                                                if (DateTime.TryParse(Val_, out Vl_T)) { bl_f.Value = Vl_T; } else { bl_f.Value = ""; }
                                                break;
                                            default:
                                                bl_f.Value = Val_;
                                                break;
                                        }

                                        LstBlockFndVal.Add(bl_f);
                                    }
                                }
                                if (isFindCom = false)
                                {
                                    if ((Ir_.FLD[i] == "ID") || (Ir_.FLD[i] == Ir_.Setting_param.Ident_User))
                                    {
                                        BlockDataFind bl_f = new BlockDataFind();
                                        bl_f.NameField = Ir_.FLD[i];
                                        bl_f.CaptionField = Ir_.CAPTION_FLD[i];
                                        bl_f.TableName = Ir_.TABLE_NAME;
                                        bl_f.type_ = tp_lst[i];
                                        Type tp_ = tp_lst[i];
                                        bl_f.Value = "";
                                        LstBlockFndVal.Add(bl_f);
                                    }
                                }
                            }
                        }
                        
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return LstBlockFndVal;
        }


        


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cntr"></param>
        /// <param name="obj"></param>
        /// <param name="TypeContainer_"></param>
        /// <param name="Prefix"></param>
        /// <param name="isIncludeChecker"></param>
        /// <param name="Curr_ID"></param>
        /// <returns></returns>
        public List<object> GetFieldFromFormFinderCreate(UserControl cntr, object obj, string TypeContainer_, string Prefix, bool isIncludeChecker, int ReadWrite, out List<BlockDataFind> LstBlockFndVal, string SectionT)
        {
            object Ret_Val = new object();
            ClassIRPObject Ir_ = new ClassIRPObject();
            LstBlockFndVal = new List<BlockDataFind>();
            List<object> Lst_Ob = new List<object>();
            string decimal_sep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            try
            {

                List<ClassIRPObject> f_info = (List<ClassIRPObject>)obj;
                if (!string.IsNullOrEmpty(TypeContainer_))
                {
                    Ir_ = f_info.Find(r => r.Setting_param.NAME == TypeContainer_ && r.StatusObject.ToString() == SectionT);
                }

                if (Ir_ != null)
                {
                    int countColumnVisible = Ir_.Setting_param.HIDDEN_COLUMNS;
                    int start_hide_idx = Ir_.FLD.Count() - countColumnVisible+1;
                    for (int i = 0; i < Ir_.FLD.Count(); i++)
                    {

                        if ((Ir_.FLD[i]=="ID")  ||  (Ir_.FLD[i] == Ir_.Setting_param.Ident_User))
                            continue;
                        
                        if (isIncludeChecker)
                        {
                            if (Ir_.FLD[i]=="ID") {
                                continue;
                            }
                        }

                        if (i >= start_hide_idx) continue;

                        ManageBlocks bm = new ManageBlocks(TypeContainer_);
                        bool isTxt = true;
                        string tNameComponent = "";
                        if (isTxt)
                            tNameComponent = "Txt_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString();
                        else
                            tNameComponent = "Combo_" + Prefix + Ir_.FLD[i] + Ir_.StatusObject.ToString();

                        bool isFindCom = false;
                        

                        if (ReadWrite == 0)
                        {
                            if (isTxt)
                            {
                                if (FindControls.FindAnyControl(cntr, tNameComponent) != null)
                                    ((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text = "";
                            }
                            else
                            {
                                if (FindControls.FindAnyControl(cntr, tNameComponent) != null)
                                    ((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text = "";
                            }
                            Lst_Ob.Add("");
                        }
                        else
                        {
                                object dbx_d = new object();
                                double Temp_d = ConnectDB.NullD;
                                Type tp_  = null;
                                List<string> Active_List_FLD = new List<string>();
                                for (int ii = 0; ii < Ir_.FLD.Count(); ii++)
                                {
                                    Active_List_FLD.Add(Ir_.FLD[ii]);
                                }
                                BlockDataFind bl_f = new BlockDataFind();
                                bl_f.type_= (typeof(string));
                                bl_f.NameField = Ir_.FLD[i];
                                bl_f.TableName = Ir_.TABLE_NAME;
                                switch (bl_f.type_.ToString())
                                {
                                    case "System.Double":
                                        double Vl_D;
                                        if (FindControls.FindAnyControl(cntr, tNameComponent) != null)
                                        {
                                            if (isTxt)
                                                double.TryParse(((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text.Replace(".", decimal_sep).Replace(",", decimal_sep), out Temp_d);
                                            else
                                                double.TryParse(((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text.Replace(".", decimal_sep).Replace(",", decimal_sep), out Temp_d);
                                        }
                                        dbx_d = Temp_d;
                                        if (double.TryParse(dbx_d.ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out Vl_D)) { bl_f.Value = Vl_D; } else { bl_f.Value = ""; }
                                        break;
                                    case "System.Int32":
                                        int Vl_I;
                                        if (FindControls.FindAnyControl(cntr, tNameComponent) != null)
                                        {
                                            if (isTxt)
                                                double.TryParse(((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text.Replace(".", decimal_sep).Replace(",", decimal_sep), out Temp_d);
                                            else
                                                double.TryParse(((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text.Replace(".", decimal_sep).Replace(",", decimal_sep), out Temp_d);
                                        }
                                        dbx_d = Temp_d;
                                        if (int.TryParse(dbx_d.ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out Vl_I)) { bl_f.Value = Vl_I; } else { bl_f.Value = ""; }
                                        break;
                                    case "System.String":
                                        if (FindControls.FindAnyControl(cntr, tNameComponent) != null)
                                        {
                                            if (isTxt)
                                                dbx_d = ((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text;
                                            else
                                                dbx_d = ((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text;
                                        }
                                        bl_f.Value = dbx_d.ToString();
                                        break;
                                    case "System.DateTime":
                                        DateTime Vl_T;
                                        if (FindControls.FindAnyControl(cntr, tNameComponent) != null)
                                        {
                                            if (isTxt)
                                                dbx_d = ((TextBox)FindControls.FindAnyControl(cntr, tNameComponent)).Text;
                                            else
                                                dbx_d = ((DropDownList)FindControls.FindAnyControl(cntr, tNameComponent)).Text;
                                        }
                                        if (DateTime.TryParse(dbx_d.ToString().Replace(".", decimal_sep).Replace(",", decimal_sep), out Vl_T)) { bl_f.Value = Vl_T; } else { bl_f.Value = ConnectDB.NullT; }
                                        break;
                                    default:
                                        bl_f.Value = dbx_d;
                                        break;
                                }
                                LstBlockFndVal.Add(bl_f);
                                Lst_Ob.Add(dbx_d);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Lst_Ob;
        }



        protected void Search()
        {
            if (Session["AuthUser"] != null)
            {
                if (Request.QueryString["TypeContainer"] != null)
                {
                    string TypeContainer = Request.QueryString["TypeContainer"].ToString();
                    string SectionT = "";
                    if (Request.QueryString["SectionT"] != null)
                    {
                        SectionT = Request.QueryString["SectionT"].ToString();
                    }

                    string NTypeContainer = "";
                    List<ClassIRPObject> Res_out = new List<ClassIRPObject>();
                    List<ClassIRPObject> LIrp = (List<ClassIRPObject>)Session["Irp"];
                    List<BlockDataFind> obj = GetFieldFromFormFinder(this, Session["Irp"], string.IsNullOrEmpty(NTypeContainer) ? TypeContainer : NTypeContainer, "Finder", false, 0, -1, false, SectionT);
                    if (obj != null)
                    {
                        Session["sort"] = null;
                        Session["isCreateNewStation"] = null;
                        Session["Search_container"] = obj;
                        Init(false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            Search();   
        }

        protected void PanelSearch_Init(object sender, EventArgs e)
        {
           
        }
    }

 

    /// <summary>
    /// 
    /// </summary>
    public enum TypeRadioButton
    {
        Less,
        More,
        Equally,
        Unknown
    }



   
    
    

    

}