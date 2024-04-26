using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus
{
    public class Asiakas
    {
        public string asiakas_id { get; set; }
        public string postinro { get; set; }
        public string etunimi { get; set; }
        public string sukunimi { get; set; }
        public string lahiosoite { get; set; }
        public string email { get; set; }
        public string puhelinnro { get; set; }

        public Asiakas() { }
        public Asiakas(string asiakas_id, string postinro, string etunimi, 
            string sukunimi, string lahiosoite, string email, string puhelinnro)
        {
            this.asiakas_id = asiakas_id;
            this.postinro = postinro;
            this.etunimi = etunimi;
            this.sukunimi = sukunimi;
            this.lahiosoite = lahiosoite;
            this.email = email;
            this.puhelinnro = puhelinnro;
        }
    }
}
