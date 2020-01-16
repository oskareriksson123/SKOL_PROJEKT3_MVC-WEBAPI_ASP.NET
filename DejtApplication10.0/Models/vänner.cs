using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class vänn 
    {
        [Key]
        public int ID { get; set; }
       public int VännensID { get; set; }

        public string Username { get; set; }
        public string TillHörUser { get; set; }
        public string kategori { get; set; } = "vänn";

    }
}