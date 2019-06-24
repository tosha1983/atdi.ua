using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.CoreServices.EntityOrm
{
    public sealed class FieldDescriptor
    {
        public FieldDescriptor(IEntityMetadata owner, IFieldMetadata ownerField)
        {
            this.Owner = owner;
            this.OwnerField = ownerField;
        }

        /// <summary>
        /// Описатель пцти ссылки
        /// </summary>
        public FieldReferenceDescriptor Reference { get; set; }
        
        /// <summary>
        /// Глубина ссылки
        /// </summary>
        public int RefDepth { get; set; }

        // очень важно хранить пару  - сущность и поле, так как поле может быть получено по наследству от базовой сущности

        /// <summary>
        /// Сущность, владелец дексриптора поля
        /// </summary>
        public IEntityMetadata Owner { get;}
        /// <summary>
        /// Поле владелеца дексриптора поля
        /// </summary>
        public IFieldMetadata OwnerField { get; }

        /// <summary>
        /// Конечная сущность, к которой ведет путь из свойства Path
        /// </summary>
        public IEntityMetadata Entity { get; set; }

        /// <summary>
        /// Конечное поле к которому ведет путь из свойства Path
        /// </summary>
        public IFieldMetadata Field { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            return $"Path = '{this.Path}', OwnerField = '{OwnerField.Name}', Field = '{Field.Name}', Owner = '{Owner.QualifiedName}', Entity = '{Entity.QualifiedName}', IsLocal = {this.IsLocal}, IsRefrence = {this.Reference != null} ";
        }

        /// <summary>
        /// Поле считается локальным, если оно определено врамках сущности а не досталось по наследству
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return this.Owner.QualifiedName == this.OwnerField.Entity.QualifiedName;
            }
        }


        public bool IsReferece
        {
            get
            {
                return this.Reference != null;
            }
        }

        /// <summary>
        /// Функция определеят имя поля в хранилище 
        ///  - для обычного поля
        ///  - для ссылочного поля зависит от наличия мапинга, если нет то имя генерируем 
        ///  исходя из имени локального поля и поля первичного ключа, на который указана ссылка
        /// </summary>
        /// <param name="entity">Сущность которой принадлежит дексрипто</param>
        /// <returns></returns>
        public bool TrySourceName(out string sourceName)
        {
            // имя можно определить только для локального поля
            if (!this.IsLocal)
            {
                sourceName = null;
                return false;
            }

            if (this.OwnerField.SourceType == FieldSourceType.Column)
            {
                sourceName = this.OwnerField.SourceName;
                return true;
            }

            if (this.OwnerField.SourceType == FieldSourceType.Reference && this.RefDepth == 1)
            {
                // this.Field должен быть первичным ключом
                if (!this.Field.IsPrimaryKey())
                {
                    sourceName = null;
                    return false;
                }
                var pkMapping = this.OwnerField.AsReference().Mapping;
                if (pkMapping == null || pkMapping.Fields == null || pkMapping.Fields.ContainsKey(this.Field.Name))
                {
                    sourceName = $"{this.OwnerField.Name}_{this.Field.SourceName}";
                    return true;
                }

                var mapping = pkMapping.Fields[this.Field.Name];
                switch (mapping.MatchWith)
                {
                    case PrimaryKeyMappedMatchWith.Field:
                        sourceName = (mapping as IFieldPrimaryKeyFieldMappedMetadata).EntityField.SourceName;
                        return true;
                    case PrimaryKeyMappedMatchWith.SourceName:
                        sourceName = (mapping as ISourceNamePrimaryKeyFieldMappedMetadata).SourceName;
                        return true;
                    case PrimaryKeyMappedMatchWith.Value:
                    default:
                        break;
                }
            }

            sourceName = null;
            return false;
        }
    }

}
