using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.DB
{
    public class localatdi_result_state_data : PropertyChangedBase
    {
        /// <summary>
        /// -1 = ничего неясно / небывает
        /// 0 = неотправил
        /// 1 = отправил
        /// 2 = подтверждено
        /// 3 = недождался (по идее небывает такого)
        /// 4 = жду до таймаута (по идее небывает такого) 
        /// 5 = принято с ошибкой
        /// </summary>
        [PgName("delivery_confirmation")]
        public int DeliveryConfirmation
        {
            get { return _DeliveryConfirmation; }
            set { _DeliveryConfirmation = value; OnPropertyChanged("delivery_confirmation"); }
        }
        private int _DeliveryConfirmation = -1;

        /// <summary>
        /// есть изменения в тру и сохранить 
        /// </summary>
        [PgName("saved_in_db")]
        public bool SaveInDB
        {
            get { return _SaveInDB; }
            set { _SaveInDB = value; /*OnPropertyChanged("SaveInDB"); */}
        }
        private bool _SaveInDB = false;

        [PgName("result_id")]
        public string ResultId
        {
            get { return _ResultId; }
            set { _ResultId = value; OnPropertyChanged("ResultId"); }
        }
        private string _ResultId = "";

        /// <summary>
        /// время отправки даже если неудачной
        /// </summary>
        [PgName("result_sended")]
        public DateTime ResultSended
        {
            get { return _ResultSended; }
            set { _ResultSended = value; OnPropertyChanged("ResultSended"); }
        }
        private DateTime _ResultSended = DateTime.MinValue;

        [PgName("response_received")]
        public DateTime ResponseReceived
        {
            get { return _ResponseReceived; }
            set { _ResponseReceived = value; OnPropertyChanged("ResponseReceived"); }
        }
        private DateTime _ResponseReceived = DateTime.MinValue;



        [PgName("error_text")]
        public string ErrorText
        {
            get { return _ErrorText; }
            set { _ErrorText = value; OnPropertyChanged("ErrorText"); }
        }
        private string _ErrorText = "";
    }
}
