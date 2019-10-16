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
        public DataConfig Load(string coverageConfigFileName)
        {
            var config = new DataConfig();

            var xdoc = new XDocument();
            using (var reader = new StreamReader(coverageConfigFileName, Encoding.UTF8))
            {
                xdoc = XDocument.Parse(reader.ReadToEnd());
                var settingStationConfig = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("GroupsStationConfig").Elements("CodeOperatorAndStatusConfig");
                var stationsConfig = settingStationConfig;
                if ((stationsConfig != null) && (stationsConfig.Count() > 0))
                {
                    config.CodeOperatorAndStatusesConfig = new CodeOperatorAndStatusConfig[stationsConfig.Count()];
                    int index = 0;
                    foreach (var x in stationsConfig)
                    {
                        config.CodeOperatorAndStatusesConfig[index] = new CodeOperatorAndStatusConfig();
                        foreach (var xel in x.Elements())
                        {
                            if ((xel.Name.ToString() == "Status") || (xel.Name.LocalName == "Status"))
                            {
                                if (!string.IsNullOrEmpty(xel.Value))
                                {
                                    config.CodeOperatorAndStatusesConfig[index].Status = xel.Value;
                                }
                            }
                            if ((xel.Name.ToString() == "Standard") || (xel.Name.LocalName == "Standard"))
                            {
                                config.CodeOperatorAndStatusesConfig[index].StandardConfig = new StandardConfig();
                                var name = xel.Element("Name");
                                if (name != null)
                                {
                                    if (!string.IsNullOrEmpty(name.Value))
                                    {
                                        config.CodeOperatorAndStatusesConfig[index].StandardConfig.Name = name.Value;
                                    }
                                }
                                var description = xel.Element("Description");
                                if (description != null)
                                {
                                    if (!string.IsNullOrEmpty(description.Value))
                                    {
                                        config.CodeOperatorAndStatusesConfig[index].StandardConfig.Description = description.Value;
                                    }
                                }

                                var outTIFFFilesDirectory = xel.Element("OutTIFFFilesDirectory");
                                if (outTIFFFilesDirectory != null)
                                {
                                    if (!string.IsNullOrEmpty(outTIFFFilesDirectory.Value))
                                    {
                                        config.CodeOperatorAndStatusesConfig[index].StandardConfig.OutTIFFFilesDirectory = outTIFFFilesDirectory.Value;
                                    }
                                }



                                var provincesCodeOperator = xel.Element("Provinces");
                                if (provincesCodeOperator != null)
                                {
                                    var provinceCodeOperator = provincesCodeOperator.Elements();
                                    config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig = new ProvinceConfig[provinceCodeOperator.Count()];

                                    for (int j = 0; j < provinceCodeOperator.Count(); j++)
                                    {
                                        config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[j] = new ProvinceConfig();
                                    }

                                    int indexProvince = 0;

                                    foreach (var province in provinceCodeOperator)
                                    {
                                        var prov = province.Element("Name");
                                        if (prov != null)
                                        {
                                            if (!string.IsNullOrEmpty(prov.Value))
                                            {
                                                config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].Name = prov.Value;
                                            }
                                        }

                                        var iCSTelecomProjectFile = province.Element("ICSTelecomProjectFile");
                                        if (iCSTelecomProjectFile != null)
                                        {
                                            if (!string.IsNullOrEmpty(iCSTelecomProjectFile.Value))
                                            {
                                                config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].ICSTelecomProjectFile = iCSTelecomProjectFile.Value;
                                            }
                                        }

                                        var outTIFFFilesDirectoryprovinceCodeOperator = province.Element("OutTIFFFilesDirectory");
                                        if (outTIFFFilesDirectoryprovinceCodeOperator != null)
                                        {
                                            if (!string.IsNullOrEmpty(outTIFFFilesDirectoryprovinceCodeOperator.Value))
                                            {
                                                config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].OutTIFFFilesDirectory = outTIFFFilesDirectoryprovinceCodeOperator.Value;
                                            }
                                        }

                                        var outBlankTIFFFile = province.Element("BlankTIFFFile");
                                        if (outBlankTIFFFile != null)
                                        {
                                            if (!string.IsNullOrEmpty(outBlankTIFFFile.Value))
                                            {
                                                config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].BlankTIFFFile = outBlankTIFFFile.Value;
                                            }
                                        }

                                        var outNameEwxFile = province.Element("NameEwxFile");
                                        if (outNameEwxFile != null)
                                        {
                                            if (!string.IsNullOrEmpty(outNameEwxFile.Value))
                                            {
                                                config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].NameEwxFile = outNameEwxFile.Value;
                                            }
                                        }


                                        



                                        var codeOperators = province.Element("CodeOperators");
                                        if (codeOperators != null)
                                        {
                                            var codeOperatorsValue = codeOperators.Elements();

                                            int indexcodeOperator = 0;
                                            config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig = new CodeOperatorConfig[codeOperatorsValue.Count()];
                                            for (int j = 0; j < codeOperatorsValue.Count(); j++)
                                            {
                                                config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[j] = new CodeOperatorConfig();
                                            }

                                            foreach (var xelcodeOperator in codeOperatorsValue)
                                            {
                                                var codeCodeOperator = xelcodeOperator.Element("Code");
                                                if (codeCodeOperator != null)
                                                {
                                                    if (!string.IsNullOrEmpty(codeCodeOperator.Value))
                                                    {
                                                        config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Code = codeCodeOperator.Value;
                                                    }
                                                }

                                                var nameCodeOperator = xelcodeOperator.Element("Name");
                                                if (nameCodeOperator != null)
                                                {
                                                    if (!string.IsNullOrEmpty(nameCodeOperator.Value))
                                                    {
                                                        config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Name = nameCodeOperator.Value;
                                                    }
                                                }

                                                var descriptionCodeOperator = xelcodeOperator.Element("Description");
                                                if (descriptionCodeOperator != null)
                                                {
                                                    if (!string.IsNullOrEmpty(descriptionCodeOperator.Value))
                                                    {
                                                        config.CodeOperatorAndStatusesConfig[index].StandardConfig.provincesConfig[indexProvince].CodeOperatorConfig[indexcodeOperator].Description = descriptionCodeOperator.Value;
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

                    //index++;
                }

                var settingGroupsColorConfig = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("GroupsColorConfig").Elements("ColorConfig");
                var colorConfig = settingGroupsColorConfig;
                if ((colorConfig != null) && (colorConfig.Count() > 0))
                {
                    config.ColorsConfig = new ColorConfig[colorConfig.Count()];
                    int index = 0;
                    foreach (var x in colorConfig)
                    {
                        config.ColorsConfig[index] = new ColorConfig();
                        foreach (var xel in x.Elements())
                        {
                            if ((xel.Name.ToString() == "GrayComponent") || (xel.Name.LocalName == "GrayComponent"))
                            {
                                if (!string.IsNullOrEmpty(xel.Value))
                                {
                                    config.ColorsConfig[index].Gray = Convert.ToByte(xel.Value);
                                }
                            }
                            if ((xel.Name.ToString() == "RedComponent") || (xel.Name.LocalName == "RedComponent"))
                            {
                                if (!string.IsNullOrEmpty(xel.Value))
                                {
                                    config.ColorsConfig[index].Red = Convert.ToByte(xel.Value);
                                }
                            }
                            if ((xel.Name.ToString() == "GreenComponent") || (xel.Name.LocalName == "GreenComponent"))
                            {
                                if (!string.IsNullOrEmpty(xel.Value))
                                {
                                    config.ColorsConfig[index].Green = Convert.ToByte(xel.Value);
                                }
                            }
                            if ((xel.Name.ToString() == "BlueComponent") || (xel.Name.LocalName == "BlueComponent"))
                            {
                                if (!string.IsNullOrEmpty(xel.Value))
                                {
                                    config.ColorsConfig[index].Blue = Convert.ToByte(xel.Value);
                                }
                            }
                            if ((xel.Name.ToString() == "AlphaComponent") || (xel.Name.LocalName == "AlphaComponent"))
                            {
                                if (!string.IsNullOrEmpty(xel.Value))
                                {
                                    config.ColorsConfig[index].Alpha = Convert.ToByte(xel.Value);
                                }
                            }
                        }
                        index++;
                    }
                }

                var settingDirectoryConfig = xdoc.Element("SettingCoverageCalculation").Element("Params").Element("ObjectDataConfig").Element("DirectoryConfig");
                var directoryConfig = settingDirectoryConfig.Elements();
                if (directoryConfig != null)
                {
                    config.DirectoryConfig = new DirectoryConfig();
                    foreach (var xel in directoryConfig)
                    {

                        if ((xel.Name.ToString() == "TempTIFFFilesDirectory") || (xel.Name.LocalName == "TempTIFFFilesDirectory"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                config.DirectoryConfig.TempTIFFFilesDirectory = xel.Value;
                            }
                        }


                        if ((xel.Name.ToString() == "MaskFilesICSTelecom") || (xel.Name.LocalName == "MaskFilesICSTelecom"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                config.DirectoryConfig.MaskFilesICSTelecom = xel.Value;
                            }
                        }


                        if ((xel.Name.ToString() == "BinICSTelecomDirectory") || (xel.Name.LocalName == "BinICSTelecomDirectory"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                config.DirectoryConfig.BinICSTelecomDirectory = xel.Value;
                            }
                        }
                        if ((xel.Name.ToString() == "ICSTelecomProjectDirectory") || (xel.Name.LocalName == "ICSTelecomProjectDirectory"))
                        {
                            if (!string.IsNullOrEmpty(xel.Value))
                            {
                                config.DirectoryConfig.ICSTelecomProjectDirectory = xel.Value;
                            }
                        }



                        if ((xel.Name.ToString() == "RunCommandICSTelecom") || (xel.Name.LocalName == "RunCommandICSTelecom"))
                        {
                            var commands = xel.Elements();
                            if (commands != null)
                            {
                                config.DirectoryConfig.CommandsConfig = new CommandsConfig();
                                var command = commands.Elements();
                                if (command != null)
                                {
                                    int idxcommand = 0;
                                    config.DirectoryConfig.CommandsConfig.CommandsConfigs = new CommandConfig[command.Count()];
                                    foreach (var xelCommand in command)
                                    {

                                        config.DirectoryConfig.CommandsConfig.CommandsConfigs[idxcommand] = new CommandConfig();
                                        config.DirectoryConfig.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig = new ArgumentsConfig();

                                        var nameFile = xelCommand.Elements();
                                        if (nameFile != null)
                                        {
                                            foreach (var xelNameFile in nameFile)
                                            {
                                                if ((xelNameFile.Name.ToString() == "NameFile") || (xelNameFile.Name.LocalName == "NameFile"))
                                                {
                                                    config.DirectoryConfig.CommandsConfig.CommandsConfigs[idxcommand].NameFile = xelNameFile.Value;
                                                }

                                                if ((xelNameFile.Name.ToString() == "Arguments") || (xelNameFile.Name.LocalName == "Arguments"))
                                                {
                                                    var argumentElements = xelNameFile.Elements();
                                                    if (argumentElements != null)
                                                    {
                                                        int idxargumentElements = 0;
                                                        config.DirectoryConfig.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig.ArgumentsConfigs = new ArgumentConfig[argumentElements.Count()];
                                                        foreach (var xelArgumentElements in argumentElements)
                                                        {
                                                            if ((xelArgumentElements.Name.ToString() == "Argument") || (xelArgumentElements.Name.LocalName == "Argument"))
                                                            {

                                                                config.DirectoryConfig.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig.ArgumentsConfigs[idxargumentElements] = new ArgumentConfig();
                                                                config.DirectoryConfig.CommandsConfig.CommandsConfigs[idxcommand].ArgumentsConfig.ArgumentsConfigs[idxargumentElements].Value = xelArgumentElements.Value;
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
            }
            return config;
        }
    }
}
