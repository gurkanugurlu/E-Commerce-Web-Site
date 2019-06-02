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
    public class YoneticiController : Controller
    {

        ETicaretEntities db = new ETicaretEntities();

        // GET: Yonetici
        public ActionResult Index()
        {
            SayilarSiparisModel sayilarSiparisModel = new SayilarSiparisModel();
            List<Siparis> sp = db.Siparis.ToList();
            double toplamFiyat=0;
            foreach(var item in sp)
            {
                toplamFiyat +=(double) item.toplamFiyat;
            }
            Sayilar sayilar = new Sayilar
            {
                altKategoriSayisi = db.AltKategori.Count(),
                kategoriSayisi = db.Kategori.Count(),
                kullaniciSayisi = db.Kullanici.Count(),
                markaSayisi = db.Marka.Count(),
                urunSayisi = db.Urun.Count(),
                aboneSayisi = db.Abone.Count(),
                ToplamSiparisSayisi = db.Siparis.Count(),
                YorumSayisi = db.Yorum.Count(),
                PuanSayisi = db.Puan.Count(),
                toplamFiyat = toplamFiyat
            };
            List<SiparisDetay> siparis = db.SiparisDetay.ToList();
            SortedDictionary<int, int> encok = new SortedDictionary<int, int>();
            foreach (var item in siparis)
            {
                encok[item.urunId] = 0;

            }
            foreach (var item in siparis)
            {
                encok[item.urunId] += (int)item.adet;

            }
            var sıralamaSonuc = from pair in encok orderby pair.Value descending select pair;
            List<int> idler = new List<int>();

            foreach (var item in sıralamaSonuc)
            {
                idler.Add(item.Key);
                if (idler.Count() > 5)
                {
                    break;

                }
            }

            List<Urun> uruns = new List<Urun>();
            foreach (var items in idler)
            {
                uruns.Add(db.Urun.Where(x => x.urunId == items).SingleOrDefault());

            }
            ViewBag.Populer = uruns.ToList();
            sayilarSiparisModel.Sayilar = sayilar;
            sayilarSiparisModel.Urun = uruns;
            int YoneticiSayisi = db.Kullanici.Where(x => x.yetkiId==1 || x.yetkiId==2).Count();
            ViewBag.YetkiliSayisi = YoneticiSayisi;
            int bize = db.BizeUlasin.Count();
            ViewBag.Bize = bize;
            List<Yorum> yorumlar = db.Yorum.ToList();
            ViewBag.Yorum = yorumlar;
            List<Siparis> sa = db.Siparis.ToList();
            ViewBag.Sipariss = sa;

            List<Kullanici> kl = db.Kullanici.ToList();
            List<Kullanici> atanacaklar = new List<Kullanici>();
            DateTime dt = DateTime.Now;
            foreach(var item in kl)
            {
                if((int) (dt - Convert.ToDateTime(item.kullaniciKayitTarih)).TotalDays <= 30)
                {
                    atanacaklar.Add(item);
                }
            }
            ViewBag.Atanacak = atanacaklar;
            ViewBag.Sayi = atanacaklar.Count;

            

            return View(sayilarSiparisModel);
        }

        [HttpGet]
        public ActionResult Ekle()
        {
            
            Kullanici k = new Kullanici();
            var yetki = db.Yetki.ToList();
            ViewBag.Yetkiler = new SelectList(yetki, "yetkiId", "yetkiAd");
            var sehirler = db.Sehirler.ToList();
            ViewBag.Sehirler = new SelectList(sehirler, "sehirId", "sehirAd");

            return View(k);
        }

        [HttpPost]
        public ActionResult Ekle(Kullanici kullanici, HttpPostedFileBase resimGelen)
        {
            
            if (db.Kullanici.Where(x => x.kullaniciMail == kullanici.kullaniciMail).SingleOrDefault() != null)
            {
                ViewData["Hata"] = "Boyle bir eposta vardır";
                var yetki = db.Yetki.ToList();
                ViewBag.Yetkiler = new SelectList(yetki, "yetkiId", "yetkiAd");
                var sehirler = db.Sehirler.ToList();
                ViewBag.Sehirler = new SelectList(sehirler, "sehirId", "sehirAd");
                return View();
            }


            if (resimGelen == null)
            {
                if (kullanici.kullaniciCinsiyet != null)
                {
                    if (kullanici.kullaniciCinsiyet == false)
                    {
                        kullanici.kullaniciResim = "bos.png";
                    }
                    else
                    {
                        kullanici.kullaniciResim = "bos2.png";
                    }
                }
                else
                {
                    kullanici.kullaniciResim = "bos.png";
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
                    kullanici.kullaniciResim = yeniResimAdi;
                }
            }
            kullanici.kullaniciKayitTarih = DateTime.Now;
            db.Kullanici.Add(kullanici);

            db.SaveChanges();
            TempData["KEkle"] = "Kullanici Eklendi!";
            return RedirectToAction("Kullanicilar", "Yonetici");
        }

        public ActionResult Kullanicilar()
        {
            ETicaretEntities db = new ETicaretEntities();
            List<Kullanici> kullanici = db.Kullanici.ToList();
            return View(kullanici);
        }

        [HttpGet]
        public ActionResult Duzenle(int id)
        {
            TempData["id"] = id;
            ETicaretEntities db = new ETicaretEntities();
            Kullanici kullanici = db.Kullanici.Where(x => x.kullaniciId == id).FirstOrDefault();
            var yetki = db.Yetki.ToList();
            ViewBag.Yetkiler = new SelectList(yetki, "yetkiId", "yetkiAd");
            if (kullanici != null)
            {
                return View(kullanici);
            }


           
            return RedirectToAction("Index", "Yonetici");
        }

        [HttpPost]
        public ActionResult Duzenle(Kullanici kullanici, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();
            string yeniResimAdi = "bos.png";
            int id = (int)TempData["id"];
            Kullanici k = db.Kullanici.Single(x => x.kullaniciId == id);
            if(k.kullaniciResim !=null)
            {
                k.kullaniciResim = k.kullaniciResim;
            }
            else
            {
                k.kullaniciResim = yeniResimAdi;
            }
            if (resimGelen != null)
            {
                yeniResimAdi = new ResimIslem().Ekle(resimGelen);
                k.kullaniciResim = yeniResimAdi;
                // uzantı ve boyut gelirse kullanıcıya bilgilendirme dönüşü yap
            }


            //if (kullanici.kullaniciResim != "bos.png") // bos png yoksa
            //{
            //    //string yol = Server.MapPath("/Content/Resimler/Kullanici/" + eskiKullanici.resim);
            //    //if(System.IO.File.Exists(yol)) // belirtilen kalasörde o dosya var mı
            //    //{
            //    //    System.IO.File.Delete(yol); // eski resmi sil
            //    //}
            //    ResimIslem ri = new ResimIslem();
            //    ri.Sil(kullanici.kullaniciResim);
            //    //new ResimIslem().Sil(eskiKullanici.resim);
            //}

            
            k.kullaniciAd = kullanici.kullaniciAd;
            k.kullaniciAdres = kullanici.kullaniciAdres;
            k.kullaniciMail = kullanici.kullaniciMail;
            k.kullaniciCinsiyet = kullanici.kullaniciCinsiyet;

            k.kullaniciSoyad = kullanici.kullaniciSoyad;
            k.yetkiId = kullanici.yetkiId;
            db.SaveChanges();
            TempData["KDuzenle"] = "Kullanici Başarıyla Duzenlendi!";
            return RedirectToAction("Kullanicilar", "Yonetici");
        }

        [HttpGet]
        public ActionResult Sil(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            Kullanici k = db.Kullanici.Where(x => x.kullaniciId == id).SingleOrDefault();
            List<Yorum> yorum = db.Yorum.Where(x => x.kullaniciId == id).ToList();
            List<Puan> puan = db.Puan.Where(x => x.kullaniciId == id).ToList();
            List<Siparis> siparis=db.Siparis.Where(x=> x.kullaniciId==id).ToList();
            if (yorum != null)
            {
                foreach (var item in yorum)
                {
                    db.Yorum.Remove(item);
                }
            }
            if(puan != null)
            {
                foreach (var item in puan)
                {
                    db.Puan.Remove(item);
                }
            }
            if (siparis != null)
            {
                foreach (var item in siparis)
                {
                    List<SiparisDetay> detay = db.SiparisDetay.Where(x => x.siparisId == item.siparisId).ToList();
                    List<OdemeDetay> odeme = db.OdemeDetay.Where(x => x.siparisId == item.siparisId).ToList();
                    if (detay != null)
                    {
                        foreach (var a in detay)
                        {
                            db.SiparisDetay.Remove(a);

                        }
                    }
                    if(odeme != null)
                    {
                        foreach(var b in odeme)
                        {
                            db.OdemeDetay.Remove(b);
                        }
                    }
                    db.Siparis.Remove(item);
                }
            }
            if (k.kullaniciResim != "bos.png" || k.kullaniciResim != "bos2.png")
            {
                new ResimIslem().Sil(k.kullaniciResim);
            }
            KullaniciDetay kullanici = db.KullaniciDetay.Where(x => x.kullaniciId == id).SingleOrDefault();
            if(kullanici != null) {
                db.KullaniciDetay.Remove(kullanici);
            }
            
            db.Kullanici.Remove(k);
           
            db.SaveChanges();
            TempData["KSil"] = "Kullanici Başariyla Silindi";
            return RedirectToAction("Kullanicilar","Yonetici");
        }

        public ActionResult Kategoriler()
        {
            ETicaretEntities db = new ETicaretEntities();
            List<Kategori> kategori = db.Kategori.ToList();
            return View(kategori);

        }

        [HttpGet]
        public ActionResult KategorilerEkle()
        {

            ETicaretEntities db = new ETicaretEntities();
            Kategori k = new Kategori();
            return View(k);

        }

        [HttpPost]
        public ActionResult KategorilerEkle(Kategori kategori, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();
            if (db.Kategori.Where(x => x.kategoriAd == kategori.kategoriAd).SingleOrDefault() != null)
            {
                ViewData["Hata"] = "Böyle bir Kategori Adı bulunmaktadır";
                return View();
            }
            if (resimGelen == null)
            {
                kategori.kategoriResim = "bos3.png";
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
                    kategori.kategoriResim = yeniResimAdi;
                }
            }
            db.Kategori.Add(kategori);
            db.SaveChanges();
            TempData["KaEkle"] = "Kategori Başariyla Eklendi!";
            return RedirectToAction("Kategoriler", "Yonetici");



        }

        [HttpGet]
        public ActionResult KategorilerDuzenle(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            TempData["kategoriId"] = id;
            Kategori kategori = db.Kategori.Where(x => x.kategoriId == id).FirstOrDefault();
            if(kategori != null)
            {
                return View(kategori);
            }
            return RedirectToAction("Index", "Yonetici");
            

        }

        [HttpPost]
        public ActionResult KategorilerDuzenle(Kategori kategori, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();
            int id = (int)TempData["kategoriId"];
            string yeniResimAdi = "bos3.png";
            Kategori k = db.Kategori.Where(x => x.kategoriId == id).FirstOrDefault();
            if (resimGelen != null)
            {
                yeniResimAdi = new ResimIslem().Ekle(resimGelen);
                // uzantı ve boyut gelirse kullanıcıya bilgilendirme dönüşü yap
            }
            k.kategoriResim = yeniResimAdi;
            k.kategoriAd = kategori.kategoriAd;
            k.kategoriTanim = kategori.kategoriTanim;
            db.SaveChanges();
            TempData["KaDuzenle"] = "Kategori Başariyla Duzenlendi!";
            return RedirectToAction("Kategoriler", "Yonetici");
        }

        [HttpGet]
        public ActionResult KategorilerSil(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            Kategori k = db.Kategori.Where(x => x.kategoriId == id).SingleOrDefault();
            if (k.kategoriResim != "bos.png")
            {
                new ResimIslem().Sil(k.kategoriResim);
            }

            db.Kategori.Remove(k);
            db.SaveChanges();
            TempData["KaSil"] = "Kategori Başariyla Silindi!";
            return RedirectToAction("Kategoriler","Yonetici");
        }

        public ActionResult AltKategoriler()
        {
            ETicaretEntities db = new ETicaretEntities();
           List<AltKategori> alt = db.AltKategori.ToList();

            return View(alt);

        }

        [HttpGet]
        public ActionResult AltKategorilerEkle()
        {
            ETicaretEntities db = new ETicaretEntities();
            AltKategori alt = new AltKategori();
            var kategoriler = db.Kategori.ToList();

            ViewBag.Kategoriler = new SelectList(kategoriler, "kategoriId", "kategoriAd");
            return View(alt);


        }

        [HttpPost]
        public ActionResult AltKategorilerEkle(AltKategori alt, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();
           
            if (db.AltKategori.Where(x => x.altKategoriAd == alt.altKategoriAd).SingleOrDefault() != null)
            {
                ViewData["Hata"] = "Böyle bir Kategori Adı bulunmaktadır";
                return View();
            }
            if (resimGelen == null)
            {
                alt.altKategoriResim = "bos3.png";
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
                    alt.altKategoriResim = yeniResimAdi;
                }
            }
            db.AltKategori.Add(alt);
            db.SaveChanges();
            TempData["AEkle"] = "Alt Kategori Başariyla Eklendi!";
            return RedirectToAction("AltKategoriler", "Yonetici");

            

        }
        [HttpGet]
        public ActionResult AltKategorilerDuzenle(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            TempData["kategoriId"] = id;
            AltKategori kategori = db.AltKategori.Where(x => x.altKategoriId == id).FirstOrDefault();
            if (kategori != null)
            {
                return View(kategori);
            }
            return RedirectToAction("Index", "Yonetici");


        }

        [HttpPost]
        public ActionResult AltKategorilerDuzenle(AltKategori kategori, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();
            int id = (int)TempData["kategoriId"];
            string yeniResimAdi = "bos3.png";
            AltKategori k = db.AltKategori.Where(x => x.altKategoriId == id).FirstOrDefault();
            if (resimGelen != null)
            {
                yeniResimAdi = new ResimIslem().Ekle(resimGelen);
                // uzantı ve boyut gelirse kullanıcıya bilgilendirme dönüşü yap
            }
            k.altKategoriResim = yeniResimAdi;
            k.altKategoriAd = kategori.altKategoriAd;
            k.altKategoriTanim = kategori.altKategoriTanim;
            db.SaveChanges();
            TempData["ADuzenle"] = "Alt Kategori Başariyla Duzenlendi!";
            return RedirectToAction("AltKategoriler", "Yonetici");
        }

        public ActionResult AltKategoriGor(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            var alt = db.AltKategori.Where(x => x.kategoriId == id).ToList();
            return View(alt);


        }
       
        public ActionResult Urunler()
        {
            ETicaretEntities db = new ETicaretEntities();
            Dictionary<int, int> puanlar = new Dictionary<int, int>();
            List<Puan> puan = db.Puan.ToList();
            foreach (var p in puan)
            {

                if (p.urunId != null)
                {

                    puanlar[(int)p.urunId] = 0;
                }

            }
            foreach (var i in puanlar.ToList())
            {
                double katsayi = 0;
                double deger = 0;
                foreach (var a in puan)
                {

                    if (a.urunId == i.Key)
                    {
                        katsayi += 1;
                        deger += (int) a.puan1;
                    }

                }
                double sonuc = deger / katsayi;
                puanlar[i.Key] += (int) sonuc ;
            }
            ViewBag.UrunPuan = puanlar;


            var item = db.Urun.ToList();
            List<Puan> urun = db.Puan.ToList();
            
            return View(item);

        }

        [HttpGet]
        public ActionResult UrunEkle()
        {
            ETicaretEntities db = new ETicaretEntities();
            Urun urun = new Urun();
            List<AltKategori> alt = db.AltKategori.ToList();
            ViewBag.AltKategoriler = new SelectList(alt, "altKategoriId", "altKategoriAd");
            List<Marka> marka = db.Marka.ToList();
            ViewBag.Markalar = new SelectList(marka, "markaId", "markaAd");

            return View(urun);


        }

        [HttpPost]
        public ActionResult UrunEkle(Urun urun, HttpPostedFileBase resimGelen)
        {
            
            
            ETicaretEntities db = new ETicaretEntities();
            urun.urunEklenmeTarih = DateTime.Now.ToShortDateString();
            urun.urunAktifMi = true;
            if (db.Urun.Where(x => x.urunId == urun.urunId).SingleOrDefault() != null)
            {
                ViewData["Hata"] = "Böyle bir Urun  bulunmaktadır";
                return View();
            }
            if (resimGelen == null)
            {
                urun.urunTekResim = "bos3.png";
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
                    urun.urunTekResim = yeniResimAdi;
                }
            }
            Puan puan = new Puan
            {
                kullaniciId = 1,
                urunId = urun.urunId,
                puan1 = 3
            };
            db.Puan.Add(puan);
            db.Urun.Add(urun);
            db.SaveChanges();
            TempData["UEkle"] = "Urun Basariyla Eklendi!";
            return RedirectToAction("Urunler", "Yonetici");
        }

        [HttpGet]
        public ActionResult UrunDuzenle(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            TempData["urunId"] = id;
            List<Marka> marka = db.Marka.ToList();
            ViewBag.Markalar = new SelectList(marka, "markaId", "markaAd");
            List<AltKategori> alt = db.AltKategori.ToList();
            ViewBag.AltKategoriler = new SelectList(alt, "altKategoriId", "altKategoriAd");
            Urun urun = db.Urun.Where(x => x.urunId == id).FirstOrDefault();
            if (urun != null)
            {
                return View(urun);
            }
            return RedirectToAction("Index", "Yonetici");


        }

        [HttpPost]
        public ActionResult UrunDuzenle(Urun urun, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();
            int id = (int)TempData["urunId"];
            string yeniResimAdi = "bos3.png";
            Urun k = db.Urun.Where(x => x.urunId == id).FirstOrDefault();
            if (resimGelen != null)
            {
                yeniResimAdi = new ResimIslem().Ekle(resimGelen);
                // uzantı ve boyut gelirse kullanıcıya bilgilendirme dönüşü yap
            }
            k.urunTekResim = yeniResimAdi;
            k.urunBoyut = urun.urunBoyut;
            k.altKategoriId = urun.altKategoriId;
            k.urunFiyat = urun.urunFiyat;
            k.urunRenk = urun.urunRenk;
            k.urunStokSayisi = urun.urunStokSayisi;
            k.urunYas = urun.urunYas;
            k.urunAd = urun.urunAd;
            k.urunTanim = urun.urunTanim;
            k.markaId = urun.markaId;
            db.SaveChanges();
            TempData["UDuzenle"] = "Urun Basariyla Duzenlendi!";
            return RedirectToAction("Urunler", "Yonetici");
        }

        [HttpGet]
        public ActionResult UrunSil(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            Urun k = db.Urun.Where(x => x.urunId == id).SingleOrDefault();
            if (k.urunTekResim != "bos3.png")
            {
                new ResimIslem().Sil(k.urunTekResim);
            }

            Puan puan = db.Puan.Where(x => x.urunId == k.urunId).SingleOrDefault();
            db.Urun.Remove(k);
            db.Puan.Remove(puan);
            db.SaveChanges();
            TempData["USil"] = "Urun Basariyla Silindi!";
            return RedirectToAction("Urunler","Yonetici");
        }

        public ActionResult UrunleriGor(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            List<Urun> urun = db.Urun.Where(x => x.altKategoriId == id).ToList();
            string isim;
            if (urun != null)
            {
                foreach (var item in urun)
                {
                    isim = item.AltKategori.altKategoriAd;
                    ViewBag.KategoriAd = isim;
                    break;
                }
            }
            
           

            return View(urun);
;        }

        public ActionResult Marka()
        {
            ETicaretEntities db = new ETicaretEntities();
            List<Marka> marka = db.Marka.ToList();
            return View(marka);
        }

        [HttpGet]
        public ActionResult MarkaEkle()
        {
            ETicaretEntities db = new ETicaretEntities();
            Marka marka = new Marka();
            return View(marka);
        }

        [HttpPost]
        public ActionResult MarkaEkle(Marka marka, HttpPostedFileBase resimGelen)
        {
            ETicaretEntities db = new ETicaretEntities();

            if (db.Marka.Where(x => x.markaId == marka.markaId).SingleOrDefault() != null)
            {
                ViewData["Hata"] = "Böyle bir Marka Adı bulunmaktadır";
                return View();
            }
            if (resimGelen == null)
            {
                marka.markaResim = "bos3.png";
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
                    marka.markaResim = yeniResimAdi;
                }
            }
            db.Marka.Add(marka);
            db.SaveChanges();
            TempData["MEkle"] = "Marka Basariyla Eklendi!";
            return RedirectToAction("Marka", "Yonetici");

        }

        [HttpGet]
        public ActionResult MarkaDuzenle(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            TempData["markaId"] = id;
            Marka marka = db.Marka.Where(x => x.markaId == id).FirstOrDefault();
            if (marka != null)
            {
                return View(marka);
            }
            return RedirectToAction("Index", "Yonetici");


        }

        [HttpPost]
        public ActionResult MarkaDuzenle(Marka marka, HttpPostedFileBase resimGelen)
        {
           
            int id = (int)TempData["markaId"];
            string yeniResimAdi = "bos3.png";
            Marka k = db.Marka.Where(x => x.markaId == id).FirstOrDefault();
            if (resimGelen != null)
            {
                yeniResimAdi = new ResimIslem().Ekle(resimGelen);
                // uzantı ve boyut gelirse kullanıcıya bilgilendirme dönüşü yap
            }
            k.markaResim = yeniResimAdi;
            k.markaAd = marka.markaAd;
            k.markaTanim = marka.markaTanim;
            db.SaveChanges();
            TempData["MDuzenle"] = "Marka Basariyla Duzenlendi!";
            return RedirectToAction("Marka", "Yonetici");
        }

        public ActionResult MarkalarGör(int id)
        {
            
            var alt = db.Urun.Where(x => x.markaId == id).ToList();
            return View(alt);


        }

        public ActionResult Abone()
        {
            List<Abone> aboneler = db.Abone.ToList();
            ViewBag.Aboneler = db.Abone.Count();
            return View(aboneler);
        }

        public ActionResult BizeUlasin()
        {
            List<BizeUlasin> bize = db.BizeUlasin.ToList();
            ViewBag.BizeUlasin = db.BizeUlasin.Count();
            return View (bize);
        }

        public ActionResult Siparisler()
        {
            List<Siparis> siparis = db.Siparis.ToList();
            
            return View(siparis);

        }

        public ActionResult OdemeDetay(int id)
        {
            OdemeDetay odeme = db.OdemeDetay.Where(x => x.siparisId == id).SingleOrDefault();

            return View(odeme);
        }

        public ActionResult SiparisDetay(int id)
        {
            List<SiparisDetay> siparis = db.SiparisDetay.Where(x => x.siparisId == id).ToList();

            return View(siparis);
        }

        [HttpGet]
        public ActionResult SiparisDuzenle(int id)
        {
            Siparis siparis = db.Siparis.Where(x => x.siparisId == id).SingleOrDefault();
            var durum = db.SiparisDurum.ToList();
            TempData["SId"] = id;
            ViewBag.Durum = new SelectList(durum, "siparisDurumId", "siparisDurum1");
            return View(siparis);
        }

        [HttpPost]
        public ActionResult SiparisDuzenle(Siparis siparis)
        {
            int id = (int)TempData["SId"];
            Siparis s = db.Siparis.Where(x => x.siparisId ==id).SingleOrDefault();
            s.toplamFiyat = siparis.toplamFiyat;
            s.siparisDurumId = siparis.siparisDurumId;
            if (s.siparisDurumId == 3)
            {
                s.SiparisTeslimTarih = DateTime.Now;
            }
            db.SaveChanges();
            return RedirectToAction("Siparisler", "Yonetici");
        }

        public ActionResult Ayarlar()
        {

            return View();
        }
        
        public ActionResult Yorumlar()
        {
            List<Yorum> yorumlar = db.Yorum.ToList();
            ViewBag.Yorumlar = yorumlar.Count;
            return View(yorumlar);
        }

        public ActionResult YorumSil(int id)
        {
            ETicaretEntities db = new ETicaretEntities();
            Yorum k = db.Yorum.Where(x => x .yorumId== id).SingleOrDefault();
           

            db.Yorum.Remove(k);
            db.SaveChanges();
            TempData["KaSil"] = "Yorum Başariyla Silindi!";
            return RedirectToAction("Kategoriler", "Yonetici");
        }

        public ActionResult SiparisDetayGor(int id)
        {
            List<SiparisDetay> siparis = db.SiparisDetay.Where(x => x.urunId == id).ToList();
            ViewBag.Siparis = siparis.Count;
            return View(siparis);
        }
        
        public ActionResult KullaniciSiparisler(int id)
        {
            
            Kullanici k = db.Kullanici.Where(x => x.kullaniciId == id).SingleOrDefault();
            List<Siparis> siparis = db.Siparis.Where(x => x.kullaniciId == k.kullaniciId).ToList();

            return View(siparis);
        }
        
        public ActionResult EPostalar()
        {
            List<EpostaYayınla> epostas = db.EpostaYayınla.ToList();
            return View(epostas);
        }

        [HttpGet]

        public ActionResult EPostaEkle()
        {
            EpostaYayınla eposta = new EpostaYayınla();

            return View(eposta);
        }

        [HttpPost]

        public ActionResult EPostaEkle(EpostaYayınla eposta)
        {
            List<Abone> abone = db.Abone.ToList();
            string konu = eposta.ePostaKonu;
            string eklenti = " < a href = 'http://localhost:60320/Home/Index";
            string mesaj = eposta.ePostaIcerik + eklenti;
            foreach (var a in abone) {
                Eposta.Gonder(konu, mesaj, a.aboneMail);
                    }
            db.EpostaYayınla.Add(eposta);
            db.SaveChanges();
            return View(eposta);
        }

        public ActionResult GeriIade()
        {
            List<GeriIade> geri = db.GeriIade.ToList();

            return View(geri);
        }
        
        public ActionResult Kampanya()
        {
            List<Kampanya> kampanyalar = db.Kampanya.ToList();

            return View(kampanyalar);
        }

        [HttpGet]

        public ActionResult KampanyaSec()
        {
            List<KampanyaTur> tur = db.KampanyaTur.ToList();
            ViewBag.Tur = new SelectList(tur, "kampanyaTurId", "kampanyaTurAd");
            return View();
        }

        [HttpPost]

        public ActionResult KampanyaSec(int kampanyaTurId)
        {
            TempData["Tur"] = kampanyaTurId;
            return RedirectToAction("KampanyaOlustur", "Yonetici");
        }

        [HttpGet]

        public ActionResult KampanyaDuzenle(int id)
        {
            Kampanya kampanya = db.Kampanya.Where(x => x.kampanyaId == id).SingleOrDefault();
            List<KampanyaTur> tur = db.KampanyaTur.ToList();

            ViewBag.Tur = new SelectList(tur, "kampanyaTurId", "kampanyaTurAd");

            return View(kampanya);

        }

        [HttpPost]

        public ActionResult KampanyaDuzenle(Kampanya k)
        {
            Kampanya kampanya = db.Kampanya.Where(x => x.kampanyaId == k.kampanyaId).SingleOrDefault();
            List<Urun> urunler = new List<Urun>();
            if (kampanya.kampanyaId == 1)

            {
                List<AltKategori> altKategori = db.AltKategori.ToList();



                foreach (var a in altKategori)

                {
                    List<Urun> u = db.Urun.Where(x => x.altKategoriId == a.altKategoriId).ToList();
                    foreach (var b in u)
                    {
                        urunler.Add(b);
                    }

                }

                foreach (var item in urunler)

                {

                    item.urunFiyat = item.urunFiyat - (item.urunFiyat * kampanya.kampanyaIndirimOran / 100);
                    item.urunKampanyaVarMı = true;
                }

            }
            else if (kampanya.kampanyaId == 2)
            {
                List<AltKategori> altKategori = db.AltKategori.ToList();



                foreach (var a in altKategori)

                {
                    List<Urun> u = db.Urun.Where(x => x.altKategoriId == a.altKategoriId).ToList();
                    foreach (var b in u)
                    {
                        urunler.Add(b);
                    }

                }
                foreach (var item in urunler)

                {

                    item.urunFiyat = item.urunFiyat - (item.urunFiyat * kampanya.kampanyaIndirimOran / 100);
                    item.urunKampanyaVarMı = true;
                }
            }
            else if (kampanya.kampanyaId == 3)
            {
                List<Marka> marka = db.Marka.ToList();

                foreach (var item in marka)
                {
                    List<Urun> u = db.Urun.Where(x => x.markaId == item.markaId).ToList();
                    foreach (var a in u)
                    {
                        urunler.Add(a);
                    }
                }
                foreach (var item in urunler)

                {

                    item.urunFiyat = item.urunFiyat - (item.urunFiyat * kampanya.kampanyaIndirimOran / 100);
                    item.urunKampanyaVarMı = true;
                }
            }
            TempData["KADuzenle"] = "Kampanya başarıyla düzenlendi";
            db.SaveChanges();
            return RedirectToAction("Kampanya", "Yonetici");

        }

        [HttpGet]

        public ActionResult KampanyaOlustur()
        {
            Kampanya kampanya = new Kampanya();
            kampanya.kampanyaTurId = (int)TempData["Tur"];
            if (kampanya.kampanyaTurId == 1)
            {
                ViewBag.Deger = 1;
                List<Kategori> kategori = db.Kategori.ToList();
                ViewBag.Sonuc = new SelectList(kategori,"kategoriId","kategoriAd");
            }
            else if (kampanya.kampanyaTurId == 2)
            {
                List<AltKategori> altKategori = db.AltKategori.ToList();
                ViewBag.Deger = 2;
                ViewBag.Sonuc = new SelectList(altKategori, "altkategoriId", "altkategoriAd");
            }
            else if (kampanya.kampanyaTurId == 3)
            {
                ViewBag.Deger = 3;
                List<Marka> marka = db.Marka.ToList();
                
                
                ViewBag.Sonuc = new SelectList(marka, "markaId", "markaAd");
            }
            TempData["T"] = kampanya.kampanyaTurId;
                return View(kampanya);

        }

        [HttpPost]

        public ActionResult KampanyaOlustur(Kampanya kampanya,int? markaId,int? altKategoriId,int? kategoriId)
        {
            kampanya.kampanyaTurId = (int)TempData["T"];
            kampanya.kampanyaBaslangicTarih = DateTime.Now;
            List<Urun> urunler = new List<Urun>();
            if (kampanya.kampanyaTurId==1)

            {
                
                Kategori kategori = db.Kategori.Where(x => x.kategoriId == kategoriId).SingleOrDefault();
                List<AltKategori> altKategori = db.AltKategori.Where(x => x.kategoriId == kategori.kategoriId).ToList();
                kampanya.kampanyaTipId = kategoriId;
                

                foreach(var a in altKategori)
                
                {
                    kampanya.kampanyaTip = a.altKategoriAd;
                    List<Urun> u = db.Urun.Where(x => x.altKategoriId == a.altKategoriId).ToList();
                    foreach(var b in u)
                    {
                        urunler.Add(b);
                    }

                }

                foreach(var item in urunler)

                {
                    item.urunEskiFiyat = item.urunFiyat;
                    item.urunFiyat = item.urunFiyat - (item.urunFiyat * kampanya.kampanyaIndirimOran / 100);
                    item.urunKampanyaVarMı = true;
                }

            }  

            else if (kampanya.kampanyaTurId == 2)
            {
                List<AltKategori> altKategori = db.AltKategori.Where(x => x.altKategoriId == altKategoriId).ToList();
                kampanya.kampanyaTipId = altKategoriId;


                foreach (var a in altKategori)

                {
                    kampanya.kampanyaTip = a.altKategoriAd;
                    List<Urun> u = db.Urun.Where(x => x.altKategoriId == a.altKategoriId).ToList();
                    foreach (var b in u)
                    {
                        urunler.Add(b);
                    }

                }
                foreach (var item in urunler)

                {
                    item.urunEskiFiyat = item.urunFiyat;
                    item.urunFiyat = item.urunFiyat - (item.urunFiyat * kampanya.kampanyaIndirimOran / 100);
                    item.urunKampanyaVarMı = true;
                }
            }
            else if (kampanya.kampanyaTurId == 3)
            {
                List<Marka> marka = db.Marka.Where(x => x.markaId == markaId).ToList();
                kampanya.kampanyaTipId = markaId;
                foreach (var item in marka)
                {
                    kampanya.kampanyaTip = item.markaAd;
                    List<Urun> u = db.Urun.Where(x => x.markaId == item.markaId).ToList();
                    foreach(var a in u)
                    {
                        urunler.Add(a);
                    }
                }
                foreach (var item in urunler)

                {
                    item.urunEskiFiyat = item.urunFiyat;
                    item.urunFiyat = item.urunFiyat - (item.urunFiyat * kampanya.kampanyaIndirimOran / 100);
                    item.urunKampanyaVarMı = true;
                }
            }

            db.Kampanya.Add(kampanya);

            db.SaveChanges();
            TempData["KAEkle"] = "Kampanya başarıyla eklendi";
            return RedirectToAction("Kampanya", "Yonetici");
        }

        [HttpGet]

        public ActionResult KampanyaIptal(int id)
        {
            Kampanya kampanya = db.Kampanya.Where(x => x.kampanyaId == id).SingleOrDefault();
            List<Urun> urunler = new List<Urun>();
            if (kampanya.kampanyaTurId == 1)

            {
                Kategori kategori = db.Kategori.Where(x => x.kategoriId == kampanya.kampanyaTipId ).SingleOrDefault();
                List<AltKategori> altKategori = db.AltKategori.Where(x => x.altKategoriId == kategori.kategoriId).ToList();



                foreach (var a in altKategori)

                {
                    List<Urun> u = db.Urun.Where(x => x.altKategoriId == a.altKategoriId).ToList();
                    foreach (var b in u)
                    {
                        urunler.Add(b);
                    }

                }

                foreach (var item in urunler)

                {

                    item.urunFiyat = item.urunEskiFiyat;
                    item.urunKampanyaVarMı = false;
                }

            }

            else if (kampanya.kampanyaTurId == 2)
            {
                List<AltKategori> altKategori = db.AltKategori.Where(x => x.altKategoriId == kampanya.kampanyaTipId).ToList();



                foreach (var a in altKategori)

                {
                    List<Urun> u = db.Urun.Where(x => x.altKategoriId == a.altKategoriId).ToList();
                    foreach (var b in u)
                    {
                        urunler.Add(b);
                    }

                }
                foreach (var item in urunler)

                {

                    item.urunFiyat = item.urunEskiFiyat;
                    item.urunKampanyaVarMı = false;

                }
            }
            else if (kampanya.kampanyaId == 3)
            {
                List<Marka> marka = db.Marka.Where(x => x.markaId == kampanya.kampanyaTipId).ToList();

                foreach (var item in marka)
                {
                    List<Urun> u = db.Urun.Where(x => x.markaId == item.markaId).ToList();
                    foreach (var a in u)
                    {
                        urunler.Add(a);
                    }
                }
                foreach (var item in urunler)

                {
                    item.urunFiyat = item.urunEskiFiyat;
                    item.urunKampanyaVarMı = false;
                }
            }
            kampanya.kampanyabitisTarih = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Kampanya", "Yonetici");
        }
        
       



    }
   

    
}