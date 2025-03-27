using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using domZdravlja.Helpers;
using domZdravlja.Models;
using Newtonsoft.Json;

namespace domZdravlja.Controllers
{
    public class PacijentController : Controller
    {
        public List<Pacijent> Pacijenti => (List<Pacijent>)HttpContext.Application["Pacijenti"];
        public List<Termin> Termini => (List<Termin>)HttpContext.Application["Termini"];
        public List<Lekar> Lekari => (List<Lekar>)HttpContext.Application["Lekari"];

        // GET: Pacijent
        public ActionResult Index()
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Pacijent")
            {
                return RedirectToAction("Prijava", "Home");
            }

            var pacijent = Pacijenti?.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme);
            if (pacijent == null)
            {
                return RedirectToAction("Prijava", "Home");
            }

            var slobodniTermini = UcitajSlobodneTermine();
            return View(slobodniTermini);
        }

        private List<Termin> UcitajSlobodneTermine()
        {
            var slobodniTermini = Termini?.Where(t => t.StatusTermina == StatusTermina.Slobodan).ToList();
            return slobodniTermini ?? new List<Termin>();
        }

        public ActionResult ZakaziTermin(int id)
        {
            // Proveravamo da li je korisnik prijavljen i da li je tip korisnika Pacijent
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Pacijent")
            {
                return RedirectToAction("Prijava", "Home");
            }

            // Pronalazak termina po ID-ju i proveravanje da li je slobodan
            var termin = Termini.FirstOrDefault(t => t.Id == id && t.StatusTermina == StatusTermina.Slobodan);

            if (termin == null)
            {
                // Ako termin nije pronađen ili nije slobodan, vraćamo grešku
                return HttpNotFound("Termin nije pronađen ili je već zakazan.");
            }

            // Ažuriramo status termina na "Zakazan"
            termin.StatusTermina = StatusTermina.Zakazan;

            // Dodajemo termin u listu zakazanih termina trenutnog korisnika (Pacijenta)
            var currentUser = Pacijenti.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme);
            if (currentUser != null)
            {
                if (!currentUser.ZakazaniTermini.Contains(termin))
                {
                    currentUser.ZakazaniTermini.Add(termin);
                }

                // Ažuriramo listu pacijenata u Application kontekstu
                HttpContext.Application["Pacijenti"] = Pacijenti;
            }

            // Pronalazak lekara povezanog sa ovim terminom i dodavanje termina u njegovu listu
            var lekar = Lekari.FirstOrDefault(l => l.KorisnickoIme == termin.Lekar.KorisnickoIme);
            if (lekar != null)
            {
                if (!lekar.ZakazaniTermini.Contains(termin))
                {
                    lekar.SlobodniTermini.Remove(termin);
                    lekar.ZakazaniTermini.Add(termin);
                }

                // Ažuriramo listu lekara u Application kontekstu
                HttpContext.Application["Lekari"] = Lekari;
            }

            // Ažuriramo listu termina u Application kontekstu
            HttpContext.Application["Termini"] = Termini;

            // Upisujemo promene u JSON fajlove
            UpisiPacijenteUJson();
            UpisiLekareUJson();
            UpisiTermineUJson();

            // Vraćamo korisnika na stranicu sa listom slobodnih termina
            return RedirectToAction("Index");
        }

        public ActionResult PregledTerapija()
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Pacijent")
            {
                return RedirectToAction("Prijava", "Home");
            }

            var pacijent = Pacijenti?.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme);
            if (pacijent == null)
            {
                return RedirectToAction("Prijava", "Home");
            }

            var zakazaniTermini = pacijent.ZakazaniTermini?.ToList() ?? new List<Termin>();
            return View(zakazaniTermini);
        }

        private void UpisiPacijenteUJson()
        {
            string filePath = Server.MapPath("~/App_Data/pacijenti.json");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(Pacijenti, Formatting.Indented, settings);
            System.IO.File.WriteAllText(filePath, json);
        }

        private void UpisiLekareUJson()
        {
            string filePath = Server.MapPath("~/App_Data/lekari.json");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(Lekari, Formatting.Indented, settings);
            System.IO.File.WriteAllText(filePath, json);
        }

        private void UpisiTermineUJson()
        {
            string filePath = Server.MapPath("~/App_Data/termini.json");
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };
            string json = JsonConvert.SerializeObject(Termini, Formatting.Indented, settings);
            System.IO.File.WriteAllText(filePath, json);
        }
    }
}