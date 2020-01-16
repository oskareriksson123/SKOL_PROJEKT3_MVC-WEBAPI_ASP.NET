using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Web.Mvc;

namespace DejtApplication10._0.Models
{

    public class AnvändareModel 
    {

        [Key]
        public virtual int ID { get; set; }
       
        [Required(ErrorMessage = "Du måste fylla i ditt Förnamn!!")]
        [RegularExpression("^([A-ZÅÄÖa-zåäö]+)$", ErrorMessage = "Ange förnamn med tecken från A-Ö & utan mellanrum!")]

        public virtual string Förnamn { get; set; }
 
        [Required(ErrorMessage = "Du måste fylla i ditt Efternamn!")]
        [RegularExpression("^([A-ZÅÄÖa-zåäö]+)$", ErrorMessage = "Ange efternamn med tecken från A-Ö & utan mellanrum!")]
        public virtual string Efternamn { get; set; }
        [Required]
        public virtual bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Du måste fylla i ett användarnamn!")]
        [Display(Name = "Användarnamn")]
        [Remote("FinnsAnvändare", "Validation", ErrorMessage = "Användarnamn finns redan")]
        public virtual string AnvändarNamn { get; set; }

        [Required(ErrorMessage = "Du måste fylla i en epost!")]
        [EmailAddress]
        [Remote("FinnsEpost", "Validation", ErrorMessage = "Epost finns redan")]
        public virtual string Epost { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Lössenord")]
        public virtual string Lössenord { get; set; }

        [Required(ErrorMessage ="Du måste välja ett födelsedatum")]
        [DataType(DataType.Date)]
        public virtual string Födelsedatum { get; set; }

       
        public virtual byte[] Profilbild { get; set; }

       
        public virtual List<VännFörFrågningar> vännFörFrågningar { get; set; }

        public virtual List<MeddelandeModel> allaMeddelanden { get; set; }

        public List<vänn> allaAnvändarensVänner { get; set; }

        public virtual List<BesökareModels> allaBesökare { get; set; }

      

        public AnvändareModel()
        {
            vännFörFrågningar = new List<VännFörFrågningar>();
            allaMeddelanden = new List<MeddelandeModel>();
            allaBesökare = new List<BesökareModels>();
            

            allaAnvändarensVänner = new List<vänn>();
    }


        //Hämtar åldern på personen
        public string getÅlder()
        {
            try
            {
                int ålder = 0;
                DateTime födelsedatum = DateTime.Parse(Födelsedatum);
                ålder = DateTime.Now.Year - födelsedatum.Year;
                if (DateTime.Now.DayOfYear < födelsedatum.DayOfYear)
                {
                    ålder = ålder - 1;
                }
                return ålder.ToString();
            }
            catch (Exception)
            {

                throw;
            }
        }


        //Hämtar antal vännförfrågningar som visas i navbarnen
        public string getAntalVännförFrågningar()
        {
            try
            {
                int? antal = vännFörFrågningar.Count();
                if (antal == null)
                {
                    return "0";
                }
                else
                {
                    return vännFörFrågningar.Count().ToString();
                }
            }

            catch (Exception)
            {

                throw;
            }
        }


    }
}