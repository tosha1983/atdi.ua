using Atdi.Contracts.Api.Sdrn.MessageBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DM = Atdi.DataModels.Sdrns.Device;

namespace Atdi.Test.Api.Sdrn.Device.BusControllerAPI2_0
{
    class SendCommandHandler : MessageHandlerBase<DM.DeviceCommand>
    {
        private readonly IBusGate _gate;

        public SendCommandHandler(IBusGate gate)
            : base("SendCommand")
        {
            this._gate = gate;
        }

        public override void OnHandle(IReceivedMessage<DM.DeviceCommand> message)
        {
            string input = message.Data.CustTxt1;
            string Status = "", ResultId = "", Message = "";
            DateTime Created = DateTime.MinValue;
            if (input.Length > 10 && input.StartsWith("{") && input.EndsWith("}"))
            {
                string[] arr = input.Substring(1, input.Length - 2).Split(',');

                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].StartsWith(" ")) { arr[i] = arr[i].TrimStart().TrimEnd(); }
                    if (arr[i].ToLower().StartsWith("\"status\"")) { int start = arr[i].IndexOf(": \"") + 3; Status = arr[i].Substring(start, arr[i].Length - start - 1); }
                    else if (arr[i].ToLower().StartsWith("\"resultid\"")) { int start = arr[i].IndexOf(": \"") + 3; ResultId = arr[i].Substring(start, arr[i].Length - start - 1); }
                    else if (arr[i].ToLower().StartsWith("\"message\"")) { int start = arr[i].IndexOf(": \"") + 3; Message = arr[i].Substring(start, arr[i].Length - start - 1); }
                    else if (arr[i].ToLower().StartsWith("\"datecreated\"")) { int start = arr[i].IndexOf(": \"") + 3; Created = DateTime.ParseExact(arr[i].Substring(start, arr[i].Length - start - 1), "dd.MM.yyyyTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); }
                }
            }
            if (message.Data.Command == "SendMeasResultsConfirmed")
            { ControlU.MainWindow.db_v2.ATDI_FindUpdateResultInfoToDB_v2(Status, ResultId, Message, Created); }
            /*
            На текущий момент определены такие команды:
            SendMeasResultsConfirmed - команда подтверждения успешного принятия результатов на стороне сервиса SDRN
            SendEntityPartResult - команада подтверждения успешного принятия объекта EntityPart на стороне сервиса SDRN
            SendEntityResult - команада подтверждения успешного принятия объета Entity на стороне сервиса SDRN
            */



            

            //"{status:"Ok", ResultId:"", Message:"Some textjadh dhjdh"}"
            //Success/ Fail
            //Debug.WriteLine($"Recieved command '{message.}'");
            if ((message.Data.Command == "SendMeasResultsConfirmed") ||
                (message.Data.Command == "SendEntityPartResult") ||
                (message.Data.Command == "SendEntityResult"))
            {
                 message.Result = MessageHandlingResult.Confirmed;
            }
            else
            {
                message.Result = MessageHandlingResult.Reject;
            }
        }

    }
}
