using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace Atdi.WpfControls.EntityOrm.Controls
{
    public class OrmButton : Button
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Margin = new Thickness() { Bottom = 5, Left = 5, Right = 5, Top = 5 };
        }
    }
}
