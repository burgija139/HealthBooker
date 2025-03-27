using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domZdravlja.Models
{
    public class Administrator
    {
        public string KorisnickoIme { get; set; }

        public string Sifra { get; set; }

        public string Ime { get; set; }

        public string Prezime { get; set; }

        public DateTime DatumRodjenja { get; set; }

        public Administrator() { }

        public Administrator(string korisnickoIme, string sifra, string ime, string prezime, DateTime datumRodjenja)
        {
            KorisnickoIme = korisnickoIme;
            Sifra = sifra;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
        }
    }
}