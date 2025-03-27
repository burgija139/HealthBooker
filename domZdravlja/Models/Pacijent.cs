using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domZdravlja.Models
{
    public class Pacijent
    {
        public string KorisnickoIme { get; set; }

        public string JMBG { get; set; }

        public string Sifra { get; set; }

        public string Ime { get; set; }

        public string Prezime { get; set; }

        public DateTime DatumRodjenja { get; set; }

        public string Email { get; set; }

        public List<Termin> ZakazaniTermini { get; set; } = new List<Termin>();

        public Pacijent() { }

        public Pacijent(string korisnickoIme, string jMBG, string sifra, string ime, string prezime, DateTime datumRodjenja, string email, List<Termin> zakazaniTermini)
        {
            KorisnickoIme = korisnickoIme;
            JMBG = jMBG;
            Sifra = sifra;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
            Email = email;
            ZakazaniTermini = zakazaniTermini;
        }
    }
}