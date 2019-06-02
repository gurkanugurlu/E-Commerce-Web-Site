using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ETicaret.Models;
namespace ETicaret.Controllers
{
    public class UrunController : Controller
    {
        ETicaretEntities db = new ETicaretEntities();
        // GET: Urun
        public ActionResult Index(String Sorting_Order)
        {
            ViewBag.Markalar = db.Marka.ToList();
            ViewBag.MarkaSayi = db.Marka.Count();
            ViewBag.Kategori = db.Kategori.ToList();
            ViewBag.AltKategori = db.AltKategori.ToList();
            List<SiparisDetay> siparis = db.SiparisDetay.ToList();
            SortedDictionary<int, int> encok = new SortedDictionary<int, int>();
            foreach(var item in siparis)
            {
                encok[item.urunId] = 0;
                
            }
            foreach(var item in siparis)
            {
                encok[item.urunId] += (int)item.adet;

            }
            var sıralamaSonuc = from pair in encok orderby pair.Value descending select pair;
            List<int> idler = new List<int>();
            
            foreach(var item in sıralamaSonuc)
            {
                idler.Add(item.Key);
                if (idler.Count() > 5) {
                    break;

                }
            }
            
            List<Urun> uruns = new List<Urun>();
            foreach(var items in idler)
            {
                uruns.Add(db.Urun.Where(x => x.urunId == items).SingleOrDefault());

            }

            ViewBag.Populer = uruns.ToList();
            Dictionary<double, double> puanlar = new Dictionary<double, double>();
            List<Puan> puan = db.Puan.ToList();
            foreach (var p in puan)
            {

                if (p.urunId != null)
                {

                    puanlar[(double)p.urunId] = 0;
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
                        deger += (double)a.puan1;
                    }

                }
                double sonuc = deger / katsayi;
                puanlar[i.Key] += sonuc;
            }
            ViewBag.UrunPuan = puanlar;


            List<Urun> urun = db.Urun.ToList();
            String tarih = DateTime.Now.ToShortDateString();
            ViewBag.Tarih = Convert.ToDateTime(tarih);


            if (TempData["ara"] != null)
            {
                return View(TempData["ara"]);
            }
            var kayitlar = db.Urun.ToList();

            if (Sorting_Order != null)
            {
                switch (Sorting_Order)
                {
                    case "Fiyata_Gore":
                        kayitlar = kayitlar.OrderBy(x => x.urunFiyat).ToList();

                        break;
                    case "Zamana_Gore":
                        kayitlar = kayitlar.OrderBy(x => Convert.ToDateTime(x.urunEklenmeTarih)).ToList();
                        break;
                    case "Ada_Gore":
                        kayitlar = kayitlar.OrderBy(x => x.urunAd).ToList();
                        break;
                    case "İndirime_Gore":
                        kayitlar = kayitlar.OrderByDescending(x => x.urunKampanyaVarMı == true).ToList();
                        break;
                    default:
                        kayitlar = kayitlar.OrderByDescending(x => x.Marka.markaAd).ToList();
                        break;
                }
                return View(kayitlar.ToList());
            }
            return View(urun);


        }
        [HttpGet]
        public ActionResult UrunDetay(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id > db.Urun.Max(x=> x.urunId))
            {
                return RedirectToAction("Index", "Home");
            }

            Dictionary<double, double> puanlar = new Dictionary<double, double>();
            List<Puan> puan = db.Puan.ToList();

            foreach (var p in puan)
            {

                if (p.urunId != null)
                {

                    puanlar[(double)p.urunId] = 0;
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
                        deger += (double)a.puan1;
                    }

                }
                double sonuc = deger / katsayi;
                puanlar[i.Key] += sonuc;
            }
            ViewBag.UrunPuan = puanlar;
            Urun urun = db.Urun.Where(x => x.urunId == id).SingleOrDefault();
            List<Yorum> yorum = db.Yorum.Where(x => x.urunId == id).ToList();
            ViewBag.UrunYorum = yorum;
            List<Urun> uruns = db.Urun.Where(x => x.altKategoriId == urun.altKategoriId).ToList();
            uruns.Remove(urun);
            ViewBag.Ur = uruns;
            return View(urun);
        }

        [HttpPost]
        public ActionResult UrunDetay(string name,string email,double? puan,string mesaj,int? id)
        {
           
            Yorum yorum = new Yorum();
            Puan p = new Puan();
            
            Kullanici kullanici = (Kullanici)Session["Kullanici"]; ;
            if (puan > 5 || puan<0)
            {
                TempData["Mesaj"] = "Sayın" +" "+ kullanici.kullaniciAd + " " + kullanici.kullaniciSoyad+" " + "5 ten büyük veya 0 dan küçük puan giremezsiniz.Teşekkürler";
                return RedirectToAction("Index", "Urun");
            }
            yorum.kullaniciId = kullanici.kullaniciId;
            yorum.urunId = id;
            yorum.yorumIcerik = mesaj;
            yorum.yorumTarih = DateTime.Now;
            p.kullaniciId = kullanici.kullaniciId;
            p.puan1 =(int) puan;
            p.urunId = id;

            db.Puan.Add(p);
            db.Yorum.Add(yorum);
            db.SaveChanges();
            return RedirectToAction("UrunDetay", "Urun");
        }

        [HttpGet]
        public ActionResult Ara()
        {
            return RedirectToAction("Index", "Urun");
        }
        [HttpPost]
        public ActionResult Ara(string aramaString)
        {
            
            var urun = db.Urun.ToList();


            if (!String.IsNullOrEmpty(aramaString))
            {
                
                var a = urun.Where(x => x.urunAd.ToUpper().Contains(aramaString.ToUpper())|| x.Marka.markaAd.ToUpper().Contains(aramaString.ToUpper()));
                TempData["ara"] = a.ToList();
                return RedirectToAction("Index", "Urun");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult KategoriAra(List<int> id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Urun");
            }
            List<Kategori> kategori = new List<Kategori>();
            List<AltKategori> altKategori = new List<AltKategori>();
            List<Urun> urun = new List<Urun>();
            foreach(var item in id)
            {
                Kategori k = db.Kategori.Where(x => x.kategoriId == item).SingleOrDefault();
                kategori.Add(k);
            }
            foreach(var i in kategori)
            {
                List<AltKategori> a = db.AltKategori.Where(x => x.kategoriId == i.kategoriId).ToList();
                foreach(var item in a)
                {
                    altKategori.Add(item);
                }
               
                
            }
            foreach(var b in altKategori)
            {
                List<Urun> uruns = db.Urun.Where(x => x.altKategoriId == b.altKategoriId).ToList();
                foreach(var item in uruns)
                {
                    urun.Add(item);
                }
            }


            TempData["ara"] = urun.ToList();
            return RedirectToAction("Index", "Urun");

        }
        public ActionResult AltKategoriAra(List<int> id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Urun");
            }
           
            List<AltKategori> altKategori = new List<AltKategori>();
            List<Urun> urun = new List<Urun>();
            foreach(var item in id)
            {
                AltKategori a = db.AltKategori.Where(x => x.altKategoriId == item).SingleOrDefault();
                altKategori.Add(a);
            }
            foreach (var b in altKategori)
            {
                List<Urun> uruns = db.Urun.Where(x => x.altKategoriId == b.altKategoriId).ToList();
                foreach (var item in uruns)
                {
                    urun.Add(item);
                }
            }


            TempData["ara"] = urun.ToList();
            return RedirectToAction("Index", "Urun");

        }
        public ActionResult YasAra(List<int> id)
        {
            List <Urun> urun= new List<Urun>();
            foreach(var item in id)
            {
                if (item == 2)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunYas <= 2 && x.urunYas > 0).ToList();
                    foreach(var a in uruns)
                    {
                        urun.Add(a);
                    }

                }
                if(item == 3)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunYas <= 6 && x.urunYas > 2).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }
                }
                
                if (item == 6)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunYas <= 10 && x.urunYas > 6).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }

                }
                if (item == 10)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunYas >10 ).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }
                }
            }
            TempData["ara"] = urun.ToList();
            return RedirectToAction("Index", "Urun");
        }
        public ActionResult FiyatAra(List<int> id)
        {
            List<Urun> urun = new List<Urun>();
            foreach (var item in id)
            {
                if (item == 0)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunFiyat <= 25 && x.urunFiyat > 0).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }

                }
                if (item == 25)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunFiyat <= 50 && x.urunFiyat > 25).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }
                }

                if (item == 50)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunFiyat <= 75 && x.urunFiyat > 50).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }

                }
                if (item == 75)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunFiyat <= 100 && x.urunFiyat > 75).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }
                }
                if (item == 100)
                {
                    List<Urun> uruns = db.Urun.Where(x => x.urunFiyat >100).ToList();
                    foreach (var a in uruns)
                    {
                        urun.Add(a);
                    }
                }
            }
            TempData["ara"] = urun.ToList();
            return RedirectToAction("Index", "Urun");
        }
        public ActionResult MarkaAra(List<int> id)
        {
            List<Urun> urun = new List<Urun>();

            foreach(var item in id)
            {
                Marka marka = db.Marka.Where(x => x.markaId == item).SingleOrDefault();
                var uruns = (db.Urun.Where(x => x.markaId == marka.markaId)).ToList();
                foreach(var a in uruns)
                {
                    urun.Add(a);
                }
            }
            TempData["ara"] = urun.ToList();
            return RedirectToAction("Index", "Urun","?==markaid");
        }





    }
}