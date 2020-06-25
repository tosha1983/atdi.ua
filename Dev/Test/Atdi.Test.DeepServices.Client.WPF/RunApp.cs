using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using GMap.NET;
using GMap.NET.WindowsPresentation;
using GMap.NET.MapProviders;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Atdi.Test.DeepServices.Client.WPF
{
    public static class RunApp
    {
        public static void Start(TypeObject typeObjectInput, Location[] locationsInput,
                                                TypeObject typeObjectOutput, Location[] locationsOutput)
        {

            WPF.App app = new WPF.App();
            var wnd = new WPF.MainWindow();
            wnd.MapX.DrawingData = WPF.MapDrawingUpdateData.UpdateData(typeObjectInput, locationsInput, typeObjectOutput, locationsOutput);
            wnd.UpdateSource();
            app.Run(wnd);
            app.Shutdown();
        }
    }
}
