using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using WebApplication3.Controllers;
using System.Web.Security;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Session["email"] = null;
            //FormsAuthentication.SignOut();
            //Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetNoStore();
            //Session.Abandon();
            return View();
        }


        public ActionResult SubscribePack(string useremail, int pid, string pname, float pcost)
        {

            DAL dal = new DAL();
            bool CheckAlreadySub = true;
            if (dal.CheckSubscribedPack(pid, useremail) == "")
                CheckAlreadySub = false;

            ViewData["AlreadySub"] = CheckAlreadySub;

            Pack p = new Pack
            {
                PackId = pid,
                PackName = pname,
                PackCost = pcost

            };

            List<ChannelsInPack> packdetails = dal.GetChannelsInPack(pid, useremail);
            ViewData["packinfo"] = p;
            ViewData["email"] = useremail;
            return View(packdetails);
        }


        public ActionResult SubscribeChannel(string useremail, int cid, string cname, float ccost)
        {
            DAL dal = new DAL();
            bool CheckAlreadySub = true;
            if (dal.CheckSubscribed(cid, useremail) == "")
                CheckAlreadySub = false;

            ViewData["AlreadySub"] = CheckAlreadySub;

            if (CheckAlreadySub)
            {
                DateTime d = dal.getexpirydate(cid, useremail);
                ViewData["ExpiryDate"] = d;
                ViewData["DaysLeft"] = (d-DateTime.Now).TotalDays;
            }

            Channel c = new Channel
            {
                ChannelId = cid,
                ChannelName = cname,
                ChannelCost = ccost
            };
            ViewData["email"] = useremail;
            return View(c);
            
        }



        [HttpGet]
        public ActionResult Welcome()
        {
            if (Session["email"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            DAL dal = new DAL();
            Customer c1 = dal.FetchCustomer(Session["email"].ToString());

            Session["email"] = c1.Email;
            List<Channel> channelsList = dal.AllChannels();
            List<Pack> packsList = dal.AllPacks();
            ViewData["CustomerName"] = c1.Name;
            Session["name"] = c1.Name;
            ViewData["email"] = c1.Email;
            ViewData["AllChannels"] = channelsList;
            ViewData["AllPacks"] = packsList;

            List<int> chids = new List<int>();
            foreach (var ch in channelsList)
                chids.Add(ch.ChannelId);
            List<bool> subornot = dal.issubornot(chids, Session["Email"].ToString());
            ViewData["SuborNot"] = subornot;

            List<int> packids = new List<int>();
            foreach (var pack in packsList)
                packids.Add(pack.PackId);
            List<bool> subornot1 = dal.issubornot1(packids, Session["Email"].ToString());
            ViewData["SuborNot1"] = subornot1;

            List<bool> subornot2 = dal.issubornotinanypack(chids, packids, Session["Email"].ToString());
            ViewData["SuborNot2"] = subornot2;

            ViewData["ProfileId"] = c1.ProfileId;


            return View("Welcome");
        }

        [HttpPost]
        public ActionResult Welcome(Customer c)
        {
            //if (Session["email"] == null)
            //{
            //    ViewBag.Message = "Your session has expired. Please login again!";
            //    ViewBag.Type = "danger";
            //    return View("Index");
            //}
            
            DAL dal = new DAL();
            if (c.Email!=null && c.Password!=null && dal.IsEmailPasswordPairPresent(c.Email,c.Password))
            {
                Customer c1 = dal.FetchCustomer(c.Email);
             
                Session["email"] = c1.Email;
                List<Channel> channelsList = dal.AllChannels();
                List<Pack> packsList = dal.AllPacks();
                ViewData["CustomerName"] = c1.Name;
                Session["name"] = c1.Name;
                ViewData["email"] = c1.Email;
                ViewData["AllChannels"] = channelsList;
                ViewData["AllPacks"] = packsList;
                List<int> chids = new List<int>();
                foreach (var ch in channelsList)
                    chids.Add(ch.ChannelId);
                List<bool> subornot = dal.issubornot(chids, Session["Email"].ToString());
                ViewData["SuborNot"] = subornot;

                List<int> packids = new List<int>();
                foreach (var pack in packsList)
                    packids.Add(pack.PackId);
                List<bool> subornot1 = dal.issubornot1(packids, Session["Email"].ToString());
                ViewData["SuborNot1"] = subornot1;

                List<bool> subornot2 = dal.issubornotinanypack(chids, packids, Session["Email"].ToString());
                ViewData["SuborNot2"] = subornot2;

                ViewData["ProfileId"]=c1.ProfileId;

                return View("Welcome");
            }
            else
            {
                ViewBag.Message = "Incorrect Username or Password !!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            
        }

        [HttpGet]
        public ActionResult Register()
        {
            Session.Abandon();
            return View();
        }

        [HttpGet]
        public ActionResult GetCard()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ViewSubscr()
        {
            if (Session["email"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }

            DAL dal = new DAL();
            
            List<Subscription> subscriptions = dal.FetchCustomerSubscriptions(Session["email"].ToString());

            ViewBag.SubDetails = subscriptions;
            ViewBag.Email = Session["email"];

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            //FormsAuthentication.SignOut();
            //Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Cache.SetNoStore();
            ViewBag.Message = "You have been successfully logged out!";
            ViewBag.Type = "success";
            //string loggedOutPageUrl = "Home/Index";
            //Response.Write("<script language=\"'javascript'\">");
            //Response.Write("function ClearHistory()");
            //Response.Write("{");
            //Response.Write(" var backlen=history.length;");
            //Response.Write(" history.go(-backlen);");
            //Response.Write(" window.location.href='" + loggedOutPageUrl + "'; ");
            //Response.Write("}");
            //Response.Write("</script>");
    //        < script type = "text/javascript" >
    //     function preventBack() { window.history.forward(); }
    //        setTimeout("preventBack()", 0);
    //        window.onunload = function() { null };
    //</ script >

            return View("Index");
        }

        [HttpPost]
        public ActionResult Register(Customer c)
        {

            DAL dal = new DAL();
            bool res=dal.AddCustomer(c);
            if (res)
            {
                ViewBag.Message = "Registered Successfully!";
                ViewBag.Type = "success";
                return View("Index");
            }
            else
            {
                ViewBag.Message = "Registered Unsuccessful, Please try again!";
               // ViewBag.Message = "Registered Successfully!";
                ViewBag.Type = "danger";
                return View("Register");
            }
        }


    }
}