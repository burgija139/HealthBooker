using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domZdravlja.Models
{
    public class Lekar
    {
        public string KorisnickoIme { get; set; }

        public string Sifra { get; set; }

        public string Ime { get; set; }

        public string Prezime { get; set; }

        public DateTime DatumRodjenja { get; set; }

        public string Email { get; set; }

        public List<Termin> ZakazaniTermini { get; set; } = new List<Termin>();

        public List<Termin> SlobodniTermini { get; set; } = new List<Termin>();

        public Lekar() { }

        public Lekar(string korisnickoIme, string sifra, string ime, string prezime, DateTime datumRodjenja, string email, List<Termin> zakazaniTermini, List<Termin> slobodniTermini)
        {
            KorisnickoIme = korisnickoIme;
            Sifra = sifra;
            Ime = ime;
            Prezime = prezime;
            DatumRodjenja = datumRodjenja;
            Email = email;
            ZakazaniTermini = zakazaniTermini;
            SlobodniTermini = slobodniTermini;
        }
    }
}