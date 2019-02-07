using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ControlU.Helpers
{
    public class ExeptionProcessing : PropertyChangedBase
    {
        private ExData _ExceptionData;
        public ExData ExceptionData
        {
            get { return _ExceptionData; }
            set
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    _ExceptionData = value; ErrorLogging(value); OnPropertyChanged("ExceptionData");
                }));
            }
        }
        private string _ExceptionText;
        public string ExceptionText
        {
            get { return _ExceptionText; }
            set { _ExceptionText = value; OnPropertyChanged("ExceptionText"); }
        }
        private ObservableCollection<ExData> _ErrorLogs = new ObservableCollection<ExData>() { };
        public ObservableCollection<ExData> ErrorLogs
        {
            get { return _ErrorLogs; }
            set { _ErrorLogs = value; OnPropertyChanged("ErrorLogs"); }
        }
        private void ErrorLogging(ExData exdt)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                ErrorLogs.Add(exdt);
                ExceptionText = "Message: " + exdt.ex.Message + "\r\n" + @"HResult: " + exdt.ex.HResult.ToString();

                string strPath = @System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\Log.txt";
                if (!System.IO.File.Exists(strPath))
                {
                    System.IO.File.Create(strPath).Dispose();
                }
                for (int i = 0; i < ErrorLogs.Count; i++)
                {
                    if (ErrorLogs[i].SavedToFile == false)
                    {
                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(strPath))
                        {
                            string str = "===========Start_" + ErrorLogs[i].ClassName + "============= " + DateTime.Now.ToString() + "\r\n";
                            if (ErrorLogs[i].AdditionalInformation != "")
                            {
                                str += "Additional Information:" + ErrorLogs[i].AdditionalInformation + "\r\n";
                            }
                            str += "Error Message: " + ErrorLogs[i].ex.Message + "\r\n";
                            str += "Stack Trace: " + ErrorLogs[i].ex.StackTrace + "\r\n";
                            str += "HResult: " + ErrorLogs[i].ex.HResult + "\r\n";
                            str += "============End_" + ErrorLogs[i].ClassName + "============== " + DateTime.Now + "\r\n\r\n";


                            sw.WriteLine(str);
                            ErrorLogs[i].SavedToFile = true;
                            sw.Dispose();
                        }
                        //App.Current.Dispatcher.BeginInvoke((Action)(() =>
                        //{
                        //    ExceptionText = "Message: " + ErrorLogs[i].ex.Message + "\r\n" + @"HResult: " + ErrorLogs[i].ex.HResult.ToString();
                        //}));
                    }
                }
            }));
        }


        //public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

        //// Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        //private void OnPropertyChanged(string propertyName)
        //{
        //    GUIThreadDispatcher.Instance.BeginInvoke(() =>
        //    {
        //        if (PropertyChanged != null)
        //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    });
        //}

    }
}
