using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus
{
    public class PalveluRaportti
    {
        public string palvelun_nimi { get; set; }
        public int varaus_maara { get; set; }
        public string alue {  get; set; }

        public PalveluRaportti() { }

        public PalveluRaportti(string palvelun_nimi, int varaus_maara, string alue)
        {
            this.palvelun_nimi = palvelun_nimi;
            this.varaus_maara = varaus_maara;
            this.alue = alue;
        }
    }
}
