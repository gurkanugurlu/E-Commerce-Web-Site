using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models
{
    public class Sepet
    {
        public List<SepetEleman> urunler = new List<SepetEleman>();
        public void UrunEkle(Urun urun,int adet)
        {
            var satir = urunler.Where(x => x.Urun.urunId == urun.urunId).FirstOrDefault();
            if (satir == null)
            {
                urunler.Add(new SepetEleman() { Urun = urun, Adet = adet });
            }
            else
            {
                satir.Adet += adet;

            }
        }
        public void UrunSil(Urun urun)
        {
            urunler.RemoveAll(x => x.Urun.urunId == urun.urunId);

        }
        public double ToplamFiyat()
        {
            return (double) urunler.Sum(x => x.Urun.urunFiyat * x.Adet);
        }
        public void SepetBosalt()
        {
            urunler.Clear();
        }
        
    }
    public class SepetEleman
    {
        public Urun Urun { get; set; }
        public int Adet { get; set; }
    }
}