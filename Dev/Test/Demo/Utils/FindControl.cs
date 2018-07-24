using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OnlinePortal;
using LitvaPortal.Utils;

namespace OnlinePortal.Utils
{
    /// <summary>
    /// Рекурсивный поиск контролов
    /// </summary>
    public static class FindControls
    {
        public static Control FindAnyControl(this Page page, string controlId)
        {
            return FindControlRecursive(controlId, page.Form);
        }

        public static Control FindAnyControl(Control page, string controlId)
        {
            return FindControlRecursive(controlId, page);
        }


        public static Control FindAnyControl(this UserControl control, string controlId)
        {
            return FindControlRecursive(controlId, control);
        }

        public static Control FindControlRecursive(string controlId, Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                Control result = FindControlRecursive(controlId, control);
                if (result != null)
                {
                    return result;
                }
            }
            return parent.FindControl(controlId);
        }


        public static void DisableAllControlForm(this Page page)
        {
            DisableControlRecursive("",page.Form);
        }

        public static Control DisableControlRecursive(string controlId, Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                Control result = DisableControlRecursive(controlId,control);


                if (control is Image)
                {
                    (control as Image).Enabled = false;
                }
                if (control is TextBox)
                {
                    (control as TextBox).Enabled = false;
                }
                if (control is CheckBox)
                {
                    (control as CheckBox).Enabled = false;
                }
                if (control is DropDownList)
                {
                    (control as DropDownList).Enabled = false;
                }
                if (control is ImageButton)
                {
                    (control as ImageButton).Enabled = false;
                }
                
               
                if (result != null)
                {
                    return result;
                }
              
            }
            return parent.FindControl(controlId);
            
        }
    }
}