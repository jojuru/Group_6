using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus
{
    public class Palvelu
    {
        public string palvelu_id { get; set; }
        public string alue_id { get; set; }
        public string nimi { get; set; }
        public string kuvaus { get; set; }
        public string hinta { get; set; }
        public string alv { get; set; }
        public string alue { get; set; }

        public Palvelu() { }

        public Palvelu(string palvelu_id, string alue_id, string nimi, string kuvaus,
                       string hinta, string alv, string alue)
        {
            this.palvelu_id = palvelu_id;
            this.alue_id = alue_id;
            this.nimi = nimi;
            this.kuvaus = kuvaus;
            this.hinta = hinta;
            this.alv = alv;
            this.alue = alue;
        }
    }
}
