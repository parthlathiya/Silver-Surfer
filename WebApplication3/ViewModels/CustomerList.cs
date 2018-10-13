using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.ViewModels
{
    public class CustomerList
    {
        public List<Customer> CustList { get; set; }
        public String Email { get; set; }

        public IEnumerable<SelectListItem> CustListItems
        {
            get
            {
                return new SelectList(CustList, "Email", "Email");
            }
        }
    }
}