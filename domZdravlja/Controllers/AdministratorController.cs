using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using domZdravlja.Models;
using Newtonsoft.Json;

namespace domZdravlja.Controllers
{
    public class AdministratorController : Controller
    {
        public List<Pacijent> Pacijenti => (List<Pacijent>)HttpContext.Application["Pacijenti"];
        public List<Administrator> Administratori => (List<Administrator>)HttpContext.Application["Administratori"];

        // GET: Administrator
        public ActionResult Index()
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Administrator")
            {
                return RedirectToAction("Prijava", "Home");
            }

            var pacijenti = Pacijenti;
            return View(pacijenti);
        }

        // GET: IzmeniPacijenta
        public ActionResult IzmeniPacijenta(string korisnickoIme)
        {
            var pacijent = Pacijenti?.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme);
            if (pacijent == null)
            {
                return HttpNotFound("Pacijent nije pronađen.");
            }

            return View(pacijent);
        }

        // POST: Azuriranje
        /*[HttpPost]
        public ActionResult Azuriranje(string korisnickoIme, string jmbg, string sifra, string ime, string prezime, DateTime datumRodjenja, string email)
        {
            // Pronađi pacijenta koji treba da se ažurira
            var existingPatient = Pacijenti?.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme);

            // Ako pacijent postoji, ažuriraj njegove podatke
            if (existingPatient != null)
            {
                existingPatient.JMBG = jmbg;
                existingPatient.Sifra = sifra;
                existingPatient.Ime = ime;
                existingPatient.Prezime = prezime;
                existingPatient.DatumRodjenja = datumRodjenja;
                existingPatient.Email = email;

                // Ažuriraj listu pacijenata u Application state
                HttpContext.Application["Pacijenti"] = Pacijenti;

                // Upis u pacijenti.json fajl nakon izmene
                UpisiPacijenteUJson();

                // Preusmerite na stranicu sa listom pacijenata
                return RedirectToAction("Index");
            }

            // Ako pacijent nije pronađen, prikazati grešku
            return HttpNotFound("Pacijent nije pronađen.");
        }*/

        [HttpPost]
        public ActionResult Azuriranje(string korisnickoIme, string jmbg, string sifra, string ime, string prezime, DateTime datumRodjenja, string email)
        {
            List<string> greske = new List<string>();

            // Pronađi pacijenta koji treba da se ažurira
            var existingPatient = Pacijenti?.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme);

            // Ako pacijent postoji
            if (existingPatient != null)
            {
                // Proveri da li je e-mail adresa jedinstvena
                bool emailExists = Pacijenti.Any(p => p.Email == email && p.KorisnickoIme != korisnickoIme);
                if (emailExists)
                {
                    greske.Add("E-mail adresa je već u upotrebi.");
                }

                if (greske.Any())
                {
                    // Sačuvaj greške u session i vrati korisnika na formu
                    Session["Error"] = greske;
                    return View("IzmeniPacijenta", existingPatient);
                }

                // Ažuriraj podatke pacijenta
                existingPatient.Sifra = sifra;
                existingPatient.Ime = ime;
                existingPatient.Prezime = prezime;
                existingPatient.DatumRodjenja = datumRodjenja;
                existingPatient.Email = email;

                // Ažuriraj listu pacijenata u Application state
                HttpContext.Application["Pacijenti"] = Pacijenti;

                // Upis u pacijenti.json fajl nakon izmene
                UpisiPacijenteUJson();

                // Preusmerite na stranicu sa listom pacijenata
                return RedirectToAction("Index");
            }

            // Ako pacijent nije pronađen, prikazati grešku
            return HttpNotFound("Pacijent nije pronađen.");
        }

        // GET: DodajPacijenta
        public ActionResult DodajPacijenta()
        {
            return View();
        }

        // POST: DodajPacijenta
        [HttpPost]
        public ActionResult Dodavanje(string korisnickoIme, string jmbg, string sifra, string ime, string prezime, DateTime datumRodjenja, string email)
        {
            List<string> greske;

            // Validiraj pacijenta
            if (!ValidirajPacijenta(korisnickoIme, jmbg, email, out greske))
            {
                Session["Error"] = greske; // Čuvanje grešaka u sesiji
                return View("DodajPacijenta"); // Vratite se na formu za dodavanje pacijenta
            }

            // Kreirajte novi pacijent
            var noviPacijent = new Pacijent
            {
                KorisnickoIme = korisnickoIme,
                JMBG = jmbg,
                Sifra = sifra,
                Ime = ime,
                Prezime = prezime,
                DatumRodjenja = datumRodjenja,
                Email = email
            };

            // Dodajte pacijenta u listu
            Pacijenti.Add(noviPacijent);
            HttpContext.Application["Pacijenti"] = Pacijenti;

            // Upis u pacijenti.json fajl
            UpisiPacijenteUJson();

            // Preusmerite na stranicu sa listom pacijenata
            return RedirectToAction("Index");
        }

        // POST: BrisiPacijenta
        [HttpPost]
        public ActionResult BrisiPacijenta(string korisnickoIme)
        {
            var pacijent = Pacijenti?.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme);
            if (pacijent != null)
            {
                Pacijenti.Remove(pacijent);
                HttpContext.Application["Pacijenti"] = Pacijenti;

                // Upis u pacijenti.json fajl nakon brisanja
                UpisiPacijenteUJson();

                return RedirectToAction("Index");
            }
            return HttpNotFound("Pacijent nije pronađen.");
        }

        public void UpisiPacijenteUJson()
        {
            string filePath = Server.MapPath("~/App_Data/pacijenti.json");
            string json = JsonConvert.SerializeObject(Pacijenti, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
        }

        private bool ValidirajPacijenta(string korisnickoIme, string jmbg, string email, out List<string> greske)
        {
            greske = new List<string>();

            // Proveri da li pacijent već postoji na osnovu korisničkog imena
            if (Pacijenti.Any(p => p.KorisnickoIme == korisnickoIme))
            {
                greske.Add("Pacijent sa ovim korisničkim imenom već postoji.");
            }

            // Proveri da li pacijent već postoji na osnovu JMBG-a
            if (Pacijenti.Any(p => p.JMBG == jmbg))
            {
                greske.Add("Pacijent sa ovim JMBG-om već postoji.");
            }

            // Proveri da li pacijent već postoji na osnovu e-pošte
            if (Pacijenti.Any(p => p.Email == email))
            {
                greske.Add("Pacijent sa ovom e-poštom već postoji.");
            }

            // Validacija JMBG-a (13 numeričkih karaktera)
            if (jmbg.Length != 13 || !jmbg.All(char.IsDigit))
            {
                greske.Add("JMBG mora biti tačno 13 numeričkih karaktera.");
            }

            // Validacija formata e-pošte
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    greske.Add("Neispravan format e-pošte.");
                }
            }
            catch
            {
                greske.Add("Neispravan format e-pošte.");
            }

            return !greske.Any();
        }
    }
}