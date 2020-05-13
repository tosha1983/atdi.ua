using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.IO;
using System.Windows.Markup;
using System.Windows;
using XICSM.ICSControlClient.ViewModels;

namespace XICSM.ICSControlClient.Forms
{
    public partial class WpfStandardForm : WpfFormBase
    {
        public WpfStandardForm(string xamlFileName, string viewModelClassName, string caption = "ICS Control Client")
        {
            InitializeComponent();

            var appFolder = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            var fileName = Path.Combine(appFolder, $"XICSM_ICSControlClient\\Xaml\\{xamlFileName}");
            this.Text = caption;
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                Type modelType = Type.GetType($"XICSM.ICSControlClient.ViewModels.{viewModelClassName}", false, true);
                if (modelType != null)
                {
                    System.Reflection.ConstructorInfo ci = modelType.GetConstructor(new Type[] { });
                    this._wpfElementHost.Child = (UIElement)XamlReader.Load(fileStream);
                    (this._wpfElementHost.Child as System.Windows.Controls.UserControl).DataContext = ci.Invoke(new object[] { });
                }
                else
                    Console.WriteLine($"Класс {viewModelClassName} не найден!");
            }
        }
    }
}
