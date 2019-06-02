using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ETicaret.Models.ModelsView
{
    public class UrunMarkaCocukYorum
    {
       public List<Urun> urun;
       public List<Marka> marka;
       public List<CocukYorum> cocuk;
        public UrunMarkaCocukYorum()
        {
            marka = new List<Marka>();
            cocuk = new List<CocukYorum>();
            urun = new List<Urun>();
        }


    }
}