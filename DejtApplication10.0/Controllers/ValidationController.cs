using DejtApplication10._0.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DejtApplication10._0.Controllers
{
    public class ValidationController : Controller
    {
        public AnvändareDbContext db { get; set; } = new AnvändareDbContext();
        public AnvändareController ac { get; set; } = new AnvändareController();
       

        public ValidationController()
        {

        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult FinnsAnvändare(string AnvändarNamn)
        {
            // Kollar om användarnamnet är upptagen då man skapar en ny användare
            try
            {
                return Json(!db.användare.Any(x => x.AnvändarNamn == AnvändarNamn), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult FinnsEpost(string Epost)
        {
            // kollar om eposten är upptagen då man skapar en ny användare
            try
            {
                return Json(!db.användare.Any(x => x.Epost == Epost), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


    } 
}