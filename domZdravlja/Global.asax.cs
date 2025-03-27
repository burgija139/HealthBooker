using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using domZdravlja.Helpers;
using domZdravlja.Models;

namespace domZdravlja
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Učitavanje podataka iz JSON fajlova
            string pacijentiPath = Server.MapPath("~/App_Data/pacijenti.json");
            List<Pacijent> pacijenti = JsonHelper.ReadPacijentiFromJson(pacijentiPath);
            HttpContext.Current.Application["Pacijenti"] = pacijenti;

            string terminiPath = Server.MapPath("~/App_Data/termini.json");
            List<Termin> termini = JsonHelper.ReadTerminiFromJson(terminiPath);
            HttpContext.Current.Application["Termini"] = termini;

            string lekariPath = Server.MapPath("~/App_Data/lekari.json");
            List<Lekar> lekari = JsonHelper.ReadLekariFromJson(lekariPath);
            HttpContext.Current.Application["Lekari"] = lekari;

            string administratoriPath = Server.MapPath("~/App_Data/administratori.json");
            List<Administrator> administratori = JsonHelper.ReadAdministratoriFromJson(administratoriPath);
            HttpContext.Current.Application["Administratori"] = administratori;

            string kartoniPath = Server.MapPath("~/App_Data/zdravstveniKartoni.json");
            List<ZdravstveniKarton> kartoni = JsonHelper.ReadZdravstveniKartoniFromJson(kartoniPath);
            HttpContext.Current.Application["Kartoni"] = kartoni;
        }
    }
}
