using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ETicaret.Settings
{
    public class Eposta
    {
        public static bool Gonder(string konu, string mesaj, string gidecekEposta )
        {
            try
            {
                MailMessage eposta = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.live.com");

                
                
                string gonderenEposta = "gurkan_henry_96@hotmail.com";
                string gonderenSifre = "5a1s120ae7d6z";
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(gonderenEposta, gonderenSifre);
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;
                eposta.IsBodyHtml = true;
                eposta.From = new MailAddress(gonderenEposta);
                eposta.To.Add(gidecekEposta);
                eposta.Subject = konu;
                eposta.Body = mesaj;

                smtp.Send(eposta);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
    }
}