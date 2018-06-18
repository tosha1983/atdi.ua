using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm.Orm
{
    public sealed class Table
    {
        public string Name;
        public string SchemaName;
        public string AliasOf;
        public string DbName;
        public string ClassDllName;
        public Module Module;
        public TableType Type;
        public TableCat Cat;
        public string TemplateTableName;
        public string ModuleName;
        public int PluginNumber;
        public string Activation;
        public string PrettyName;
        public string ShortDesc;
        public string FullDesc;
        public Type OrmClass;
        public Field[] Fields;
        public Field[] ClassFields;
        public Join CompJoin;
        public Join CompJoinBis;
        public bool HasID;
        public bool HasDateModified;
        public bool HasModifiedBy;
        public bool HasDateCreated;
        public bool HasCreatedBy;
        public bool IsCompByPointer;
        public bool IsCompByList;
        public bool CompAddWhenMergeParent;
        public Table CompOwner;
        public Table CompOwnerBis;
        public Field CompOrder1;
        public Field CompOrder2;
        public Field[] CompOrder;
        public Field CompNumbered;
        public bool HasComponents;
        public bool HasComponentsWithID;
        public int ComponentsCount;
        public Table[] Components;
        public Index[] Indexes;
        public Index PrimaryIndex;
        public Index CodeIndex;
        public Join[] Joins;
        public Join ImportJoin;
        internal Dictionary<string, Field> dicoFields;
        public List<FieldF> MappingFields;

        public void AddField(int index, Field field)
        {
            this.Fields[index] = field;
            this.dicoFields[field.Name] = field;
            if (this.MappingFields == null)
            {
                this.MappingFields = new List<FieldF>();
            }
            if (field is FieldF && (((FieldF)field).Options & FieldFOption.fld_OPT) == FieldFOption.fld_OPT)
            {
                this.MappingFields.Add((FieldF)field);
            }
        }
        public bool IsInEdition
        {
            get
            {
                return this.Module.IsInEdition;
            }
        }
        public Table(string name)
        {
            this.Name = name;
        }
        public Field Field(string fldName)
        {
            Field result = null;
            if (fldName != null)
            {
                this.dicoFields.TryGetValue(fldName, out result);
            }
            return result;
        }
        public Join Join(string fldName)
        {
            Join[] joins = this.Joins;
            for (int i = 0; i < joins.Length; i++)
            {
                Join ormJoin = joins[i];
                if (ormJoin.Repr.Name == fldName)
                {
                    return ormJoin;
                }
            }
            return null;
        }
    }
}
