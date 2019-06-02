using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ETicaret.Models;
namespace ETicaret.Controllers
{
    public class AltKategoriController : Controller
    {
        ETicaretEntities db = new ETicaretEntities();
        // GET: AltKategori
        public ActionResult Index(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id > db.AltKategori.Count())
            {
                return RedirectToAction("Index", "Home");
            }
            AltKategori altKategori = db.AltKategori.Where(x => x.altKategoriId == id).SingleOrDefault();
            List<Urun> urun = db.Urun.Where(x => x.altKategoriId == altKategori.altKategoriId).ToList();
            ViewBag.UrunSayisi = urun.Count();
            HashSet<Marka> marka = new HashSet<Marka>();

            foreach (var item in urun)
            {
                marka.Add(item.Marka);


            }
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
            ViewBag.Markalar = marka;
            ViewBag.MarkaSayi = marka.Count();
            return View(urun);
        }
    }
}