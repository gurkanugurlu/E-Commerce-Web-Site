using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETicaret.Models;

namespace ETicaret.Controllers
{
    public class KullaniciController : Controller
    {
        ETicaretEntities db = new ETicaretEntities();
        public ActionResult Index()
        {
            
            Kullanici k = (Kullanici) Session["Kullanici"];
            int siparişSayısı = db.Siparis.Where(X => X.kullaniciId == k.kullaniciId).Count();
            ViewBag.Sayi = siparişSayısı;
            int yorumSayisi = db.Yorum.Where(x => x.kullaniciId == k.kullaniciId).Count();
            int puanSayisi = db.Puan.Where(x => x.kullaniciId == k.kullaniciId).Count();
            ViewBag.YSayi = siparişSayısı;
            ViewBag.PSayi = puanSayisi;
            List<Siparis> siparis = db.Siparis.Where(x => x.kullaniciId == k.kullaniciId).ToList();
            List<Yorum> yorum = db.Yorum.Where(x => x.kullaniciId == k.kullaniciId).ToList();
            ViewBag.Yorum = yorum;
            try{
                ViewBag.İlkYorum = yorum[0].yorumTarih.ToString().Substring(0, 10);
            }
            catch
            {
                ViewBag.İlkYorum = "Yorum Bulunmamaktadır";
            }
            try
            {
                ViewBag.İlkSiparis = siparis[0].siparisTarih.ToString().Substring(0, 10);
            }
            catch
            {
                ViewBag.İlkSiparis = "Sipariş Bulunmamaktadır.";
            }
            


            return View(k);
        }


        public ActionResult Siparisler()
        {
            List<OdemeDetay> odemeDetay=new List<OdemeDetay>();
        List<List<SiparisDetay>> siparisDetay=new List<List<SiparisDetay>>();

        
            Kullanici kullanici = (Kullanici)Session["Kullanici"];
            List<Siparis> siparis = db.Siparis.Where(x => x.kullaniciId == kullanici.kullaniciId).ToList();
          
            
            
            foreach(var item in siparis)
            {
                var sonuc= db.SiparisDetay.Where(l => l.siparisId == item.siparisId).ToList();
                var sonuc2= db.OdemeDetay.Where(k => k.siparisId == item.siparisId).SingleOrDefault();
                odemeDetay.Add(sonuc2);
                siparisDetay.Add(sonuc);


                
                

            }
            
            ViewBag.kullanici = kullanici;
            ViewBag.odemeDetay = odemeDetay;
            ViewBag.siparisDetay = siparisDetay;
            
            
           
            return View(siparis);
        }
        [HttpGet]
        public ActionResult BilgilerimiGuncelle(int id)
        {
            Kullanici k = db.Kullanici.Where(x => x.kullaniciId == id).SingleOrDefault();
            if (k == null)
            {
                return RedirectToAction("Index", "Kullanici");
            }
            Kullanici kullanici = (Kullanici)Session["Kullanici"];
            if(k.kullaniciId != kullanici.kullaniciId)
            {
                return RedirectToAction("Index", "Kullanici");
            }
            TempData["Kid"] = id;

            return View(k);
        }

        [HttpPost]
        public ActionResult BilgilerimiGuncelle(Kullanici k)
        {
           
            int id = (int)TempData["Kid"];
            Kullanici kullanici = db.Kullanici.Where(x => x.kullaniciId == id).SingleOrDefault();
            kullanici.kullaniciAd = k.kullaniciAd;
            kullanici.kullaniciSoyad = k.kullaniciSoyad;
            kullanici.kullaniciAdres = k.kullaniciAdres;
            kullanici.kullaniciMail = k.kullaniciMail;
            kullanici.kullaniciTelefon = k.kullaniciTelefon;
            db.SaveChanges();
            TempData["PDuzenle"] = "Profiliniz Başarıyla Düzenlendi!";
            return RedirectToAction("Index", "Kullanici");
        }
        
        public ActionResult UrunIade()
        {
            Kullanici k = (Kullanici)Session["Kullanici"];
            List<Siparis> siparis = db.Siparis.Where(x => x.kullaniciId == k.kullaniciId).ToList();

            return View(siparis);
        }
        [HttpGet]
        public ActionResult UrunIadeForm(int id)
        {
            try
            {
                TempData["Sid"] = id;
            }
            catch
            {
                return RedirectToAction("Siparisler", "Kullanici");
            }
            return View();
        }
        [HttpPost]
        public ActionResult UrunIadeForm(string sebep,string talep)
        {
            GeriIade iade = new GeriIade
            {
                geriIadesiparisId = (int)TempData["Sid"],
                geriIadeSebep = sebep,
                geriIadeTalep = talep
            };
            Siparis siparis = db.Siparis.Where(x => x.siparisId == iade.geriIadesiparisId).SingleOrDefault();
            siparis.siparisDurumId = 5;
            db.GeriIade.Add(iade);
            db.SaveChanges();
            return RedirectToAction("Siparisler", "Kullanici");
        }

        public ActionResult Cikis()
        {

            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}