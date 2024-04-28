using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus
{
    public class Laskutus
    {
        public string lasku_id { get; set; }
        public string varaus_id { get; set; }
        public string summa { get; set; }
        public string alv { get; set; }
        public string maksettu { get; set; }

        public Laskutus() { }

        public Laskutus(string lasku_id, string varaus_id, string summa, string alv, string alue, string maksettu)
        {
            this.lasku_id = lasku_id;
            this.varaus_id = varaus_id;
            this.summa = summa;
            this.alv = alv;
            this.maksettu = maksettu;
        }
    }
}
