using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels;
using Atdi.DataModels.WebQuery;

namespace Atdi.AppServices.WebQuery
{
    internal sealed class QueryDescriptor
    {
        public QueryToken Token { get; set; }
       
        /// <summary>
        /// Класс для хранения свойств запроса
        /// </summary>
        public class SettingIRPClass
        {
            /// <summary>
            /// ID запроса
            /// </summary>
            public int ID { get; set; }
            /// <summary>
            /// Наименование запроса
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Категория запроса - стандартная или пользовательская
            /// </summary>
            public AccessCategory STATUS_ { get; set; }
            /// <summary>
            /// Код запроса (в виде структури IRP или чистый SQL)
            /// </summary>
            public string Query { get; set; }
            /// <summary>
            /// Комментарии к запросу
            /// </summary>
            public string Comments { get; set; }
            /// <summary>
            /// Идентификатор поля, которое содержит номер ID пользователя (для фильтрации данных конкретного пользователя)
            /// </summary>
            public string Ident_User { get; set; }
            /// <summary>
            /// Наименование группы прав taskforce
            /// </summary>
            public string Right_Group_Name { get; set; }
            /// <summary>
            /// Расширенные права, указывающие на ограничение в виде только просмотр или полный доступ к данным 
            /// </summary>
            public ExtendedControlRight ControlRight { get; set; }
            /// <summary>
            /// Признак - является ли данный запрос чистым SQL или это IRP
            /// </summary>
            public bool Is_Sql_Request { get; set; }
            /// <summary>
            /// Описание запроса
            /// </summary>
            public string Description { get; set; }
            /// <summary>
            /// Права пользователя - чтение, модификация, удаление
            /// </summary>
            public UserRights userRights { get; set; }
            /// <summary>
            /// Набор ограничений
            /// </summary>
            public Constraints[] SettingConstraint { get; set; }
            /// <summary>
            /// Основная таблица запроса
            /// </summary>
            public string TableName { get; set; }
        }

        /// <summary>
        /// Категории запроса
        /// </summary>
        public enum AccessCategory
        {
            /// <summary>
            /// стандартная 
            /// </summary>
            STD,
            /// <summary>
            /// Пользовательская
            /// </summary>
            CUS,
        }

        /// <summary>
        /// Права на доступ к данным 
        /// </summary>
        public enum ExtendedControlRight
        {
            /// <summary>
            /// только чтение
            /// </summary>
            OnlyView,
            /// <summary>
            /// полный доступ
            /// </summary>
            FullRight
        }

        /// <summary>
        /// Перечень прав, определяющих список возможных действий с данными (чтение, модификация, удаление)
        /// </summary>
        public class UserRights
        {
            /// <summary>
            /// право на вставку
            /// </summary>
            public bool Insert;
            /// <summary>
            /// на модификацию
            /// </summary>
            public bool Update;
            /// <summary>
            /// на удаление
            /// </summary>
            public bool Delete;
            /// <summary>
            /// На чтение
            /// </summary>
            public bool Read;
        }

        /// <summary>
        /// Ограничение, которое накладывается на отдельное поле запроса
        /// </summary>
        public class Constraints
        {
            /// <summary>
            /// Идентификатор ограничения в таблице
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// Ccылка на запрос 
            /// </summary>
            public int Query_ID { get; set; }
            /// <summary>
            /// Наименование ограничения
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Наименование поля, на которое накладывается ограничение
            /// </summary>
            public string Path { get; set; }
            /// <summary>
            /// диапазон чисел от (Min)
            /// </summary>
            public double Min { get; set; }
            /// <summary>
            /// диапазон чисел до (Max)
            /// </summary>
            public double Max { get; set; }
            /// <summary>
            /// Строка 
            /// </summary>
            public string Str_Value { get; set; }
            /// <summary>
            /// диапазон дат от (Date_Value_Min)
            /// </summary>
            public DateTime Date_Value_Min { get; set; }
            /// <summary>
            /// диапазон дат до (Date_Value_Max)
            /// </summary>
            public DateTime Date_Value_Max { get; set; }
            /// <summary>
            /// призак прямого или инверсного включения
            /// т.е. если  Include = true - это эквивалентно  (column_name BETWEEN value1 AND value2)
            /// если  Include = false - это эквивалентно  (column_name NOT BETWEEN value1 AND value2)
            /// </summary>
            public bool Include { get; set; }

        }

    }
}
