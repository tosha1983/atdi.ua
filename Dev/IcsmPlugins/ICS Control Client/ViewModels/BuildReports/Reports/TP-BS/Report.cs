using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Atdi.Contracts.WcfServices.Sdrn.Server.IeStation;
using XICSM.ICSControlClient.Models.Views;
using System.Linq;
using Atdi.Common;
using XICSM.ICSControlClient.ViewModels.Reports;

namespace XICSM.ICSControlClient.ViewModels.Reports
{
    /// <summary>
    /// Класс генерации отчета 
    /// </summary>
    public static class Report
    {

        public static PagesWithProtocols[] PrepareData(List<HeadProtocols> allHeadProtocols)
        {
           PagesByOwnerAndStandard[] allPages = new PagesByOwnerAndStandard[]
           {
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''КИЇВСТАР''GSM-900",
                      OwnerName = "ПРИВАТНЕ АКЦІОНЕРНЕ ТОВАРИСТВО ''КИЇВСТАР''",
                      Standard = "GSM-900"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''КИЇВСТАР''DCS-1800",
                     OwnerName = "ПРИВАТНЕ АКЦІОНЕРНЕ ТОВАРИСТВО ''КИЇВСТАР''",
                     Standard = "DECT",
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''КИЇВСТАР''LTE-1800",
                     OwnerName ="ПРИВАТНЕ АКЦІОНЕРНЕ ТОВАРИСТВО ''КИЇВСТАР''",
                     Standard ="LTE-1800"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''КИЇВСТАР''LTE-1800TH",
                     OwnerName ="ПРИВАТНЕ АКЦІОНЕРНЕ ТОВАРИСТВО ''КИЇВСТАР''",
                     Standard ="LTE-1800TN"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''КИЇВСТАР''LTE-2600",
                     OwnerName ="ПРИВАТНЕ АКЦІОНЕРНЕ ТОВАРИСТВО ''КИЇВСТАР''",
                     Standard ="LTE-2600"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''КИЇВСТАР''UMTS-WCDMA",
                     OwnerName ="ПРИВАТНЕ АКЦІОНЕРНЕ ТОВАРИСТВО ''КИЇВСТАР''",
                     Standard ="UMTS"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName = "ТОВ ''лайфселл''GSM-900",
                     OwnerName ="Товариство з обмеженою відповідальністю ''лайфселл''",
                     Standard = "GSM-900"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName = "ТОВ ''лайфселл''DCS-1800",
                     OwnerName ="Товариство з обмеженою відповідальністю ''лайфселл''",
                     Standard ="DECT"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ТОВ ''лайфселл''LTE-1800",
                     OwnerName ="Товариство з обмеженою відповідальністю ''лайфселл''",
                     Standard ="LTE-1800"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ТОВ ''лайфселл''LTE-1800TH",
                     OwnerName ="Товариство з обмеженою відповідальністю ''лайфселл''",
                     Standard ="LTE-1800TN"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ТОВ ''лайфселл''LTE-2600",
                     OwnerName ="Товариство з обмеженою відповідальністю ''лайфселл''",
                     Standard ="LTE-2600"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ТОВ ''лайфселл''(CDMA) IS-95",
                     OwnerName ="Товариство з обмеженою відповідальністю ''лайфселл''",
                     Standard ="CDMA-800"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName = "ТОВ ''лайфселл''UMTS-WCDMA",
                     OwnerName ="Товариство з обмеженою відповідальністю ''лайфселл''",
                     Standard ="UMTS"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''ВФ Україна''GSM-900",
                     OwnerName ="Приватне акціонерне товариство ''ВФ Україна''",
                     Standard ="GSM-900"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''ВФ Україна''DCS-1800",
                     OwnerName ="Приватне акціонерне товариство ''ВФ Україна''",
                     Standard ="DECT"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''ВФ Україна''LTE-1800TH",
                     OwnerName ="Приватне акціонерне товариство ''ВФ Україна''",
                     Standard ="LTE-1800TN"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''ВФ Україна''LTE-2600",
                     OwnerName ="Приватне акціонерне товариство ''ВФ Україна''",
                     Standard = "LTE-2600"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ПрАТ ''ВФ Україна''UMTS-WCDMA",
                     OwnerName ="Приватне акціонерне товариство ''ВФ Україна''",
                     Standard ="UMTS"
                },

                new PagesByOwnerAndStandard()
                {
                     PageName ="ТОВ ''ТриМоб''UMTS-WCDMA",
                     OwnerName ="Товариство з обмеженою відповідальністю ''ТриМоб''",
                     Standard ="UMTS"
                },
                new PagesByOwnerAndStandard()
                {
                     PageName ="ТОВ ''Інтер. комунік.''(CDMA) I",
                     OwnerName ="ТОВАРИСТВО З ОБМЕЖЕНОЮ ВІДПОВІДАЛЬНІСТЮ ''ІНТЕРНАЦІОНАЛЬНІ ТЕЛЕКОМУНІКАЦІЇ''",
                     Standard ="CDMA-800"
                },
       };
            var dozvLTE1800_TN = "ТН";

            var pagesWithProtocols = new PagesWithProtocols[allPages.Length];
            for (int h = 0; h < allPages.Length; h++)
            {
                pagesWithProtocols[h] = new PagesWithProtocols();
                pagesWithProtocols[h].PageName = allPages[h].PageName;
                pagesWithProtocols[h].Standard = allPages[h].Standard;
                pagesWithProtocols[h].OperatorName = allPages[h].OwnerName;
                var linkedStandards = allPages[h];

                var listHeadProtocols = new List<HeadProtocols>();
                var allHeaders = allHeadProtocols.FindAll(x => x.OwnerName == linkedStandards.OwnerName);
                if ((allHeaders != null) && (allHeaders.Count > 0))
                {
                    for (int i = 0; i < allHeaders.Count; i++)
                    {
                        var header = allHeaders[i];
                        if ((header.DetailProtocols != null) && (header.DetailProtocols.Length > 0))
                        {
                            var cpyHeader = CopyHelper.CreateDeepCopy(header);
                            var lstDetail = cpyHeader.DetailProtocols.ToList();
                            if ((lstDetail != null) && (lstDetail.Count > 0))
                            {
                                if ((linkedStandards.Standard.Contains("LTE-1800")) && (linkedStandards.Standard.EndsWith("TN")))
                                {
                                    var allLTE_1800TN = lstDetail.FindAll(v => linkedStandards.Standard.Contains(v.Standard) && v.PermissionNumber.StartsWith(dozvLTE1800_TN));
                                    if ((allLTE_1800TN != null) && (allLTE_1800TN.Count > 0))
                                    {
                                        cpyHeader.DetailProtocols = CopyHelper.CreateDeepCopy(allLTE_1800TN).ToArray();
                                        listHeadProtocols.Add(cpyHeader);
                                    }
                                }
                                else
                                {
                                    var allLTE_1800TN = lstDetail.FindAll(v => linkedStandards.Standard.Contains(v.Standard) && v.PermissionNumber.StartsWith(dozvLTE1800_TN));
                                    if (((allLTE_1800TN != null) && (allLTE_1800TN.Count == 0)) || (allLTE_1800TN == null))
                                    {
                                        var allDetail = lstDetail.FindAll(v => v.Standard == linkedStandards.Standard);
                                        if ((allDetail != null) && (allDetail.Count > 0))
                                        {
                                            cpyHeader.DetailProtocols = CopyHelper.CreateDeepCopy(allDetail).ToArray();
                                            listHeadProtocols.Add(cpyHeader);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                pagesWithProtocols[h].HeadProtocols = listHeadProtocols.ToArray();
            }
            return pagesWithProtocols;
        }


            /// <summary>
            /// Сохранить в файл
            /// </summary>
            /// <param name="path"></param>
            /// <param name="province"></param>
            /// <param name="repType"></param>
        public static void Save(string path, string year, string month,  PagesWithProtocols[] headProtocols)
        {
            ReportExcelFast rep = new ReportExcelFast();
            for (int g = 0; g < headProtocols.Length; g++)
            {

                double[] ListHeadColumnWidth = { 5.57, 14, 14, 18, 20.14, 11.57, 11, 9.86, 9.71, 10.71, 10.86, 10.43, 10.71, 10.2, 10.2, 10.2, 10.2, 10 };
                string[] ListHead = {"№ запису",
                                     "Означення станції, фактично",
                                     "Означення станції (заявлено)",
                                      "№ дозволу на експлуатацію",
                                        "Адреса станції",
                                            "Широта вимірювання",
                                            "Довгота вимірювання",
                                            "Номер каналу фактичний",
                                            "Номери каналів заявлені",
                                            "Частота прийому МГц фактична",
                                            "Частота прийому МГц заявлена",
                                            "Частота передачі МГц фактична",
                                            "Частота передачі МГц заявлена",
                                            "Рівень сигналу, dBm",
                                            "Ширина займаної смуги радіочастот (відповідає/не відповідає)",
                                            "Дата контролю",
                                            "Час вимірювання",
                                            "Примітка"
                                           };

                if (g == 0)
                {
                    rep.Init(Path.Combine(path, $"Тр - бс УРЧМ УДЦР {year} рік {month} {year}"), headProtocols[g].PageName, "");
                }
                else
                {
                    rep.AddSheet(headProtocols[g].PageName);
                }
                var line = new List<string>();
                rep.UnionCellByHeight(0, 1, 18, 1, rep.GetCellStyleBackgroundDefaultColor(), "ДАНІ",1.2f);
                rep.UnionCellByHeight(0, 2, 18, 2, rep.GetCellStyleBackgroundDefaultColor(), $"щодо контролю базових станцій оператора {headProtocols[g].OperatorName}", 1.2f);
                rep.UnionCellByHeight(0, 3, 18, 3, rep.GetCellStyleBackgroundDefaultColor(), $"системи цифрового стільникового радіозв'язку загального користування стандарту {headProtocols[g].Standard} (ТР-бс)", 1.2f);
                rep.UnionCellByHeight(0, 4, 18, 4, rep.GetCellStyleBackgroundDefaultColor(), $"у м. Київ та Київська обл. філії (за м. Київ та Київська обл. область) за {month} {year} р.", 1.2f);
                

                line.Clear();
                for (int i = 0; i < ListHeadColumnWidth.Length; i++)
                {
                    rep.SetColumnWidth(i, (int)ListHeadColumnWidth[i]);
                    line.Add(ListHead[i]);
                }
                int curr_num = 6;
                rep.SetIndexRow(curr_num);
                rep.WriteLine(line.ToArray());
                for (int i = 0; i < ListHeadColumnWidth.Length; i++)
                {
                    rep.SetCellBackgroundColor(i, curr_num);
                }


                line.Clear();
                curr_num++;
                rep.SetIndexRow(curr_num);
                for (int i = 0; i < ListHeadColumnWidth.Length; i++)
                {
                    line.Add((i + 1).ToString());
                }
                rep.WriteLine(line.ToArray());


                curr_num++;
                rep.SetIndexRow(curr_num);




                int counter = 1;
                for (int rx = 0; rx < headProtocols[g].HeadProtocols.Length; rx++)
                {
                    var protocolDetails = headProtocols[g].HeadProtocols;
                    if ((protocolDetails != null) && (protocolDetails.Length > 0))
                    {
                        if (protocolDetails[rx].DetailProtocols != null)
                        {
                            //for (int sub = 0; sub < protocolDetails[rx].DetailProtocols.Length; sub++)
                            {
                                line.Clear();

                                //  № запису
                                line.Add(string.Format("{0}", counter++));

                                // Означення станціїї фактично
                                var globalSID = protocolDetails[rx].DetailProtocols.Select(x => x.GlobalSID);
                                if ((globalSID != null) && (globalSID.Count() > 0))
                                {
                                    line.Add(GenerateMultyData(globalSID.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }

                                // Означення станції (заявлено)
                                line.Add(protocolDetails[rx].PermissionGlobalSID);
                                // № дозволу на експлуатацію
                                line.Add(protocolDetails[rx].PermissionNumber);
                                // Адреса станції
                                line.Add(protocolDetails[rx].Address);
                                // Широта вимірювання
                                var latitudes = protocolDetails[rx].DetailProtocols.Select(x => x.Latitude);
                                if ((latitudes != null) && (latitudes.Count() > 0))
                                {
                                    var arrlat = latitudes.ToArray();
                                    var lstLat = new List<string>();
                                    for (int d = 0; d < arrlat.Length; d++)
                                    {
                                        lstLat.Add(ConvertCoordinates.DecToDmsToString2(arrlat[d].Value, Coordinates.EnumCoordLine.Lat));
                                    }
                                    line.Add(GenerateMultyData(lstLat.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }
                                // Довгота вимірювання
                                var longitudes = protocolDetails[rx].DetailProtocols.Select(x => x.Longitude);
                                if ((longitudes != null) && (longitudes.Count() > 0))
                                {
                                    var arrlon = longitudes.ToArray();
                                    var lstLon = new List<string>();
                                    for (int d = 0; d < arrlon.Length; d++)
                                    {
                                        lstLon.Add(ConvertCoordinates.DecToDmsToString2(arrlon[d].Value, Coordinates.EnumCoordLine.Lon));
                                    }
                                    line.Add(GenerateMultyData(lstLon.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }


                                //Номер каналу фактичний
                                var chennelsTx = protocolDetails[rx].DetailProtocols.Select(x => x.StationTxChannel);
                                var chennelsRx = protocolDetails[rx].DetailProtocols.Select(x => x.StationRxChannel);
                                var standards = protocolDetails[rx].DetailProtocols.Select(x => x.Standard).ToArray();
                                if (((chennelsRx != null) && (chennelsRx.Count() > 0)) && ((chennelsTx != null) && (chennelsTx.Count() > 0)) && (chennelsTx.Count() == chennelsTx.Count()) && (standards!=null) && (standards.Length>0) && (standards[0]== "UMTS"))
                                {
                                    // для UMTS формируем номер канала в формате RXChannel/TXChanel
                                    var lstChannels = new List<string>();
                                    var arrChannelTx = chennelsTx.ToArray();
                                    var arrChannelRx = chennelsRx.ToArray();

                                   
                                    for (int d = 0; d < arrChannelTx.Count(); d++)
                                    {
                                        var chan = "";

                                        var chArrRx = SeparateString(arrChannelRx[d], ';');
                                        if ((chArrRx != null) && (chArrRx.Length > 0))
                                        {
                                            chan = chArrRx[0];
                                        }

                                        var chArrTx = SeparateString(arrChannelTx[d], ';');
                                        if ((chArrTx!=null) && (chArrTx.Length>0))
                                        {
                                            chan += "/"+chArrTx[0];
                                        }

                                        lstChannels.Add(chan);
                                    }

                                    line.Add(GenerateMultyData(lstChannels.ToArray()));

                                }
                                else if ((chennelsTx != null) && (chennelsTx.Count() > 0))
                                {
                                    // для остальных  стандартов номер канала используем тот который идет первым в списке TXChanel
                                    var lstChannels = new List<string>();
                                    var arrChannelTx = chennelsTx.ToArray();

                                    for (int d = 0; d < arrChannelTx.Count(); d++)
                                    {
                                        var chan = "";

                                        var chArrTx = SeparateString(arrChannelTx[d], ';');
                                        if ((chArrTx != null) && (chArrTx.Length > 0))
                                        {
                                            chan = chArrTx[0];
                                        }

                                        lstChannels.Add(chan);
                                    }

                                    line.Add(GenerateMultyData(lstChannels.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }


                                // Номери каналів заявлені
                                if ((standards != null) && (standards.Length > 0) && (standards[0] != "UMTS"))
                                {
                                    //  для остальных  стандартов номера каналов формируем путем поиска из всего списка каналов минимального и максмального
                                    var lstchennelsTx = new List<string>();
                                    var channelsTx = protocolDetails[rx].DetailProtocols.Select(x => x.StationTxChannel);
                                    if ((channelsTx != null) && (channelsTx.Count() > 0))
                                    {
                                        var arrchannels = channelsTx.ToArray();
                                        var lstchannels = new List<string>();
                                        for (int d = 0; d < arrchannels.Count(); d++)
                                        {
                                            var chann = SeparateString(arrchannels[d], ';');
                                            if ((chann != null) && (chann.Length > 0))
                                            {
                                                var doubleArrchann = ConvertToDoubleArray(chann);
                                                if ((doubleArrchann != null) && (doubleArrchann.Length > 0))
                                                {
                                                    if (doubleArrchann.Min() != doubleArrchann.Max())
                                                    {
                                                        lstchennelsTx.Add(GenerateMultyData(new string[2] { doubleArrchann.Min().ToString(), doubleArrchann.Max().ToString() + ";" }, "-"));
                                                    }
                                                    else
                                                    {
                                                        lstchennelsTx.Add(GenerateMultyData(new string[1] { doubleArrchann.Min() + ";" }, "-"));
                                                    }
                                                }
                                            }
                                        }
                                        line.Add(GenerateMultyData(lstchennelsTx.Distinct().ToArray()));
                                    }
                                    else
                                    {
                                        line.Add("");
                                    }
                                }
                                else
                                {
                                    //  для UMTS номера канала извлекаем из RxChannel тот который идет первым в списке
                                    var lstChannels = new List<string>();
                                    var arrChannelRx = chennelsRx.ToArray();
                                    if ((arrChannelRx != null) && (arrChannelRx.Length > 0))
                                    {

                                        for (int k = 0; k < arrChannelRx.Length; k++)
                                        {
                                            var chan = "";
                                            var chArrRx = SeparateString(arrChannelRx[k], ';');
                                            if ((chArrRx != null) && (chArrRx.Length > 0))
                                            {
                                                chan = chArrRx[0];
                                            }
                                            lstChannels.Add(chan);
                                            break;
                                        }
                                        line.Add(GenerateMultyData(lstChannels.ToArray()));
                                    }
                                    else
                                    {
                                        line.Add("");
                                    }
                                }




                                // Частота прийому МГц фактична та Частота прийому МГц заявлена
                                var lstFreqRxD = new List<string>();
                                var allFreqRxDouble = new List<double>();
                                var freqRx = protocolDetails[rx].DetailProtocols.Select(x => x.StationRxFreq);
                                if ((freqRx != null) && (freqRx.Count() > 0))
                                {
                                    var arrFreq = freqRx.ToArray();
                                    var lstFreqRx = new List<string>();
                                    for (int d = 0; d < arrFreq.Count(); d++)
                                    {
                                        var freqRxArr = SeparateString(arrFreq[d], ';');
                                        if ((freqRxArr != null) && (freqRxArr.Length > 0))
                                        {
                                            lstFreqRx.Add(freqRxArr[0]);
                                            var doubleArrFreq = ConvertToDoubleArray(freqRxArr);
                                            if ((doubleArrFreq != null) && (doubleArrFreq.Length > 0))
                                            {
                                                allFreqRxDouble.AddRange(doubleArrFreq);
                                            }
                                        }
                                    }
                                    if ((allFreqRxDouble != null) && (allFreqRxDouble.Count > 0))
                                    {
                                        if (allFreqRxDouble.Min() != allFreqRxDouble.Max())
                                        {
                                            lstFreqRxD.Add(GenerateMultyData(new string[2] { allFreqRxDouble.Min().ToString(), allFreqRxDouble.Max().ToString() + ";" }, "-"));
                                        }
                                        else
                                        {
                                            lstFreqRxD.Add(GenerateMultyData(new string[1] { allFreqRxDouble.Min().ToString() + ";" }, "-"));
                                        }
                                    }
                                    // Частота прийому МГц фактична
                                    line.Add(GenerateMultyData(lstFreqRx.ToArray()));
                                    // Частота прийому МГц заявлена
                                    line.Add(GenerateMultyData(lstFreqRxD.Distinct().ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }


                                // Частота передачі МГц фактична
                                var freqTx = protocolDetails[rx].DetailProtocols.Select(x => x.Freq_MHz);
                                if ((freqTx != null) && (freqTx.Count() > 0))
                                {
                                    line.Add(GenerateMultyData(freqTx.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }


                                // Частота передачі МГц заявлена
                                var lstFreqTxD = new List<string>();
                                var stationTxFreq = protocolDetails[rx].DetailProtocols.Select(x => x.StationTxFreq);
                                if ((stationTxFreq != null) && (stationTxFreq.Count() > 0))
                                {
                                    var arrFreq = stationTxFreq.ToArray();
                                    
                                    for (int d = 0; d < arrFreq.Count(); d++)
                                    {
                                        var freqTxArr = SeparateString(arrFreq[d], ';');
                                        if ((freqTxArr != null) && (freqTxArr.Length > 0))
                                        {
                                            var doubleArrFreq = ConvertToDoubleArray(freqTxArr);
                                            if ((doubleArrFreq != null) && (doubleArrFreq.Length > 0))
                                            {
                                                if (doubleArrFreq.Min() != doubleArrFreq.Max())
                                                {
                                                    lstFreqTxD.Add(GenerateMultyData(new string[2] { doubleArrFreq.Min().ToString(), doubleArrFreq.Max().ToString() + ";" }, "-"));
                                                }
                                                else
                                                {
                                                    lstFreqTxD.Add(GenerateMultyData(new string[1] { doubleArrFreq.Min().ToString() + ";" }, "-"));
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    line.Add("");
                                }

                                var txFreqVal = GenerateMultyData(lstFreqTxD.Distinct().ToArray());
                                line.Add(txFreqVal);

                                // Рівень сигналу, dBm
                                var Level_dBm = protocolDetails[rx].DetailProtocols.Select(x => x.Level_dBm);
                                if ((Level_dBm != null) && (Level_dBm.Count() > 0))
                                {
                                    var lstLevels = new List<string>();
                                    var arrLevels = Level_dBm.ToArray();
                                    for (int d = 0; d < arrLevels.Length; d++)
                                    {
                                        lstLevels.Add(Math.Round(arrLevels[d].Value,2).ToString());
                                    }

                                    line.Add(GenerateMultyData(lstLevels.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }

                                //Ширина займаної смуги радіочастот (відповідає/не відповідає)
                                line.Add("відповідає");
                                
                                // Дата контролю
                                var dateMeas_OnlyDate = protocolDetails[rx].DetailProtocols.Select(x => x.DateMeas_OnlyDate);
                                if ((dateMeas_OnlyDate != null) && (dateMeas_OnlyDate.Count() > 0))
                                {
                                    line.Add(GenerateMultyData(dateMeas_OnlyDate.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }

                                // Час вимірювання
                                var DateMeas_OnlyTime = protocolDetails[rx].DetailProtocols.Select(x => x.DateMeas_OnlyTime);
                                if ((DateMeas_OnlyTime != null) && (DateMeas_OnlyTime.Count() > 0))
                                {
                                    line.Add(GenerateMultyData(DateMeas_OnlyTime.ToArray()));
                                }
                                else
                                {
                                    line.Add("");
                                }

                                // Примітка
                                line.Add("");
                                rep.WriteLine(line.ToArray());

                                curr_num++;
                                rep.SetIndexRow(curr_num);

                            }
                        }
                    }
                }

                curr_num++;
                rep.SetIndexRow(curr_num);



            }

            rep.Save();
        }

        private static string GenerateMultyData(string[] values)
        {
            return string.Join(System.Environment.NewLine, values.Select(x=>x.Replace(",",".")));
        }

        private static string GenerateMultyData(string[] values, string delimiter)
        {
            var str = string.Join(delimiter, values);
            if (str.EndsWith(";"))
            {
                str = str.Remove(str.Length-1, 1);
            }
            return str;
        }

        private static string GenerateMultyData(double[] values)
        {
            return string.Join(System.Environment.NewLine, values);
        }

        private static string GenerateMultyData(double?[] values)
        {
            return string.Join(System.Environment.NewLine, values);
        }

        private static string GenerateMultyData(DateTime?[] values)
        {
            return string.Join(System.Environment.NewLine, values.Select(x => x.Value.ToString("dd.MM.yyyy")));
        }

        private static string GenerateMultyData(TimeSpan?[] values)
        {
            return string.Join(System.Environment.NewLine, values);
        }

        private static string[] SeparateString(string value, char delimiter)
        {
            if (value != null)
            {
                return value.Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return null;
            }
        }

        private static string GenerateFreqs(string[] values, string delimiter)
        {
            var str = string.Join(delimiter, values);
            if (str.EndsWith(";"))
            {
                str = str.Remove(str.Length - 1, 1);
            }
            return str;
        }


        private static double[] ConvertToDoubleArray(string[] value)
        {
            var listArr = new List<double>();
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    var val = value[i].ConvertStringToDouble();
                    if (val != null)
                    {
                        listArr.Add(val.Value);
                    }
                }
            }
            listArr.Sort();
            var distArr  = listArr.Distinct();
            return distArr.ToArray();
        }
    }

}
