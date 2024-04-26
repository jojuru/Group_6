using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mokkivaraus
{
    public class Alue
    {
        public string alue_id { get; set; }
        public string nimi { get; set; }
    
        public Alue() { }
        public Alue(string alue_id, string nimi)
        {
            this.alue_id = alue_id;
            this.nimi = nimi;
        }
    }
}
