using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domZdravlja.Models
{
    public class Termin
    {
        public int Id { get; set; }

        public Lekar Lekar { get; set; }

        public StatusTermina StatusTermina { get; set; }

        public DateTime DatumZakazanogTermina { get; set; }

        public string OpisTerapije { get; set; }

        public Termin() { }

        public Termin(Lekar lekar, StatusTermina statusTermina, DateTime datumZakazanogTermina, string opisTerapije)
        {
            Lekar = lekar;
            StatusTermina = statusTermina;
            DatumZakazanogTermina = datumZakazanogTermina;
            OpisTerapije = opisTerapije;
        }
    }
}