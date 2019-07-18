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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace Atdi.Tools.Sdrn.Monitoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var endpointUrls = new Dictionary<string, string>();

            foreach (var keySetting in ConfigurationManager.AppSettings.Keys)
            {
                var key = keySetting.ToString();
                if (key.StartsWith("Endpoint"))
                {
                    endpointUrls[key] = ConfigurationManager.AppSettings[key];
                }
            } 
            InitializeComponent();

            // тут нужно пройтись по каждому ендпоинту, изучить его возможности и сформировать дереов
            //  в корне вывести  Key и Адрес в следующем формате"Endpoint1(http://localhost:15030/appserver/v1)"
            // потом для каждой точки, проверить вызовы
            //  - 1  api/Host/Info - это чтение конфигурации  сервера , если есть ответ создать ноду с именем Config - на ее клик выводить коно с тексовым описанием конфигурации - форма и на ней слошной этит бокс
            //  - 2. проверим api/SdrnServer/Config - если есть ответ - добавить ноду с кепшеном SDRN Srever (InsanceName) - InsanceName взять из вернутого объекта . Также добавить под ноды описывающие остальную информацию - номер лицензии,ю дата оончания
            //  - 3. проверим наличие api/orm/Config, если ест ьответ  создать ноду ORM в ее детализацию вывести ноды с полученнго объекта
            //  - 4. проверит наличие сущности api/orm/metadata/entity/Atdi.DataModels.Sdrns.Server.Entities.Monitoring/LogEvent - если есть ответ , то добавляем ноду Log Events при клик ена которую открывать окнос таблицей лога

        }
    }
}
