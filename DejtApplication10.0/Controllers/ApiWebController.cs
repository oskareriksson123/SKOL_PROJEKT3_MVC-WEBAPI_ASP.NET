using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DejtApplication10._0.Models;

namespace DejtApplication10._0.Controllers
{
    [RoutePrefix("api/meddelande")]
    public class ApiWebController : ApiController
    {
        [HttpGet]
        [Route("")]
        public List<MeddelandeModel> getMeddelande(string användarnamn)
        {
            var ctx = new AnvändareDbContext();
            AnvändareModel användare = new AnvändareModel();

            foreach (var användaren in ctx.användare)
            {
                if (användaren.AnvändarNamn == användarnamn)
                {
                    användare = användaren;
                }
            }

            return användare.Meddelanden.ToList();

        }
    }
}
