using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public class Subscription
    {
        public int sid { get; set; }
        public string email { get; set; }
        public float cost { get; set; }
        public DateTime expirydate { get; set; }
        public List<Pack> packs { get; set; }
        public List<Channel> channels { get; set; }
    
    }
}