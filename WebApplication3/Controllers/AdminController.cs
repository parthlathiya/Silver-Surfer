using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;
using WebApplication3.ViewModels;

namespace WebApplication3.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [HttpGet]
        public ActionResult Index()
        {
            Session.Abandon();
            return View();
        }

        [HttpPost]
        public ActionResult Index(Admin a)
        {
            DAL dal = new DAL();
            if (a.Username!=null && a.Password!=null && dal.IsAdminUsernamePasswordPairPresent(a.Username, a.Password))
            {
                Admin a1 = dal.FetchAdmin(a.Username);
                ViewData["AdminName"] = a1.Name;
                Session["admin"] = a1.Username;

                List<Customer> clist = dal.AllCustomers();

                //List<string> res = new List<string>();

                //foreach (var c in clist)
                //    res.Add(c.Email);
                CustomerList cl = new CustomerList();
                cl.CustList = clist;

                return View("Welcome",cl);
            }
            else
            {
                ViewBag.Message = "Incorrect Username or Password !! ";
                ViewBag.Type = "danger";
                return View("Index");
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            ViewBag.Message = "You have been successfully logged out!";
            ViewBag.Type = "success";
            //string loggedOutPageUrl = "Logout.aspx";
            //Response.Write("<script language=\"'javascript'\">" );
            //Response.Write("function ClearHistory()");
            //Response.Write("{");
            //Response.Write(" var backlen=history.length;");
            //Response.Write(" history.go(-backlen);");
            //Response.Write(" window.location.href='" + loggedOutPageUrl + "'; ");
            //Response.Write("}");
            //Response.Write("</script>");
            return View("Index");
        }

        [HttpGet]
        public ActionResult Welcome()
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            DAL dal = new DAL();
            List<Customer> clist = dal.AllCustomers();
            CustomerList cl1 = new CustomerList();
            cl1.CustList = clist;
            return View("Welcome", cl1);
        }

        [HttpPost]
        public ActionResult Welcome(CustomerList cl)
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            DAL dal = new DAL();
            List<Customer> clist = dal.AllCustomers();
            CustomerList cl1 = new CustomerList();
            cl1.CustList = clist;

            List<Subscription> subscriptions =  dal.FetchCustomerSubscriptions(cl.Email);

            ViewBag.SubDetails = subscriptions;
            ViewBag.Email = cl.Email;

            return View("Welcome",cl1);
        }

        public ActionResult AllCh()
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            DAL dal = new DAL();
            List<Channel> chlist = dal.AllChannels();
            return View("AllCh", chlist);
        }
        public ActionResult AllPack()
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            DAL dal = new DAL();
            List<Pack> packlist = dal.AllPacks();
            return View("AllPack", packlist);
        }

        

        [HttpGet]
        public ActionResult AddCh()
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult AddPack()
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            DAL dal = new DAL();
            Pack p = new Pack() {
                Channels = dal.AllChannels()
                
            }
            ;
            return View(p);
        }

        [HttpPost]
        public ActionResult AddCh(Channel c)
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            DAL dal = new DAL();
            bool b = dal.AddCh(c);

            List<Customer> clist = dal.AllCustomers();
            CustomerList cl1 = new CustomerList();
            cl1.CustList = clist;
            if (!b)
            {
                ViewBag.Message = "Sorry, Something went wrong. Can not add channel!";
                ViewBag.Type = "danger";
                return View("Welcome", cl1);
            }
            else
            {
                ViewBag.Message = "Channel has been addded successfully!";
                ViewBag.Type = "success";
                return View("Welcome", cl1);
            }
        }

        [HttpPost]
        public ActionResult AddPack(Pack p)
        {
            if (Session["admin"] == null)
            {
                ViewBag.Message = "Your session has expired. Please login again!";
                ViewBag.Type = "danger";
                return View("Index");
            }
            DAL dal = new DAL();
            bool b = dal.AddPack(p);

            List<Customer> clist = dal.AllCustomers();
            CustomerList cl1 = new CustomerList();
            cl1.CustList = clist;


            if (!b)
            {
                ViewBag.Message = "Sorry, Something went wrong. Can not add pack!";
                ViewBag.Type = "danger";
                return View("Welcome",cl1);
            }
            else
            {
                ViewBag.Message = "Pack has been addded successfully!";
                ViewBag.Type = "success";
                return View("Welcome",cl1);
            }
        }
    }
}