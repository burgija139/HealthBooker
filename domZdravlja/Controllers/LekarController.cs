using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using domZdravlja.Models;
using Newtonsoft.Json;

namespace domZdravlja.Controllers
{
    public class LekarController : Controller
    {
        public List<Lekar> Lekari => (List<Lekar>)HttpContext.Application["Lekari"];
        public List<Pacijent> Pacijenti => (List<Pacijent>)HttpContext.Application["Pacijenti"];
        public List<Termin> Termini => (List<Termin>)HttpContext.Application["Termini"];

        // GET: Lekar
        public ActionResult Index()
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Lekar")
            {
                return RedirectToAction("Prijava", "Home");
            }

            var lekar = Lekari.FirstOrDefault(l => l.KorisnickoIme == korisnickoIme);
            if (lekar != null)
            {
                var sviTermini = lekar.ZakazaniTermini.Concat(lekar.SlobodniTermini).ToList();
                return View(sviTermini);
            }

            return View(new List<Termin>());
        }

        // GET: Lekar/KreirajTermin
        public ActionResult KreirajTermin()
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Lekar")
            {
                return RedirectToAction("Prijava", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult DodajTermin(DateTime datumZakazanogTermina, string opisTerapije)
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Lekar")
            {
                return RedirectToAction("Prijava", "Home");
            }

            // Pronađi lekara koji je prijavljen
            var lekar = Lekari.FirstOrDefault(l => l.KorisnickoIme == korisnickoIme);
            if (lekar == null)
            {
                return HttpNotFound("Lekar nije pronađen.");
            }

            // Kreiraj novi termin
            var noviTermin = new Termin
            {
                Id = new Random().Next(1000, 9999),
                DatumZakazanogTermina = datumZakazanogTermina,
                StatusTermina = StatusTermina.Slobodan,
                OpisTerapije = opisTerapije,
                Lekar = lekar
            };

            // Dodaj novi termin u listu slobodnih termina za trenutnog lekara
            lekar.SlobodniTermini.Add(noviTermin);

            // Dodaj novi termin u globalnu listu Termini
            Termini.Add(noviTermin);

            // Ažuriraj Application state
            HttpContext.Application["Lekari"] = Lekari;
            HttpContext.Application["Termini"] = Termini;

            // Sačuvaj izmenjene liste u JSON fajlove
            UpisiTermineUJson();
            UpisiLekareUJson();

            return RedirectToAction("Index");
        }

        public ActionResult PrepisivanjeTerapije()
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Lekar")
            {
                return RedirectToAction("Prijava", "Home");
            }

            var lekar = Lekari.FirstOrDefault(l => l.KorisnickoIme == korisnickoIme);
            if (lekar == null)
            {
                return HttpNotFound("Lekar nije pronađen.");
            }

            ViewBag.ZakazaniTermini = lekar.ZakazaniTermini
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.DatumZakazanogTermina.ToString("dd.MM.yyyy HH:mm")
                })
                .ToList();

            return View();
        }

        [HttpPost]
        public ActionResult PrepisivanjeTerapije(int terminId, string opisTerapije)
        {
            var korisnickoIme = Session["KorisnickoIme"] as string;
            var tipKorisnika = Session["TipKorisnika"] as string;

            if (string.IsNullOrEmpty(korisnickoIme) || tipKorisnika != "Lekar")
            {
                return RedirectToAction("Prijava", "Home");
            }

            // Pronađi lekara koji je prijavljen
            var lekar = Lekari.FirstOrDefault(l => l.KorisnickoIme == korisnickoIme);
            if (lekar == null)
            {
                return HttpNotFound("Lekar nije pronađen.");
            }

            // Pronađi zakazani termin kod lekara
            var termin = lekar.ZakazaniTermini.FirstOrDefault(t => t.Id == terminId);
            if (termin == null)
            {
                return HttpNotFound("Termin nije pronađen.");
            }

            // Proveri da li terapija već postoji
            if (!string.IsNullOrEmpty(termin.OpisTerapije))
            {
                return RedirectToAction("Index");
            }

            // Ažuriraj opis terapije za termin kod lekara
            termin.OpisTerapije = opisTerapije;

            // Pronađi pacijenta kome je termin zakazan
            var pacijent = Pacijenti.FirstOrDefault(p => p.ZakazaniTermini.Any(t => t.Id == terminId));
            if (pacijent != null)
            {
                // Pronađi odgovarajući termin kod pacijenta i ažuriraj terapiju
                var pacijentovTermin = pacijent.ZakazaniTermini.FirstOrDefault(t => t.Id == terminId);
                if (pacijentovTermin != null)
                {
                    pacijentovTermin.OpisTerapije = opisTerapije;
                }
            }

            // Ažuriraj application state za lekare i pacijente
            HttpContext.Application["Lekari"] = Lekari;
            HttpContext.Application["Pacijenti"] = Pacijenti;

            // Sačuvaj izmenjene liste u JSON fajlove
            UpisiTermineUJson();
            UpisiLekareUJson();
            UpisiPacijenteUJson();

            return RedirectToAction("Index");
        }


        public void UpisiLekareUJson()
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

        public void UpisiTermineUJson()
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

        public void UpisiPacijenteUJson()
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
    }
}

