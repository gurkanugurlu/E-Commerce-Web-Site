//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ETicaret.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Kullanici
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kullanici()
        {
            this.Puan = new HashSet<Puan>();
            this.Siparis = new HashSet<Siparis>();
            this.Yorum = new HashSet<Yorum>();
        }
    
        public int kullaniciId { get; set; }
        public string kullaniciAd { get; set; }
        public string kullaniciSoyad { get; set; }
        public string kullaniciTelefon { get; set; }
        public string kullaniciMail { get; set; }
        public string kullaniciSifre { get; set; }
        public Nullable<bool> kullaniciAktifMi { get; set; }
        public string kullaniciAdres { get; set; }
        public Nullable<int> yetkiId { get; set; }
        public string kullaniciResim { get; set; }
        public Nullable<bool> kullaniciCinsiyet { get; set; }
        public string kullaniciSoru { get; set; }
        public string kullaniciSoruCevap { get; set; }
        public Nullable<System.DateTime> kullaniciKayitTarih { get; set; }
    
        public virtual Yetki Yetki { get; set; }
        public virtual KullaniciDetay KullaniciDetay { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Puan> Puan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Siparis> Siparis { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Yorum> Yorum { get; set; }
    }
}