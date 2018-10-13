using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public class Channel
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }

        public float ChannelCost { get; set; }
        public int month { get; set; }
    }
}