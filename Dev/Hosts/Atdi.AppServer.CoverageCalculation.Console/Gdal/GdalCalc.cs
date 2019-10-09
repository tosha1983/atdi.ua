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
using Atdi.AppServer.CoverageCalculation;
using Atdi.DataModels.CoverageCalculation;
using Atdi.Platform.Logging;
using Atdi.WebQuery.CoverageCalculation;


namespace Atdi.AppServer.CoverageCalculation
{

    public class GdalCalc
    {
        private ILogger _logger { get; set; }
        public GdalCalc(ILogger logger)
        {
            this._logger = logger;
        }


        public TFWParameter GetTFWParameter(string TiffFile)
        {
            var tFWParameter = new TFWParameter();
            var fileTFWBlank = System.IO.Path.GetDirectoryName(TiffFile) + @"\" + System.IO.Path.GetFileNameWithoutExtension(TiffFile) + ".TFW";
            var words = System.IO.File.ReadAllText(fileTFWBlank).Split(new char[] { '\t', '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

        public Coordinate GetCoordinateAfterRecalc(int MatrixY, int MatrixX, int MatrixHeight, int MatrixWidth, TFWParameter tFWParameterBlank, TFWParameter tFWParameterStation)
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

        public bool SaveRecalcTIFFFile(DataConfig dataConfig, string ICSTelecomProjectDir, string NameBlankFile)
        {
            bool isSuccessCreateFiles = false;
            GdalConfiguration.ConfigureGdal();
            Gdal.AllRegister();
            try
            {
                var tFWBlank = GetTFWParameter(dataConfig.DirectoryConfig.BlankTIFFFile);

                using (var image = Gdal.Open(dataConfig.DirectoryConfig.BlankTIFFFile, Access.GA_ReadOnly))
                {
                    var redBand = image.GetRasterBand(1);
                    var greenBand = image.GetRasterBand(2);
                    var blueBand = image.GetRasterBand(3);
                    var alphaBand = image.GetRasterBand(4);

                    int width = redBand.XSize;
                    int height = redBand.YSize;


                    var filesSource = Directory.GetFiles(ICSTelecomProjectDir, "*.TIF");
                    for (int i = 0; i < filesSource.Length; i++)
                    {
                        if (System.IO.Path.GetFileNameWithoutExtension(filesSource[i].ToLower()) == System.IO.Path.GetFileNameWithoutExtension(NameBlankFile.ToLower()))
                        {
                            continue;
                        }
                        var tFWSourceTIFFile = GetTFWParameter(filesSource[i]);
                        var tempCoverageFile = dataConfig.DirectoryConfig.TempTIFFFilesDirectory + @"\" + System.IO.Path.GetFileNameWithoutExtension(filesSource[i]) + "_out.TIF";
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
                            outImage.Dispose();
                            isSuccessCreateFiles = true;
                            this._logger.Info(Contexts.CalcCoverages, string.Format(Events.OperationSaveTempCovarageFileCompleted.ToString(), tempCoverageFile));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
            return isSuccessCreateFiles;
        }

        public void ClearBlank(DataConfig dataConfig)
        {
            GdalConfiguration.ConfigureGdal();
            Gdal.AllRegister();
            string clearName = "";
            try
            {
                using (var image = Gdal.Open(dataConfig.DirectoryConfig.BlankTIFFFile, Access.GA_ReadOnly))
                {
                    var redBand = image.GetRasterBand(1);
                    //var greenBand = image.GetRasterBand(2);
                    //var blueBand = image.GetRasterBand(3);
                    //var alphaBand = image.GetRasterBand(4);

                    int width = redBand.XSize;
                    int height = redBand.YSize;
                    clearName = System.IO.Path.GetDirectoryName(dataConfig.DirectoryConfig.BlankTIFFFile) + @"\" + System.IO.Path.GetFileNameWithoutExtension(dataConfig.DirectoryConfig.BlankTIFFFile) + @"._clear.TIF";

                    using (var outImage = Gdal.GetDriverByName("GTiff").Create(clearName, width, height, 4, DataType.GDT_Byte, new string[] { "COMPRESS=PACKBITS" }))
                    {
                        var outRBand = outImage.GetRasterBand(1);
                        var outGBand = outImage.GetRasterBand(2);
                        var outBBand = outImage.GetRasterBand(3);
                        var outABand = outImage.GetRasterBand(4);

                        double[] geoTransformerData = new double[6];
                        image.GetGeoTransform(geoTransformerData);
                        var proj = image.GetProjection();
                        outImage.SetProjection(proj);
                        outImage.SetGeoTransform(geoTransformerData);

                        for (int h = 0; h < height; h++)
                        {
                            int[] red = new int[width];
                            //int[] green = new int[width];
                            //int[] blue = new int[width];
                            //int[] alpha = new int[width];

                            for (int w = 0; w < width; w++)
                            {
                                red[w] = 0;
                                //green[w] = 0;
                                //blue[w] = 0;
                                //alpha[w] = 0;
                            }
                            outRBand.WriteRaster(0, h, width, 1, red, width, 1, 0, 0);
                            //outGBand.WriteRaster(0, h, width, 1, green, width, 1, 0, 0);
                            //outBBand.WriteRaster(0, h, width, 1, blue, width, 1, 0, 0);
                            //outABand.WriteRaster(0, h, width, 1, alpha, width, 1, 0, 0);
                        }
                        outImage.FlushCache();
                        outImage.Dispose();
                    }
                }
                if (System.IO.File.Exists(clearName))
                {
                    if (System.IO.File.Exists(dataConfig.DirectoryConfig.BlankTIFFFile))
                    {
                        System.IO.File.Delete(dataConfig.DirectoryConfig.BlankTIFFFile);
                        System.IO.File.Copy(clearName, dataConfig.DirectoryConfig.BlankTIFFFile);
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.CalcCoverages, e);
            }
        }

        public bool Run(DataConfig dataConfig, string OutDirectory, string FileName)
        {
            var isSuccessCreateFile = false;
            GdalConfiguration.ConfigureGdal();
            Gdal.AllRegister();
            try
            {
                using (var image = Gdal.Open(dataConfig.DirectoryConfig.BlankTIFFFile, Access.GA_ReadOnly))
                {
                    var redBand = image.GetRasterBand(1);
                    var greenBand = image.GetRasterBand(2);
                    var blueBand = image.GetRasterBand(3);
                    var alphaBand = image.GetRasterBand(4);

                    int width = redBand.XSize;
                    int height = redBand.YSize;

                    var outPutFileName = OutDirectory + @"\" + FileName;
                    using (var outImage = Gdal.GetDriverByName("GTiff").Create(outPutFileName, width, height, 4, DataType.GDT_Byte, new string[] { "COMPRESS=PACKBITS" }))
                    {
                        var outRBand = outImage.GetRasterBand(1);
                        var outGBand = outImage.GetRasterBand(2);
                        var outBBand = outImage.GetRasterBand(3);
                        var outABand = outImage.GetRasterBand(4);

                        double[] geoTransformerData = new double[6];
                        image.GetGeoTransform(geoTransformerData);
                        var proj = image.GetProjection();
                        outImage.SetProjection(proj);
                        outImage.SetGeoTransform(geoTransformerData);
                        var grayMatrix = GetMaxRasterFromGeoTiffFiles(dataConfig, width, height);

                        for (int h = 0; h < height; h++)
                        {
                            int[] red = new int[width];
                            int[] green = new int[width];
                            int[] blue = new int[width];
                            int[] alpha = new int[width];

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
                        outImage.Dispose();
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

        public int[,] GetMaxRasterFromGeoTiffFiles(DataConfig dataConfig, int widthValue, int heightValue)
        {
            var grayMatrix = new int[heightValue, widthValue];
            var filesSource = Directory.GetFiles(System.IO.Path.GetDirectoryName(dataConfig.DirectoryConfig.TempTIFFFilesDirectory), "*_out.TIF");
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
                imageSource.Dispose();
            }
            return grayMatrix;
        }



        public int GetMinimumGrayValue(DataConfig dataConfig)
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

        public int GetMaximumGrayValue(DataConfig dataConfig)
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

        public ColorConfig GetColorValue(DataConfig dataConfig, int GrayValue)
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

        public List<int> GetRangeColorValue(DataConfig dataConfig)
        {
            List<int> rangeColorConfig = new List<int>();
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
            return rangeColorConfig;
        }

        public ColorConfig GetColorByRangeValue(DataConfig dataConfig, int GrayValue)
        {
            ColorConfig colorConfig = null;
            var rangeColorConfig = GetRangeColorValue(dataConfig);
            for (int i = 0; i < rangeColorConfig.Count; i++)
            {
                if ((i + 1) <= (rangeColorConfig.Count - 1))
                {
                    if ((GrayValue > rangeColorConfig[i]) && (GrayValue < rangeColorConfig[i + 1]))
                    {
                        var color1 = GetColorValue(dataConfig, rangeColorConfig[i]);
                        var color2 = GetColorValue(dataConfig, rangeColorConfig[i + 1]);
                        if ((color1 != null) && (color2 != null))
                        {
                            colorConfig = new ColorConfig();
                            colorConfig.Red = (byte)((byte)(color1.Red + color2.Red) / 2);
                            colorConfig.Green = (byte)((byte)(color1.Green + color2.Green) / 2);
                            colorConfig.Blue = (byte)((byte)(color1.Blue + color2.Blue) / 2);
                            colorConfig.Alpha = (byte)((byte)(color1.Alpha + color2.Alpha) / 2);
                            break;
                        }
                    }
                }
            }
            return colorConfig;
        }

        public void CheckOutTIFFFilesDirectorys(DataConfig dataConfig)
        {
            for (int i = 0; i < dataConfig.CodeOperatorAndStatusesConfig.Length; i++)
            {
                var codeOperator = dataConfig.CodeOperatorAndStatusesConfig[i];
                for (int l = 0; l < codeOperator.StandardConfig.provincesConfig.Length; l++)
                {
                    var provincesConfig = codeOperator.StandardConfig.provincesConfig[l];
                    if (!string.IsNullOrEmpty(provincesConfig.OutTIFFFilesDirectory))
                    {
                        if (!System.IO.Directory.Exists(provincesConfig.OutTIFFFilesDirectory))
                        {
                            System.IO.Directory.CreateDirectory(provincesConfig.OutTIFFFilesDirectory);
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
                    var filesTempICSTelecomDirectory = Directory.GetFiles(System.IO.Path.GetDirectoryName(dataConfig.DirectoryConfig.TempTIFFFilesDirectory), masks[i]);
                    for (int j = 0; j < filesTempICSTelecomDirectory.Length; j++)
                    {
                        if (System.IO.File.Exists(filesTempICSTelecomDirectory[j]))
                        {
                            System.IO.File.Delete(filesTempICSTelecomDirectory[j]);
                        }
                    }
                }
            }
            this._logger.Info(Contexts.CalcCoverages, Events.ClearFilesFromTempTIFFFilesDirectory);
        }


        public void ClearResultFilesICSTelecomProject(DataConfig dataConfig, string ICSTelecomProjectDir)
        {
            var masks = dataConfig.DirectoryConfig.MaskFilesICSTelecom.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if ((masks != null) && (masks.Length > 0))
            {
                for (int i = 0; i < masks.Length; i++)
                {
                    var filesICSTelecomDirectory = Directory.GetFiles(ICSTelecomProjectDir, masks[i]);
                    for (int j = 0; j < filesICSTelecomDirectory.Length; j++)
                    {
                        if (System.IO.Path.GetFileNameWithoutExtension(dataConfig.DirectoryConfig.BlankTIFFFile.ToLower()) != System.IO.Path.GetFileNameWithoutExtension(filesICSTelecomDirectory[j].ToLower()))
                        {
                            if (System.IO.File.Exists(filesICSTelecomDirectory[j]))
                            {
                                System.IO.File.Delete(filesICSTelecomDirectory[j]);
                            }
                        }
                    }
                }
            }
            this._logger.Info(Contexts.CalcCoverages, Events.ClearFilesFromICSTelecomProjectDir);
        }

        public void ClearOutTIFFFilesDirectory(DataConfig dataConfig)
        {
            for (int i = 0; i < dataConfig.CodeOperatorAndStatusesConfig.Length; i++)
            {
                var codeOperator = dataConfig.CodeOperatorAndStatusesConfig[i];
                for (int l = 0; l < codeOperator.StandardConfig.provincesConfig.Length; l++)
                {
                    var provincesConfig = codeOperator.StandardConfig.provincesConfig[l];
                    if (!string.IsNullOrEmpty(provincesConfig.OutTIFFFilesDirectory))
                    {
                        var filesOutTIFFFilesDirectory = Directory.GetFiles(provincesConfig.OutTIFFFilesDirectory);
                        for (int j = 0; j < filesOutTIFFFilesDirectory.Length; j++)
                        {
                            if (!System.IO.File.Exists(filesOutTIFFFilesDirectory[j]))
                            {
                                System.IO.File.Delete(filesOutTIFFFilesDirectory[j]);
                            }
                        }
                    }
                }
            }
            this._logger.Info(Contexts.CalcCoverages, Events.ClearFilesFromOutTIFFFilesDirectory);
        }
    }
}



