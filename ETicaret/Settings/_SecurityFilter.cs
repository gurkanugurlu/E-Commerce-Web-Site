using ETicaret.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;




namespace ETicaret.Settings
{
    public class _SecurityFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (HttpContext.Current.Session["Kullanici"] == null && (controllerName == "Yonetici"))
            {
                filterContext.Result = new RedirectResult("/Home/Index");
                return;
            }

            if(HttpContext.Current.Session["Kullanici"] != null && (controllerName=="Login"))
            {
                filterContext.Result = new RedirectResult("/Home/Index");
                return;
            }
            if ((string) HttpContext.Current.Session["Yetki"] == "Kullanici" && (controllerName == "Yonetici"))
            {
               filterContext.Result= new RedirectResult("/Home/Index");
                return;
            }
            
            if(HttpContext.Current.Session["Kullanici"]==null && (controllerName == "Kullanici"))
            {
                filterContext.Result = new RedirectResult("/Home/Index");
                return;
            }
            if(HttpContext.Current.Session["Kullanici"]!= null && controllerName == "Kayit")
            {
                filterContext.Result = new RedirectResult("/Home/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}