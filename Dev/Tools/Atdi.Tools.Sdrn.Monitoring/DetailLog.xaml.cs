using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Atdi.Tools.Sdrn.Monitoring
{
    /// <summary>
    /// Interaction logic for DetailLog.xaml
    /// </summary>
    public partial class DetailLog : Window
    {
        private ExceptionData _log;
        public DetailLog(ExceptionData log)
        {
            InitializeComponent();
            this._log = log;
            this.BuildLog();
        }
        private void BuildLog()
        {
            var item = _log.Inner;

            if (item != null)
            {
                mainTree.Items.Add(new TreeViewItem() { Header = "Type: " + _log.Type });
                mainTree.Items.Add(new TreeViewItem() { Header = "Source: " + _log.Source });
                mainTree.Items.Add(new TreeViewItem() { Header = "StackTrace" + _log.StackTrace });
                mainTree.Items.Add(new TreeViewItem() { Header = "TargetSite: " + _log.TargetSite });
                mainTree.Items.Add(new TreeViewItem() { Header = "Message: " + _log.Message });

                Dictionary<int, TreeViewItem> dic = new Dictionary<int, TreeViewItem>();
                var twItem = new TreeViewItem();
                twItem.Header = "Inner:";
                twItem.IsExpanded = true;
                int i = 0;
                mainTree.Items.Add(twItem);
                dic[i] = twItem;

                while (item != null)
                {
                    dic[i].Items.Add(new TreeViewItem() { Header = "Type: " + item.Type });
                    dic[i].Items.Add(new TreeViewItem() { Header = "Source: " + item.Source });
                    dic[i].Items.Add(new TreeViewItem() { Header = "StackTrace" + item.StackTrace });
                    dic[i].Items.Add(new TreeViewItem() { Header = "TargetSite: " + item.TargetSite });
                    dic[i].Items.Add(new TreeViewItem() { Header = "Message: " + item.Message });
                    if (item.Inner != null)
                    {
                        dic[++i] = new TreeViewItem();
                        dic[i].Header = "Inner:";
                        dic[i].IsExpanded = true;
                        dic[i - 1].Items.Add(dic[i]);
                    }
                    
                    item = item.Inner;
                }
            }
        }
    }
}
