using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using XICSM.ICSControlClient.Models;
using XICSM.ICSControlClient.ViewModels;
using XICSM.ICSControlClient.Models.Views;

namespace XICSM.ICSControlClient.Forms
{
    public partial class MeasTaskListForm : WpfFormBase
    {
        private MeasTask[] _taskData;
        public bool IsPresOK = false;
        public MeasTaskListForm(MeasTask[] taskData)
        {
            this._taskData = taskData;
            InitializeComponent();

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

            var fileName = Path.Combine(appFolder, "XICSM_ICSControlClient\\Xaml\\MeasTaskListForm.xaml");
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = new MeasTaskListViewModel(this._taskData) { _form = this };
            }
        }
    }
}
