using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class BesökareModels
    {
        [Key]
        public virtual int ID { get; set; }

        [Required]
        public virtual DateTime Datum { get; set; }

        [Required]
        public virtual string BesökandeAnvändarnamn { get; set; }
    }
}