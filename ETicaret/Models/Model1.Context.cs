﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ETicaretEntities : DbContext
    {
        public ETicaretEntities()
            : base("name=ETicaretEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Abone> Abone { get; set; }
        public virtual DbSet<AltKategori> AltKategori { get; set; }
        public virtual DbSet<BizeUlasin> BizeUlasin { get; set; }
        public virtual DbSet<CocukRutbe> CocukRutbe { get; set; }
        public virtual DbSet<CocukYorum> CocukYorum { get; set; }
        public virtual DbSet<EpostaYayınla> EpostaYayınla { get; set; }
        public virtual DbSet<GeriIade> GeriIade { get; set; }
        public virtual DbSet<Kampanya> Kampanya { get; set; }
        public virtual DbSet<KampanyaTur> KampanyaTur { get; set; }
        public virtual DbSet<Kargo> Kargo { get; set; }
        public virtual DbSet<Kategori> Kategori { get; set; }
        public virtual DbSet<Kullanici> Kullanici { get; set; }
        public virtual DbSet<KullaniciDetay> KullaniciDetay { get; set; }
        public virtual DbSet<Marka> Marka { get; set; }
        public virtual DbSet<OdemeDetay> OdemeDetay { get; set; }
        public virtual DbSet<OdemeTipi> OdemeTipi { get; set; }
        public virtual DbSet<Puan> Puan { get; set; }
        public virtual DbSet<Sehirler> Sehirler { get; set; }
        public virtual DbSet<SifreSifirla> SifreSifirla { get; set; }
        public virtual DbSet<Siparis> Siparis { get; set; }
        public virtual DbSet<SiparisDurum> SiparisDurum { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Ulkeler> Ulkeler { get; set; }
        public virtual DbSet<Urun> Urun { get; set; }
        public virtual DbSet<UrunResim> UrunResim { get; set; }
        public virtual DbSet<Yetki> Yetki { get; set; }
        public virtual DbSet<Yorum> Yorum { get; set; }
        public virtual DbSet<SiparisDetay> SiparisDetay { get; set; }
    }
}
