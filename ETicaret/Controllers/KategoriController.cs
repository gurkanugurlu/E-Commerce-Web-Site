using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETicaret.Models;

namespace ETicaret.Controllers
{
    public class KategoriController : Controller
    {
        // GET: Kategori
        ETicaretEntities db = new ETicaretEntities();
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id > db.Kategori.Count())
            {
                return RedirectToAction("Index", "Home");
            }

            List<AltKategori> altKategoriler = db.AltKategori.Where(x => x.kategoriId == id).ToList();
            List<ICollection<Urun>> urun = new List<ICollection<Urun>>();
            Kategori kategori = db.Kategori.Where(x => x.kategoriId == id).SingleOrDefault();
            ViewBag.Kategori = kategori;
            ViewBag.AltKategori = altKategoriler;
            Dictionary<double, double> puanlar = new Dictionary<double, double>();
            List<Puan> puan = db.Puan.ToList();
            
            String tarih = DateTime.Now.ToShortDateString();
            ViewBag.Tarih = Convert.ToDateTime(tarih);

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
            HashSet<Marka> marka = new HashSet<Marka>();
            
            foreach (var item in altKategoriler)
            {
                urun.Add(item.Urun);
                foreach(var a in item.Urun)
                {
                    marka.Add(a.Marka);
                }
                

            }
            ViewBag.Markalar = marka;
            ViewBag.MarkaSayi = marka.Count();


            int urunSayisi = 0;
            foreach (var item in altKategoriler)
            {

                urunSayisi += item.Urun.Count();
            }
            ViewBag.UrunSayisi = urunSayisi;

            ViewBag.AltKategoriSayisi = altKategoriler.Count();
            return View(urun);
        }

    }
}