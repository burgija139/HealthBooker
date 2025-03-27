using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using domZdravlja.Models;

namespace domZdravlja.Controllers
{
    public class HomeController : Controller
    {
        public List<Administrator> Administratori => (List<Administrator>)HttpContext.Application["Administratori"];
        public List<Lekar> Lekari => (List<Lekar>)HttpContext.Application["Lekari"];
        public List<Pacijent> Pacijenti => (List<Pacijent>)HttpContext.Application["Pacijenti"];

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Prijava(string korisnickoIme, string sifra)
        {
            Session["Error"] = null;

            // Proveravamo administratore
            var admin = Administratori?.FirstOrDefault(a => a.KorisnickoIme == korisnickoIme && a.Sifra == sifra);
            if (admin != null)
            {
                Session["KorisnickoIme"] = korisnickoIme;
                Session["TipKorisnika"] = "Administrator";
                return RedirectToAction("Index", "Administrator");
            }

            // Proveravamo lekare
            var lekar = Lekari?.FirstOrDefault(l => l.KorisnickoIme == korisnickoIme && l.Sifra == sifra);
            if (lekar != null)
            {
                Session["KorisnickoIme"] = korisnickoIme;
                Session["TipKorisnika"] = "Lekar";
                return RedirectToAction("Index", "Lekar");
            }

            // Proveravamo pacijente
            var pacijent = Pacijenti?.FirstOrDefault(p => p.KorisnickoIme == korisnickoIme && p.Sifra == sifra);
            if (pacijent != null)
            {
                Session["KorisnickoIme"] = korisnickoIme;
                Session["TipKorisnika"] = "Pacijent";
                return RedirectToAction("Index", "Pacijent");
            }

            // Ako prijava nije uspešna, prikazujemo grešku
            Session["Error"] = "Pogrešno korisničko ime ili šifra!";
            return View("Index");
        }

        public ActionResult Odjava()
        {
            Session["KorisnickoIme"] = null;
            Session["TipKorisnika"] = null;
            Session["Error"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}