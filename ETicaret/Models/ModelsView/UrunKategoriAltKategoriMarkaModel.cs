using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models.ModelsView
{
    public class UrunKategoriAltKategoriMarkaModel
    {
        public List<ICollection<Urun>> urun;
        public List<Marka> marka;
        public Kategori kategori;
        public List<AltKategori> altKategori;

        public UrunKategoriAltKategoriMarkaModel()
        {
            altKategori = new List<AltKategori>();
            marka = new List<Marka>();
            kategori = new Kategori();
            urun = new List<ICollection<Urun>>();
        }
    }
}