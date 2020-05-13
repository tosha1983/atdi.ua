//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Windows;
//using System.Windows.Controls;

//namespace Atdi.Icsm.Plugins.SdrnCalcServerClient.Environment.Wpf
//{
//    //public static class DataGridExtensions
//    //{
//    //    public static void SetDataAdapter<TSource, TData>(this System.Windows.Controls.DataGrid dataGrid, IWpfDataAdapter<TSource, TData> adapter)
//    //        where TData : class, new()
//    //    {
//    //        var dataType = typeof(TData);
//    //        dataType.GetProperties().ToList().ForEach(property =>
//    //        {
//    //            var metadata = property.GetCustomAttribute<WpfColumnAttribute>(true);

//    //            if (metadata == null)
//    //            {
//    //                return;
//    //            }

//    //            var column =
//    //                new DataGridTextColumn()
//    //                {
//    //                    Header = metadata.DisplayName,
//    //                    Binding = new System.Windows.Data.Binding(property.Name),
//    //                    IsReadOnly = true
//    //                };

//    //            switch (metadata.WidthRule)
//    //            {
//    //                case ColumnWidthRule.Value:
//    //                    column.Width = metadata.Width;
//    //                    break;
//    //                case ColumnWidthRule.AutoSize:
//    //                    column.Width = DataGridLength.Auto;
//    //                    break;
//    //                case ColumnWidthRule.SizeToCells:
//    //                    column.Width = DataGridLength.SizeToCells;
//    //                    break;
//    //                case ColumnWidthRule.SizeToHeader:
//    //                    column.Width = DataGridLength.SizeToHeader;
//    //                    break;
//    //                default:
//    //                    throw new InvalidOperationException($"Unsupported width rule '{metadata.WidthRule}'");
//    //            }

//    //            if (!string.IsNullOrEmpty(metadata.CellStyle))
//    //            {
//    //                column.CellStyle = dataGrid.FindResource(metadata.CellStyle) as Style;
//    //            }

//    //            if (!string.IsNullOrEmpty(metadata.HeaderStyle))
//    //            {
//    //                column.HeaderStyle = dataGrid.FindResource(metadata.HeaderStyle) as Style;
//    //            }

//    //            dataGrid.Columns.Add(column);
//    //        });
//    //        dataGrid.ItemsSource = adapter;
//    //    }
//    //}
//}
