using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using DejtApplication10._0.Models;

namespace DejtApplication10._0.Controllers
{

    public class AnvändareController : Controller
    {
        
        //Get Loginsida
        public ActionResult Login()
        {
            return View();
        }


        //Kontrollerar om Användarefinns
        [HttpPost]
        public ActionResult ValidateLogin(AnvändareModel model)
        {
            try
            {
                var ctx = new AnvändareDbContext();
                //Kontrollerar inputen ifrån login
                if (model.AnvändarNamn !=null && model.AnvändarNamn !="" && model.Lössenord !=null && model.Lössenord != "" && ctx.användare.Any(x=>x.AnvändarNamn == model.AnvändarNamn) ) { 
            
                //kollar lössenordet med hjälp av att avkrypter de som finns
                bool resultat = false;
                var PWEncrypter = new PasswordEncrypter();
                resultat = PWEncrypter.ValidatePW(model.AnvändarNamn,model.Lössenord);

                if (resultat == true)
                    AnvändareInloggadVerifeierare.användaren = ctx.användare.Single(x => x.AnvändarNamn == model.AnvändarNamn); // Sätter modellen till vår "AIV" klass
                                                                            // då kommer vi åt den inloggades model överallt
               

                if (resultat == true && AnvändareInloggadVerifeierare.användaren.IsActive == true)
                {
                    AnvändareInloggadVerifeierare.isInloogad = true;    // kollar så det är sant och att profilen är aktiv
                    return RedirectToAction("UserPage");                 // sätter att man är inloggad till sant   

                }
                else if (resultat == true && AnvändareInloggadVerifeierare.användaren.IsActive == false)
                {
                    AnvändareInloggadVerifeierare.isInloogad = true;        // om kontot är icke aktiv så kommmer man till
                    return View("InaktiveratKonto");                        // "InaktiveratKonto"
                }

                else
                {
                        ViewData["errorFältTomt"] = "Fel användarnamn eller lösenord, försök igen!";
                    return View("Login");
                        
                }
                }
                else
                {
                    ViewData["errorFältTomt"] = "Fel användarnamn eller lösenord, försök igen!";
                    return View("Login");  // Kollar så att man inte ändrar till ett användarnamn som finns
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult UserPage()

        {
            try
            {
                var ctx = new AnvändareDbContext();

                var AllaAnv = new AnvändareIndexViewModel();

                AllaAnv.Användare = ctx.användare.ToList();         

                var AllaVänner = ctx.vänner.ToList();      

                MainViewModel view = new MainViewModel() { model = AnvändareInloggadVerifeierare.användaren, allaAnvändare = AllaAnv, allaVänner = AllaVänner };


                return View(view);  // returnerar modelen på användaren, alla användare som finns i databasen och alla vänner som finns i databasen
                                     // så man kan loopa igenom dessa på UserPage sidan   
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult AnotherUserPage(string username)
        {
            try
            {
                AnvändareDbContext db = new AnvändareDbContext();

                var AllaAnv = new AnvändareIndexViewModel();
                AllaAnv.Användare = db.användare.ToList();

                var user = AllaAnv.Användare.Where(s => s.AnvändarNamn == username).FirstOrDefault();
                var AllaVänner = db.vänner.ToList();  // Tar in username som parameter och söker fram modellen för den användaren

                MainViewModel view = new MainViewModel() { model = user, allaAnvändare = AllaAnv, allaVänner = AllaVänner };
                                                    //skapar en view med den modellen och skickar med allaanvändare och alla vänner
                                                    //från databasen. Som man kan loopa igenom
                if (username == AnvändareInloggadVerifeierare.användaren.AnvändarNamn)
                {
                    return RedirectToAction("UserPage");   // om användaren klickar på sin egen profilbild kommer     
                }                                          // man till sin egen sida         
                return View(view);
            }
            catch (Exception)
            {

                throw;
            }


        }



        //Denna metod körs när man ska registrera sig den retunerar rätt GUI 
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

       
        //När man registrerat en användare så körs denn metod för att spara användaren och visa Att registreringen lyckades
        [HttpPost]
        public ActionResult addAnvändare(AnvändareModel model, HttpPostedFileBase File1)
        {
            try
            {
                var ctx = new AnvändareDbContext();
                var passwordEncrypter = new PasswordEncrypter();


                var födelse = model.Födelsedatum;
                var datumarray = födelse.Split('-');
                var året = datumarray[0];     // Kollar så personen har vettig ålder
                var intåret = Int32.Parse(året);
                if (intåret < 1920 || intåret > 2002)
                {
                    ViewData["errorfödelse"] = "Angivit fel födelsedatum, bara mellan 1920 till 2002 tillåtet";
                    return View("CreateAcc");
                }


                if (model.Lössenord == null || model.Lössenord == "")
                {

                    return View("CreateAcc");

                }
                if (File1 == null)
                {
                    ViewData["error"] = "Du måste ladda upp en bild";
                    return View("CreateAcc");
                }
                if (File1 != null && File1.ContentLength > 0 && File1.ContentType.Equals("image/jpeg") || File1.ContentType.Equals("image/png"))
                {
                    var encryptedPass = passwordEncrypter.EncryptPassword(model.Lössenord);
                    model.Lössenord = encryptedPass; // krypterar lösenordet

                    model.Förnamn = model.Förnamn.Substring(0, 1).ToUpper() + model.Förnamn.Substring(1);
                    model.Efternamn = model.Efternamn.Substring(0, 1).ToUpper() + model.Efternamn.Substring(1); // Gör att förnamn och efternamn första bokstav sparas som en stor bokstav

                    model.Profilbild = new byte[File1.ContentLength]; // Gör om filen man valt till En byte[]
                    File1.InputStream.Read(model.Profilbild, 0, File1.ContentLength);
                    ctx.användare.Add(model);
                    ctx.SaveChanges();    // kollar så att filen är av jpeg, png och att det finns en fil     
                                           // sparar hela användaren i databasen 
                    var nyaanvändarenMedID = new AnvändareModel();
                    foreach (var användare in ctx.användare)
                    {
                        if (användare.AnvändarNamn.Equals(model.AnvändarNamn))
                        {
                            nyaanvändarenMedID = användare;   // hämtar den nya användaren från databasen och skickar
                        }                                       // till bekräftelse
                    }
                    TempData["NyAnvändare"] = nyaanvändarenMedID;

                    return RedirectToAction("Bekräftelse", "Användare");
                }
                ViewData["error"] = "Fel format på bilden, Bara .jpg och .png är tillåtna";
                return View("CreateAcc");


            }
            catch (Exception)
            {

                throw;
            }
        }

        //När man registrerat en användare så körs denn metod för att spara användaren och visa Att registreringen lyckades

        [HttpGet]
        public ActionResult Bekräftelse()
        {
            try
            {
                var view = new AnvändareModel();
                view = TempData["NyAnvändare"] as AnvändareModel; //hämtar den ny användaren och visar den
                return View(view);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult EditUser(string username)
        {
            try
            {
                AnvändareDbContext db = new AnvändareDbContext();
                var view = new AnvändareIndexViewModel();
                view.Användare = db.användare.ToList();
                var user = view.Användare.Where(s => s.AnvändarNamn == username).FirstOrDefault();


                return View(user);   // Hämtar användaren och visar datan
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult EditUser(AnvändareModel model, HttpPostedFileBase File1)
         {
            try
            {
                AnvändareDbContext db = new AnvändareDbContext();
                AnvändareModel användare1 = db.användare.FirstOrDefault(a => a.AnvändarNamn.Equals(model.AnvändarNamn));
                AnvändareModel användare2 = db.användare.FirstOrDefault(a => a.Epost.Equals(model.Epost));
                // Hämtar model för epost respektive användarnamn


                var födelse = model.Födelsedatum;
                var datumarray = födelse.Split('-');
                var året = datumarray[0];     // Kollar så personen har vettig ålder
                var intåret = Int32.Parse(året);
                if (intåret < 1920 || intåret > 2002)
                {
                    ViewData["errorfödelse"] = "Angivit fel födelsedatum, bara mellan 1920 till 2002 tillåtet";
                    return View("EditUser");
                }

                var förnamn = model.Förnamn = model.Förnamn.Substring(0, 1).ToUpper() + model.Förnamn.Substring(1); //Gör om första bokstaven till stor ifall man angav liten
                var efternamn = model.Efternamn.Substring(0, 1).ToUpper() + model.Efternamn.Substring(1);

                var pattern = "^([A-ZÅÄÖa-zåäö]+)$"; //Regex som kontrollerar förnamn/efternamn tecken a-ö
                Regex rgx = new Regex(pattern);

                if (!rgx.IsMatch(förnamn))
                {
                    return View("EditUser");
                }

                if (!rgx.IsMatch(efternamn))
                {
                    return View("EditUser");
                }

                if (model.Förnamn == null || model.Förnamn == "")
                {

                    return View("EditUser");            // validerar så att det inte är tomt 
                }
                if (model.Efternamn == null || model.Efternamn == "")
                {
                    return View("EditUser");

                }
                if (model.AnvändarNamn == null || model.AnvändarNamn == "")
                {

                    return View("EditUser");

                }
                if (model.Lössenord == null || model.Lössenord == "")
                {

                    return View("EditUser");

                }

                if (db.användare.Any(a => a.AnvändarNamn.Equals(model.AnvändarNamn)) && AnvändareInloggadVerifeierare.användaren.ID != användare1.ID)
                {
                    ViewData["erroranvändarnamn"] = "Användarnamn upptaget";
                    return View("EditUser");  // Kollar så att man inte ändrar till ett användarnamn som finns
                }
                if (db.användare.Any(a => a.Epost.Equals(model.Epost)) && AnvändareInloggadVerifeierare.användaren.ID != användare2.ID)
                {
                    ViewData["errorepost"] = "Epost upptaget";
                    return View("EditUser"); // Kollar så att man inte ändrar till en epost som finns
                }
                if (ModelState.IsValid && File1 == null)  // om användare inte väljer ny bild
                {

                    int id = model.ID;
                    AnvändareModel användare = db.användare.FirstOrDefault(a => a.ID.Equals(id)); // Hämta användare



                    användare.Efternamn = model.Efternamn;
                    användare.Förnamn = model.Förnamn;
                    användare.Födelsedatum = model.Födelsedatum;    // uppdatera i databasen
                    användare.Epost = model.Epost;
                    användare.AnvändarNamn = model.AnvändarNamn;

                    //Kryptera det nya lössenordet
                    var encrypter = new PasswordEncrypter();
                    var nyttPwEncrypted= encrypter.EncryptPassword(model.Lössenord);
                    användare.Lössenord = nyttPwEncrypted;


                    AnvändareInloggadVerifeierare.användaren.Förnamn = förnamn;
                    AnvändareInloggadVerifeierare.användaren.Efternamn = efternamn;
                    AnvändareInloggadVerifeierare.användaren.Epost = model.Epost;                   // uppdaterar direkt
                    AnvändareInloggadVerifeierare.användaren.Födelsedatum = model.Födelsedatum;
                    AnvändareInloggadVerifeierare.användaren.AnvändarNamn = model.AnvändarNamn;
                    AnvändareInloggadVerifeierare.användaren.Lössenord = model.Lössenord;
                    AnvändareInloggadVerifeierare.användaren.IsActive = model.IsActive;




                    db.Entry(användare).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("UserPage");

                }

                if (ModelState.IsValid && File1 != null && File1.ContentLength > 0 && File1.ContentType.Equals("image/jpeg") || File1.ContentType.Equals("image/png"))
                {

                    int id = model.ID;    // om användaren väljer att byta bild + annan information så körs denna if sats
                    AnvändareModel användare = db.användare.FirstOrDefault(a => a.ID.Equals(id)); // Hämta användare

                    
                    model.Profilbild = new byte[File1.ContentLength]; // Gör om filen man valt till En byte[]
                    File1.InputStream.Read(model.Profilbild, 0, File1.ContentLength);



                    användare.Profilbild = model.Profilbild;
                    AnvändareInloggadVerifeierare.användaren.Profilbild = model.Profilbild;

                    användare.Efternamn = model.Efternamn;
                    användare.Förnamn = model.Förnamn;
                    användare.Födelsedatum = model.Födelsedatum;    // uppdatera i databasen
                    användare.Epost = model.Epost;
                    användare.Lössenord = model.Lössenord;
                    användare.IsActive = model.IsActive;
                    användare.AnvändarNamn = model.AnvändarNamn;

                    AnvändareInloggadVerifeierare.användaren.Förnamn = model.Förnamn;
                    AnvändareInloggadVerifeierare.användaren.Efternamn = model.Efternamn;
                    AnvändareInloggadVerifeierare.användaren.Epost = model.Epost;                   // uppdaterar direkt
                    AnvändareInloggadVerifeierare.användaren.Födelsedatum = model.Födelsedatum;
                    AnvändareInloggadVerifeierare.användaren.AnvändarNamn = model.AnvändarNamn;
                    AnvändareInloggadVerifeierare.användaren.Lössenord = model.Lössenord;
                    AnvändareInloggadVerifeierare.användaren.IsActive = model.IsActive;




                    db.Entry(användare).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("UserPage");
                }
                else if (File1 != null && File1.ContentLength > 0 && !File1.ContentType.Equals("image/jpeg") || !File1.ContentType.Equals("image/png"))

                { // om det är fel format på bilden

                    ViewData["error"] = "Fel format på bilden, Bara .jpg och .png är tillåtna";
                    return View("EditUser");
                }

                return RedirectToAction("UserPage");
            }
            catch (Exception)
            {

                throw;
            }

        }

        // Denna metod körs när man söker efter något på söksidan, samt sorterar sökningen.
        [HttpPost]
        public ActionResult Sök(string id)
        {
          
            try
            {
                
                var mainview = new MainViewModel();
                string searchString = id;
                var ctx = new AnvändareDbContext();
               
                mainview.allaVänner = ctx.vänner.ToList();


                //Ifall det söks med för + efternamn körs denna del
                if (!String.IsNullOrEmpty(searchString) && searchString != "" && searchString.Contains(" "))
                {
                    string[] deladSök = searchString.Split(null);
                    string förnamnet1 = deladSök[0];
                    string förnamnet = förnamnet1.Substring(0, 1).ToUpper() + förnamnet1.Substring(1);
                    string efternamnet1 = deladSök[1];

                    //Kontrollerar så man inte skrivit in förnamnet + Blankspace
                    if (efternamnet1 == null || efternamnet1 == "")
                    {
                        return RedirectToAction("Sök");

                    }


                    else
                    {
                        string efternamnet = efternamnet1.Substring(0, 1).ToUpper() + efternamnet1.Substring(1);

                        var inloggadAnvändarnamn = AnvändareInloggadVerifeierare.användaren.AnvändarNamn;

                        foreach (var användare in ctx.användare)
                        {
                            if (användare.Förnamn.Contains(förnamnet) && användare.Efternamn.Contains(efternamnet))
                            {
                                mainview.allaAnvändare.Användare.Add(användare);
                            }
                            if (användare.AnvändarNamn == inloggadAnvändarnamn)
                            {
                                mainview.allaAnvändare.Användare.Remove(användare);
                            }
                        }
                        return View(mainview);
                    }
                }

                //Ifall det söks med för ELLER efternamn körs denna del
                else if (!String.IsNullOrEmpty(searchString) && searchString != "")
                {
                    string sökningen = searchString;
                    sökningen = sökningen.Substring(0, 1).ToUpper();

                    var inloggadAnvändarnamn = AnvändareInloggadVerifeierare.användaren.AnvändarNamn;

                    foreach (var användare in ctx.användare)
                    {
                        if (användare.Förnamn.Contains(sökningen) || användare.Efternamn.Contains(sökningen))
                        {
                            mainview.allaAnvändare.Användare.Add(användare);
                        }
                        if (användare.AnvändarNamn == inloggadAnvändarnamn)
                        {
                            mainview.allaAnvändare.Användare.Remove(användare);
                        }
                    }
                    return View(mainview);
                }

                return RedirectToAction("Sök");
            }
            catch (Exception)
            {

                throw;
            }
        }

        // Metoden körs när man går in på söksidan, listar alla användar som är registrerade utom den inloggade.
        [HttpGet]
        public ActionResult Sök()
        {
            try
            {
                var ctx = new AnvändareDbContext();
            
                var mainview = new MainViewModel();

                var ListaAttvisa = ctx.användare;
                foreach (var användare in ctx.användare)
                {
                    if (användare.AnvändarNamn == AnvändareInloggadVerifeierare.användaren.AnvändarNamn)
                    {
                        //Här hittar vi den inloggades vänner och tar bort dessa eftersom man redan är vänn.
                        var SorteradLista = from envänn in ctx.användare
                                            where envänn.allaAnvändarensVänner.Any(vänn => vänn.VännensID == användare.ID)
                                            select envänn;

                        var allaanvändare = ctx.användare.ToList();
                        mainview.allaAnvändare.Användare.AddRange(allaanvändare); 
                        foreach (var vän in SorteradLista)
                        {
                            mainview.allaAnvändare.Användare.Remove(vän);
                        }

                        mainview.allaAnvändare.Användare.Remove(användare);
                        break;
                    }

                }

                return View(mainview);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public ActionResult PassandePartner()
        {
            try
            {
                var ctx = new AnvändareDbContext();
                var view = new AnvändareIndexViewModel();
                view.Användare = ctx.användare.ToList();

                foreach (var användare in ctx.användare)
                {
                    if (användare.AnvändarNamn == AnvändareInloggadVerifeierare.användaren.AnvändarNamn)
                    {
                        view.Användare.Remove(användare);
                    }
                }
                return View(view);  // Tar bort sig själv från view så man inte matchar med sig själv
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult LoggaUt()
        {
            try
            {
                AnvändareInloggadVerifeierare.isInloogad = false;
                AnvändareInloggadVerifeierare.användaren = null;
                Session.Abandon();
                Session.Clear();
                return RedirectToAction("Login");
            }
            catch (Exception)
            {

                throw;
            }
        }

        //När man skickar en vännförfrågan körs denna metod
        public ActionResult AddVänn(string Användarnamn)
        {
            try
            {
                int inloggadesID = AnvändareInloggadVerifeierare.användaren.ID;
                var ctx = new AnvändareDbContext();
                var nyFörfrågan = new VännFörFrågningar();
                //Hitta vännen och lägg till en förfrågan i hens förfrågningar
                foreach (var hittaVännen in ctx.användare)
                {
                    if (hittaVännen.AnvändarNamn.Equals(Användarnamn))
                    {
                        nyFörfrågan.FörFråganIfrånID = inloggadesID;
                        hittaVännen.vännFörFrågningar.Add(nyFörfrågan);
                        break;
                    }
                }
                ctx.SaveChanges();

                return Redirect("/");
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult seVännförfrågningar()
        {
            return View();
        }


        //Denna metod körs när man INTE vill acceptera en vännförfrågan!
        public ActionResult TaBortVännförfrågan(int FörfrågningsID)
        {
            try
            {

                var ctx = new AnvändareDbContext();

                //Först så hittar man den som är inloggad genom att matcha med den som är inloggad
                foreach (var användare in ctx.användare)
                {
                    if (användare.AnvändarNamn.Equals(AnvändareInloggadVerifeierare.användaren.AnvändarNamn))
                    {
                        //Sedan så hittar vi rätt vännförfrågan genom att loopa igenom dennes vännförfrågningar och tar bort den aktuella
                        foreach (var vännRequest in användare.vännFörFrågningar)
                        {
                            if (vännRequest.FörFrågningsID == FörfrågningsID)
                            {
                                användare.vännFörFrågningar.Remove(vännRequest);

                                AnvändareInloggadVerifeierare.användaren = användare;
                                break;
                            }
                        }
                        break;
                    }
                }
                ctx.SaveChanges();
                return RedirectToAction("seVännförfrågningar");
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult AccepteraVänn(int VännAttLäggaTill, int vännFörfrågansID)
        {
            try
            {
                var ctx = new AnvändareDbContext();

                var InloggadesUsername = AnvändareInloggadVerifeierare.användaren.AnvändarNamn;
                var VännensUsername = ctx.användare.FirstOrDefault(x => x.ID == VännAttLäggaTill).AnvändarNamn;

                //Börja med att hitta den inloggade och lägger in Id på den man blev vänn med.     
                var användare = ctx.användare.FirstOrDefault(x => x.ID == AnvändareInloggadVerifeierare.användaren.ID);
                användare.allaAnvändarensVänner.Add(new vänn { VännensID = VännAttLäggaTill, Username = VännensUsername, TillHörUser = InloggadesUsername });

                //Sedan hittar vi vännen  och lägger in Id på den inloggade i hens vännlista.  
                var vännenSomLaTill = ctx.användare.FirstOrDefault(x => x.ID == VännAttLäggaTill);
                vännenSomLaTill.allaAnvändarensVänner.Add(new vänn { VännensID = användare.ID, Username = InloggadesUsername, TillHörUser = VännensUsername });

                //sedan måste vännförfrågan bort (Eftersom man accepterat)
                var förfråganAttTaBort = användare.vännFörFrågningar.FirstOrDefault(x => x.FörFrågningsID == vännFörfrågansID);
                användare.vännFörFrågningar.Remove(förfråganAttTaBort);
                AnvändareInloggadVerifeierare.användaren = användare;
                ctx.SaveChanges();

                return RedirectToAction("seVännförfrågningar");
            }
            catch (Exception)
            {

                throw;
            }
        }



        public ActionResult SparaTillXml()
        {
            try
            {
                var ctx = new AnvändareDbContext();
                ctx.Configuration.ProxyCreationEnabled = false; // Stänger av "lazy loading så man kan serialisera

                var Inloggad = ctx.användare.FirstOrDefault(x => x.AnvändarNamn == AnvändareInloggadVerifeierare.användaren.AnvändarNamn);

                if (Inloggad != null)
                {

                    XmlSerializer XmlS = new XmlSerializer(typeof(AnvändareModel));
                    Directory.CreateDirectory(@"c:\Logs");   //hämtat användaren och skapar en mapp och filväg
                    string logPath = @"c:\Logs";
                    string logFile = Path.Combine(logPath, DateTime.UtcNow.ToString("HH-mm") + ".xml");

                    using (TextWriter tw = new StreamWriter(logFile))
                    {

                        XmlS.Serialize(tw, Inloggad); // serialiserar 

                    }

                    if (System.IO.File.Exists(logFile))
                    {
                        ctx.Configuration.ProxyCreationEnabled = true; // när vi har skapat en fil så aktiverar vi lazy loading och spårning igen
                    }
                }


                return RedirectToAction("SparaResterandeInfoXml");
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult SparaResterandeInfoXml()
        {
            try
            {
                var ctx = new AnvändareDbContext();

                var meddelande = "";
                // hämtar alla meddelande som användaren har
                foreach (var msg in AnvändareInloggadVerifeierare.användaren.allaMeddelanden)
                {
                    meddelande += msg.Meddelande + " Från '" + msg.InläggIfrånAnvändarnamn + "' ";


                }
                // Räknar antal förfrågningar
                var antaletförfrågningar = AnvändareInloggadVerifeierare.användaren.vännFörFrågningar.Count;
                var antalf = "Antal vänförfrågningar: " + antaletförfrågningar.ToString();

                var vänner = "";
                var inloggadesId = AnvändareInloggadVerifeierare.användaren.ID;

                var vännerna = from användare in ctx.användare
                               where användare.allaAnvändarensVänner.Any(vänn => vänn.VännensID == inloggadesId)
                               select användare;

                // Hämtar namnen på alla vännerna
                foreach (var item in vännerna)
                {
                    vänner += item.AnvändarNamn + " ";



                }

                string logPa = @"c:\Logs";
                string logFil = Path.Combine(logPa, DateTime.UtcNow.ToString("HH-mm") + ".xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(logFil);
                XmlNode root = doc.DocumentElement;     // öppnar den skapad xml filen och redigerar in info
                XmlNode myNode = root.SelectSingleNode("allaMeddelanden");
                myNode.InnerText = meddelande;

                XmlNode myNode1 = root.SelectSingleNode("vännFörFrågningar");
                myNode1.InnerText = antalf;

                XmlNode myNode2 = root.SelectSingleNode("allaAnvändarensVänner");
                myNode2.InnerText = vänner;

                doc.Save(logFil);

                return RedirectToAction("Userpage");

            }
            catch (Exception)
            {

                throw;
            }
        }

    

    //Sätter Inloggades IsAktiv till false , därav kommer alla sidor som kontrollerar om kontot är aktivt eller inte känna av det
    public ActionResult InaktiveraAnvändare()
        {
            try
            {
                var ctx = new AnvändareDbContext();
                var Inloggad = ctx.användare.FirstOrDefault(x => x.AnvändarNamn == AnvändareInloggadVerifeierare.användaren.AnvändarNamn);
                Inloggad.IsActive = false;

                ctx.SaveChanges();
                AnvändareInloggadVerifeierare.användaren = Inloggad;
                return View("InaktiveratKonto");
            }
            catch (Exception)
            {

                throw;
            }
        }

        //Sätter Inloggades IsAktiv till true , därav kommer alla sidor som kontrollerar om kontot är aktivt eller inte känna av det
        public ActionResult AktiveraAnvändare()
        {
            try
            {
                var ctx = new AnvändareDbContext();
                var Inloggad = ctx.användare.FirstOrDefault(x => x.AnvändarNamn == AnvändareInloggadVerifeierare.användaren.AnvändarNamn);

                Inloggad.IsActive = true;
                ctx.SaveChanges();
                AnvändareInloggadVerifeierare.användaren = Inloggad;
                return RedirectToAction("UserPage");
            }
            catch (Exception)
            {

                throw;
            }
        }


    }


   


}