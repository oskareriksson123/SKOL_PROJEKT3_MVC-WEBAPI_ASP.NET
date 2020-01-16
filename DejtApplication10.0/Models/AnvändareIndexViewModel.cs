using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class AnvändareIndexViewModel
    {
        public List<AnvändareModel> Användare { get; set; }

        public AnvändareIndexViewModel()
        {
            Användare = new List<AnvändareModel>();
        }
    }
}