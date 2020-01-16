using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace DejtApplication10._0.Models
{
    public class PasswordEncrypter
    {

       
     public string EncryptPassword(string PW)
     {
            //Börjar med att generera Random värden, så kallat "Salt"
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            //Genererar en Hash med 10000 itterationer av algoritmen Rfc
            var PBKDF2 = new Rfc2898DeriveBytes(PW, salt, 10000);

            //Lägger in strängen till en byte[] så kallad hash
            byte[] hash = PBKDF2.GetBytes(20);

            //Gör en sista byte[] som lägger ihop saltet och hashen
            byte[] hashBytes = new byte[36];
        
            //Kopierar in hashen och saltet på rätt ställen
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //Gör om lössenordet som just ny är en byte[] till en sträng som man kan spara i databasen 
            string PWWithHashAndSalt = Convert.ToBase64String(hashBytes);


            return PWWithHashAndSalt;
     }


        public bool ValidatePW(string username, string PwToCheck)
        {

            //Först Hämtar vi ut rätt användare
            bool test = false;
            var ctx = new AnvändareDbContext();
            var användaren = ctx.användare.Single(x => x.AnvändarNamn == username);
            string AnvändarensPW = användaren.Lössenord;

            //Eftersom lössenordet är sparat som en sträng och för att jämföra m,åste man ta bort saltet, eftersom saltet är random så gör vi om lössenordet
            //till en byte[]
            byte[] hashBytes = Convert.FromBase64String(AnvändarensPW);


            //Samma process som när man krypterar fast med de lössenordet man skrev in
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var PBKDF2 = new Rfc2898DeriveBytes(PwToCheck, salt, 10000);
            byte[] hash = PBKDF2.GetBytes(20);

            //Sedan använder vi en for loop som kontrollerar byte efter byte om de båda är lika - saltet;
            //Eftersom saltet är random. Därför börjar man på 16 i ena byte[] och 0 i andra vid jämförelsen! 

            for(int i =0; i <20; i++)
            {
                if(hashBytes[i + 16] != hash[i])
                {
                    test = false;
                    //Reunerar false om något är fel och avbryter processen
                    break;
                }

                else
                {
                    test = true;
                }

            }


            return test;
        }

    }
}