using System;
using System.Text;
using System.Collections.Generic;

namespace Atdi.AppUnits.Icsm.CoverageEstimation.Localization
{
    public class CLocaliz
    {
        static Localization valLocal = null;
        static CLocaliz()
        {
            valLocal = new Localization(Environment.CurrentDirectory, "WebQueryTranslate");
        }

        //===================================================
        // Переводим фразу
        //===================================================
        public static string TxT(string trunsl)
        {
            if (valLocal != null)
                return valLocal.Translate(trunsl);
            return trunsl;
        }
        //===================================================
    }
}
