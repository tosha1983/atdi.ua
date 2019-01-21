using System;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Interactivity;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Globalization;
using System.Collections.Generic;

namespace ControlU.Controls
{
    public class OvertypeBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            SetOvertypeMode(true);
        }

        private void SetOvertypeMode(bool enabled)
        {
            PropertyInfo textEditorProperty = typeof(TextBox).GetProperty(
               "TextEditor", BindingFlags.NonPublic | BindingFlags.Instance);
            object textEditor = textEditorProperty.GetValue(AssociatedObject, null);
            // set _OvertypeMode on the TextEditor
            PropertyInfo overtypeModeProperty = textEditor.GetType().GetProperty(
               "_OvertypeMode", BindingFlags.NonPublic | BindingFlags.Instance);
            overtypeModeProperty.SetValue(textEditor, enabled, null);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            SetOvertypeMode(false);
        }
    }

    /// <summary>
    /// Конвертер владельца в короткую форму
    /// </summary>
    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class OwnerToShortConverter : IValueConverter
    {
        /// <summary> 
        /// Convert bool or Nullable<bool> to Visibility
        /// </summary> 
        /// <param name="value">bool or Nullable<bool>
        /// <param name="targetType">Visibility
        /// <param name="parameter">null
        /// <param name="culture">null 
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            str = MainWindow.help.helpOwnerNameAbbreviation(str);
            return str;
        }

        /// <summary>
        /// Convert Visibility to boolean 
        /// </summary>
        /// <param name="value"> 
        /// <param name="targetType"> 
        /// <param name="parameter">
        /// <param name="culture"> 
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Конвертер bool to Visibility прямой
    /// </summary>
    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class LocalBooleanDirectToVisibilityConverter : IValueConverter
    {
        /// <summary> 
        /// Convert bool or Nullable<bool> to Visibility
        /// </summary> 
        /// <param name="value">bool or Nullable<bool>
        /// <param name="targetType">Visibility
        /// <param name="parameter">null
        /// <param name="culture">null 
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            return (bValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Convert Visibility to boolean 
        /// </summary>
        /// <param name="value"> 
        /// <param name="targetType"> 
        /// <param name="parameter">
        /// <param name="culture"> 
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Visible;
            }
            else
            {
                return false;
            }
        }
    }
    /// <summary>
    /// Конвертер bool to Visibility инвертированный
    /// </summary>
    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class LocalBooleanInvertedToVisibilityConverter : IValueConverter
    {
        /// <summary> 
        /// Convert bool or Nullable<bool> to Visibility
        /// </summary> 
        /// <param name="value">bool or Nullable<bool>
        /// <param name="targetType">Visibility
        /// <param name="parameter">null
        /// <param name="culture">null 
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            return (bValue) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Convert Visibility to boolean 
        /// </summary>
        /// <param name="value"> 
        /// <param name="targetType"> 
        /// <param name="parameter">
        /// <param name="culture"> 
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Collapsed;
            }
            else
            {
                return false;
            }
        }
    }


    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class Measured_ToColor_Converter : IValueConverter
    {
        /// <summary> 
        /// Convert bool or Nullable<bool> to Visibility
        /// </summary> 
        /// <param name="value">bool or Nullable<bool>
        /// <param name="targetType">Visibility
        /// <param name="parameter">null
        /// <param name="culture">null 
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            return (bValue) ? new SolidColorBrush(Color.FromArgb(255, (byte)100, (byte)255, (byte)100)) : new SolidColorBrush(Color.FromArgb(0, (byte)255, (byte)255, (byte)255));
        }

        /// <summary>
        /// Convert Visibility to boolean 
        /// </summary>
        /// <param name="value"> 
        /// <param name="targetType"> 
        /// <param name="parameter">
        /// <param name="culture"> 
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class Identified_ToColor_Converter : IValueConverter
    {
        /// <summary> 
        /// Convert bool or Nullable<bool> to Visibility
        /// </summary> 
        /// <param name="value">bool or Nullable<bool>
        /// <param name="targetType">Visibility
        /// <param name="parameter">null
        /// <param name="culture">null 
        /// <returns>Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            return (bValue) ? new SolidColorBrush(Color.FromArgb(255, (byte)70, (byte)150, (byte)204)) : new SolidColorBrush(Color.FromArgb(0, (byte)255, (byte)255, (byte)255));
        }

        /// <summary>
        /// Convert Visibility to boolean 
        /// </summary>
        /// <param name="value"> 
        /// <param name="targetType"> 
        /// <param name="parameter">
        /// <param name="culture"> 
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    [ValueConversion(typeof(decimal), typeof(string))]
    public class FreqConverterDecimal : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal)
            {
                decimal freq = (decimal)value;
                string str = "";
                if (freq > -1000000000000000 && freq <= -1000000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000000, 12), " THz");
                }
                else if (freq > -1000000000000 && freq <= -1000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
                }
                else if (freq > -1000000000 && freq <= -1000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
                }
                else if (freq > -1000000 && freq <= -1000)
                {
                    str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
                }
                else if (freq > -1000 && freq < 0)
                {
                    str = String.Concat(Math.Round(freq, 2), " Hz");
                }
                else if (freq >= 0 && freq < 1000)
                {
                    str = String.Concat(Math.Round(freq, 2), " Hz");
                }
                else if (freq >= 1000 && freq < 1000000)
                {
                    str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
                }
                else if (freq >= 1000000 && freq < 1000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
                }
                else if (freq >= 1000000000 && freq < 1000000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
                }
                else if (freq >= 1000000000000 && freq < 1000000000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000000, 12), " THz");
                }
                return str;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            //string str = MainWindow.help.LeaveOnlyDec(((string)value).Replace(" ", ""));
            //decimal res = 0;
            //if (((string)value).Contains(" Hz"))
            //    res = decimal.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
            //if (((string)value).Contains(" kHz"))
            //    res = 1000 * decimal.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
            //if (((string)value).Contains(" MHz"))
            //    res = 1000000 * decimal.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
            //if (((string)value).Contains(" GHz"))
            //    res = 1000000000 * decimal.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
            throw new NotSupportedException();
            //return res;
        }

        #endregion
    }
    [ValueConversion(typeof(decimal), typeof(string))]
    public class FreqConverterDecimalPositive : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is decimal)
            {
                decimal freq = (decimal)value;
                string str = "";
                if (freq <= 0)
                {
                    str = "-";
                }
                else if (freq >= 0 && freq < 1000)
                {
                    str = String.Concat(Math.Round(freq, 2), " Hz");
                }
                else if (freq >= 1000 && freq < 1000000)
                {
                    str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
                }
                else if (freq >= 1000000 && freq < 1000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
                }
                else if (freq >= 1000000000 && freq < 1000000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
                }
                else if (freq >= 1000000000000 && freq < 1000000000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000000, 12), " THz");
                }
                return str;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    [ValueConversion(typeof(double), typeof(string))]
    public class FreqConverterDouble : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double freq = (double)value;
            if (freq > 0)
            {
                string str = "";
                if (freq > -1000000000000000 && freq < -999999999999)
                {
                    str = String.Concat(Math.Round(freq / 1000000000000, 12), " THz");
                }
                else if (freq > -1000000000000 && freq < -999999999)
                {
                    str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
                }
                else if (freq > -1000000000 && freq < -999999)
                {
                    str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
                }
                else if (freq > -1000000 && freq < -999)
                {
                    str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
                }
                else if (freq > -1000 && freq < 0)
                {
                    str = String.Concat(freq, " Hz");
                }
                else if (freq >= 0 && freq < 1000)
                {
                    str = String.Concat(freq, " Hz");
                }
                else if (freq > 999 && freq < 1000000)
                {
                    str = String.Concat(Math.Round(freq / 1000, 3), " kHz");
                }
                else if (freq > 999999 && freq < 1000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000, 6), " MHz");
                }
                else if (freq > 999999999 && freq < 1000000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000, 9), " GHz");
                }
                else if (freq > 999999999999 && freq < 1000000000000000)
                {
                    str = String.Concat(Math.Round(freq / 1000000000000, 12), " THz");
                }
                return str;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class ConverterDecimalWithDefaultMinusOne : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal)
            {
                decimal dec = (decimal)value;
                string str = "";
                if (dec == -1)
                {
                    str = "-";
                }
                else { str = dec.ToString(); }
                return str;
            }
            else if (value is int)
            {
                int dec = (int)value;
                string str = "";
                if (dec == -1)
                {
                    str = "-";
                }
                else { str = dec.ToString(); }
                return str;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    [ValueConversion(typeof(object), typeof(string))]
    public class ConverterDecimalWithDefaultMinusThousand : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal)
            {
                decimal dec = (decimal)value;
                string str = "";
                if (dec == -1000)
                {
                    str = "-";
                }
                else { str = Math.Round(dec, 2).ToString(); }
                return str;
            }
            else if (value is double)
            {
                double dec = (double)value;
                string str = "";
                if (dec == -1000)
                {
                    str = "-";
                }
                else { str = Math.Round(dec, 2).ToString(); }
                return str;
            }
            else if (value is int)
            {
                int dec = (int)value;
                string str = "";
                if (dec == -1000)
                {
                    str = "-";
                }
                else { str = dec.ToString(); }
                return str;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    [ValueConversion(typeof(decimal), typeof(string))]
    public class LevelRSRConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (MainWindow.Rcvr.LevelUnitInd == 0)
            {
                return ((decimal)value).ToString() + " " + MainWindow.Rcvr.LevelUnitStr;
            }
            else if (MainWindow.Rcvr.LevelUnitInd == 1)
            {
                return (((decimal)value) - 107).ToString() + " " + MainWindow.Rcvr.LevelUnitStr;

            }
            else
            {
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class MultiValueLevelConverter : IMultiValueConverter
    {
        #region IValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is decimal)
                return Math.Round(((decimal)values[0]), 2).ToString() + " " + (string)values[1];
            else if (values[0] is double)
                return Math.Round(((double)values[0]), 2).ToString() + " " + (string)values[1];
            else return "";
            // Do your logic with the properties here.
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
    [ValueConversion(typeof(object), typeof(string))]
    public class LevelConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is decimal)
                return Math.Round(((decimal)value), 2).ToString() + " " + (string)parameter;
            else if (value is double)
                return Math.Round(((double)value), 2).ToString() + " " + (string)parameter;
            else if (value is int)
                return ((int)value).ToString() + " " + (string)parameter;
            else return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }
    [ValueConversion(typeof(int), typeof(bool))]
    public class PrintScreenTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is int && parameter is string)
            {
                return ((string)parameter == ((int)value).ToString()) ? true : false;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    [ValueConversion(typeof(double), typeof(double))]
    public class DFTextConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0 - (decimal)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    [ValueConversion(typeof(decimal), typeof(string))]
    public class TimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal time = (decimal)value;
            return MainWindow.help.helpTime(time);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    [ValueConversion(typeof(DB.atdi_frequency_for_sector[]), typeof(string))]
    public class ATDIFreqToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = "";
            decimal bw = 100000;
            if (parameter is decimal) bw = ((decimal)parameter) * 1000;
            else if (parameter is double) bw = ((decimal)(double)parameter) * 1000;
            //else if (parameter is string) bw = (decimal.Parse((string)parameter)) * 1000;
            DB.atdi_frequency_for_sector[] temp = (DB.atdi_frequency_for_sector[])value;

            for (int i = 0; i < temp.Length; i++)
            { s += (temp[i].frequency / 1000000).ToString() + "; "; }//s += MainWindow.help.helpFreq((double)temp[i].frequency) + "; "; }

            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    
    [ValueConversion(typeof(DB.localatdi_sector_frequency[]), typeof(string))]
    public class ATDIFreqsToStringMultiConverter : IMultiValueConverter
    {
        #region IValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = "";

            decimal step = 0;
            if (values[1] is decimal) step = ((decimal)values[1]);
            else if (values[1] is double) step = ((decimal)(double)values[1]);
            else if (values[1] is string) step = (decimal.Parse((string)values[1]));
            //DB.localatdi_sector_frequency[] temp = (DB.localatdi_sector_frequency[])values[0];
            List<DB.localatdi_sector_frequency> t = ((DB.localatdi_sector_frequency[])values[0]).OrderBy(order => order.frequency).ToList();
            if (t.Count() > 3)
            {
                bool onstep = false;
                for (int i = 0; i < t.Count(); i++)
                {
                    if (i != t.Count() - 1)
                    {
                        if (t[i + 1].frequency - t[i].frequency == step)
                        {
                            if (onstep == false)
                            {
                                s += t[i].frequency / 1000000 + "-";//open
                                onstep = true;
                            }
                        }
                        else
                        {
                            if (onstep)//close
                            {
                                s += t[i].frequency / 1000000 + ";";
                                onstep = false;
                            }
                            else { s += t[i].frequency / 1000000 + ";"; }
                        }
                    }
                    else
                    {
                        s += t[i].frequency / 1000000 + ";";
                    }
                }
                //bool nextfreqwithstep = false;
                //for (int i = 0; i < temp.Length; i++)
                //{
                //    nextfreqwithstep = false;
                //    if (i > 0)
                //    {
                //        if (temp[i].frequency - temp[i - 1].frequency == bw)
                //        {
                //            nextfreqwithstep = true;
                //        }

                //        if (!nextfreqwithstep)
                //        {
                //            s += " - " + (temp[i - 1].frequency / 1000000).ToString() + "; ";
                //            s += (temp[i].frequency / 1000000).ToString();
                //        }
                //        //else
                //        //{
                //        //    s += "; " + (temp[i].frequency / 1000000).ToString();
                //        //}
                //        if (i == temp.Length - 1) s += " - " + (temp[i].frequency / 1000000).ToString() + "; ";
                //    }
                //    else
                //    {
                //        s += (temp[i].frequency / 1000000).ToString();
                //    }
                //}
            }
            else
            {
                for (int i = 0; i < t.Count(); i++)
                { s += (t[i].frequency / 1000000).ToString() + "; "; }//s += MainWindow.help.helpFreq((double)temp[i].frequency) + "; "; }
            }
            return s;
            // return Math.Round(((decimal)values[0]), 2).ToString() + " " + (string)values[1];
            // Do your logic with the properties here.
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    [ValueConversion(typeof(DB.localatdi_sector_frequency[]), typeof(string))]
    public class ATDIFreqToStringMultiConverterTest : IMultiValueConverter
    {
        #region IValueConverter Members
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = "";
            decimal bw = 0;
            if (values[1] is decimal) bw = ((decimal)values[1]) * 1000;
            else if (values[1] is double) bw = ((decimal)(double)values[1]) * 1000;
            else if (values[1] is string) bw = (decimal.Parse((string)values[1])) * 1000;
            DB.localatdi_sector_frequency[] temp = (DB.localatdi_sector_frequency[])values[0];
            if (temp.Length > 1)
            {
                int nextfreqwithstep = 0;
                bool open = false;
                for (int i = 0; i < temp.Length; i++)
                {
                    if (i > 0)
                    {
                        if (temp[i].frequency - temp[i - 1].frequency == bw)
                        {
                            if (nextfreqwithstep == 0) s += (temp[i - 1].frequency / 1000000).ToString();
                            nextfreqwithstep++;
                            open = true;
                        }
                        else
                        {



                            if (nextfreqwithstep > 1)
                            { s += " - " + (temp[i - 1].frequency / 1000000).ToString() + "; "; nextfreqwithstep = 0; }
                            else { s += "; " + (temp[i].frequency / 1000000).ToString(); }
                            open = false;
                        }


                        if (i == temp.Length - 1) s += " - " + (temp[i].frequency / 1000000).ToString() + "; ";
                    }
                    //else
                    //{
                    //    s += (temp[i].frequency / 1000000).ToString();
                    //    open = true;
                    //}
                }
            }
            else
            {
                for (int i = 0; i < temp.Length; i++)
                { s += (temp[i].frequency / 1000000).ToString() + "; "; }//s += MainWindow.help.helpFreq((double)temp[i].frequency) + "; "; }
            }
            return s;
            // return Math.Round(((decimal)values[0]), 2).ToString() + " " + (string)values[1];
            // Do your logic with the properties here.
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        #endregion
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class MonthToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                string str = "";
                str = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetMonthName((int)value);
                return str;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class CoorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal DDD = 0;
            if (value is decimal) DDD = (decimal)value;
            else if (value is double) DDD = (decimal)((double)value);
            if (DDD > 0)
            {
                string t = "";
                try
                {
                    int DD;
                    int MM;
                    float SS;

                    DD = (int)DDD;
                    MM = (int)((DDD - DD) * 60);
                    SS = (float)Math.Round(((DDD - DD) * 60 - MM) * 60, 1);


                    if (DD < 10) t += "0" + DD.ToString() + "° ";
                    else t += DD.ToString() + "° ";
                    if (MM < 10) t += "0" + MM.ToString() + "' ";
                    else t += MM.ToString() + "' ";
                    if (SS < 10) t += "0" + SS.ToString() + "\" ";
                    else t += SS.ToString() + "\" ";

                }
                catch { }
                return t;
            }
            else
            {
                return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
    [ValueConversion(typeof(int), typeof(Brush))]
    public class GNSSRatingConverter : IValueConverter
    {
        public Brush Brush02 { get; set; }
        public Brush Brush34 { get; set; }
        public Brush Brush5pl { get; set; }
        public Brush BrushOff { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                int rating = 0;
                int number = 0;
                if (int.TryParse(value.ToString(), out rating) && int.TryParse(parameter.ToString(), out number))
                {
                    if (rating >= number)
                    {
                        if (rating <= 2)
                        {
                            return Brush02;
                        }
                        else if (rating > 2 && rating <= 4)
                        {
                            return Brush34;
                        }
                        else if (rating > 4)
                        {
                            return Brush5pl;
                        }
                    }
                    return BrushOff;
                }
            }
            catch { }
            return Brushes.Transparent;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BearingPolarPlacementItem : ContentControl
    {
        public BearingPolarPlacementItem()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
        }
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeBounds)
        {
            try
            {
                if (Azimuth != null && Elevation != null && AzimuthShift != null)
                {
                    double az = (Azimuth + AzimuthShift - 90) / 180 * Math.PI;
                    double e = (90 - Elevation) / 90;
                    double X = Math.Cos(az) * e;
                    double Y = Math.Sin(az) * e;
                    X = arrangeBounds.Width * .5 * X;
                    Y = arrangeBounds.Height * .5 * Y;
                    RenderTransform = new TranslateTransform(X, Y);
                }
            }
            catch { }
            return base.ArrangeOverride(arrangeBounds);
        }

        public double Azimuth
        {
            get { return (double)GetValue(AzimuthProperty); }
            set { SetValue(AzimuthProperty, value); }
        }

        public static readonly DependencyProperty AzimuthProperty =
            DependencyProperty.Register("Azimuth", typeof(double), typeof(BearingPolarPlacementItem), new PropertyMetadata(0d, OnAzimuthPropertyChanged));
        private static void OnAzimuthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BearingPolarPlacementItem).InvalidateArrange();
        }

        public double AzimuthShift
        {
            get { return (double)GetValue(AzimuthShiftProperty); }
            set { SetValue(AzimuthShiftProperty, value); }
        }
        public static readonly DependencyProperty AzimuthShiftProperty =
            DependencyProperty.Register("AzimuthShift", typeof(double), typeof(BearingPolarPlacementItem), new PropertyMetadata(0d, OnAzimuthShiftPropertyChanged));

        private static void OnAzimuthShiftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BearingPolarPlacementItem).InvalidateArrange();
        }
        public double Elevation
        {
            get { return (double)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
        }

        public static readonly DependencyProperty ElevationProperty =
            DependencyProperty.Register("Elevation", typeof(double), typeof(BearingPolarPlacementItem), new PropertyMetadata(0d, OnElevationPropertyChanged));

        private static void OnElevationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BearingPolarPlacementItem).InvalidateArrange();
        }

    }

    public class PolarPlacementItem : ContentControl
    {
        public PolarPlacementItem()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
        }
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            try
            {
                if (Azimuth != null && Elevation != null)
                {
                    double az = (Azimuth - 90) / 180 * Math.PI;
                    double e = (90 - Elevation) / 90;
                    double X = Math.Cos(az) * e;
                    double Y = Math.Sin(az) * e;
                    X = arrangeBounds.Width * .5 * X;
                    Y = arrangeBounds.Height * .5 * Y;
                    RenderTransform = new TranslateTransform(X, Y);
                }
            }
            catch { }
            return base.ArrangeOverride(arrangeBounds);
        }

        public double Azimuth
        {
            get { return (double)GetValue(AzimuthProperty); }
            set { if (value != null) SetValue(AzimuthProperty, value); }
        }

        public static readonly DependencyProperty AzimuthProperty =
            DependencyProperty.Register("Azimuth", typeof(double), typeof(PolarPlacementItem), new PropertyMetadata(0d, OnAzimuthPropertyChanged));

        private static void OnAzimuthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarPlacementItem).InvalidateArrange();
        }

        public double Elevation
        {
            get { return (double)GetValue(ElevationProperty); }
            set { if (value != null) SetValue(ElevationProperty, value); }
        }

        public static readonly DependencyProperty ElevationProperty =
            DependencyProperty.Register("Elevation", typeof(double), typeof(PolarPlacementItem), new PropertyMetadata(0d, OnElevationPropertyChanged));

        private static void OnElevationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PolarPlacementItem).InvalidateArrange();
        }

    }


    #region Filters

    public class Filter : INotifyPropertyChanged
    {
        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertychanged("Name"); }
        }
        string _Name;

        private bool _AnyIsChecked;
        public bool AnyIsChecked
        {
            get { return _AnyIsChecked; }
            set { _AnyIsChecked = value; OnPropertychanged("AnyIsChecked"); }
        }

        public Predicate<object> Method
        {
            get { return _Method; }
            set { _Method = value; OnPropertychanged("Method"); }
        }
        Predicate<object> _Method;

        public PropertyInfo TypeFiltered
        {
            get { return _TypeFiltered; }
            set { _TypeFiltered = value; OnPropertychanged("TypeFiltered"); }
        }
        PropertyInfo _TypeFiltered;

        public Type Type
        {
            get { return _Type; }
            set { _Type = value; OnPropertychanged("Type"); }
        }
        Type _Type;

        public ObservableCollection<CheckedListItem<object>> Value
        {
            get { return _Value; }
            set { _Value = value; OnPropertychanged("Value"); }
        }
        ObservableCollection<CheckedListItem<object>> _Value = new ObservableCollection<CheckedListItem<object>>();

        public ObservableCollection<FromToFilter> FromTo
        {
            get { return _FromTo; }
            set { _FromTo = value; OnPropertychanged("FromTo"); }
        }
        ObservableCollection<FromToFilter> _FromTo = new ObservableCollection<FromToFilter>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertychanged(string pName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pName));
            }
        }
    }
    public class FromToFilter : INotifyPropertyChanged
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; OnPropertychanged("IsChecked"); }
        }
        public CheckedListItem<object> From
        {
            get { return _From; }
            set { _From = value; OnPropertychanged("From"); }
        }
        CheckedListItem<object> _From = new CheckedListItem<object>();

        public CheckedListItem<object> To
        {
            get { return _To; }
            set { _To = value; OnPropertychanged("To"); }
        }
        CheckedListItem<object> _To = new CheckedListItem<object>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertychanged(string pName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pName));
            }
        }
    }
    public class CheckedListItem<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isChecked;
        private bool isEnabled;
        private T item;

        public CheckedListItem()
        { }

        public CheckedListItem(T item, bool isChecked = false, bool isEnabled = false)
        {
            this.item = item;
            this.isChecked = isChecked;
            this.isEnabled = isEnabled;
        }

        public T Item
        {
            get { return item; }
            set
            {
                item = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item"));
            }
        }


        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsEnabled"));
            }
        }
    }
    public class GroupFilter
    {
        private List<Predicate<object>> _filters;

        public Predicate<object> Filter { get; private set; }

        public GroupFilter()
        {
            _filters = new List<Predicate<object>>();
            Filter = InternalFilter;
        }

        private bool InternalFilter(object o)
        {
            foreach (var filter in _filters)
            {
                if (!filter(o))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddFilter(Predicate<object> filter)
        {
            _filters.Add(filter);
        }

        public void RemoveFilter(Predicate<object> filter)
        {
            if (_filters.Contains(filter))
            {
                _filters.Remove(filter);
            }
        }

        public void RemoveAllFilter()
        {
            if (_filters != null)
                _filters.Clear();

        }
    }
    #endregion

    public class Header
    {
        public string Name { get; set; }
        public bool Visible
        {
            get { return (col.Visibility == Visibility.Visible) ? true : false; }
            set
            {
                if (col != null)
                {
                    if (value == true) col.Visibility = Visibility.Visible;
                    else col.Visibility = Visibility.Collapsed;
                }
            }
        }
        public DataGridColumn col { get; set; }
    }
}
