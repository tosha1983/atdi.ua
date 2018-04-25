using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using OrmCs;
using FormsCs;
using System.Text;

namespace Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities
{
    public class Class_RulesExecQuery
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class WebAddParams
    {

        public int ID { get; set; }
        public int WEB_QUERY_ID { get; set; }
        public string NAME { get; set; }
        public int TYPE_COMP { get; set; }
        public string DOMAIN { get; set; }
        public string ERI_FILE_NAME { get; set; }
        public string ERI_CONTENT { get; set; }
        public WebAddParams()
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~WebAddParams()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class WebDelegatedStations
    {

        public int ID { get; set; }
        public string TABLE_NAME { get; set; }
        public int RECORD_ID { get; set; }
        public int USER_ID { get; set; }
        public int QUERY_ID { get; set; }
        public int IS_ACTIVATE { get; set; }


        public WebDelegatedStations()
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~WebDelegatedStations()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class XWeb_Del
    {

        public int ID { get; set; }
        public string TABLE_NAME { get; set; }
        public int RECORD_ID { get; set; }


        public XWeb_Del()
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~XWeb_Del()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class WebConsultation
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public TypeStatus STATUS_ { get; set; }
        public int WEB_QUERY_CUR_ID { get; set; }
        public int WEB_QUERY_EXP_ID { get; set; }



        public WebConsultation()
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~WebConsultation()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebConstraint
    {
        public int ID { get; set; }
        public int QUERY_ID { get; set; }
        public string NAME { get; set; }
        public string PATH { get; set; }
        public double MIN { get; set; }
        public double MAX { get; set; }
        public string STR_VALUE { get; set; }
        public DateTime DATE_VALUE { get; set; }
        public DateTime DATE_VALUE_MAX { get; set; }
        public bool INCLUDE { get; set; }


        public WebConstraint()
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~WebConstraint()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebConsultationQuestion
    {
        public int ID { get; set; }
        public TypeStatus STATUS_ { get; set; }
        public int CONSULTATION_LINK_ID { get; set; }
        public DateTime DATE_QUESTION { get; set; }
        public string AUTHOR_QUESTION { get; set; }
        public string QUESTION { get; set; }
        public DateTime DATE_ANSWER { get; set; }
        public string AUTHOR_ANSWER { get; set; }
        public string ANSWER { get; set; }
        public int USER_ID { get; set; }


        public WebConsultationQuestion()
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~WebConsultationQuestion()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WebConsultationLink
    {
        public int ID { get; set; }
        public TypeStatus STATUS_ { get; set; }
        public int WEB_CONSULTATION_ID { get; set; }
        public int RECORD_ID { get; set; }
        public string RECORD_TABLE { get; set; }
        public DateTime DATA_START_CONSULTATION { get; set; }
        public DateTime DATA_END_CONSULTATION { get; set; }


        public WebConsultationLink()
        {

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~WebConsultationLink()
        {
            Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}