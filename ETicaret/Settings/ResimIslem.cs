using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace ETicaret.Settings
{
    public class ResimIslem
    {
        public string Ekle(HttpPostedFileBase orjResim)
        {
            string uzanti = Path.GetExtension(orjResim.FileName);
            if (!(uzanti == ".jpg" || uzanti == ".png"))
            {
                return "uzanti";
            }

            if (orjResim.ContentLength > 100000000)
            {
                return "boyut";
            }

            string ad = Guid.NewGuid().ToString() + uzanti;
            Bitmap res = new Bitmap(Image.FromStream(orjResim.InputStream));
            res.Save(HttpContext.Current.Server.MapPath("/Content/Resimler/Kullanici/" + ad));
            return ad;
        }

        public string Sil(string resimAdi)
        {
            string yol = HttpContext.Current.Server.MapPath("/Content/Resimler/Kullanici/" + resimAdi);
            if (System.IO.File.Exists(yol)) // belirtilen kalasörde o dosya var mı
            {
                System.IO.File.Delete(yol); // eski resmi sil
            }
            else
            {
                return "Bulunamadı";
            }
            return "Silindi";
        }
    }

}