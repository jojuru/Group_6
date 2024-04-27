using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus
{
    public class Mokki
    {
        public string mokki_id { get; set; }
        public string alue_id { get; set; }
        public string postinro { get; set; }
        public string mokkinimi { get; set; }
        public string katuosoite { get; set; }
        public string hinta { get; set; }
        public string kuvaus { get; set; }
        public string henkilomaara { get; set; }
        public string varustelu { get; set; }
        public string alue { get; set; }

        public Mokki() { }
        public Mokki(string mokki_id, string alue_id, string postinro, string mokkinimi,
        string katuosoite, string hinta, string kuvaus, string henkilomaara, string varustelu, string alue)
        {
            this.mokki_id = mokki_id;
            this.alue_id = alue_id;
            this.postinro = postinro;
            this.mokkinimi = mokkinimi;
            this.katuosoite = katuosoite;
            this.hinta = hinta;
            this.kuvaus = kuvaus;
            this.henkilomaara = henkilomaara;
            this.varustelu = varustelu;
            this.alue = alue;
        }
    }
}
