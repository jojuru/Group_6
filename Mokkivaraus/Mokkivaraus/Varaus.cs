using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus
{
    public class Varaus
    {
        public string varaus_id { get; set; }
        public string asiakas_id { get; set; }
        public string mokki_id { get; set; }
        public string varattupvm { get; set; }
        public string vahvistuspvm { get; set; }
        public string varattualkupvm { get; set; }
        public string varattuloppupvm { get; set; }

        public Varaus(string varausid, string asiakasid, string mokkiid,
                      string varattupvm, string vahvistuspvm,
                      string varattualkupvm, string varattuloppupvm)
        {
            this.varaus_id = varausid;
            this.asiakas_id = asiakasid;
            this.mokki_id = mokkiid;
            this.varattupvm = varattupvm;
            this.vahvistuspvm = vahvistuspvm;
            this.varattualkupvm = varattualkupvm;
            this.varattuloppupvm = varattuloppupvm;
        }

        public Varaus() { }
    }

}
