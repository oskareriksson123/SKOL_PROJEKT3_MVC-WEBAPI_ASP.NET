using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DejtApplication10._0.Models;

namespace DejtApplication10._0.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var ctx = new AnvändareDbContext();
                var view = new AnvändareIndexViewModel();


                // hämtar 3 första användarna genom ID


                view.Användare = ctx.användare.OrderByDescending(x => x.ID).Where(x => x.IsActive == true).Take(3).ToList();

                return View(view);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}