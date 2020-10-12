using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Shell;

namespace ControlU
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        //public static App application;
        public static bool Lic = false;
        public static Settings.XMLSettings Sett;
        public static Helpers.ExeptionProcessing exp = new Helpers.ExeptionProcessing();



        private static List<CultureInfo> m_Languages = new List<CultureInfo>();

        public static List<CultureInfo> Languages
        {
            get
            {
                return m_Languages;
            }
        }
        private const string Unique = "0bd32d71-0d3b-4e07-b8aa-3bb88142990f";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
                
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        #endregion
        public App()
        {
            GUIThreadDispatcher.Instance.Init();
            App.LanguageChanged += App_LanguageChanged;
            m_Languages.Clear();
            m_Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
            m_Languages.Add(new CultureInfo("ru-RU"));
            m_Languages.Add(new CultureInfo("uk-UA"));
            //Language = (CultureInfo)Enum.Parse(typeof(CultureInfo), Sett.GlogalApps_Settings.UILanguage);
            //Language = new CultureInfo("ru-RU");//Sett.GlogalApps_Settings.UILanguage;
            Sett = new Settings.XMLSettings();
            //MVSSett = new Settings.MVSSettings();
            
        }
        // Событие, которое нужно вызывать при изменении
        public static event PropertyChangedEventHandler PropertyChanged; 
        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void OnPropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        //Евент для оповещения всех окон приложения
        public static event EventHandler LanguageChanged;

        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                try
                {
                    if (value == null) throw new ArgumentNullException("value");
                    if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

                    //1. Меняем язык приложения:
                    System.Threading.Thread.CurrentThread.CurrentUICulture = value;
                    //MessageBox.Show(value.Name);
                    //2. Создаём ResourceDictionary для новой культуры
                    ResourceDictionary dict = new ResourceDictionary();
                    switch (value.Name)
                    {
                        case "ru-RU":
                            dict.Source = new Uri(String.Format("Lang/lang.{0}.xaml", value.Name), UriKind.Relative);
                            break;
                        case "uk-UA":
                            dict.Source = new Uri(String.Format("Lang/lang.{0}.xaml", value.Name), UriKind.Relative);
                            break;
                        default:
                            dict.Source = new Uri("Lang/lang.xaml", UriKind.Relative);
                            break;
                    }
                    //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                    ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                                  where d.Source != null && d.Source.OriginalString.StartsWith("Lang/lang.")
                                                  select d).First();
                    if (oldDict != null)
                    {
                        int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                        Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                        Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                    }
                    else
                    {
                        Application.Current.Resources.MergedDictionaries.Add(dict);
                    }
                    // кастыль для смены языков в настройках
                    Controls.Map.ATDI_TooltipStation c = new Controls.Map.ATDI_TooltipStation();
                    c = null;
                    //4. Вызываем евент для оповещения всех окон.
                    LanguageChanged(Application.Current, new EventArgs());
                }
                catch { }
            }
        }
        private void App_LanguageChanged(Object sender, EventArgs e)
        {
            Sett.GlogalApps_Settings.UILanguage = Language.ToString();
            Sett.SaveGlogalApps();
        }

    }
    public class ExData : INotifyPropertyChanged
    {
        public Exception ex
        {
            get { return _ex; }
            set { _ex = value; OnPropertyChanged("ex"); }
        }
        private Exception _ex;

        public bool SavedToFile
        {
            get { return _SavedToFile; }
            set { _SavedToFile = value; OnPropertyChanged("SavedToFile"); }
        }
        private bool _SavedToFile = false;

        public string ClassName
        {
            get { return _ClassName; }
            set { _ClassName = value; OnPropertyChanged("ClassName"); }
        }
        private string _ClassName = "";

        public DateTime DT
        {
            get { return _DT; }
            set { _DT = value; OnPropertyChanged("DT"); }
        }
        private DateTime _DT = DateTime.Now;

        public string AdditionalInformation
        {
            get { return _AdditionalInformation; }
            set { _AdditionalInformation = value; OnPropertyChanged("AdditionalInformation"); }
        }
        private string _AdditionalInformation = "";

        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        private void OnPropertyChanged(string propertyName)
        {
            GUIThreadDispatcher.Instance.BeginInvoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    }
    public class GUIThreadDispatcher
    {
        private static volatile GUIThreadDispatcher itsSingleton;
        private Dispatcher itsDispatcher;

        private GUIThreadDispatcher() { }
        public static GUIThreadDispatcher Instance
        {
            get
            {
                if (itsSingleton == null)
                    itsSingleton = new GUIThreadDispatcher();

                return itsSingleton;
            }
        }

        public void Init()
        {
            itsDispatcher = Dispatcher.CurrentDispatcher;
        }

        public object Invoke(Action method, DispatcherPriority priority = DispatcherPriority.Render, params object[] args)
        {
            return itsDispatcher.Invoke(method, priority, args);
        }

        public DispatcherOperation BeginInvoke(Action method, DispatcherPriority priority = DispatcherPriority.Render, params object[] args)
        {
            return itsDispatcher.BeginInvoke(method, priority, args);
        }
    }

    class MagicAttribute : Attribute { }
    class NoMagicAttribute : Attribute { }

    [Magic]
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        public virtual void OnPropertyChanged(string propName)
        {
            GUIThreadDispatcher.Instance.BeginInvoke(() =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
            });
            //var e = PropertyChanged;
            //if (e != null)
            //    e(this, new PropertyChangedEventArgs(propName)); // некоторые из нас здесь используют Dispatcher, для безопасного взаимодействия с UI thread
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }
    }
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public ObservableRangeCollection()
            : base()
        {
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Add or Reset for AddRange.", "notificationMode");
            if (collection == null)
                throw new ArgumentNullException("collection");

            CheckReentrancy();

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {
                foreach (var i in collection)
                {
                    Items.Add(i);
                }

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                return;
            }

            int startIndex = Count;
            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
            foreach (var i in changedItems)
            {
                Items.Add(i);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, startIndex));
        }

        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). NOTE: with notificationMode = Remove, removed items starting index is not set because items are not guaranteed to be consecutive.
        /// </summary> 
        public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Reset)
        {
            if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", "notificationMode");
            if (collection == null)
                throw new ArgumentNullException("collection");

            CheckReentrancy();

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {

                foreach (var i in collection)
                    Items.Remove(i);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                return;
            }

            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
            for (int i = 0; i < changedItems.Count; i++)
            {
                if (!Items.Remove(changedItems[i]))
                {
                    changedItems.RemoveAt(i); //Can't use a foreach because changedItems is intended to be (carefully) modified
                    i--;
                }
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems, -1));
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified item. 
        /// </summary> 
        public void Replace(T item)
        {
            ReplaceRange(new T[] { item });
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// </summary> 
        public void ReplaceRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            Items.Clear();
            AddRange(collection, NotifyCollectionChangedAction.Reset);
        }

        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }
    }

}
