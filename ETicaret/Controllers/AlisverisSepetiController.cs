using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETicaret.Models;
using ETicaret.Settings;

namespace ETicaret.Controllers
{
    public class AlisverisSepetiController : Controller
    {
        // GET: AlisverisSepeti
        [HttpGet]
        public ActionResult Index()
        {
            Kullanici k = (Kullanici)Session["Kullanici"];
            if(k != null)
            {
                ViewBag.Kullanici = k;
            }
            return View(GetSepet());
        }
        public ActionResult SepeteEkle(int urunId)
        {
            ETicaretEntities db = new ETicaretEntities();
            var urunler = db.Urun.Where(x => x.urunId == urunId).FirstOrDefault();
            if(urunler != null)
            {
                GetSepet().UrunEkle(urunler, 1);
            }
            return RedirectToAction("Index", "Urun");
        }
        public ActionResult SepetSil(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            var urunler = db.Urun.Where(x => x.urunId == id).FirstOrDefault();
            if (urunler != null)
            {
                GetSepet().SepetBosalt();
            }
            return RedirectToAction("Index", "Urun");
        }
        public Sepet GetSepet()
        {
            var sepet = (Sepet)Session["Sepet"];
            if (sepet == null)
            {
                sepet = new Sepet();
                Session["sepet"] = sepet;
            }
            return sepet;

        }

        
        public ActionResult Adres(string name, string telNo, string bolge, string secim)
        {
            ETicaretEntities db = new ETicaretEntities();
            if (Session["Kullanici"] == null)
            {
                TempData["Hata"] = "Alışverişe Devam etmek İçin Kullanici Girişi Yapmalisiniz";
                return RedirectToAction("Index", "AlisverisSepeti", TempData["Hata"]);
            }



            return RedirectToAction("OdemeYontemi", "AlisverisSepeti");
            
        }

        public ActionResult OdemeYontemi()
        {
            return View();
        }
       
        public ActionResult SiparisTamamla(string name,string number, int? guvenlikKod,string ayYıl,string radio)
        {
            Kullanici k = (Kullanici)Session["Kullanici"];
            ETicaretEntities db = new ETicaretEntities();
            Siparis siparis = new Siparis();  
            var sepet = (Sepet)Session["Sepet"];
            var kullanici = (Kullanici)Session["Kullanici"];
            var a = sepet.urunler.ToList();
            var b = sepet.ToplamFiyat();
            int sayi = a.Count();
            string urunler = "";
            siparis.kullaniciId = kullanici.kullaniciId;
            siparis.sepetteMi = true;
            siparis.kargoId = 1;
            siparis.siparisDurumId = 1;
            siparis.odemeTipiId = 1;
            siparis.toplamFiyat =(decimal) b;
           
            db.Siparis.Add(siparis);

            db.SaveChanges();
            ViewBag.Odeme = null;
            if ((name !=null) || (number!= null))
            {
                OdemeDetay odemeDetay = new OdemeDetay();
                odemeDetay.ayYıl = ayYıl;
                odemeDetay.kartUzerindekiAd = name;
                odemeDetay.kartUzerindekiNo = number;
                odemeDetay.odemeTipiId = 1;
                odemeDetay.siparisId = siparis.siparisId;
                ViewBag.Odeme = odemeDetay;
                db.OdemeDetay.Add(odemeDetay);
                db.SaveChanges();
            }


            


          
            foreach (var item in a)
            {
                SiparisDetay siparisDetay = new SiparisDetay();
                siparisDetay.adet = item.Adet;
                urunler += item.Urun.urunAd;
                siparisDetay.fiyat = item.Urun.urunFiyat;
                siparisDetay.urunId = item.Urun.urunId;
                siparisDetay.siparisId = siparis.siparisId;
                db.SiparisDetay.Add(siparisDetay);
                db.SaveChanges();
               
            }
            db.SaveChanges();
            List<SiparisDetay> liste = new List<SiparisDetay>();
            foreach (var item in a)
            {
                SiparisDetay siparisDetay = new SiparisDetay();
                siparisDetay.adet = item.Adet;
                siparisDetay.fiyat = item.Urun.urunFiyat;
                siparisDetay.urunId = item.Urun.urunId;
                siparisDetay.siparisId = siparis.siparisId;
                siparisDetay.Urun = item.Urun;
                siparisDetay.Siparis = siparis;



                liste.Add(siparisDetay);



            }


            foreach(var urun in a)
            {
                if (urun.Urun.urunStokSayisi == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                urun.Urun.urunStokSayisi--;
                var item= db.Urun.Where(x => x.urunId == urun.Urun.urunId).SingleOrDefault();
                item.urunStokSayisi = urun.Urun.urunStokSayisi;
                db.SaveChanges();

            }
            string konu = "Sipariş İşleminiz Onaylanmıştır.";
            string icerik = siparis.siparisId.ToString() + " " + "Nolu siparişiniz işleme alınmıştır.";
            string mesaj = icerik +" "+" Aldığınız Ürünler "+ urunler+" " + "< a href = 'http://localhost:60320/Home/Index>'>  Anasayfaya Dönmek için tıklayınız.";
            Boolean sonuc = Eposta.Gonder(konu, mesaj, k.kullaniciMail) ? true : false;
            



            ViewBag.SiparisDetay = liste.ToList();
            ViewBag.Sayi = sayi;
            sepet.SepetBosalt();
            return View(siparis);
        }
    }
}