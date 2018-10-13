using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public class Pack
    {
        public int PackId { get; set; }
        public string PackName { get; set; }
        public float PackCost { get; set; }
        public float month { get; set; }
        public List<Channel> Channels { get; set; }
        public List<bool> Checked { get; set; }
    }
}