using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class MeddelandeModel 
    {
        [Key]
        public virtual int ID { get; set; }

        [Required]
        public virtual string Meddelande { get; set; }

        [Required] 
        public virtual string InläggIfrånAnvändarnamn { get; set; }

        public MeddelandeModel()
        {
                
        }
    }
}