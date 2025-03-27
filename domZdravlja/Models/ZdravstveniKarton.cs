using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domZdravlja.Models
{
    public class ZdravstveniKarton
    {
        public Pacijent Pacijent { get; set; }

        public List<Termin> Termini { get; set; } = new List<Termin>();

        public ZdravstveniKarton() { }

        public ZdravstveniKarton(Pacijent pacijent, List<Termin> termini)
        {
            Pacijent = pacijent;
            Termini = termini;
        }
    }
}