using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public class ChannelsInPack

    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public double ChannelCost { get; set; }
        public bool subscribe { get; set; } = false;
        public DateTime expirydate { get; set; }

    }
}