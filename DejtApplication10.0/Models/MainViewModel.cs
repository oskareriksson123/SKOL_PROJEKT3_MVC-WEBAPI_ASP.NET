using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class MainViewModel
    {
        public AnvändareModel model { get; set; }
        public AnvändareIndexViewModel allaAnvändare { get; set; }

        public List<vänn> allaVänner { get; set; }

        public MainViewModel()
        {
            model = new AnvändareModel();
            allaAnvändare = new AnvändareIndexViewModel();
            allaVänner = new List<vänn>();

        }
    }

    

}