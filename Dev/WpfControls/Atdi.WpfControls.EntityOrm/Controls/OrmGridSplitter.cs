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
using System.Windows.Media;

namespace Atdi.WpfControls.EntityOrm.Controls
{
    public class OrmGridSplitter : GridSplitter
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Background = Brushes.LightGray;
            this.ShowsPreview = false;
        }
    }
}
