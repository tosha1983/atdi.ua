using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.LegacyServices.Icsm
{

    public  class Semant
    {

        public typSemant m_type;
        public string m_sym;
        public double m_div;
        public double m_min, m_max;
        public string mName; 

        public Semant(string name, typSemant t)
        {
            mName = name;
            m_type = t; m_div = 0.0; m_min = m_max = 1e-99; m_sym = null;
            Dico[name] = this;
        }

        static Dictionary<string, Semant> Dico = new Dictionary<string, Semant>();
        public enum Tlcomb
        {
            NORMAL = 0, UPPERCASE = 1,
            EXTREM = 2 //Add comma at extremities 
        }

        static public Semant Get(string specname)
        {
            if (specname.IsNull()) return null;
            Semant res = null;
            if (Dico.TryGetValue(specname, out res)) return res;
            if (specname.StartsWith("eri_"))
            {
                res = new Semant(specname, typSemant.tCombo);
                res.m_sym = specname.Substring(4); res.m_max = 400; res.m_div = 1;
            }
            else if (specname.StartsWith("lov_"))
            {
                res = new Semant(specname, typSemant.tComboUser);
                res.m_sym = specname.Substring(4); res.m_max = 400; res.m_div = 1;
            }
            else if (specname.StartsWith("stat_"))
            {
                res = new Semant(specname, typSemant.tComboWrkf);
                res.m_sym = specname.Substring(5); res.m_max = 400;
            }
            else if (specname.StartsWith("fk_"))
            {
                res = new Semant(specname, typSemant.tForeignId);
                res.m_sym = specname.Substring(3);
            }
            else if (specname.StartsWith("list_eri_"))
            {
                res = new Semant(specname, typSemant.tListCombo);
                res.m_sym = specname.Substring(9); res.m_div = (int)Tlcomb.NORMAL;
            }
            else if (specname.StartsWith("list_lov_"))
            {
                res = new Semant(specname, typSemant.tListComboUser);
                res.m_sym = specname.Substring(9); res.m_div = (int)Tlcomb.NORMAL;
            }
            else if (specname.StartsWith("String("))
            {
                res = new Semant(specname, typSemant.tStri);
                res.m_max = int.Parse(specname.Substring(7, specname.Length - 8));
            }
            else if (specname.StartsWith("Integer("))
            {
                res = new Semant(specname, typSemant.tInteger);
                int i = specname[8] - '0'; double mm = Math.Pow(10, i) - 1; res.m_max = mm - 1; res.m_min = -mm;
            }
            else if (specname == "Folder") { res = new Semant(specname, typSemant.tFolder); res.m_max = 256; }
            return res;
        }
    }
}
