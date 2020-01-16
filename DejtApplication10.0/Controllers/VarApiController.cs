using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Description;
using DejtApplication10._0.Models;

namespace DejtApplication10._0.Controllers
{
    [RoutePrefix("api/VarApi")]
    public class VarApiController : ApiController
    {
        private AnvändareDbContext db = new AnvändareDbContext();

        
        [HttpPost]
        [ActionName("PostEditDescription")]
        public void PostEditDescription(string[] idn)
        {
            // ändrar kategorin på vännen
            try
            {
                int IdAsInt = int.Parse(idn[0]);
                string desc = idn[1];
                string nyaKAt = idn[2];  
                db.vänner.Single(x => x.VännensID == IdAsInt && x.Username == desc && x.TillHörUser == AnvändareInloggadVerifeierare.användaren.AnvändarNamn).kategori = nyaKAt;
                db.SaveChanges();
            }
            catch (Exception)     
            {

                throw;
            }
        }

        [HttpGet]
        [ActionName("Get")]
        public List<MeddelandeModel> Get(int id)
        {
            // Hämtar alla meddelande
            try
            {
                AnvändareModel användaren = new AnvändareModel();
                foreach (var användare in db.användare)
                {
                    if (användare.ID == id)
                    {
                        användaren = användare;
                    }
                }
                List<MeddelandeModel> sortedList = användaren.allaMeddelanden.OrderByDescending(p => p.ID).ToList();
                return sortedList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        [ActionName("GetBesökare")]
        public List<BesökareModels> GetBesökare(int id)
        {
            // Hämtar alla besökare
            try
            {
                AnvändareModel användaren = new AnvändareModel();
                foreach (var användare in db.användare)
                {
                    if (användare.ID == id)
                    {
                        användaren = användare;
                    }
                }
                List<BesökareModels> sortedList = användaren.allaBesökare.OrderByDescending(p => p.ID).ToList();
                return sortedList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ActionName("PostBesökare")]
        public void PostBesökare(int[] idn)
        {
            // sparar besökarna till profilen
            try
            {
                int inloggadAnvändareID = idn[0];
                int besöksAnvändarID = idn[1];

                BesökareModels besökare = new BesökareModels();
                foreach (var användare in db.användare)
                {
                    if (användare.ID == inloggadAnvändareID)
                    {
                        // Hämta datum och lägg till det till besökare.
                        besökare.Datum = DateTime.Now;
                        // Hämta inloggade användarens användarnamn och lägg till det i besökaren
                        besökare.BesökandeAnvändarnamn = användare.AnvändarNamn;
                    }
                }

                foreach (var användare in db.användare)
                {
                    if (användare.ID == besöksAnvändarID)
                    {
                        //Ifall användare har  5 besökare, ta bort den äldsta besökaren och lägg till nya besökaren.
                        if (användare.allaBesökare.Count == 5)
                        {
                            var list = användare.allaBesökare.OrderByDescending(p => p.ID).Take(5).ToList();
                            BesökareModels sistaBesökaren = list[0];
                            användare.allaBesökare.Remove(sistaBesökaren);
                            användare.allaBesökare.Add(besökare);
                        }
                        //Ifall användaren har mindre än 5 besökare, lägg till besökaren.
                        if (användare.allaBesökare.Count < 5)
                        {
                            användare.allaBesökare.Add(besökare);
                        }
                    }
                }

                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

   

        [HttpPost]
        [ActionName("PostMeddelandeTaBort")]
        public void PostMeddelandeTaBort(int[] idn)   
        {
            // Ta bort meddelande
            try
            {
                int meddelandeid = idn[0];
                int inloggadid = idn[1];
                List<MeddelandeModel> meddelanden = new List<MeddelandeModel>();
                AnvändareModel användaren = new AnvändareModel();
                foreach (var användare in db.användare)
                {
                    if (användare.ID == inloggadid)
                    {
                        användaren = användare;
                        meddelanden = användare.allaMeddelanden;
                    }
                }



                if (meddelanden.Count > 0)
                {
                    foreach (var meddelande in meddelanden)
                    {
                        if (meddelande.ID == meddelandeid)
                        {
                            användaren.allaMeddelanden.Remove(meddelande);
                            db.meddelanden.Remove(meddelande);
                            break;
                        }
                    }
                }

                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ActionName("Post")]
        public void Post(FormDataCollection form)

        {
            // Sparar meddelande
            try
            {
                if (form != null)
                {
                    var meddelande = new MeddelandeModel();
                    var med = form.GetValues("meddelande");
                    meddelande.Meddelande = med[0];

                    var id = form.GetValues("inloggaUsername");
                    meddelande.InläggIfrånAnvändarnamn = id[0];

                    var userId = form.GetValues("userpageID");
                    int userAnvändarId = Int32.Parse(userId[0]);

                    foreach (var användaren in db.användare)
                    {
                        if (användaren.ID == userAnvändarId)
                        {
                            användaren.allaMeddelanden.Add(meddelande);
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
