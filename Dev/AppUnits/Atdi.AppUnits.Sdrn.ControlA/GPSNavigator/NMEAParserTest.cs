
using System.Collections.Generic;
using System.Text;
namespace NMEA
{
    public class NMEAParserTest
    {
        #region Properties

        List<string> output;

        #endregion

        #region Constructor

        public NMEAParserTest()
        {
            output = new List<string>();
        }

        #endregion

        #region Methods

        public void ProcessMessage(string source)
        {
            //NMEAParser.Parse(source);
        }

        private string GetParametersAsStrings(object[] parameters)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parameters.Length; i++)
            {
                sb.Append(parameters[i].ToString());
                sb.Append(" (");
                sb.Append(parameters.GetType().Name);
                sb.Append(")");

                if (i != parameters.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        public List<string> GetOutput()
        {
            return output;
        }

        #endregion

        #region Handlers        

        private void sentenceParsed(TalkerIdentifiers talkerID, SentenceIdentifiers sentenseID, object[] parameters)
        {
            output.Add(string.Format("talker: {0}, sentense: {1}, parameters: {2}", talkerID, sentenseID, GetParametersAsStrings(parameters)));
        }

        private void proprietarySentenseParsed(ManufacturerCodes manufacturer, string senteseIDString, object[] parameters)
        {
            output.Add(string.Format("Manufacturer: {0}, sentense: {1}, parameters: {2}", manufacturer, senteseIDString, GetParametersAsStrings(parameters)));
        }

        private void parserException(string message)
        {
            output.Add(message);
        }

        private void UnknownProprietarySentense(ManufacturerCodes manufacturer, string senteseIDString, string[] parameters)
        {
            output.Add(string.Format("Unknown proprietary sequence, manufacturer: {0}, sentenseID: {1}, parameters: {3}", manufacturer, senteseIDString, GetParametersAsStrings(parameters)));
        }

        #endregion

    }
}
