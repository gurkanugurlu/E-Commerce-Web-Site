using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ETicaret.Models;
using ETicaret.Settings;
using ETicaret.Controllers;
namespace ETicaret.Models.ModelsView
{
    public class SayilarSiparisModel
    {
       public SayilarSiparisModel()
        {
            Sayilar = new Sayilar();
            Urun=new List<Urun>();
        }
        public Sayilar Sayilar { get; set; }
        public List<Urun> Urun { get; set; }
    }
}