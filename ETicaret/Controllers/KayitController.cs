using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETicaret.Models;
using ETicaret.Settings;
namespace ETicaret.Controllers
{
    public class KayitController : Controller
    {
        // GET: Kayit
        [HttpGet]
        public ActionResult Index()
        {
            ETicaretEntities db = new ETicaretEntities();
            Kullanici k = new Kullanici();
            var sehirler = db.Sehirler.ToList();
            var ulkeler = db.Ulkeler.ToList();
            ViewBag.Sehirler = new SelectList(sehirler, "sehirId", "sehirAd");
            ViewBag.ulkeler = new SelectList(ulkeler, "ulkeId", "ulkeAd");
            
            return View(k);
        }
        
        [HttpPost]
        public ActionResult Index(Kullanici k,string kullaniciAd, string kullaniciSoyad,string kullaniciMail ,string cinsiyet, string kullaniciSoru,string kullaniciSoruCevap,string kullaniciTelefon,string kullaniciSifre, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();
            if (db.Kullanici.Where(x => x.kullaniciMail == kullaniciMail).SingleOrDefault() != null)
            {
               
                ViewData["Hata"] = "Boyle bir eposta vardır";
                var sehirler = db.Sehirler.ToList();
                var ulkeler = db.Ulkeler.ToList();
                ViewBag.Sehirler = new SelectList(sehirler, "sehirId", "sehirAd");
                return View();
            }
            if (resimGelen == null)
            {
                if (cinsiyet != null)
                {
                    if (cinsiyet == "erkek")
                    {
                        k.kullaniciResim = "bos.png";
                    }
                    else
                    {
                        k.kullaniciResim = "bos2.png";
                    }
                }
                else
                {
                    k.kullaniciResim = "bos.png";
                }
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
                    ViewBag.Yetkiler = new SelectList(yetkiler, "yetkiID", "ad");
                    //ViewBag.Hata = "Lütfen .png veya .jpg uzantılı dosya giriniz.";
                    ViewData["Hata"] = "Lütfen .png veya .jpg uzantılı dosya giriniz.";
                    return View();
                }
                else if (yeniResimAdi == "boyut")
                {
                    var yetkiler = db.Yetki.ToList();
                    ViewBag.Yetkiler = new SelectList(yetkiler, "yetkiID", "ad");
                    //ViewBag.Hata = "En fazla 1MB boyutunda dosya girebilirsiniz.";
                    ViewData["Hata"] = "En fazla 1MB boyutunda dosya girebilirsiniz.";
                    return View();
                }
                else
                {
                    k.kullaniciResim = yeniResimAdi;
                }
            }

            k.kullaniciAd = kullaniciAd;
            k.kullaniciSoyad = kullaniciSoyad;
            k.kullaniciMail = kullaniciMail;
            k.kullaniciAktifMi = true;
            k.kullaniciSoru = kullaniciSoru;
            k.kullaniciSoruCevap = kullaniciSoruCevap;
            if (cinsiyet == "kadin")
            {
                k.kullaniciCinsiyet = true;
            }
            else
            {
                k.kullaniciCinsiyet = false;
            }
            k.kullaniciTelefon = kullaniciTelefon;
            k.kullaniciSifre = kullaniciSifre;
            k.kullaniciMail = kullaniciMail;
            k.yetkiId = 4;
            k.kullaniciKayitTarih = DateTime.Now;
            db.Kullanici.Add(k);
            db.SaveChanges();
            
            return RedirectToAction("Index", "Home");

        }
    }
}