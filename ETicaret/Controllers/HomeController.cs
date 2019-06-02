using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETicaret.Settings;
using ETicaret.Models;
using ETicaret.Models.ModelsView;

namespace ETicaret.Controllers
{
    public class HomeController : Controller
    {
        ETicaretEntities db = new ETicaretEntities();
        // GET: Home
        [HttpGet]
        public ActionResult Index()//Default olarak gelen ana sayfamız.
        {

            UrunMarkaCocukYorum urunMarkaCocukYorum = new UrunMarkaCocukYorum
            {
                urun = db.Urun.Where(x => x.urunId >= 1 && x.urunId < 21).ToList()
            };
            ViewBag.AltKategoriler = db.AltKategori.ToList();
            urunMarkaCocukYorum.marka = db.Marka.ToList();
           
            List<Urun> uruns= db.Urun.Where(x => x.urunId > 21 && x.urunId < 30).ToList();
            ViewBag.Urunler = uruns;
            urunMarkaCocukYorum.cocuk = db.CocukYorum.ToList();
            List<Urun> indirimliUrun = db.Urun.Where(x => x.urunKampanyaVarMı == true).ToList();
            ViewBag.Indirim = indirimliUrun;
            return View(urunMarkaCocukYorum);
        }
        [HttpPost]
        public ActionResult Index(string email)//Default olarak gelen ana sayfamız.
        {
          
            
            Abone abone = new Abone();
            if(db.Abone.Where(x=> x.aboneMail== email).FirstOrDefault() !=null)
            {
                return View();
            }
            abone.aboneMail = email;
            abone.aboneTarih = DateTime.Now.ToShortDateString();
           
            db.Abone.Add(abone);
            db.SaveChanges();
            UrunMarkaCocukYorum urunMarkaCocukYorum = new UrunMarkaCocukYorum
            {
                urun = db.Urun.Where(x => x.urunId >= 1 && x.urunId < 21).ToList()
            };
            ViewBag.AltKategoriler = db.AltKategori.ToList();
            urunMarkaCocukYorum.marka = db.Marka.ToList();

            List<Urun> uruns = db.Urun.Where(x => x.urunId > 21 && x.urunId < 30).ToList();
            ViewBag.Urunler = uruns;
            urunMarkaCocukYorum.cocuk = db.CocukYorum.ToList();
            return View(urunMarkaCocukYorum);
        }
        [HttpGet]
        public ActionResult YorumGonder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult YorumGonder(HttpPostedFileBase resimGelen, string name, string surname, string comment)
        {
            CocukYorum cocuk = new CocukYorum();
            Random rnd = new Random();
            int sayi = rnd.Next(1, 3);
            if (resimGelen == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string yeniResimAdi = "";
                ResimIslem r = new ResimIslem();
                yeniResimAdi = r.Ekle(resimGelen);
                //yeniResimAdi = new ResimIslem().Ekle(resimGelen);

                if (yeniResimAdi == "uzanti")
                {
                    var yetkiler = db.Yetki.ToList();
                   
                    //ViewBag.Hata = "Lütfen .png veya .jpg uzantılı dosya giriniz.";
                    ViewData["Hata"] = "Lütfen .png veya .jpg uzantılı dosya giriniz.";
                    return RedirectToAction("Index", "Home");
                }
                else if (yeniResimAdi == "boyut")
                {
                    var yetkiler = db.Yetki.ToList();
                   
                    //ViewBag.Hata = "En fazla 1MB boyutunda dosya girebilirsiniz.";
                    ViewData["Hata"] = "En fazla 1MB boyutunda dosya girebilirsiniz.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    cocuk.cocukFoto = yeniResimAdi;
                }
            }
            cocuk.cocukAd = name;
            cocuk.cocukSoyad = surname;
            cocuk.cocukYorum1 = comment;
            cocuk.cocukRutbeId = sayi;
            db.CocukYorum.Add(cocuk);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult About()//Hakkinda Sayfasi
        {
            return View();
        }
        public ActionResult Service()//Servisler Sayfasi.
        {
            return View();
        }
        public ActionResult AlisVeris()//Alisveris Sayfasi
        {
            
            return View();
        }
        [HttpGet]
        public ActionResult Contact()//İletisim Sayfasi.
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string name,string email,string phone,string message)
        {
            ETicaretEntities db = new ETicaretEntities();
            BizeUlasin bize = new BizeUlasin
            {
                mesajAd = name,
                mesajDetay = message,
                mesajEmail = email,
                mesajTelefon = phone,
                mesajTarih = DateTime.Now.ToShortDateString()
                
            };
            db.BizeUlasin.Add(bize);
            
            db.SaveChanges();

            return View();
        }
        public ActionResult Brand()
        {
            return View();
        }
    }
}