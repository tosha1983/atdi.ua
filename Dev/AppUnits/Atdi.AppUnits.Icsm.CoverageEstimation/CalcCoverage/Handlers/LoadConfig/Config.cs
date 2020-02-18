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
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;


namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public class Config
    {
        private DirectoryConfig GetDirectoryConfig(XDocument xdoc)
        {
            DirectoryConfig directoryConfiguration = null;
            var settingDirectoryConfig = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("DirectoryConfig");
            var directoryConfig = settingDirectoryConfig.Elements();
            if (directoryConfig != null)
            {
                directoryConfiguration = new DirectoryConfig();
                foreach (var xel in directoryConfig)
                {

                    if ((xel.Name.ToString() == "TempTIFFFilesDirectory") || (xel.Name.LocalName == "TempTIFFFilesDirectory"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.TempTIFFFilesDirectory = xel.Value;
                        }
                    }


                    if ((xel.Name.ToString() == "MaskFilesICSTelecom") || (xel.Name.LocalName == "MaskFilesICSTelecom"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.MaskFilesICSTelecom = xel.Value;
                        }
                    }


                    if ((xel.Name.ToString() == "TemplateOutputFileNameForMobStation") || (xel.Name.LocalName == "TemplateOutputFileNameForMobStation"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.TemplateOutputFileNameForMobStation = xel.Value;
                        }
                    }

                    if ((xel.Name.ToString() == "TemplateOutputFileNameForMobStation2") || (xel.Name.LocalName == "TemplateOutputFileNameForMobStation2"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.TemplateOutputFileNameForMobStation2 = xel.Value;
                        }
                    }

                    if ((xel.Name.ToString() == "SpecifiedLogFile") || (xel.Name.LocalName == "SpecifiedLogFile"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.SpecifiedLogFile = xel.Value;
                        }
                    }


                    if ((xel.Name.ToString() == "BinICSTelecomDirectory") || (xel.Name.LocalName == "BinICSTelecomDirectory"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.BinICSTelecomDirectory = xel.Value;
                        }
                    }
                    if ((xel.Name.ToString() == "ICSTelecomProjectDirectory") || (xel.Name.LocalName == "ICSTelecomProjectDirectory"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.ICSTelecomProjectDirectory = xel.Value;
                        }
                    }

                    if ((xel.Name.ToString() == "TempEwxFilesDirectory") || (xel.Name.LocalName == "TempEwxFilesDirectory"))
                    {
                        if (!string.IsNullOrEmpty(xel.Value))
                        {
                            directoryConfiguration.TempEwxFilesDirectory = xel.Value;
                        }
                    }


                    

                    if ((xel.Name.ToString() == "RunCommandICSTelecom") || (xel.Name.LocalName == "RunCommandICSTelecom"))
                    {
                        var commands = xel.Elements();
                        if (commands != null)
                        {
                            directoryConfiguration.CommandsConfig = new CommandsConfig();
                            var command = commands.Elements();
                            if (command != null)
                            {
                                int idxcommand = 0;
                                directoryConfiguration.CommandsConfig.CommandsConfigs = new CommandConfig[command.Count()];
                                foreach (var xelCommand in command)
                                {

                                    directoryConfiguration.CommandsConfig.CommandsConfigs[idxcommand] = new CommandConfig();
                                    directoryConfiguration.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig = new ArgumentsConfig();

                                    var nameFile = xelCommand.Elements();
                                    if (nameFile != null)
                                    {
                                        foreach (var xelNameFile in nameFile)
                                        {
                                            if ((xelNameFile.Name.ToString() == "NameFile") || (xelNameFile.Name.LocalName == "NameFile"))
                                            {
                                                directoryConfiguration.CommandsConfig.CommandsConfigs[idxcommand].NameFile = xelNameFile.Value;
                                            }

                                            if ((xelNameFile.Name.ToString() == "Arguments") || (xelNameFile.Name.LocalName == "Arguments"))
                                            {
                                                var argumentElements = xelNameFile.Elements();
                                                if (argumentElements != null)
                                                {
                                                    int idxargumentElements = 0;
                                                    directoryConfiguration.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig.ArgumentsConfigs = new ArgumentConfig[argumentElements.Count()];
                                                    foreach (var xelArgumentElements in argumentElements)
                                                    {
                                                        if ((xelArgumentElements.Name.ToString() == "Argument") || (xelArgumentElements.Name.LocalName == "Argument"))
                                                        {

                                                            directoryConfiguration.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig.ArgumentsConfigs[idxargumentElements] = new ArgumentConfig();
                                                            directoryConfiguration.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig.ArgumentsConfigs[idxargumentElements].Value = xelArgumentElements.Value;
                                                        }
                                                        idxargumentElements++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    idxcommand++;
                                }
                            }
                        }
                    }
                }
            }
            return directoryConfiguration;
        }

        private ColorConfig[] GetColorConfig(XDocument xdoc)
        {
            ColorConfig[] colorConfigs = null;
            var settingGroupsColorConfig = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("GroupsColorConfig").Elements("ColorConfig");
            var colorConfig = settingGroupsColorConfig;
            if ((colorConfig != null) && (colorConfig.Count() > 0))
            {
                colorConfigs = new ColorConfig[colorConfig.Count()];
                int index = 0;
                foreach (var x in colorConfig)
                {
                    colorConfigs[index] = new ColorConfig();
                    foreach (var xel in x.Elements())
                    {
                        if ((xel.Name.ToString() == "GrayComponent") || (xel.Name.LocalName == "GrayComponent"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                colorConfigs[index].Gray = Convert.ToByte(xel.Value);
                            }
                        }
                        if ((xel.Name.ToString() == "RedComponent") || (xel.Name.LocalName == "RedComponent"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                colorConfigs[index].Red = Convert.ToByte(xel.Value);
                            }
                        }
                        if ((xel.Name.ToString() == "GreenComponent") || (xel.Name.LocalName == "GreenComponent"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                colorConfigs[index].Green = Convert.ToByte(xel.Value);
                            }
                        }
                        if ((xel.Name.ToString() == "BlueComponent") || (xel.Name.LocalName == "BlueComponent"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                colorConfigs[index].Blue = Convert.ToByte(xel.Value);
                            }
                        }
                        if ((xel.Name.ToString() == "AlphaComponent") || (xel.Name.LocalName == "AlphaComponent"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                colorConfigs[index].Alpha = Convert.ToByte(xel.Value);
                            }
                        }
                    }
                    index++;
                }
            }
            return colorConfigs;
        }


        private ProvinceCodeConfig[] GetGroupsProvinceCodeConfig(XDocument xdoc)
        {
            ProvinceCodeConfig[] provinceCodeConfigs = null;
            var settingProvinceCodeConfig = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("GroupsProvinceConfig").Elements("ProvinceCodeConfig");
            var provConfig = settingProvinceCodeConfig;
            if ((provConfig != null) && (provConfig.Count() > 0))
            {
                provinceCodeConfigs = new ProvinceCodeConfig[provConfig.Count()];
                int index = 0;
                foreach (var x in provConfig)
                {
                    provinceCodeConfigs[index] = new ProvinceCodeConfig();
                    foreach (var xel in x.Elements())
                    {
                        if ((xel.Name.ToString() == "NameProvince") || (xel.Name.LocalName == "NameProvince"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                provinceCodeConfigs[index].NameProvince = xel.Value;
                            }
                        }
                        if ((xel.Name.ToString() == "Code") || (xel.Name.LocalName == "Code"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                provinceCodeConfigs[index].Code = xel.Value;
                            }
                        }
                    }
                    index++;
                }
            }
            return provinceCodeConfigs;
        }

        private CodeOperatorAndStatusConfig[] GetConfigGroupsMobStation2(XDocument xdoc)
        {
            CodeOperatorAndStatusConfig[] mobStationConfig2 = null;
            var settingMobStation2Config = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("GroupsMobStation2Config").Elements("CodeOperatorAndStatusConfig");
            var stationsConfig = settingMobStation2Config;
            if ((stationsConfig != null) && (stationsConfig.Count() > 0))
            {
                mobStationConfig2 = new CodeOperatorAndStatusConfig[stationsConfig.Count()];
                int index = 0;
                foreach (var x in stationsConfig)
                {
                    mobStationConfig2[index] = new CodeOperatorAndStatusConfig();
                    foreach (var xel in x.Elements())
                    {
                        if ((xel.Name.ToString() == "Status") || (xel.Name.LocalName == "Status"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                mobStationConfig2[index].Status = xel.Value;
                            }
                        }
                        if ((xel.Name.ToString() == "Frequency") || (xel.Name.LocalName == "Frequency"))
                        {
                            mobStationConfig2[index].FreqConfig = new FreqConfig();
                            var name = xel.Element("Values");
                            if (name != null)
                            {
                                if (!string.IsNullOrEmpty(name.Value))
                                {
                                    mobStationConfig2[index].FreqConfig.Values = name.Value;
                                }
                            }


                            var provincesCodeOperator = xel.Element("Provinces");
                            if (provincesCodeOperator != null)
                            {
                                var provinceCodeOperator = provincesCodeOperator.Elements();
                                mobStationConfig2[index].FreqConfig.provincesConfig = new ProvinceConfig[provinceCodeOperator.Count()];

                                for (int j = 0; j < provinceCodeOperator.Count(); j++)
                                {
                                    mobStationConfig2[index].FreqConfig.provincesConfig[j] = new ProvinceConfig();
                                }

                                int indexProvince = 0;

                                foreach (var province in provinceCodeOperator)
                                {
                                    var prov = province.Element("Name");
                                    if (prov != null)
                                    {
                                        if (!string.IsNullOrEmpty(prov.Value))
                                        {
                                            mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].Name = prov.Value;
                                        }
                                    }

                                    var iCSTelecomProjectFile = province.Element("ICSTelecomProjectFile");
                                    if (iCSTelecomProjectFile != null)
                                    {
                                        if (!string.IsNullOrEmpty(iCSTelecomProjectFile.Value))
                                        {
                                            mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].ICSTelecomProjectFile = iCSTelecomProjectFile.Value;
                                        }
                                    }

                                    var outTIFFFilesDirectoryprovinceCodeOperator = province.Element("OutTIFFFilesDirectory");
                                    if (outTIFFFilesDirectoryprovinceCodeOperator != null)
                                    {
                                        if (!string.IsNullOrEmpty(outTIFFFilesDirectoryprovinceCodeOperator.Value))
                                        {
                                            mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].OutTIFFFilesDirectory = outTIFFFilesDirectoryprovinceCodeOperator.Value;
                                        }
                                    }

                                    var outBlankTIFFFile = province.Element("BlankTIFFFile");
                                    if (outBlankTIFFFile != null)
                                    {
                                        if (!string.IsNullOrEmpty(outBlankTIFFFile.Value))
                                        {
                                            mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].BlankTIFFFile = outBlankTIFFFile.Value;
                                        }
                                    }

                                    var outNameEwxFile = province.Element("NameEwxFile");
                                    if (outNameEwxFile != null)
                                    {
                                        if (!string.IsNullOrEmpty(outNameEwxFile.Value))
                                        {
                                            mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].NameEwxFile = outNameEwxFile.Value;
                                        }
                                    }


                                    var codeOperators = province.Element("CodeOperators");
                                    if (codeOperators != null)
                                    {
                                        var codeOperatorsValue = codeOperators.Elements();

                                        int indexcodeOperator = 0;
                                        mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].CodeOperatorConfig = new CodeOperatorConfig[codeOperatorsValue.Count()];
                                        for (int j = 0; j < codeOperatorsValue.Count(); j++)
                                        {
                                            mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].CodeOperatorConfig[j] = new CodeOperatorConfig();
                                        }

                                        foreach (var xelcodeOperator in codeOperatorsValue)
                                        {
                                            var codeCodeOperator = xelcodeOperator.Element("Code");
                                            if (codeCodeOperator != null)
                                            {
                                                if (!string.IsNullOrEmpty(codeCodeOperator.Value))
                                                {
                                                    mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Code = codeCodeOperator.Value;
                                                }
                                            }

                                            var nameCodeOperator = xelcodeOperator.Element("Name");
                                            if (nameCodeOperator != null)
                                            {
                                                if (!string.IsNullOrEmpty(nameCodeOperator.Value))
                                                {
                                                    mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Name = nameCodeOperator.Value;
                                                }
                                            }

                                            var descriptionCodeOperator = xelcodeOperator.Element("Description");
                                            if (descriptionCodeOperator != null)
                                            {
                                                if (!string.IsNullOrEmpty(descriptionCodeOperator.Value))
                                                {
                                                    mobStationConfig2[index].FreqConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Description = descriptionCodeOperator.Value;
                                                }
                                            }

                                            indexcodeOperator++;
                                        }
                                    }
                                    indexProvince++;
                                }
                            }
                        }
                    }
                    index++;
                }
            }
            return mobStationConfig2;
        }

        private CodeOperatorAndStatusConfig[] GetConfigGroupsMobStation(XDocument xdoc)
        {
            CodeOperatorAndStatusConfig[] mobStationConfig = null;
            var settingMobStationConfig = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("GroupsMobStationConfig").Elements("CodeOperatorAndStatusConfig");
            var stationsConfig = settingMobStationConfig;
            if ((stationsConfig != null) && (stationsConfig.Count() > 0))
            {
                mobStationConfig = new CodeOperatorAndStatusConfig[stationsConfig.Count()];
                int index = 0;
                foreach (var x in stationsConfig)
                {
                    mobStationConfig[index] = new CodeOperatorAndStatusConfig();
                    foreach (var xel in x.Elements())
                    {
                        if ((xel.Name.ToString() == "Status") || (xel.Name.LocalName == "Status"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                mobStationConfig[index].Status = xel.Value;
                            }
                        }
                        if ((xel.Name.ToString() == "Standard") || (xel.Name.LocalName == "Standard"))
                        {
                            mobStationConfig[index].StandardConfig = new StandardConfig();
                            var name = xel.Element("Name");
                            if (name != null)
                            {
                                if (!string.IsNullOrEmpty(name.Value))
                                {
                                    mobStationConfig[index].StandardConfig.Name = name.Value;
                                }
                            }
                            var description = xel.Element("Description");
                            if (description != null)
                            {
                                if (!string.IsNullOrEmpty(description.Value))
                                {
                                    mobStationConfig[index].StandardConfig.Description = description.Value;
                                }
                            }


                            var provincesCodeOperator = xel.Element("Provinces");
                            if (provincesCodeOperator != null)
                            {
                                var provinceCodeOperator = provincesCodeOperator.Elements();
                                mobStationConfig[index].StandardConfig.provincesConfig = new ProvinceConfig[provinceCodeOperator.Count()];

                                for (int j = 0; j < provinceCodeOperator.Count(); j++)
                                {
                                    mobStationConfig[index].StandardConfig.provincesConfig[j] = new ProvinceConfig();
                                }

                                int indexProvince = 0;

                                foreach (var province in provinceCodeOperator)
                                {
                                    var prov = province.Element("Name");
                                    if (prov != null)
                                    {
                                        if (!string.IsNullOrEmpty(prov.Value))
                                        {
                                            mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].Name = prov.Value;
                                        }
                                    }

                                    var iCSTelecomProjectFile = province.Element("ICSTelecomProjectFile");
                                    if (iCSTelecomProjectFile != null)
                                    {
                                        if (!string.IsNullOrEmpty(iCSTelecomProjectFile.Value))
                                        {
                                            mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].ICSTelecomProjectFile = iCSTelecomProjectFile.Value;
                                        }
                                    }

                                    var outTIFFFilesDirectoryprovinceCodeOperator = province.Element("OutTIFFFilesDirectory");
                                    if (outTIFFFilesDirectoryprovinceCodeOperator != null)
                                    {
                                        if (!string.IsNullOrEmpty(outTIFFFilesDirectoryprovinceCodeOperator.Value))
                                        {
                                            mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].OutTIFFFilesDirectory = outTIFFFilesDirectoryprovinceCodeOperator.Value;
                                        }
                                    }

                                    var outBlankTIFFFile = province.Element("BlankTIFFFile");
                                    if (outBlankTIFFFile != null)
                                    {
                                        if (!string.IsNullOrEmpty(outBlankTIFFFile.Value))
                                        {
                                            mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].BlankTIFFFile = outBlankTIFFFile.Value;
                                        }
                                    }

                                    var outNameEwxFile = province.Element("NameEwxFile");
                                    if (outNameEwxFile != null)
                                    {
                                        if (!string.IsNullOrEmpty(outNameEwxFile.Value))
                                        {
                                            mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].NameEwxFile = outNameEwxFile.Value;
                                        }
                                    }


                                    var codeOperators = province.Element("CodeOperators");
                                    if (codeOperators != null)
                                    {
                                        var codeOperatorsValue = codeOperators.Elements();

                                        int indexcodeOperator = 0;
                                        mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig = new CodeOperatorConfig[codeOperatorsValue.Count()];
                                        for (int j = 0; j < codeOperatorsValue.Count(); j++)
                                        {
                                            mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[j] = new CodeOperatorConfig();
                                        }

                                        foreach (var xelcodeOperator in codeOperatorsValue)
                                        {
                                            var codeCodeOperator = xelcodeOperator.Element("Code");
                                            if (codeCodeOperator != null)
                                            {
                                                if (!string.IsNullOrEmpty(codeCodeOperator.Value))
                                                {
                                                    mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Code = codeCodeOperator.Value;
                                                }
                                            }

                                            var nameCodeOperator = xelcodeOperator.Element("Name");
                                            if (nameCodeOperator != null)
                                            {
                                                if (!string.IsNullOrEmpty(nameCodeOperator.Value))
                                                {
                                                    mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Name = nameCodeOperator.Value;
                                                }
                                            }

                                            var descriptionCodeOperator = xelcodeOperator.Element("Description");
                                            if (descriptionCodeOperator != null)
                                            {
                                                if (!string.IsNullOrEmpty(descriptionCodeOperator.Value))
                                                {
                                                    mobStationConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Description = descriptionCodeOperator.Value;
                                                }
                                            }

                                            indexcodeOperator++;
                                        }
                                    }
                                    indexProvince++;
                                }
                            }
                        }
                    }
                    index++;
                }
            }
            return mobStationConfig;
        }
        public DataConfig Load(string coverageConfigFileName)
        {
            var config = new DataConfig();
            var xdoc = new XDocument();
            using (var reader = new StreamReader(coverageConfigFileName, Encoding.UTF8))
            {
                xdoc = XDocument.Parse(reader.ReadToEnd());
                config.BlockStationsConfig = new BlockStationsConfig();
                config.BlockStationsConfig.MobStation2Config = GetConfigGroupsMobStation2(xdoc);
                config.BlockStationsConfig.MobStationConfig = GetConfigGroupsMobStation(xdoc);
                config.ColorsConfig = GetColorConfig(xdoc);
                config.ProvinceCodeConfig = GetGroupsProvinceCodeConfig(xdoc);
                config.DirectoryConfig = GetDirectoryConfig(xdoc);
            }
            return config;
        }
    }
}
