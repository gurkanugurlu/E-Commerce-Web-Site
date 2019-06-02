using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ETicaret.Models;
using ETicaret.Settings;

namespace ETicaret.Controllers
{
    public class LoginController : Controller
    {
        ETicaretEntities db = new ETicaretEntities();

        [HttpGet]
        public ActionResult Index()
        {
            
           


            return View();
        }
        
        [HttpPost]
        public ActionResult Index(string kullaniciMail, string kullaniciSifre)
        {
            ETicaretEntities db = new ETicaretEntities();
            if (kullaniciMail != null && kullaniciSifre != null)
            {
                Kullanici kullanici = db.Kullanici.Where(x => x.kullaniciMail == kullaniciMail && x.kullaniciSifre == kullaniciSifre).FirstOrDefault();
                string yetki;
               
                if (kullanici == null)
                {
                    ViewBag.Sonuc = "Kullanici Bulunamadi.";
                    return View();
                }
                else
                {
                    yetki = kullanici.Yetki.yetkiAd;
                    Session["Kullanici"] = kullanici;
                    Session["Yetki"] = yetki;
                    if ((kullanici.Yetki.yetkiAd == "Yonetici" || kullanici.Yetki.yetkiAd == "Analizist") || (kullanici.Yetki.yetkiAd == "Editor"))
                    {
                        return RedirectToAction("Index", "Yonetici");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else
            {
                ViewBag.Sonuc = "Kullanici Bulunamadi.";
                return View();
            }
        }
        [HttpGet]
       public ActionResult SifreUnuttum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SifreUnuttum(string mail)
        {
           
            
            Kullanici k = db.Kullanici.Where(x => x.kullaniciMail == mail).SingleOrDefault();
            if(k == null)
            {
                string uyari = "Böyle bir kullanıcı bulunmamakktadır.";
                ViewData["Hata"] = uyari;
                return View();
            }
            SifreSifirla sifre = new SifreSifirla
            {
                eskiSifre = k.kullaniciSifre,
                sifre = Guid.NewGuid(),
                kullaniciId = k.kullaniciId,
                guncellenmeTarih = DateTime.Now
            };
            db.SifreSifirla.Add(sifre);
            db.SaveChanges();
            string konu = "Şifre Sıfırlama";
            string mesaj = "Şifrenizi sıfırlama talebiniz için. <a href='http://localhost:60320/Login/SifreSifirla?kod=" + sifre.sifre + "'> tıklayınız";
            Boolean sonuc = Eposta.Gonder(konu, mesaj, k.kullaniciMail) ? true : false;
            ViewBag.Uyari = "Epostanıza şifreniz gönderilmiştir.";
            return View();
        }
        [HttpGet]
        public ActionResult SifreSifirla(Guid kod)
        {
            if (kod == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SifreSifirla s = db.SifreSifirla.Where(x => x.sifre == kod).SingleOrDefault();
            Kullanici k = db.Kullanici.Where(b => b.kullaniciId == s.kullaniciId).SingleOrDefault();


            return View(k);
        }
        [HttpPost]
        public ActionResult SifreSifirla(int kullaniciId,string sifre1,string sifre2)
        {
           
            if(sifre1 != sifre2)
            {
                ViewData["Hata"] = "Şifreniz birbiriyle uyuşmamaktadır.";
                return View();
            }
            Kullanici k = db.Kullanici.Where(x => x.kullaniciId == kullaniciId).SingleOrDefault();
            k.kullaniciSifre = sifre1;
            db.SaveChanges();
            return RedirectToAction("Index", "Login");

        }
        
       
    }
}