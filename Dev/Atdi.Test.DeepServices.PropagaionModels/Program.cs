using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.DeepServices.PropagaionModels
{
    class Program
    {
        static void Main(string[] args)
        {
            land_sea ls = new land_sea();
            ls.land = 10;
            ls.sea = 0;
            land_sea ls1 = new land_sea();
            ls1.land = 10;
            ls1.sea = 0;
            double e_dBuVm = 0;
            double ha = 100;
            double hef = 100;
            double d = 4;
            double f = 999;
            double p = 10;
            double h_gr = 100;
            double h2 = 10;
            land_sea[] list1 = { ls, ls1 };

            e_dBuVm = _1546_4.Get_E(ha, hef, d, f, p, h_gr, h2, list1);

        }
    }
}
