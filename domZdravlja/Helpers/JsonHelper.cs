using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using domZdravlja.Models;
using Newtonsoft.Json;

namespace domZdravlja.Helpers
{
    public static class JsonHelper
    {
        public static List<Pacijent> ReadPacijentiFromJson(string path)
        {
            string jsonString = File.ReadAllText(path);
            List<Pacijent> pacijenti = JsonConvert.DeserializeObject<List<Pacijent>>(jsonString);
            return pacijenti;
        }

        public static List<Termin> ReadTerminiFromJson(string path)
        {
            string jsonString = File.ReadAllText(path);
            List<Termin> termini = JsonConvert.DeserializeObject<List<Termin>>(jsonString);
            return termini;
        }

        public static List<Lekar> ReadLekariFromJson(string path)
        {
            string jsonString = File.ReadAllText(path);
            List<Lekar> lekari = JsonConvert.DeserializeObject<List<Lekar>>(jsonString);
            return lekari;
        }

        public static List<Administrator> ReadAdministratoriFromJson(string path)
        {
            string jsonString = File.ReadAllText(path);
            List<Administrator> administratori = JsonConvert.DeserializeObject<List<Administrator>>(jsonString);
            return administratori;
        }

        public static List<ZdravstveniKarton> ReadZdravstveniKartoniFromJson(string path)
        {
            string jsonString = File.ReadAllText(path);
            List<ZdravstveniKarton> kartoni = JsonConvert.DeserializeObject<List<ZdravstveniKarton>>(jsonString);
            return kartoni;
        }
    }
}