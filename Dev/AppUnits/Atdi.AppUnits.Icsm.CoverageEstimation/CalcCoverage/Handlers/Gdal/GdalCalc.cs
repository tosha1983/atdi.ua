using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using OSGeo.GDAL;
using Atdi.Platform.Logging;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;



namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{

    public class GdalCalc
    {
        private const int MaxCountThreadFilesForFinalCoverage = 250;
        private const int MaxCountThreadFilesForConcatBlank = 50;
        private static int[,] grayMatrixGlobal { get; set; }
        private ILogger _logger { get; set; }
        public GdalCalc(ILogger logger)
        {
            this._logger = logger;
            GdalConfiguration.ConfigureGdal();
            Gdal.AllRegister();
        }


        private static TFWParameter GetTFWParameter(string TiffFile)
        {
            var tFWParameter = new TFWParameter();
            var fileTFWBlank = Path.GetDirectoryName(TiffFile) + @"\" + Path.GetFileNameWithoutExtension(TiffFile) + ".TFW";
            var words = File.ReadAllText(fileTFWBlank).Split(new char[] { '\t', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words == null)
            {
                throw new InvalidOperationException($"File '{TiffFile}' is null content");
            }
            else
            {
                if (words.Length != 6)
                {
                    throw new InvalidOperationException($"Incorrect TFW file '{fileTFWBlank}'");
                }
                else
                {
                    tFWParameter.Step = Convert.ToDouble(words[0]);
                    tFWParameter.Coordinate = new Coordinate();
                    tFWParameter.Coordinate.X = (int)Convert.ToDouble(words[4]);
                    tFWParameter.Coordinate.Y = (int)Convert.ToDouble(words[5]);
                }
            }
            return tFWParameter;
        }

        private static Coordinate GetCoordinateAfterRecalc(int MatrixY, int MatrixX, int MatrixHeight, int MatrixWidth, TFWParameter tFWParameterBlank, TFWParameter tFWParameterStation)
        {
            var coordinate = new Coordinate();
            if (tFWParameterBlank.Step > 0)
            {
                if ((tFWParameterStation.Coordinate.X >= tFWParameterBlank.Coordinate.X) && (tFWParameterStation.Coordinate.Y <= tFWParameterBlank.Coordinate.Y))
                {
                    int deltaX = (int)((tFWParameterStation.Coordinate.X - tFWParameterBlank.Coordinate.X) / (tFWParameterBlank.Step));
                    int deltaY = (int)((-tFWParameterStation.Coordinate.Y + tFWParameterBlank.Coordinate.Y) / (tFWParameterBlank.Step));
                    coordinate.X = deltaX + MatrixX;
                    coordinate.Y = deltaY + MatrixY;
                }
                else
                {
                    throw new InvalidOperationException($"Incorrect TFWParameter's");
                }
            }
            else
            {
                throw new InvalidOperationException($"Incorrect TFWParameter.Step = '{tFWParameterBlank.Step}'");
            }
            return coordinate;
        }

        private System.Threading.Thread CreateNewThreadConcatBlankWithStation(DataForThread dataForThread)
        {
            var thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadConcatBlankWithStation))
            {
                Priority = System.Threading.ThreadPriority.Lowest
            };
            thread.Start(dataForThread);
            return thread;
        }

        private static void ThreadConcatBlankWithStation(object data)
        {
            var dataForThread = data as DataForThread;
            var redBand = dataForThread.DatasetBlank.GetRasterBand(1);
            var greenBand = dataForThread.DatasetBlank.GetRasterBand(2);
            var blueBand = dataForThread.DatasetBlank.GetRasterBand(3);
            var alphaBand = dataForThread.DatasetBlank.GetRasterBand(4);
            int width = redBand.XSize;
            int height = redBand.YSize;

            if (Path.GetFileNameWithoutExtension(dataForThread.SourceFileName.ToLower()) != Path.GetFileNameWithoutExtension(dataForThread.BlankFileName.ToLower()))
            {
                var tFWSourceTIFFile = GetTFWParameter(dataForThread.SourceFileName);
                var tempCoverageFile = dataForThread.DataConfig.DirectoryConfig.TempTIFFFilesDirectory + @"\" + Path.GetFileNameWithoutExtension(dataForThread.SourceFileName) + $"_{Path.GetFileNameWithoutExtension(dataForThread.NameEwxFile)}_out.TIF";
                using (var outImage = Gdal.GetDriverByName("GTiff").Create(tempCoverageFile, width, height, 5, DataType.GDT_Byte, new string[] { "COMPRESS=PACKBITS" }))
                {
                    var outGrayBand = outImage.GetRasterBand(5);
                    outGrayBand.SetColorInterpretation(ColorInterp.GCI_GrayIndex);

                 
                    outImage.SetProjection(dataForThread.Projection);
                    outImage.SetGeoTransform(dataForThread.GeoTransform);


                    var imageNameSourceTIFFile = Gdal.Open(dataForThread.SourceFileName, Access.GA_ReadOnly);
                    var grayBandImageNameSourceTIFFile = imageNameSourceTIFFile.GetRasterBand(1);
                    var widthImageNameSourceTIFFile = grayBandImageNameSourceTIFFile.XSize;
                    var heightImageNameSourceTIFFile = grayBandImageNameSourceTIFFile.YSize;

                    var MergedMatrix = new int[height, width];
                    for (int h = 0; h < heightImageNameSourceTIFFile; h++)
                    {
                        int[] gray = new int[widthImageNameSourceTIFFile];

                        grayBandImageNameSourceTIFFile.ReadRaster(0, h, widthImageNameSourceTIFFile, 1, gray, widthImageNameSourceTIFFile, 1, 0, 0);

                        for (int w = 0; w < widthImageNameSourceTIFFile; w++)
                        {
                            var coord = GetCoordinateAfterRecalc(h, w, heightImageNameSourceTIFFile, widthImageNameSourceTIFFile, dataForThread.TFWBlank, tFWSourceTIFFile);
                            if ((coord.X < width) && (coord.Y < height))
                            {
                                MergedMatrix[coord.Y, coord.X] = gray[w];
                            }
                        }
                    }

                    imageNameSourceTIFFile.Dispose();

                    for (int h = 0; h < height; h++)
                    {
                        int[] gray = new int[width];
                        for (int w = 0; w < width; w++)
                        {
                            gray[w] = MergedMatrix[h, w];
                        }
                        outGrayBand.WriteRaster(0, h, width, 1, gray, width, 1, 0, 0);
                    }
                    outImage.FlushCache();
                }
                GC.Collect();
            }
        }

        private static List<string[]> BreakDown(string[] elements, int CountInParams)
        {
            var arrstring = new List<string[]>();
            var liststring = new List<string>();
            int cnt = 1;
            for (int i = 0; i < elements.Length; i++)
            {
                liststring.Add(elements[i]);
                if (cnt >= CountInParams)
                {
                    arrstring.Add(liststring.ToArray());
                    liststring.Clear();
                    cnt = 0;
                }
                ++cnt;
            }
            if ((liststring != null) && (liststring.Count > 0))
            {
                arrstring.Add(liststring.ToArray());
            }
            return arrstring;
        }

        public bool StartProcessConcatBlankWithStation(DataConfig dataConfig, string ICSTelecomProjectDir, string NameBlankFile, string NameEwxFileValue)
        {
            bool isSuccessCreateFiles = false;
            try
            {
                var image = Gdal.Open(NameBlankFile, Access.GA_ReadOnly);
                var tFWBlank = GetTFWParameter(NameBlankFile);
                var files = Directory.GetFiles(ICSTelecomProjectDir, "*.TIF");
                var geoTransformerData = new double[6];
                image.GetGeoTransform(geoTransformerData);
                var proj = image.GetProjection();


                var lstFiles = BreakDown(files, MaxCountThreadFilesForConcatBlank);
                for (int j = 0; j < lstFiles.Count; j++)
                {
                    var filesSource = lstFiles[j];
                    var listThreads = new System.Threading.Thread[filesSource.Length];
                    for (int i = 0; i < filesSource.Length; i++)
                    {
                        var param = new DataForThread()
                        {
                            TFWBlank = tFWBlank,
                            BlankFileName = NameBlankFile,
                            DataConfig = dataConfig,
                            SourceFileName = filesSource[i],
                            DatasetBlank = image,
                            Projection = proj,
                            GeoTransform = geoTransformerData,
                            NameEwxFile = NameEwxFileValue
                        };
                        listThreads[i] = CreateNewThreadConcatBlankWithStation(param);
                    }
                    for (int i = 0; i < listThreads.Length; i++)
                    {
                        listThreads[i].Join();
                        this._logger.Info(Contexts.CalcCoverages, string.Format(Events.OperationSaveTempCovarageFileCompleted.ToString(), filesSource[i]));
                    }
                }
                image.Dispose();
                isSuccessCreateFiles = true;
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return isSuccessCreateFiles;
        }

        public bool SaveRecalcTIFFFile(DataConfig dataConfig, string ICSTelecomProjectDir, string NameBlankFile)
        {
            bool isSuccessCreateFiles = false;
            try
            {
                var tFWBlank = GetTFWParameter(NameBlankFile);
                var filesSource = Directory.GetFiles(ICSTelecomProjectDir, "*.TIF");

                using (var image = Gdal.Open(NameBlankFile, Access.GA_ReadOnly))
                {
                    var redBand = image.GetRasterBand(1);
                    var greenBand = image.GetRasterBand(2);
                    var blueBand = image.GetRasterBand(3);
                    var alphaBand = image.GetRasterBand(4);

                    int width = redBand.XSize;
                    int height = redBand.YSize;
                    
                    for (int i = 0; i < filesSource.Length; i++)
                    {
                        if (Path.GetFileNameWithoutExtension(filesSource[i].ToLower()) == Path.GetFileNameWithoutExtension(NameBlankFile.ToLower()))
                        {
                            continue;
                        }
                        var tFWSourceTIFFile = GetTFWParameter(filesSource[i]);
                        var tempCoverageFile = dataConfig.DirectoryConfig.TempTIFFFilesDirectory + @"\" + Path.GetFileNameWithoutExtension(filesSource[i]) + "_out.TIF";
                        using (var outImage = Gdal.GetDriverByName("GTiff").Create(tempCoverageFile, width, height, 5, DataType.GDT_Byte, new string[] { "COMPRESS=PACKBITS" }))
                        {
                            var outGrayBand = outImage.GetRasterBand(5);
                            outGrayBand.SetColorInterpretation(ColorInterp.GCI_GrayIndex);

                            var geoTransformerData = new double[6];
                            image.GetGeoTransform(geoTransformerData);
                            var proj = image.GetProjection();
                            outImage.SetProjection(proj);
                            outImage.SetGeoTransform(geoTransformerData);


                            var imageNameSourceTIFFile = Gdal.Open(filesSource[i], Access.GA_ReadOnly);
                            var grayBandImageNameSourceTIFFile = imageNameSourceTIFFile.GetRasterBand(1);
                            var widthImageNameSourceTIFFile = grayBandImageNameSourceTIFFile.XSize;
                            var heightImageNameSourceTIFFile = grayBandImageNameSourceTIFFile.YSize;

                            var MergedMatrix = new int[height, width];
                            for (int h = 0; h < heightImageNameSourceTIFFile; h++)
                            {
                                int[] gray = new int[widthImageNameSourceTIFFile];

                                grayBandImageNameSourceTIFFile.ReadRaster(0, h, widthImageNameSourceTIFFile, 1, gray, widthImageNameSourceTIFFile, 1, 0, 0);

                                for (int w = 0; w < widthImageNameSourceTIFFile; w++)
                                {
                                    var coord = GetCoordinateAfterRecalc(h, w, heightImageNameSourceTIFFile, widthImageNameSourceTIFFile, tFWBlank, tFWSourceTIFFile);
                                    if ((coord.X < width) && (coord.Y < height))
                                    {
                                        MergedMatrix[coord.Y, coord.X] = gray[w];
                                    }
                                }
                            }

                            imageNameSourceTIFFile.Dispose();

                            for (int h = 0; h < height; h++)
                            {
                                int[] gray = new int[width];
                                for (int w = 0; w < width; w++)
                                {
                                    gray[w] = MergedMatrix[h, w];
                                }
                                outGrayBand.WriteRaster(0, h, width, 1, gray, width, 1, 0, 0);
                            }
                            outImage.FlushCache();
                            isSuccessCreateFiles = true;
                            this._logger.Info(Contexts.CalcCoverages, string.Format(Events.OperationSaveTempCovarageFileCompleted.ToString(), tempCoverageFile));
                        }
                        GC.Collect();
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return isSuccessCreateFiles;
        }

        public void ClearBlank(DataConfig dataConfig, string NameBlankFile)
        {
            string clearName = "";
            try
            {
                using (var image = Gdal.Open(NameBlankFile, Access.GA_ReadOnly))
                {
                    var redBand = image.GetRasterBand(1);
                    int width = redBand.XSize;
                    int height = redBand.YSize;
                    clearName = Path.GetDirectoryName(NameBlankFile) + @"\" + Path.GetFileNameWithoutExtension(NameBlankFile) + @"._clear.TIF";

                    using (var outImage = Gdal.GetDriverByName("GTiff").Create(clearName, width, height, 4, DataType.GDT_Byte, new string[] { "COMPRESS=PACKBITS" }))
                    {
                        var outRBand = outImage.GetRasterBand(1);
                        var outGBand = outImage.GetRasterBand(2);
                        var outBBand = outImage.GetRasterBand(3);
                        var outABand = outImage.GetRasterBand(4);

                        var geoTransformerData = new double[6];
                        image.GetGeoTransform(geoTransformerData);
                        var proj = image.GetProjection();
                        outImage.SetProjection(proj);
                        outImage.SetGeoTransform(geoTransformerData);

                        for (int h = 0; h < height; h++)
                        {
                            int[] red = new int[width];
                            for (int w = 0; w < width; w++)
                            {
                                red[w] = 0;
                            }
                            outRBand.WriteRaster(0, h, width, 1, red, width, 1, 0, 0);
                        }
                        outImage.FlushCache();
                    }
                }
                if (File.Exists(clearName))
                {
                    if (File.Exists(NameBlankFile))
                    {
                        File.Delete(NameBlankFile);
                        File.Copy(clearName, NameBlankFile);
                        File.Delete(clearName);
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
        }

        public bool Run(DataConfig dataConfig, string OutDirectory, string FileName, string NameBlankFile)
        {
            var isSuccessCreateFile = false;
            try
            {
                using (var image = Gdal.Open(NameBlankFile, Access.GA_ReadOnly))
                {
                    var redBand = image.GetRasterBand(1);
                    var greenBand = image.GetRasterBand(2);
                    var blueBand = image.GetRasterBand(3);
                    var alphaBand = image.GetRasterBand(4);
                    var width = redBand.XSize;
                    var height = redBand.YSize;

                    var outPutFileName = OutDirectory + @"\" + FileName;
                    using (var outImage = Gdal.GetDriverByName("GTiff").Create(outPutFileName, width, height, 4, DataType.GDT_Byte, new string[] { "COMPRESS=PACKBITS" }))
                    {
                        var outRBand = outImage.GetRasterBand(1);
                        var outGBand = outImage.GetRasterBand(2);
                        var outBBand = outImage.GetRasterBand(3);
                        var outABand = outImage.GetRasterBand(4);

                        var geoTransformerData = new double[6];
                        image.GetGeoTransform(geoTransformerData);
                        var proj = image.GetProjection();
                        outImage.SetProjection(proj);
                        outImage.SetGeoTransform(geoTransformerData);
                        var grayMatrix = GetMaxRasterFromGeoTiffFilesWithThread(dataConfig, width, height, this._logger);
                       

                        for (int h = 0; h < height; h++)
                        {
                            var red = new int[width];
                            var green = new int[width];
                            var blue = new int[width];
                            var alpha = new int[width];

                            for (int w = 0; w < width; w++)
                            {
                                if (grayMatrix[h, w] != 0)
                                {
                                    var color = GetColorValue(dataConfig, grayMatrix[h, w]);
                                    if (color != null)
                                    {
                                        if (grayMatrix[h, w] == color.Gray)
                                        {
                                            red[w] = color.Red;
                                            green[w] = color.Green;
                                            blue[w] = color.Blue;
                                            alpha[w] = color.Alpha;

                                        }
                                    }

                                    var colorValue = GetColorByRangeValue(dataConfig, grayMatrix[h, w]);
                                    if (colorValue != null)
                                    {
                                        red[w] = colorValue.Red;
                                        green[w] = colorValue.Green;
                                        blue[w] = colorValue.Blue;
                                        alpha[w] = colorValue.Alpha;
                                    }

                                    if (grayMatrix[h, w] < GetMinimumGrayValue(dataConfig))
                                    {
                                        red[w] = 0;
                                        green[w] = 0;
                                        blue[w] = 0;
                                        alpha[w] = 0;
                                    }
                                    else if (grayMatrix[h, w] >= GetMaximumGrayValue(dataConfig))
                                    {
                                        var colorMaximum = GetColorValue(dataConfig, GetMaximumGrayValue(dataConfig));
                                        if (colorMaximum != null)
                                        {
                                            red[w] = colorMaximum.Red;
                                            green[w] = colorMaximum.Green;
                                            blue[w] = colorMaximum.Blue;
                                            alpha[w] = colorMaximum.Alpha;
                                        }
                                    }
                                }
                            }
                            outRBand.WriteRaster(0, h, width, 1, red, width, 1, 0, 0);
                            outGBand.WriteRaster(0, h, width, 1, green, width, 1, 0, 0);
                            outBBand.WriteRaster(0, h, width, 1, blue, width, 1, 0, 0);
                            outABand.WriteRaster(0, h, width, 1, alpha, width, 1, 0, 0);
                        }
                        outImage.FlushCache();
                        isSuccessCreateFile = true;
                        this._logger.Info(Contexts.CalcCoverages, string.Format(Events.OperationSaveFinalCovarageFileCompleted.ToString(), outPutFileName));
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return isSuccessCreateFile;
        }

        private System.Threading.Thread CreateNewThreadGetMaxRasterFromGeoTiffFiles(DataForThread dataForThread)
        {
          
            var thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadGetMaxRasterFromGeoTiffFiles))
            {
                Priority = System.Threading.ThreadPriority.Highest
            };
            thread.Start(dataForThread);
            return thread;
        }

        private static void ThreadGetMaxRasterFromGeoTiffFiles(object data)
        {
            var dataForThread = data as DataForThread;
            var imageSource = Gdal.Open(dataForThread.SourceFileName, Access.GA_ReadOnly);
            var grayBand = imageSource.GetRasterBand(5);
            var width = grayBand.XSize;
            var height = grayBand.YSize;
            for (int h = 0; h < height; h++)
            {
                var gray = new int[width];
                grayBand.ReadRaster(0, h, width, 1, gray, width, 1, 0, 0);
                for (int w = 0; w < width; w++)
                {
                    if (gray[w] != 0)
                    {
                        if ((grayMatrixGlobal[h, w] != 0) && (gray[w] != 0))
                        {
                            if (grayMatrixGlobal[h, w] < gray[w])
                            {
                                grayMatrixGlobal[h, w] = gray[w];
                            }
                        }
                        else if (grayMatrixGlobal[h, w] == 0)
                        {
                            grayMatrixGlobal[h, w] = gray[w];
                        }
                    }
                }
            }
            imageSource.Dispose();
        }

        private int[,] GetMaxRasterFromGeoTiffFilesWithThread(DataConfig dataConfig, int widthValue, int heightValue, ILogger logger)
        {
            grayMatrixGlobal = new int[heightValue, widthValue];
            var filesSources = Directory.GetFiles(Path.GetDirectoryName(dataConfig.DirectoryConfig.TempTIFFFilesDirectory), "*_out.TIF");
            var lstFiles = BreakDown(filesSources, MaxCountThreadFilesForFinalCoverage);
            for (int j = 0; j < lstFiles.Count; j++)
            {
                var filesSource = lstFiles[j];
                var listThreads = new System.Threading.Thread[filesSource.Length];
                for (int i = 0; i < filesSource.Length; i++)
                {
                    var param = new DataForThread()
                    {
                        SourceFileName = filesSource[i]
                    };
                    listThreads[i] = CreateNewThreadGetMaxRasterFromGeoTiffFiles(param);
                }
                int cnt = 0;
                for (int i = 0; i < listThreads.Length; i++)
                {
                    listThreads[i].Join();
                    this._logger.Info(Contexts.CalcCoverages, string.Format(Events.OperationSaveTempCovarageFileCompleted.ToString(), filesSource[i])+$" count = {cnt}");
                    cnt++;
                }
            }
            return grayMatrixGlobal;
        }


        private int[,] GetMaxRasterFromGeoTiffFiles(DataConfig dataConfig, int widthValue, int heightValue, ILogger logger)
        {
            var grayMatrix = new int[heightValue, widthValue];
            var filesSource = Directory.GetFiles(Path.GetDirectoryName(dataConfig.DirectoryConfig.TempTIFFFilesDirectory), "*_out.TIF");
            for (int i = 0; i < filesSource.Length; i++)
            {
                var imageSource = Gdal.Open(filesSource[i], Access.GA_ReadOnly);
                var grayBand = imageSource.GetRasterBand(5);
                var width = grayBand.XSize;
                var height = grayBand.YSize;
                for (int h = 0; h < height; h++)
                {
                    var gray = new int[width];
                    grayBand.ReadRaster(0, h, width, 1, gray, width, 1, 0, 0);
                    for (int w = 0; w < width; w++)
                    {
                        if (gray[w] != 0)
                        {
                            if ((grayMatrix[h, w] != 0) && (gray[w] != 0))
                            {
                                if (grayMatrix[h, w] < gray[w])
                                {
                                    grayMatrix[h, w] = gray[w];
                                }
                            }
                            else if (grayMatrix[h, w] == 0)
                            {
                                grayMatrix[h, w] = gray[w];
                            }
                        }
                    }
                }
                imageSource.Dispose();
            }
            return grayMatrix;
        }



        private int GetMinimumGrayValue(DataConfig dataConfig)
        {
            int temp = 0;
            if (dataConfig.ColorsConfig.Length > 0)
            {
                temp = dataConfig.ColorsConfig[0].Gray;
                for (int i = 0; i < dataConfig.ColorsConfig.Length; i++)
                {
                    var value = dataConfig.ColorsConfig[i];
                    if (temp > value.Gray)
                    {
                        temp = value.Gray;
                    }
                }
            }
            return temp;
        }

        private int GetMaximumGrayValue(DataConfig dataConfig)
        {
            int temp = 0;
            if (dataConfig.ColorsConfig.Length > 0)
            {
                temp = dataConfig.ColorsConfig[0].Gray;
                for (int i = 0; i < dataConfig.ColorsConfig.Length; i++)
                {
                    var value = dataConfig.ColorsConfig[i];
                    if (temp < value.Gray)
                    {
                        temp = value.Gray;
                    }
                }
            }
            return temp;
        }

        private ColorConfig GetColorValue(DataConfig dataConfig, int GrayValue)
        {
            ColorConfig tempColorConfig = null;
            if (dataConfig.ColorsConfig.Length > 0)
            {
                for (int i = 0; i < dataConfig.ColorsConfig.Length; i++)
                {
                    var value = dataConfig.ColorsConfig[i];
                    if (value.Gray == GrayValue)
                    {
                        tempColorConfig = value;
                        break;
                    }
                }
            }
            return tempColorConfig;
        }

        private int[] GetRangeColorValue(DataConfig dataConfig)
        {
            var rangeColorConfig = new List<int>();
            if (dataConfig.ColorsConfig.Length > 0)
            {
                rangeColorConfig = new List<int>();
                for (int i = 0; i < dataConfig.ColorsConfig.Length; i++)
                {
                    var value = dataConfig.ColorsConfig[i];
                    rangeColorConfig.Add(value.Gray);
                }
            }
            rangeColorConfig.Sort();
            return rangeColorConfig.ToArray();
        }

        private ColorConfig GetColorByRangeValue(DataConfig dataConfig, int GrayValue)
        {
            ColorConfig colorConfig = null;
            var rangeColorConfig = GetRangeColorValue(dataConfig);
            for (int i = 0; i < rangeColorConfig.Length; i++)
            {
                if ((i + 1) <= (rangeColorConfig.Length - 1))
                {
                    if ((GrayValue > rangeColorConfig[i]) && (GrayValue < rangeColorConfig[i + 1]))
                    {
                        var color1 = GetColorValue(dataConfig, rangeColorConfig[i]);
                        var color2 = GetColorValue(dataConfig, rangeColorConfig[i + 1]);
                        if ((color1 != null) && (color2 != null))
                        {
                            colorConfig = new ColorConfig();
                            colorConfig.Red = color1.Red;
                            colorConfig.Green = color1.Green;
                            colorConfig.Blue = color1.Blue;
                            colorConfig.Alpha = color1.Alpha;
                            //colorConfig.Red = (byte)((byte)(color1.Red + color2.Red) / 2);
                            //colorConfig.Green = (byte)((byte)(color1.Green + color2.Green) / 2);
                            //colorConfig.Blue = (byte)((byte)(color1.Blue + color2.Blue) / 2);
                            //colorConfig.Alpha = (byte)((byte)(color1.Alpha + color2.Alpha) / 2);
                            break;
                        }
                    }
                }
            }
            return colorConfig;
        }

        public void CheckOutTIFFFilesDirectorysForMobStation(DataConfig dataConfig)
        {
            for (int i = 0; i < dataConfig.BlockStationsConfig.MobStationConfig.Length; i++)
            {
                var codeOperator = dataConfig.BlockStationsConfig.MobStationConfig[i];
                if (codeOperator != null)
                {
                    if (codeOperator.StandardConfig != null)
                    {
                        for (int l = 0; l < codeOperator.StandardConfig.provincesConfig.Length; l++)
                        {
                            var provincesConfig = codeOperator.StandardConfig.provincesConfig[l];
                            if (!string.IsNullOrEmpty(provincesConfig.OutTIFFFilesDirectory))
                            {
                                if (!Directory.Exists(provincesConfig.OutTIFFFilesDirectory))
                                {
                                    Directory.CreateDirectory(provincesConfig.OutTIFFFilesDirectory);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CheckOutTIFFFilesDirectorysForMobStation2(DataConfig dataConfig)
        {
            for (int i = 0; i < dataConfig.BlockStationsConfig.MobStation2Config.Length; i++)
            {
                var codeOperator = dataConfig.BlockStationsConfig.MobStation2Config[i];
                if (codeOperator != null)
                {
                    if (codeOperator.FreqConfig != null)
                    {
                        for (int l = 0; l < codeOperator.FreqConfig.provincesConfig.Length; l++)
                        {
                            var provincesConfig = codeOperator.FreqConfig.provincesConfig[l];
                            if (!string.IsNullOrEmpty(provincesConfig.OutTIFFFilesDirectory))
                            {
                                if (!Directory.Exists(provincesConfig.OutTIFFFilesDirectory))
                                {
                                    Directory.CreateDirectory(provincesConfig.OutTIFFFilesDirectory);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ClearTempFiles(DataConfig dataConfig)
        {
            var masks = new string[] { "*.TIF", "*.TFW" };
            if ((masks != null) && (masks.Length > 0))
            {
                for (int i = 0; i < masks.Length; i++)
                {
                    var filesTempICSTelecomDirectory = Directory.GetFiles(Path.GetDirectoryName(dataConfig.DirectoryConfig.TempTIFFFilesDirectory), masks[i]);
                    for (int j = 0; j < filesTempICSTelecomDirectory.Length; j++)
                    {
                        if (File.Exists(filesTempICSTelecomDirectory[j]))
                        {
                            File.Delete(filesTempICSTelecomDirectory[j]);
                        }
                    }
                }
            }
            this._logger.Info(Contexts.CalcCoverages, Events.ClearFilesFromTempTIFFFilesDirectory);
        }


        public void ClearResultFilesICSTelecomProject(DataConfig dataConfig, string ICSTelecomProjectDir, string NameBlankFile)
        {
            var masks = dataConfig.DirectoryConfig.MaskFilesICSTelecom.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if ((masks != null) && (masks.Length > 0))
            {
                for (int i = 0; i < masks.Length; i++)
                {
                    var filesICSTelecomDirectory = Directory.GetFiles(ICSTelecomProjectDir, masks[i]);
                    for (int j = 0; j < filesICSTelecomDirectory.Length; j++)
                    {
                        if (Path.GetFileNameWithoutExtension(NameBlankFile.ToLower()) != Path.GetFileNameWithoutExtension(filesICSTelecomDirectory[j].ToLower()))
                        {
                            if (File.Exists(filesICSTelecomDirectory[j]))
                            {
                                File.Delete(filesICSTelecomDirectory[j]);
                            }
                        }
                    }
                }
            }
            this._logger.Info(Contexts.CalcCoverages, Events.ClearFilesFromICSTelecomProjectDir);
        }

        /*
        public void ClearOutTIFFFilesDirectoryForMobStation(DataConfig dataConfig)
        {
            for (int i = 0; i < dataConfig.BlockStationsConfig.MobStationConfig.Length; i++)
            {
                var codeOperator = dataConfig.BlockStationsConfig.MobStationConfig[i];
                for (int l = 0; l < codeOperator.StandardConfig.provincesConfig.Length; l++)
                {
                    var provincesConfig = codeOperator.StandardConfig.provincesConfig[l];
                    if (!string.IsNullOrEmpty(provincesConfig.OutTIFFFilesDirectory))
                    {
                        var filesOutTIFFFilesDirectory = Directory.GetFiles(provincesConfig.OutTIFFFilesDirectory);
                        for (int j = 0; j < filesOutTIFFFilesDirectory.Length; j++)
                        {
                            if (!File.Exists(filesOutTIFFFilesDirectory[j]))
                            {
                                File.Delete(filesOutTIFFFilesDirectory[j]);
                            }
                        }
                    }
                }
            }
            this._logger.Info(Contexts.CalcCoverages, Events.ClearFilesFromOutTIFFFilesDirectory);
        }
        */
    }
}



