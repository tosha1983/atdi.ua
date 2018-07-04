using Atdi.Contracts.CoreServices.DataLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class SchemasMetadata
    {
        public IDataEngine _configDataEngine;

        private readonly SchemasMetadataConfig _config;
        private readonly Dictionary<string, Module> _modules;
        private readonly Dictionary<string, Semant> _dicoSemants;
        private readonly List<Semant> _specStrings;
        private Semant[] _specInteger;
        private readonly Dictionary<string, Table> _tables;
        private readonly List<Table> _tablesList;
        private readonly Dictionary<string, DataDesc> _dataDescs;



        private bool _hasSemant = false;
        private bool _singlePosEqpAnt = false;

        public string DbSchema => this._config.SchemaPrefix;

        public SchemasMetadata(SchemasMetadataConfig config)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));

            this._singlePosEqpAnt = (config.Edition == "Developer1" || config.Edition == "Standard1");
            this._hasSemant = false;
            
            this._tables = new Dictionary<string, Table>();
            this._tablesList = new List<Table>();
            this._dataDescs = new Dictionary<string, DataDesc>();
            this._dicoSemants = new Dictionary<string, Semant>();
            this._specStrings = new List<Semant>();
            this._modules = new Dictionary<string, Module>();

            if (config.Modules != null && config.Modules.Length > 0)
            {
                for (int i = 0; i < config.Modules.Length; i++)
                {
                    var moduleName = config.Modules[i];
                    this._modules.Add(moduleName, new Module(moduleName));
                }
            }
            
            for (int i = 0; i < config.Schemas.Length; i++)
            {
                var schemaName = config.Schemas[i];
                var fileName = Path.Combine(config.SchemasPath, schemaName + ".Schema");
                LoadSchema(fileName, schemaName);
            }
        }

        private void LoadSchema(string fileName, string schemaName)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    throw new InvalidOperationException($"The file '{fileName}' does not exist");
                }

                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(fileStream))
                    {
                        this.ReadSchema(fileStream, reader, schemaName);
                    }
                }
            }
            catch(Exception e)
            {
                throw new InvalidOperationException($"The schema with file name '{fileName}' was not loaded", e);
            }
        }

        private void ReadSchema(FileStream fileStream, BinaryReader reader, string schemaName)
        {
            var isStopped = false;
            while (fileStream.CanRead)
            {
                var data = reader.ReadString();
                if (data == "__STOP__!")
                {
                    isStopped = true;
                    break;
                }
                if (data == "__SEMANT__!")
                {
                    this._hasSemant = true;
                    this.ReadSemants(reader);
                }
                else
                {
                    this.ReadTable(data, reader, schemaName);
                }
            }
            if (!isStopped)
            {
                throw new InvalidOperationException($"Invalid a struct of schema '{schemaName}'");
            }
        }

        private void ReadSemants(BinaryReader reader)
        {
            while (true)
            {
                string name = reader.ReadString();
                if (string.IsNullOrEmpty(name))
                {
                    break;
                }

                var type = (SemantType)reader.ReadSByte();
                var subType = reader.ReadSByte();

                var semant = new Semant(name, type)
                {
                    Div = (((subType & 1) != 0) ? reader.ReadDouble() : 0.0),
                    Min = (((subType & 2) != 0) ? reader.ReadDouble() : 1E-99),
                    Max = (((subType & 4) != 0) ? reader.ReadDouble() : 1E-99),
                    Sym = (((subType & 8) != 0) ? reader.ReadString() : null)
                };

                this._dicoSemants[name] = semant;
            }
        }

        private void ReadTable(string name, BinaryReader reader, string schemaName)
        {
            var table = this.LoadTable(name);
            if (table.Name != name)
            {
                var alias = table.Name;
                table = new Table(name);
                table.AliasOf = alias;
            }

            table.SchemaName = schemaName;
            table.DbName = reader.ReadString();
            int fieldsCount = reader.ReadInt32();
            table.Fields = new Field[fieldsCount];
            table.dicoFields = new Dictionary<string, Field>(fieldsCount);

            int num2 = 0;
            for (int i = 0; i < fieldsCount; i++)
            {
                var field = this.ReadTableField(reader);
                field.Index = i;
                field.Table = table;
                table.AddField(i, field);
                //table.Fields[i] = field;
                //table.dicoFields[field.Name] = field;
                if (field.ClassIndex >= 0)
                {
                    num2++;
                }
            }
            table.ClassFields = new Field[num2];
            for (int j = 0; j < fieldsCount; j++)
            {
                var field2 = table.Fields[j];
                if (field2.ClassIndex >= 0)
                {
                    table.ClassFields[field2.ClassIndex] = field2;
                }
            }

            int num3 = reader.ReadInt32();
            table.Indexes = new Index[num3];
            for (int k = 0; k < num3; k++)
            {
                var ormIndex = new Index();
                table.Indexes[k] = ormIndex;
                ormIndex.Type = (IndexType)reader.ReadInt32();
                ormIndex.Module = this.GetModule(reader.ReadString());
                ormIndex.Name = reader.ReadString();
                int num4 = reader.ReadInt32();
                ormIndex.Fields = new FieldF[num4];
                for (int l = 0; l < num4; l++)
                {
                    ormIndex.Fields[l] = (FieldF)table.Field(reader.ReadString());
                }
            }

            int num5 = reader.ReadInt32();
            table.Joins = new Join[num5];
            for (int m = 0; m < num5; m++)
            {
                var ormJoin = new Join();
                table.Joins[m] = ormJoin;
                ormJoin.Type = (JoinType)reader.ReadInt32();
                ormJoin.Module = this.GetModule(reader.ReadString());
                ormJoin.Repr = (FieldJ)table.Field(reader.ReadString());
                ormJoin.JoinedTable = this.LoadTable(reader.ReadString());
                ormJoin.JoinedListName = ReadNullableString(reader);
                int num6 = reader.ReadInt32();
                ormJoin.BadConstr = new string[num6];
                for (int n = 0; n < num6; n++)
                {
                    ormJoin.BadConstr[n] = reader.ReadString();
                }
                ormJoin.Nequ = reader.ReadInt32();
                ormJoin.From = new Field[ormJoin.Nequ];
                ormJoin.ToNames = new string[ormJoin.Nequ];
                for (int n = 0; n < ormJoin.Nequ; n++)
                {
                    ormJoin.ToNames[n] = reader.ReadString();
                    ormJoin.From[n] = table.Field(reader.ReadString());
                }
            }

            table.TemplateTableName = ReadNullableString(reader);
            table.Type = (TableType)reader.ReadInt32();
            table.Cat = (TableCat)reader.ReadInt32();
            table.Module = this.GetModule(reader.ReadString());
            table.PluginNumber = reader.ReadInt32();
            table.Activation = reader.ReadString();
            table.PrettyName = reader.ReadString();
            table.ShortDesc = reader.ReadString();
            table.FullDesc = reader.ReadString();
            table.CompOwner = this.LoadTable(ReadNullableString(reader));
            table.CompOwnerBis = this.LoadTable(ReadNullableString(reader));
            table.CompOrder1 = table.Field(ReadNullableString(reader));
            table.CompOrder2 = table.Field(ReadNullableString(reader));
            table.CompOrder = new FieldF[(table.CompOrder1 == null) ? 0 : ((table.CompOrder2 == null) ? 1 : 2)];
            if (table.CompOrder1 != null)
            {
                table.CompOrder[0] = table.CompOrder1;
                if (table.CompOrder2 != null)
                {
                    table.CompOrder[1] = table.CompOrder2;
                }
            }
            table.CompNumbered = table.Field(ReadNullableString(reader));
            int num8 = reader.ReadInt32();
            table.CompJoin = ((num8 >= 0) ? table.Joins[num8] : null);
            num8 = reader.ReadInt32();
            table.CompJoinBis = ((num8 >= 0) ? table.Joins[num8] : null);
            table.HasID = reader.ReadBoolean();
            table.IsCompByPointer = reader.ReadBoolean();
            table.IsCompByList = reader.ReadBoolean();
            table.CompAddWhenMergeParent = reader.ReadBoolean();
            table.HasComponents = reader.ReadBoolean();
            table.HasComponentsWithID = reader.ReadBoolean();
            table.ComponentsCount = reader.ReadInt32();
            table.Components = new Table[table.ComponentsCount];
            for (int num9 = 0; num9 < table.ComponentsCount; num9++)
            {
                table.Components[num9] = this.LoadTable(reader.ReadString());
            }
            table.HasDateModified = reader.ReadBoolean();
            table.HasModifiedBy = reader.ReadBoolean();
            table.HasDateCreated = reader.ReadBoolean();
            table.HasCreatedBy = reader.ReadBoolean();
            num8 = reader.ReadInt32();
            table.PrimaryIndex = ((num8 >= 0) ? table.Indexes[num8] : null);
            num8 = reader.ReadInt32();
            table.CodeIndex = ((num8 >= 0) ? table.Indexes[num8] : null);
            num8 = reader.ReadInt32();
            table.ImportJoin = ((num8 >= 0) ? table.Joins[num8] : null);
        }

        private static string ReadNullableString(BinaryReader reader)
        {
            var text = reader.ReadString();
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            return null;
        }

        private Module GetModule(string name)
        {
            if (!this._modules.TryGetValue(name, out Module result))
            {
                result = (this._modules[name] = new Module(name));
            }
            return result;
        }

        private Semant GetSemant(string specname)
        {
            if (specname.IsNull())
            {
                return null;
            }
            if (this._dicoSemants.TryGetValue(specname, out Semant semant))
            {
                return semant;
            }
            if (specname.StartsWith("eri_"))
            {
                semant = new Semant(specname, SemantType.tCombo);
                semant.Sym = specname.Substring(4);
                semant.Max = 400.0;
                semant.Div = 1.0;
            }
            else if (specname.StartsWith("lov_"))
            {
                semant = new Semant(specname, SemantType.tComboUser);
                semant.Sym = specname.Substring(4);
                semant.Max = 400.0;
                semant.Div = 1.0;
            }
            else if (specname.StartsWith("stat_"))
            {
                semant = new Semant(specname, SemantType.tComboWrkf);
                semant.Sym = specname.Substring(5);
                semant.Max = 400.0;
            }
            else if (specname.StartsWith("fk_"))
            {
                semant = new Semant(specname, SemantType.tForeignId);
                semant.Sym = specname.Substring(3);
            }
            else if (specname.StartsWith("list_eri_"))
            {
                semant = new Semant(specname, SemantType.tListCombo);
                semant.Sym = specname.Substring(9);
                semant.Div = 0.0;
            }
            else if (specname.StartsWith("list_lov_"))
            {
                semant = new Semant(specname, SemantType.tListComboUser);
                semant.Sym = specname.Substring(9);
                semant.Div = 0.0;
            }
            else if (specname.StartsWith("String("))
            {
                semant = new Semant(specname, SemantType.tStri);
                semant.Max = (double)int.Parse(specname.Substring(7, specname.Length - 8));
            }
            else if (specname.StartsWith("Integer("))
            {
                semant = new Semant(specname, SemantType.tInteger);
                int num = (int)(specname[8] - '0');
                double num2 = Math.Pow(10.0, (double)num) - 1.0;
                semant.Max = num2 - 1.0;
                semant.Min = -num2;
            }
            else if (specname == "Folder")
            {
                semant = new Semant(specname, SemantType.tFolder);
                semant.Max = 256.0;
            }
            return semant;
        }

        public Table GetTableByName(string name)
        {
            if (name == null)
            {
                return null;
            }
            if (this._tables.TryGetValue(name, out Table table))
            {
                return table;
            }
            return null;
        }

        private Table LoadTable(string name)
        {
            if (name == null)
            {
                return null;
            }
            if (this._tables.TryGetValue(name, out Table table))
            {
                return table;
            }

            string alias = null;
            if (this._singlePosEqpAnt)
            {
                alias = this.GetAliasOf(name);
            }

            if (alias != null)
            {
                table = this.LoadTable(alias);
                this._tables[name] = table;
            }
            else
            {
                table = new Table(name);
                this._tables[name] = table;
                this._tablesList.Add(table);
            }
            return table;
        }

        private string GetAliasOf(string tname)
        {
            if (tname.StartsWith("BAG_"))
            {
                string aliasOf = this.GetAliasOf(tname.Substring(4));
                if (aliasOf != null)
                {
                    return "BAG_" + aliasOf;
                }
                return null;
            }
            else
            {
                if (!tname.StartsWith("BG_"))
                {
                    if (tname.StartsWith("EQUIP_"))
                    {
                        if (tname == "EQUIP_PMR" || tname == "EQUIP_MOB2" || tname == "EQUIP_BRO" || tname == "EQUIP_ESTA")
                        {
                            return "EQUIP_MW";
                        }
                        if (tname == "EQUIP_PMR_MPT" || tname == "EQUIP_MOB2_MPT" || tname == "EQUIP_BRO_MPT" || tname == "EQUIP_ESTA_MPT")
                        {
                            return "EQUIP_MW_MPT";
                        }
                        if (tname == "EQUIP_PMR_SYN" || tname == "EQUIP_MOB2_SYN" || tname == "EQUIP_BRO_SYN" || tname == "EQUIP_ESTA_SYN")
                        {
                            return "EQUIP_MW_SYN";
                        }
                    }
                    else if (tname.StartsWith("POSITION_"))
                    {
                        if (tname == "POSITION_ES" || tname == "POSITION_FWA" || tname == "POSITION_HF" || tname == "POSITION_FMN" || tname == "POSITION_MOB2" || tname == "POSITION_WIM" || tname == "POSITION_BRO" || tname == "POSITION_MW")
                        {
                            return "SITE";
                        }
                        if (tname == "POSITION_ES_SYN" || tname == "POSITION_FWA_SYN" || tname == "POSITION_HF_SYN" || tname == "POSITION_FMN_SYN" || tname == "POSITION_MOB2_SYN" || tname == "POSITION_WIM_SYN" || tname == "POSITION_BRO_SYN" || tname == "POSITION_MW_SYN")
                        {
                            return "SITE_SYN";
                        }
                    }
                    else if (tname.StartsWith("ANTENNA_"))
                    {
                        if (tname == "ANTENNA_MOB" || tname == "ANTENNA_MOB2" || tname == "ANTENNA_MW" || tname == "ANTENNA_HF" || tname == "ANTENNA_BRO")
                        {
                            return "ANTENNA";
                        }
                        if (tname == "ANTENNA_MOB_MPT" || tname == "ANTENNA_MOB2_MPT" || tname == "ANTENNA_MW_MPT" || tname == "ANTENNA_HF_MPT" || tname == "ANTENNA_BRO")
                        {
                            return "ANTENNA_MPT";
                        }
                        if (tname == "ANTENNA_MOB_SYN" || tname == "ANTENNA_MOB2_SYN" || tname == "ANTENNA_MW_SYN" || tname == "ANTENNA_HF_SYN" || tname == "ANTENNA_BRO")
                        {
                            return "ANTENNA_SYN";
                        }
                    }
                    return null;
                }
                string aliasOf2 = this.GetAliasOf(tname.Substring(3));
                if (aliasOf2 != null)
                {
                    return "BG_" + aliasOf2;
                }
                return null;
            }
        }

        private Field ReadTableField(BinaryReader reader)
        {
            var ormFieldNature = (FieldNature)reader.ReadInt32();
            Field ormField = null;
            FieldNature ormFieldNature2 = ormFieldNature;

            switch (ormFieldNature2)
            {
                case FieldNature.Calc:
                    ormField = new FieldC();
                    break;
                case (FieldNature)68:
                case (FieldNature)71:
                case (FieldNature)72:
                case (FieldNature)73:
                case (FieldNature)75:
                    break;
                case FieldNature.Expr:
                    ormField = new FieldE();
                    break;
                case FieldNature.Column:
                    ormField = new FieldF();
                    break;
                case FieldNature.Join:
                    ormField = new FieldJ();
                    break;
                case FieldNature.List:
                    ormField = new FieldL();
                    break;
                case FieldNature.MultipleValues:
                    ormField = new FieldM();
                    break;
                default:
                    if (ormFieldNature2 == FieldNature.Update)
                    {
                        ormField = new FieldU();
                    }
                    break;
            }
            ormField.Nature = ormFieldNature;
            ormField.Name = reader.ReadString();
            ormField.Module = this.GetModule(reader.ReadString());
            if (this._hasSemant)
            {
                string specname = reader.ReadString();
                ormField.Special = this.GetSemant(specname);
            }
            ormField.Info = reader.ReadString();
            ormField.DDesc = this.ReadDataDesc(reader.ReadString());
            FieldNature ormFieldNature3 = ormFieldNature;
            switch (ormFieldNature3)
            {
                case FieldNature.Calc:
                    {
                        FieldC ormFieldC = (FieldC)ormField;
                        ormFieldC.calcCode = reader.ReadInt32();
                        int num = reader.ReadInt32();
                        ormFieldC.Columns = new string[num];
                        for (int i = 0; i < num; i++)
                        {
                            ormFieldC.Columns[i] = reader.ReadString();
                        }
                        break;
                    }
                case (FieldNature)68:
                case (FieldNature)71:
                case (FieldNature)72:
                case (FieldNature)73:
                case (FieldNature)75:
                    break;
                case FieldNature.Expr:
                    {
                        if (ormField.Special == null)
                        {
                            ormField.Special = this.GetDefaultSemant(ormField.DDesc);
                        }
                        FieldE ormFieldE = (FieldE)ormField;
                        ormFieldE.Expr = reader.ReadString();
                        break;
                    }
                case FieldNature.Column:
                    {
                        if (ormField.Special == null)
                        {
                            ormField.Special = this.GetDefaultSemant(ormField.DDesc);
                        }
                        FieldF ormFieldF = (FieldF)ormField;
                        ormFieldF.Options = (FieldFOption)reader.ReadInt32();
                        ormFieldF.DefVal = ReadNullableString(reader);
                        break;
                    }
                case FieldNature.Join:
                    {
                        FieldJ ormFieldJ = (FieldJ)ormField;
                        ormFieldJ._joinIdx = reader.ReadInt32();
                        ormFieldJ.Syn = reader.ReadString();
                        break;
                    }
                case FieldNature.List:
                    {
                        FieldL ormFieldL = (FieldL)ormField;
                        ormFieldL.ComponentTable = this.LoadTable(reader.ReadString());
                        ormFieldL._componentJoin = reader.ReadString();
                        break;
                    }
                case FieldNature.MultipleValues:
                    {
                        FieldM ormFieldM = (FieldM)ormField;
                        ormFieldM.VarType = (VarType)reader.ReadInt32();
                        ormFieldM.Size = reader.ReadInt32();
                        break;
                    }
                default:
                    if (ormFieldNature3 == FieldNature.Update)
                    {
                        FieldU ormFieldU = (FieldU)ormField;
                        ormFieldU.updCode = reader.ReadInt32();
                        ormFieldU.s1 = reader.ReadString();
                        ormFieldU.s2 = reader.ReadString();
                        ormFieldU.s3 = reader.ReadString();
                        ormFieldU.s4 = reader.ReadString();
                    }
                    break;
            }
            ormField.ClassIndex = reader.ReadInt32();
            return ormField;
        }

        private Semant GetDefaultSemant(DataDesc desc)
        {
            if (desc == null)
            {
                return null;
            }
            switch (desc.Coding)
            {
                case DataCoding.tvalNUMBER:
                    if (this._specInteger == null)
                    {
                        this._specInteger = new Semant[13];
                        for (int i = 1; i <= 9; i++)
                        {
                            this._specInteger[i] = GetSemant(string.Format("Integer({0})", i));
                        }
                    }
                    if (desc.Scale == 0 && desc.Precision >= 1 && desc.Precision <= 9)
                    {
                        return this._specInteger[desc.Precision];
                    }
                    if (desc.ClassType == VarType.var_Int)
                    {
                        return GetSemant("Integer");
                    }
                    return GetSemant("Number");
                case DataCoding.tvalSTRING:
                    {
                        foreach (Semant current in this._specStrings)
                        {
                            if (current.Max == (double)desc.Precision)
                            {
                                return current;
                            }
                        }
                        Semant semant = GetSemant(string.Format("String({0})", desc.Precision));
                        this._specStrings.Add(semant);
                        return semant;
                    }
                case DataCoding.tvalDATETIME:
                    return GetSemant("Date");
                case DataCoding.tvalBINARY:
                    return GetSemant("Binary");
                case DataCoding.tvalGUID:
                    return GetSemant("Guid");
                default:
                    return null;
            }
        }

        private DataDesc ReadDataDesc(string typ)
        {
            if (string.IsNullOrEmpty(typ))
            {
                return null;
            }

            if (this._dataDescs.TryGetValue(typ, out DataDesc ormDataDesc))
            {
                return ormDataDesc;
            }

            ormDataDesc = new DataDesc();
            if (typ == "GUID")
            {
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalGUID;
                ormDataDesc.ClassType = VarType.var_Guid;
                ormDataDesc.ClassNull = Guid.Empty;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            if (typ == "BIT")
            {
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalNUMBER;
                ormDataDesc.ClassType = VarType.var_Int;
                ormDataDesc.ClassNull = 2147483647;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            if (typ == "SMALLINT")
            {
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalNUMBER;
                ormDataDesc.ClassType = VarType.var_Int;
                ormDataDesc.ClassNull = 2147483647;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            if (typ == "INT4")
            {
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalNUMBER;
                ormDataDesc.ClassType = VarType.var_Int;
                ormDataDesc.ClassNull = 2147483647;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            if (typ == "FLOAT4")
            {
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalNUMBER;
                ormDataDesc.ClassType = VarType.var_Dou;
                ormDataDesc.ClassNull = 1E-99;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            if (typ == "FLOAT8")
            {
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalNUMBER;
                ormDataDesc.ClassType = VarType.var_Dou;
                ormDataDesc.ClassNull = 1E-99;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            if (typ == "DATE")
            {
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalDATETIME;
                ormDataDesc.ClassType = VarType.var_Tim;
                DateTime dateTime = new DateTime(0L);
                ormDataDesc.ClassNull = dateTime;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            if (typ.IndexOf("NUMBER(") == 0)
            {
                int num = typ.IndexOf(',');
                if (num < 0)
                {
                    return null;
                }
                int num2 = typ.IndexOf(')');
                if (num2 <= num)
                {
                    return null;
                }
                int num3;
                if (!int.TryParse(typ.Substring(7, num - 7), out num3))
                {
                    return null;
                }
                int num4;
                if (!int.TryParse(typ.Substring(num + 1, num2 - num - 1), out num4))
                {
                    return null;
                }
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalNUMBER;
                ormDataDesc.Precision = num3;
                ormDataDesc.Scale = num4;
                if (num3 < 10 && num4 == 0)
                {
                    ormDataDesc.ClassType = VarType.var_Int;
                    ormDataDesc.ClassNull = 2147483647;
                }
                else
                {
                    ormDataDesc.ClassType = VarType.var_Dou;
                    ormDataDesc.ClassNull = 1E-99;
                }
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            else if (typ.IndexOf("VARCHAR(") == 0)
            {
                int num5 = typ.IndexOf(')');
                if (num5 < 0)
                {
                    return null;
                }
                int precision;
                if (!int.TryParse(typ.Substring(8, num5 - 8), out precision))
                {
                    return null;
                }
                ormDataDesc.Precision = precision;
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalSTRING;
                ormDataDesc.ClassType = VarType.var_String;
                ormDataDesc.ClassNull = "";
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
            else
            {
                if (typ.IndexOf("BINARY(") != 0)
                {
                    return null;
                }
                int num6 = typ.IndexOf(')');
                if (num6 < 0)
                {
                    return null;
                }
                int precision2;
                if (!int.TryParse(typ.Substring(7, num6 - 7), out precision2))
                {
                    return null;
                }
                ormDataDesc.Name = typ;
                ormDataDesc.Coding = DataCoding.tvalBINARY;
                ormDataDesc.Precision = precision2;
                ormDataDesc.ClassType = VarType.var_Bytes;
                ormDataDesc.ClassNull = null;
                this._dataDescs[typ] = ormDataDesc;
                return ormDataDesc;
            }
        }


        public string BuildJoinStatement(IDataEngine config, QuerySelectStatement statement, string[] fieldPaths, out DbField[] selectedFields) // DBMS dbms, string quoteColumn)
        {
            var schemaPrefix = this._config.SchemaPrefix + ".";
            var expressColumns = statement.Table.Columns.ToList().FindAll(z => !string.IsNullOrEmpty(z.Value.Expression)).Select(t => t.Value).ToList();
            //this._expressColumns = statement.Table.Columns.ToList().FindAll(z => !string.IsNullOrEmpty(z.Value.Expression)).Select(t => t.Value).ToList();
            this._configDataEngine = config;
            var dbTables = new Dictionary<string, DbTable>();
            var dbJoines = new List<DbJoin>();
            var dbFields = new List<DbField>();
            var dbWorldFields = new Dictionary<string, DbField>();

            selectedFields = new DbField[fieldPaths.Length];
            for (int i = 0; i < fieldPaths.Length; i++)
            {
                var fieldPath = fieldPaths[i];

                var dbField = this.AddField(statement.Table.Name, fieldPath, schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields,expressColumns);
                dbField.Path = fieldPath;
                selectedFields[i] = dbField;
            }

            var joinSql = BuildJoinStatement(config.Syntax, statement.Table.Name, schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields);
            var formatedSql = FormatJoinStatement(joinSql);

            return formatedSql;
        }

        public string BuildJoinStatement(IEngineSyntax engineSyntax, string tableName, string[] fieldPaths, out DbField[] selectedFields) // DBMS dbms, string quoteColumn)
        {
            var schemaPrefix = this._config.SchemaPrefix + ".";

            var dbTables = new Dictionary<string, DbTable>();
            var dbJoines = new List<DbJoin>();
            var dbFields = new List<DbField>();
            var dbWorldFields = new Dictionary<string, DbField>();

            selectedFields = new DbField[fieldPaths.Length];
            for (int i = 0; i < fieldPaths.Length; i++)
            {
                var fieldPath = fieldPaths[i];

                var dbField = this.AddField(tableName, fieldPath, schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields);
                dbField.Path = fieldPath;
                selectedFields[i] = dbField;
            }

            var joinSql = BuildJoinStatement(engineSyntax, tableName, schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields);
            var formatedSql = FormatJoinStatement(joinSql);

            return formatedSql;
        }



        private static string FormatJoinStatement(string expression)
        {
            int ident = -1;
            string identValue = "";

            var sql = new StringBuilder();

            foreach (var symbol in expression)
            {
                if (symbol == '(')
                {
                    ++ident;
                    if (ident > 0 && symbol != '(')
                    {
                        //identValue += "    ";
                        sql.Append(Environment.NewLine);
                        sql.Append(identValue);

                    }
                    sql.Append(symbol);
                    ++ident;
                    identValue += "    ";
                    sql.Append(Environment.NewLine);
                    sql.Append(identValue);
                }
                else if (symbol == ')')
                {
                    --ident;
                    identValue = identValue.Substring(0, identValue.Length - 4);
                    sql.Append(Environment.NewLine);
                    sql.Append(identValue);
                    sql.Append(symbol);
                }
                else
                {
                    sql.Append(symbol);
                }

            }
            return sql.ToString();
        }

        private string BuildJoinStatement(IEngineSyntax engineSyntax, string tableName, string schemaPrefix, Dictionary<string, DbTable> dbTables, List<DbJoin> dbJoines, List<DbField> dbFields, Dictionary<string, DbField> dbWorldFields)
        {
            string joinSql = string.Empty;
            //string str;
            //string str2;
            //if (quoteColumn.Length == 2)
            //{
            //    str = quoteColumn.Substring(0, 1);
            //    str2 = quoteColumn.Substring(1, 1);
            //}
            //else
            //{
            //    str2 = (str = "");
            //}
            List<string> list = new List<string>();


            foreach (var current in dbFields)
            {
                bool flag = true;
                string text2 = null;
                string text3 = null;
                DbTable dbrecordtable = null;
                foreach (var current2 in dbTables.Values)
                {
                    string text4 = current2.alias;
                    if (text4 == null)
                    {
                        text4 = current2.logTab;
                    }
                    if (text4 == current.m_logTab)
                    {
                        dbrecordtable = current2;
                    }
                    else if (this.FindColumnInMappedTable(current2.logTab, current.m_realFld, true, ref text3, ref text2))
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    foreach (string current3 in list)
                    {
                        if (this.FindColumnInMappedTable(current3, current.m_realFld, true, ref text3, ref text2))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                string text5 = engineSyntax.EncodeFieldName(current.m_realFld); // str + current.m_realFld + str2;
                if (flag)
                {
                    string str3;
                    if (dbrecordtable.alias != null)
                    {
                        str3 = dbrecordtable.Tcaz;
                        dbrecordtable.aliasUsed = true;
                    }
                    else
                    {
                        str3 = dbrecordtable.tbNameInDb;
                    }
                    current.m_name = str3 + "." + text5;
                }
                else
                {
                    current.m_name = text5;
                }
            }
            foreach (var current4 in dbTables.Values)
            {
                current4.tmp = new TotoDBR();
                current4.tmp.outerEd = null;
                current4.tmp.NbLeft = 0;
                current4.tmp.nam = current4.tbNameInDb;
                if (current4.alias != null && (dbTables.Values.Count + list.Count > 1 || current4.aliasUsed))
                {
                    var dbr = current4.tmp;
                    dbr.nam = dbr.nam + " " + current4.Tcaz;
                }
            }

            
            if (dbJoines != null)
            {
                foreach (var current5 in dbJoines)
                {
                    if (current5.outer)
                    {
                        var dbrecordtable2 = this.GetDbTable(current5.sTab, dbTables);
                        var dbrecordtable3 = this.GetDbTable(current5.dTab, dbTables);
                        string text6 = (dbrecordtable2.alias != null) ? dbrecordtable2.Tcaz : dbrecordtable2.tbNameInDb;
                        string text7 = (dbrecordtable3.alias != null) ? dbrecordtable3.Tcaz : dbrecordtable3.tbNameInDb;
                        dbrecordtable3.tmp.cond = "";
                        //string text8 = str; // "[";
                        //string text9 = str2; // "]";
                        for (int i = 0; i < current5.nequ; i++)
                        {
                            var ormItem = current5.sItem[i];
                            var ormItem2 = current5.dItem[i];
                            if (!(current5.sTab != ormItem.m_logTab))
                            {
                                if (ormItem is DbExpressionField)
                                {
                                    TotoDBR expr_3E3 = dbrecordtable3.tmp;
                                    expr_3E3.cond += string.Format("{0}{1} = {2}.{3}", new object[]
                                    {
                                        (i != 0) ? " AND " : "",
                                        ormItem.GetDataName(),
                                        text7,
                                        //text8,
                                        engineSyntax.EncodeFieldName(ormItem2.m_realFld),
                                        //text9
                                    });
                                }
                                else
                                {
                                    TotoDBR expr_44C = dbrecordtable3.tmp;
                                    expr_44C.cond += string.Format("{0}{1}.{2} = {3}.{4}", new object[]
                                    {
                                        (i != 0) ? " AND " : "",
                                        text6,
                                        //text8,
                                        engineSyntax.EncodeFieldName(ormItem.m_realFld),
                                        //text9,
                                        text7,
                                        //text8,
                                        engineSyntax.EncodeFieldName(ormItem2.m_realFld),
                                        //text9
                                    });
                                }
                            }
                        }
                        dbrecordtable3.tmp.outerEd = dbrecordtable2;
                        dbrecordtable2.tmp.NbLeft++;
                    }
                }
            }
            bool flag2;
            do
            {
                flag2 = true;
                foreach (var current6 in dbTables.Values)
                {
                    if (current6.tmp.outerEd != null && current6.tmp.NbLeft == 0)
                    {
                        var dbrecordtable2 = current6.tmp.outerEd;
                        var dbrecordtable3 = current6;
                        dbrecordtable2.tmp.nam = string.Format("({0} LEFT JOIN {1} ON {2})", dbrecordtable2.tmp.nam, dbrecordtable3.tmp.nam, dbrecordtable3.tmp.cond);
                        dbrecordtable2.tmp.NbLeft--;
                        dbrecordtable3.tmp.nam = null;
                        dbrecordtable3.tmp.outerEd = null;
                        flag2 = false;
                    }
                }
            }
            while (!flag2);

            string str4 = "";
            foreach (var current7 in dbTables.Values)
            {
                if (current7.tmp.nam != null)
                {
                    joinSql = joinSql + str4 + current7.tmp.nam;
                    str4 = ", ";
                }
                current7.tmp = null;
            }

            return joinSql;
        }

        private DbField AddField(string tableName, string fieldPath, string schemaPrefix, Dictionary<string, DbTable> dbTables, List<DbJoin> dbJoines, List<DbField> dbFields, Dictionary<string, DbField> dbWorldFields, List<QuerySelectStatement.ColumnDescriptor> expressColumns=null)
        {
            dbTables.TryGetValue(tableName, out DbTable dbTable);
            var tableName1 = tableName;
            var tableName2 = (dbTable != null) ? dbTable.logTab : tableName;
            var tableName3 = tableName;

            int pos1 = 0;
            int pos2 = 0;

            while ((pos2 = fieldPath.IndexOf('.', pos1)) >= 0) 
            {
                var joinedTableName = fieldPath.Substring(pos1, pos2 - pos1);
                var table = this.GetTableByName(tableName2);
                var tableJoin = table?.Join(joinedTableName);

                if (tableJoin == null)
                {
                    throw new InvalidOperationException($"Not found table join by name '{joinedTableName}' for table with name '{tableName2}'");
                }

                tableName3 += tableJoin.Repr.Syn;
                if (!dbTables.ContainsKey(tableName3))
                {
                    var addedDbTable = this.AddDbTable(dbTables, tableJoin.JoinedTable.Name, tableName3, schemaPrefix);
                    var result = this.AddJoin(
                        
                        tableName1, tableName3, true, joinedTableName,
                        tableJoin.From.Select(o => o.Name).ToArray(),
                        tableJoin.To.Select(o => o.Name).ToArray(),
                        schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields
                        );

                    if (!result)
                    {
                        throw new InvalidOperationException($"Not added table join by name '{joinedTableName}' for table with name '{tableName2}'");
                    }
                }

                pos1 = pos2 + 1;
                tableName1 = tableName3;
                tableName2 = tableJoin.JoinedTable.Name;
            }

            if (pos1 < fieldPath.Length)
            {
                string nextFieldPath = fieldPath.Substring(pos1);
                return this.AddNextField(tableName1, nextFieldPath, schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields, expressColumns);
            }

            throw new InvalidOperationException($"Incorrect field path '{fieldPath}' for table with name '{tableName}'");
        }

        private string UnaliasTable(string tableName, Dictionary<string, DbTable> dbTables)
        {
            foreach (var dbTable in dbTables.Values)
            {
                if (dbTable.alias == tableName)
                {
                    return dbTable.logTab;
                }
            }
            return tableName;
        }

        private DbTable GetDbTable(string tableName, Dictionary<string, DbTable> dbTables)
        {
            if (tableName == null)
            {
                return null;
            }
            dbTables.TryGetValue(tableName, out DbTable dbTable);
            return dbTable;
        }

        private DbField GetDbField(string tableName, string fieldPath, Dictionary<string, DbTable> dbTables, List<DbJoin> dbJoines, List<DbField> dbFields, Dictionary<string, DbField> dbWorldFields)
        {
            int num = 0;
            if (tableName == null)
            {
                num = fieldPath.IndexOf('.');
                if (num < 0)
                {
                    return null;
                }
                tableName = fieldPath.Substring(0, num);
                num++;
            }
            else
            {
                int num2 = tableName.IndexOf('.');
                if (num2 > 0)
                {
                    fieldPath = tableName.Substring(num2 + 1) + "." + fieldPath;
                    tableName = tableName.Substring(0, num2);
                }
            }
            string str = tableName;
            var dbrecordtable = this.GetDbTable(tableName, dbTables);
            string name = (dbrecordtable != null) ? dbrecordtable.logTab : tableName;
            string text = tableName;
            int num3;
            while ((num3 = fieldPath.IndexOf('.', num)) >= 0)
            {
                string fldName = fieldPath.Substring(num, num3 - num);
                var ormTable = this.GetTableByName(name);
                var ormJoin = (ormTable == null) ? null : ormTable.Join(fldName);
                if (ormJoin == null)
                {
                    return null;
                }
                text += ormJoin.Repr.Syn;
                num = num3 + 1;
                str = text;
                name = ormJoin.JoinedTable.Name;
            }
            string str2 = fieldPath.Substring(num);

            dbWorldFields.TryGetValue(str + "/" + str2, out DbField result);
            return result;
        }
        private bool Translate(string tName, string fld, ref string tabr, ref string fldr, string schemaPrefix)
        {
            var ormTable = this.GetTableByName(tName);
            if (ormTable == null)
            {
                tabr = null;
                fldr = null;
                return false;
            }
            tabr = schemaPrefix + ormTable.DbName;
            List<FieldF> list = ormTable.MappingFields;

            foreach (var current in list)
            {
                if (string.Equals(current.Name, fld, StringComparison.OrdinalIgnoreCase))
                {
                    fldr = null;
                    return false;
                }
            }
            fldr = fld;
            return true;
        }

        private bool FindColumnInMappedTable(string tName, string fldName, bool caseSensitive, ref string desc, ref string fldr)
        {
            desc = null;
            fldr = null;
            Table ormTable = this.GetTableByName(tName);
            if (ormTable == null)
            {
                return false;
            }
            List<FieldF> list = ormTable.MappingFields;
            foreach (FieldF current in list)
            {
                if (string.Equals(current.Name, fldName, StringComparison.OrdinalIgnoreCase))
                {
                    fldr = null;
                    bool result = false;
                    return result;
                }
            }
            Field ormField = ormTable.Field(fldName);
            if (ormField != null && ormField.Nature == FieldNature.Column)
            {
                desc = ormField.DDesc.Name;
                fldr = fldName;
                return true;
            }
            if (!caseSensitive)
            {
                string b = fldName.ToUpper();
                Field[] fields = ormTable.Fields;
                for (int i = 0; i < fields.Length; i++)
                {
                    Field ormField2 = fields[i];
                    if (ormField2.Nature == FieldNature.Column && !(ormField2.Name.ToUpper() != b))
                    {
                        desc = ormField2.DDesc.Name;
                        fldr = fldName;
                        bool result = true;
                        return result;
                    }
                }
            }
            return false;
        }
        private DbField AddNextField(string tableName, string fieldPath, string schemaPrefix, Dictionary<string, DbTable> dbTables, List<DbJoin> dbJoines, List<DbField> dbFields, Dictionary<string, DbField> dbWorldFields, List<QuerySelectStatement.ColumnDescriptor> expressColumns=null)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            var name = this.UnaliasTable(tableName, dbTables);
            if (expressColumns == null) expressColumns = new List<QuerySelectStatement.ColumnDescriptor>();

            QuerySelectStatement.ColumnDescriptor descriptExpress = expressColumns.Find(t => t.Name == fieldPath);
            var ormTable = this.GetTableByName(name);
            var ormField = (ormTable == null) ? null : ormTable.Field(fieldPath);
            if (descriptExpress != null) ormField = (ormTable == null) ? null : ormTable.Field("CustomExpression");

            bool flag = false;
            Semant sp = null;
            bool fetch = true;


            if (ormField != null)
            {
                if (ormField.Nature == FieldNature.Column)
                {
                    sp = ormField.Special;
                }
                else if (ormField.Nature == FieldNature.Expr)
                {
                    
                    if (descriptExpress != null)
                    {
                        sp = ormField.Special;

                        var ormItemExpr = new DbExpressionField();
                        ormItemExpr.logFd = ormField;
                        if (ormField.DDesc == null)
                        {
                            ormField.DDesc = ReadDataDesc("VARCHAR(4000)");
                        }
                        ormItemExpr.Init(ormField.DDesc, sp, FieldFOption.fld_NONE);
                        ormItemExpr.m_expression = descriptExpress.Expression;
                        ormItemExpr.m_name = descriptExpress.Name;
                        ormItemExpr.m_fetched = fetch;
                        dbFields.Add(ormItemExpr);
                        dbWorldFields[tableName + "/" + fieldPath] = ormItemExpr;
                        ormItemExpr.m_logTab = tableName;
                        //ormItemExpr.m_logFld = ormField.Name;
                        
                        ormItemExpr.AddFldsInExpression(this, dbTables[tableName].Tcaz);
                        ormItemExpr.m_logFld = ormItemExpr.m_fmt;// ormField.Name;
                    }
                }
            }
            string text = null;
            string text2 = null;
            dbTables.TryGetValue(tableName, out DbTable dbrecordtable);
            DbField ormItem;
            if (dbrecordtable == null)
            {
                ormTable = this.GetTableByName(tableName);
                if (ormTable != null)
                {
                    string tabn2 = null;
                    string text3 = null;
                    this.Translate(ormTable.Name, null, ref tabn2, ref text3, schemaPrefix);
                    //dbrecordtable = this.tableFind(tabn2);
                    dbTables.TryGetValue(tabn2, out dbrecordtable);
                    if (dbrecordtable != null)
                    {
                        flag = true;
                        text2 = null;
                        this.Translate(ormTable.Name, fieldPath, ref text, ref text2, schemaPrefix);
                        if ((ormItem = this.GetDbField(text, text2, dbTables, dbJoines, dbFields, dbWorldFields)) != null)
                        {
                            dbWorldFields[tableName + "/" + fieldPath] = ormItem;
                        }
                    }
                }
            }
            if (dbrecordtable != null)
            {
                if ((ormItem = this.GetDbField(tableName, fieldPath, dbTables, dbJoines, dbFields, dbWorldFields)) != null)
                {
                    if (fetch && !ormItem.m_fetched)
                    {
                       // this.nFields++;
                        ormItem.m_fetched = true;
                    }
                    ormItem.m_idxTable = dbrecordtable;
                    return ormItem;
                }
            }
            else
            {
                dbrecordtable = this.AddDbTable(dbTables, tableName, null, schemaPrefix);
            }
            string logTab = dbrecordtable.logTab;
            ormTable = dbrecordtable.zetb;
            var initedForDataExch = false;
            if (dbTables.Count == 0 && (ormTable.Type & TableType.tbl_MEMORY) == TableType.tbl_MEMORY)
            {
                initedForDataExch = true;
            }

            if (ormTable != null && !initedForDataExch)
            {
                if (!this.Translate(ormTable.Name, fieldPath, ref text, ref text2, schemaPrefix))
                {
                    string text4 = null;
                    if (!this.FindColumnInMappedTable(logTab, fieldPath, true, ref text4, ref text2))
                    {
                        var ormField2 = ormTable.Field(fieldPath);
                        if (ormField2 != null && ormField2.Nature == FieldNature.Column)
                        {
                            FieldFOption arg_267_0 = ((FieldF)ormField2).Options & FieldFOption.fld_OPT;
                        }
                        return null;
                    }
                    text = logTab;
                }
            }
            else
            {
                text = logTab;
                text2 = fieldPath;
            }
            DataDesc ormDataDesc = null;
            Field ormField3;
            if (initedForDataExch)
            {
                ormField3 = ormField;
                if (ormField != null && ormField.Nature == FieldNature.Column)
                {
                    ormDataDesc = ((FieldF)ormField).DDesc;
                }
            }
            else
            {
                ormField3 = null;
                string typ = null;
                string text5 = null;
                if (this.FindColumnInMappedTable(logTab, text2, true, ref typ, ref text5))
                {
                    ormDataDesc = ReadDataDesc(typ);
                }
            }
            if (ormDataDesc == null)
            {
                return null;
            }
            ormItem = new DbField();
            FieldFOption ormFieldFOption = FieldFOption.fld_NONE;
            if (ormField3 != null && ormField3 is FieldF)
            {
                ormFieldFOption = ((FieldF)ormField3).Options;
            }
            if (ormField != null && ormField is FieldF && (((FieldF)ormField).Options & FieldFOption.fld_PAST) == FieldFOption.fld_PAST)
            {
                ormFieldFOption |= FieldFOption.fld_PAST;
            }
            ormItem.logFd = ((ormField != null) ? ormField : ormField3);
            ormItem.Init(ormDataDesc, sp, ormFieldFOption);
            ormItem.m_idxTable = dbrecordtable;
            ormItem.m_logTab = (flag ? text : tableName);
            ormItem.m_logFld = (flag ? text2 : fieldPath);
            ormItem.m_realTab = text;
            ormItem.m_realFld = text2;
            ormItem.m_fetched = fetch;
            //if (fetch)
            //{
            //    this.nFields++;
            //}
            ormItem.Index = dbFields.Count;
            dbFields.Add(ormItem);
            dbWorldFields[tableName + "/" + fieldPath] = ormItem;
            if (dbrecordtable.alias == null || dbrecordtable.alias == tableName)
            {
                dbWorldFields[text + "/" + text2] = ormItem;
            }
            return ormItem;
        }
        private bool AddJoin(string tSrc, string tDst, bool outerLeft, string joinName, string[] sfi, string[] dfi, string schemaPrefix, Dictionary<string, DbTable> dbTables, List<DbJoin> dbJoines, List<DbField> dbFields, Dictionary<string, DbField> dbWorldFields)
        {
            var joining = new DbJoin();
            joining.nequ = sfi.GetLength(0);
            joining.sFld = sfi;
            joining.dFld = dfi;
            joining.sTab = tSrc;
            joining.slTab = this.UnaliasTable(tSrc, dbTables);
            joining.dTab = tDst;
            joining.dlTab = this.UnaliasTable(tDst, dbTables);
            joining.outer = outerLeft;
            int count = dbTables.Count;
            joining.sItem = new DbField[joining.nequ];
            joining.dItem = new DbField[joining.nequ];
            for (int i = 0; i < joining.nequ; i++)
            {
                if ((joining.sItem[i] = this.AddField(tSrc, joining.sFld[i], schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields)) == null)
                {
                    return false;
                }
                if ((joining.dItem[i] = this.AddField(tDst, joining.dFld[i], schemaPrefix, dbTables, dbJoines, dbFields, dbWorldFields)) == null)
                {
                    return false;
                }
            }
            if (dbTables.Count > count)
            {
                var dbrecordtable = this.GetDbTable(tDst, dbTables);
                if (dbrecordtable != null && dbTables.Values.Last() != dbrecordtable)
                {
                    dbTables.Remove(dbrecordtable.Key);
                    dbTables.Add(dbrecordtable.Key, dbrecordtable);
                }
            }
            Table ormTable;
            if (joinName == null && (ormTable = this.GetTableByName(joining.slTab)) != null)
            {
                var array = ormTable.Joins;
                for (int j = 0; j < array.Length; j++)
                {
                    var ormJoin = array[j];
                    if (ormJoin.Nequ == joining.nequ)
                    {
                        int num = 0;
                        int num2 = 0;
                        while (num2 < ormJoin.Nequ && num2 <= num)
                        {
                            for (int k = 0; k < joining.nequ; k++)
                            {
                                if (ormJoin.To[num2].Name == joining.dFld[k] && ormJoin.From[num2].Name == joining.sFld[k])
                                {
                                    num++;
                                    break;
                                }
                            }
                            num2++;
                        }
                        if (num == ormJoin.Nequ)
                        {
                            joinName = ormJoin.Repr.Name;
                        }
                    }
                    if (joinName != null)
                    {
                        break;
                    }
                }
            }
            joining.name = joinName;
            //if (this.joins == null)
            //{
            //    this.joins = new List<OrmRs.joining>();
            //}
            foreach (var current in dbJoines)
            {
                if (joining.Equal(current))
                {
                    current.outer |= outerLeft;
                    if (current.name == null)
                    {
                        current.name = joinName;
                    }
                    return true;
                }
            }
            dbJoines.Add(joining);
            return true;
        }

        private DbTable AddDbTable(Dictionary<string, DbTable> dbTables, string tableName, string alias, string schemaPrefix)
        {
            var initedForDataExch = false;
            var table = this.GetTableByName(tableName);
            if (table != null)
            {
                if (alias == null)
                {
                    alias = tableName;
                }
                if (dbTables.Count == 0 && (table.Type & TableType.tbl_MEMORY) == TableType.tbl_MEMORY)
                {
                    initedForDataExch = true;
                }
            }

            string tableDbName = null;
            if (!initedForDataExch)
            {
                tableDbName = this.GetTableDbName(tableName, schemaPrefix);
            }

            if (alias != null)
            {
                int i = 0;
                var dbTablesArray = dbTables.Values.ToArray();
                while (i < dbTablesArray.Length)
                {
                    var dbTable = dbTablesArray[i];
                    if (dbTable.alias == alias)
                    {
                        if (dbTable.tbNameInDb == tableDbName)
                        {
                            return dbTable;
                        }
                        throw new InvalidOperationException($"Not match DB Names '{tableDbName}' and '{dbTable.tbNameInDb}' for alias '{alias}'");
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            var tableCount = dbTables.Count;
            var newDbTable = new DbTable
            {
                tbNameInDb = tableDbName,
                logTab = tableName,
                alias = alias,
                zetb = table,
                Tcaz = "Tcaz_" + tableCount.ToString(),
                aliasUsed = false
            };
            dbTables.Add(newDbTable.Key, newDbTable);

            return newDbTable;
        }

        private string GetTableDbName(string tableName, string schemaPrefix)
        {
            var table = this.GetTableByName(tableName);
            var  tableDbName = (table == null) ? tableName : table.DbName;
            return schemaPrefix + tableDbName;
        }
    }
}
