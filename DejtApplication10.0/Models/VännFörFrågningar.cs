using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class VännFörFrågningar 
    {
        
        [Key]
        public int FörFrågningsID { get; set; }
        public bool Harsvarat { get; set; } = false;
       
        public int FörFråganIfrånID { get; set; }

    }
}