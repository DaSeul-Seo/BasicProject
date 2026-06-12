using PartyInvites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PartyInvites.Controllers
{
    public class TestController : Controller
    {
        public ViewResult Index()
        {
            int hour = DateTime.Now.Hour;
            ViewBag.Greeting = hour < 12 ? "Good Morning" : "Good Atfernoon";
            return View();
            //return View();
        }

        [HttpGet]
        public ViewResult RsvpForm()
        {
            return View();
        }

        [HttpPost]
        public ViewResult RsvpForm(GuestResponce guestResponce)
        {
            if (ModelState.IsValid)
            {
                // 파티 주최자에게 전자 메일로 응답한다.
                return View("Thanks", guestResponce);
            }else
            {
                // 유효성 오류 존재
                return View();
            }
        }

        // GET: Home
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public string Index()
        //{
        //    return "Hello World";
        //}
    }
}