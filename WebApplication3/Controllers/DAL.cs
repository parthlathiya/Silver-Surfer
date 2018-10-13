using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class DAL
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString);

        public bool IsEmailPasswordPairPresent(string email, string password)
        {
            string checkuser = "select count(*) from customer where email='" + email + "' and password = '" + (password) + "'";
            this.conn.Open();
            int temp = Convert.ToInt32(new SqlCommand(checkuser, conn).ExecuteScalar().ToString());
            this.conn.Close();
            return temp == 1;
        }

        public bool IsAdminUsernamePasswordPairPresent(string username, string password)
        {
            string checkuser = "select count(*) from admin where username='" + username + "' and password = '" + (password) + "'";
            this.conn.Open();
            int temp = Convert.ToInt32(new SqlCommand(checkuser, conn).ExecuteScalar().ToString());
            this.conn.Close();
            return temp == 1;
        }

        public Customer FetchCustomer(string email)
        {
            string checkuser = "select * from customer WHERE email='" + email + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checkuser, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Customer c = new Customer()
                {
                    Name = reader["name"].ToString(),
                    Email = reader["email"].ToString(),
                    ContactNo = reader["contactno"].ToString(),
                    Address = reader["address"].ToString(),
                    DOB = reader["dob"].ToString(),
                    ProfileId = reader["profileid"].ToString()

                };
                this.conn.Close();
                return c;
            }
            else
            {
                this.conn.Close();
                return new Customer();
            }
        }


        public Channel FetchChannel(int chid)
        {
            string checkuser = "select * from channel WHERE channelid='" + chid + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checkuser, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            Channel ch = new Channel()
            {

                ChannelId = Convert.ToInt32(reader["channelid"]),
                ChannelName = reader["name"].ToString(),
                ChannelCost = (float)reader.GetDouble(2)

            };
            this.conn.Close();
            return ch;
            
        }

        public bool UpdateCustProfileId(string email, string profileid, string paymentid)
        {
            string updatecust = "update customer set profileid='" + profileid + "', paymentid='"+paymentid+"' where email='" + email+"'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(updatecust, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();
            this.conn.Close();
            return res >= 1;
        }

        public List<bool> issubornot(List<int> chids,string email)
        {
            this.conn.Open();
            List<bool> ans = new List<bool>();
            foreach (int chid in chids)
            {
                string checksubscribed = "select count(*) from subscriptionmaster,subscriptionchannel WHERE subscriptionmaster.sid=subscriptionchannel.sid and email = '" + email + "' and channelid = '" + chid + "'";
                int temp = Convert.ToInt32(new SqlCommand(checksubscribed, conn).ExecuteScalar().ToString());
                ans.Add(temp >= 1);
            }
            this.conn.Close();
            return ans;
        }

        public List<bool> issubornotinanypack(List<int> chids, List<int> packids, string email)
        {
            List<bool> res = issubornot1(packids, email);
            List<int> final = new List<int>();
            for (int i = 0; i < res.Count; i++)
                if (res[i])
                    final.Add(packids[i]);
            List<bool> output = new List<bool>();
            foreach (var chid in chids)
            {
                bool f =false;
                foreach (var packid in final)
                {
                    List<ChannelsInPack> temp = GetChannelsInPack(packid, email);
                    foreach (var chh in temp)
                        if (chh.ChannelId == chid)
                        {
                            f = true;
                            break;
                        }
                    if (f) break;
                }
                output.Add(f);
            }
            return output;

        }


        public List<bool> issubornot1(List<int> packids, string email)
        {
            this.conn.Open();
            List<bool> ans = new List<bool>();
            foreach (int packid in packids)
            {
                string checksubscribed = "select count(*) from subscriptionmaster,subscriptionpack WHERE subscriptionmaster.sid=subscriptionpack.sid and email = '" + email + "' and packid = '" + packid + "'";
                int temp = Convert.ToInt32(new SqlCommand(checksubscribed, conn).ExecuteScalar().ToString());
                ans.Add(temp >= 1);
            }
            this.conn.Close();
            return ans;
        }

        public string getCustomerProfileId(string email)
        {
            string selectprofileid = "select profileid from customer WHERE email='" + email + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(selectprofileid, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string res = reader["profileid"].ToString();
            this.conn.Close();
            return res;
        }
        public string getPaymentProfileId(string email)
        {
            string selectprofileid = "select paymentid from customer WHERE email='" + email + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(selectprofileid, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            string res = reader["paymentid"].ToString();
            this.conn.Close();
            return res;
        }

        public int AddChannelSubscriptionMaster(string useremail, Channel c,string transid)
        {
            //DateTime theDate = DateTime.Now;
            //DateTime expdate = theDate.AddYears(1).Date;

            //return c.ChannelCost.ToString() + " " + c.ChannelName + " " + useremail + " " + expdate.ToString();
            //return true;
            int sid = 0;
            DateTime theDate = DateTime.Now;
            DateTime expdate = theDate.AddMonths(1).Date;
            //string insertchannelsub = "insert into subscriptionmaster values ('" + 7 + "',' " + useremail + "','" + c.ChannelCost + "','" + expdate + "')";
            string insertchannelsub = "insert into subscriptionmaster values ('" + useremail + "','" + c.ChannelCost + "','" + expdate + "','"+ transid + "')";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(insertchannelsub, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();
            this.conn.Close();

            string getsid = "SELECT TOP(1) sid FROM subscriptionmaster ORDER BY 1 DESC";
            this.conn.Open();
            cmd = new SqlCommand(getsid, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                sid = Convert.ToInt32(reader["sid"]);
            }
            this.conn.Close();
            return sid;
        }


        public int AddPackSubscriptionMaster(string useremail, Pack p, string transid)
        {
            //DateTime theDate = DateTime.Now;
            //DateTime expdate = theDate.AddYears(1).Date;

            //return c.ChannelCost.ToString() + " " + c.ChannelName + " " + useremail + " " + expdate.ToString();
            //return true;
            int sid = 0;
            DateTime theDate = DateTime.Now;
            DateTime expdate = theDate.AddMonths(1).Date;
            //string insertchannelsub = "insert into subscriptionmaster values ('" + 7 + "',' " + useremail + "','" + c.ChannelCost + "','" + expdate + "')";
            string insertchannelsub = "insert into subscriptionmaster values ('" + useremail + "','" + p.PackCost + "','" + expdate + "','" + transid + "')";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(insertchannelsub, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();
            this.conn.Close();

            string getsid = "SELECT TOP(1) sid FROM subscriptionmaster ORDER BY 1 DESC";
            this.conn.Open();
            cmd = new SqlCommand(getsid, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                sid = Convert.ToInt32(reader["sid"]);
            }
            this.conn.Close();
            return sid;
        }


        public bool AddSubscriptionChannel(int sid, Channel c)
        {


            string insertchannelsub = "insert into subscriptionchannel values ('" + sid + "','" + c.ChannelId + "')";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(insertchannelsub, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();
            this.conn.Close();
            return res > 0;
        }

        public bool AddSubscriptionPack(int sid, Pack p)
        {


            string insertchannelsub = "insert into subscriptionpack values ('" + sid + "','" + p.PackId + "')";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(insertchannelsub, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();
            this.conn.Close();
            return res > 0;
        }

        public DateTime getexpirydate(int cid, string useremail)
        {
            string checksubscribed = "select expirydate from subscriptionmaster,subscriptionchannel WHERE subscriptionmaster.sid=subscriptionchannel.sid and email = '" + useremail + "' and channelid = '" + cid + "'";

            //string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "' and sid = '" + ("select sid from subscriptionchannel WHERE channelid = '" + cid + "'") + "' ";
            // string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checksubscribed, this.conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            //DateTime d = DateTime.ParseExact(reader["expirydate"].ToString(), "dd/MM/yyyy hh:mm:ss", null);

            //DateTime d = Convert.ToDateTime(DateTime.ParseExact(reader["expirydate"].ToString(), "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture));

            DateTime d = Convert.ToDateTime(reader["expirydate"]);
            this.conn.Close();
            return d;

        }

        public DateTime getexpirydatepack(int packid, string useremail)
        {
            string checksubscribed = "select expirydate from subscriptionmaster,subscriptionpack WHERE subscriptionmaster.sid=subscriptionpack.sid and email = '" + useremail + "' and packid = '" + packid + "'";

            //string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "' and sid = '" + ("select sid from subscriptionchannel WHERE channelid = '" + cid + "'") + "' ";
            // string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checksubscribed, this.conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            //DateTime d = DateTime.ParseExact(reader["expirydate"].ToString(), "dd/MM/yyyy hh:mm:ss", null);

            //DateTime d = Convert.ToDateTime(DateTime.ParseExact(reader["expirydate"].ToString(), "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture));

            DateTime d = Convert.ToDateTime(reader["expirydate"]);
            this.conn.Close();
            return d;

        }
        public Admin FetchAdmin(string username)
        {
            string checkuser = "select * from admin where username='" + username + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checkuser, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Admin a = new Admin()
                {
                    Name = reader["name"].ToString(),
                    Username = reader["username"].ToString()
                };
                this.conn.Close();
                return a;
            }
            else
            {
                this.conn.Close();
                return new Admin();
            }
        }



        public bool AddCustomer(Customer c)
        {
            string insertuser = "insert into customer values ('" + c.Email + "',' " + c.Name + "','" + c.ContactNo + "','" + c.Address + "','" + c.DOB + "','" + c.Password + "')";

            this.conn.Open();
            SqlCommand cmd = new SqlCommand(insertuser, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();

            this.conn.Close();
            return res > 0;

            // Queries the database for the particular user identified by
            // personID. If the user is located then the DataSet contains a
            // single record corresponding to the requested user.  If the user
            // is not found then the DataSet does not contain any records.
        }


        public bool deleteChannelSubscr(int sid)
        {

            this.conn.Open();


            string deletechsubc = "delete from subscriptionchannel where sid='" + sid + "'";
            SqlCommand cmd1 = new SqlCommand(deletechsubc, this.conn);
            cmd1.CommandType = CommandType.Text;
            int res1 = cmd1.ExecuteNonQuery();

            string deletechsubm = "delete from subscriptionmaster where sid='"+sid+"'";
            SqlCommand cmd = new SqlCommand(deletechsubm, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();

           

            this.conn.Close();

            return res > 0 && res1 >0;
        }

        public bool deletePackSubscr(int sid)
        {

            this.conn.Open();


            string deletechsubc = "delete from subscriptionpack where sid='" + sid + "'";
            SqlCommand cmd1 = new SqlCommand(deletechsubc, this.conn);
            cmd1.CommandType = CommandType.Text;
            int res1 = cmd1.ExecuteNonQuery();

            string deletechsubm = "delete from subscriptionmaster where sid='" + sid + "'";
            SqlCommand cmd = new SqlCommand(deletechsubm, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();



            this.conn.Close();

            return res > 0 && res1 > 0;
        }

        public int getSid(int chid, string email)
        {

            string selectsid = "select subscriptionmaster.sid from subscriptionmaster,subscriptionchannel WHERE subscriptionmaster.sid=subscriptionchannel.sid and email = '" + email + "' and channelid = '" + chid + "'";

            //string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "' and sid = '" + ("select sid from subscriptionchannel WHERE channelid = '" + cid + "'") + "' ";
            // string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(selectsid, this.conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            //DateTime d = DateTime.ParseExact(reader["expirydate"].ToString(), "dd/MM/yyyy hh:mm:ss", null);

            //DateTime d = Convert.ToDateTime(DateTime.ParseExact(reader["expirydate"].ToString(), "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture));

            int  sid = Convert.ToInt32(reader["sid"]);
            this.conn.Close();
            return sid;


        }

        public int getSidPack(int packid, string email)
        {

            string selectsid = "select subscriptionmaster.sid from subscriptionmaster,subscriptionpack WHERE subscriptionmaster.sid=subscriptionpack.sid and email = '" + email + "' and packid = '" + packid + "'";

            //string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "' and sid = '" + ("select sid from subscriptionchannel WHERE channelid = '" + cid + "'") + "' ";
            // string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(selectsid, this.conn);
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            //DateTime d = DateTime.ParseExact(reader["expirydate"].ToString(), "dd/MM/yyyy hh:mm:ss", null);

            //DateTime d = Convert.ToDateTime(DateTime.ParseExact(reader["expirydate"].ToString(), "dd-MM-yyyy hh:mm:ss", CultureInfo.InvariantCulture));

            int sid = Convert.ToInt32(reader["sid"]);
            this.conn.Close();
            return sid;


        }

        public List<Customer> AllCustomers()
        {
            string checkuser = "select * from customer";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checkuser, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            List<Customer> clist = new List<Customer>();

            if (reader.HasRows)
            {
                Customer c = null;
                while (reader.Read())
                {
                    c = new Customer()
                    {
                        Email = reader.GetString(0),
                        Name = reader.GetString(1),
                        ContactNo = reader.GetString(2),
                        Address = reader.GetString(3),
                        DOB = reader.GetString(4)
                    };
                    clist.Add(c);
                }
            }
            this.conn.Close();
            return clist;
        }

        public List<Channel> AllChannels()
        {
            string checkuser = "select * from channel";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checkuser, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            List<Channel> chlist = new List<Channel>();

            if (reader.HasRows)
            {
                Channel ch = null;
                while (reader.Read())
                {
                    ch = new Channel()
                    {
                        ChannelId = reader.GetInt32(0),
                        ChannelName = reader.GetString(1),
                        ChannelCost = (float)reader.GetDouble(2)
                    };
                    chlist.Add(ch);
                }
            }
            this.conn.Close();
            return chlist;
        }

        public List<ChannelsInPack> GetChannelsInPack(int packid, string useremail)
        {
            int id = 0;
            string name = "";
            double cost = 0;
            List<ChannelsInPack> result = new List<ChannelsInPack>();
            SqlCommand command;
            SqlDataReader dataReader;
            String sql = "select channelid from packchannelmapping WHERE packid='" + packid + "'";
            this.conn.Open();
            command = new SqlCommand(sql, this.conn);
            dataReader = command.ExecuteReader();
            List<int> cids = new List<int>();
            while (dataReader.Read())
            {
                cids.Add(Convert.ToInt32(dataReader.GetValue(0)));
            }
            this.conn.Close();

            for (int i = 0; i < cids.Count; i++)
            {
                int cid = cids[i];
                sql = "select name,cost from Channel WHERE channelid='" + cid + "'";
                this.conn.Open();
                command = new SqlCommand(sql, this.conn);
                dataReader = command.ExecuteReader();
                string expirydatestring = "";
                bool ifsubscribe = false;
                bool flag = false;
                if (dataReader.Read())
                {
                    flag = true;
                    id = cid;
                    name = dataReader.GetValue(0).ToString();
                    cost = Convert.ToDouble(dataReader.GetValue(1));

                }
                this.conn.Close();
                if (flag)
                {
                    expirydatestring = CheckSubscribed(cid, useremail);
                    DateTime expirydatedt = default(DateTime);
                    if (expirydatestring != "")
                    {
                        ifsubscribe = true;
                        expirydatedt = Convert.ToDateTime(expirydatestring);

                    }
                    ChannelsInPack c = new ChannelsInPack()
                    {
                        ChannelId = id,
                        ChannelName = name,
                        ChannelCost = cost,
                        subscribe = ifsubscribe,
                        expirydate = expirydatedt,

                    };
                    result.Add(c);
                }

            }
            return result;
        }

        public string CheckSubscribed(int cid, string useremail)
        {
            //string allch = "select name from channel , packchannelmapping where channel.channelid=packchannelmapping.channelid and packid='" + reader.GetInt32(0) + "'";


            string checksubscribed = "select expirydate from subscriptionmaster,subscriptionchannel WHERE subscriptionmaster.sid=subscriptionchannel.sid and email = '" + useremail + "' and channelid = '" + cid + "'";

            //string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "' and sid = '" + ("select sid from subscriptionchannel WHERE channelid = '" + cid + "'") + "' ";
            // string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checksubscribed, this.conn);
            SqlDataReader reader = cmd.ExecuteReader();
            string datetimestring = "";
            if (reader.Read())
            {
                datetimestring = reader["expirydate"].ToString();
            }
            this.conn.Close();
            return datetimestring;
        }

        public string CheckSubscribedPack(int pid, string useremail)
        {

            string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "' and sid = (select sid from subscriptionpack WHERE packid = '" + pid + "') ";

            //string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "' and sid = '" + ("select sid from subscriptionchannel WHERE channelid = '" + cid + "'") + "' ";
            // string checksubscribed = "select expirydate from subscriptionmaster WHERE email = '" + useremail + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checksubscribed, this.conn);
            SqlDataReader reader = cmd.ExecuteReader();
            string datetimestring = "";
            if (reader.Read())
            {
                datetimestring = reader["expirydate"].ToString();
            }
            this.conn.Close();
            return datetimestring;
        }


        public List<Pack> AllPacks()
        {
            string checkuser = "select * from pack";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checkuser, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            List<Pack> packlist = new List<Pack>();

            if (reader.HasRows)
            {
                Pack pack = null;
                while (reader.Read())
                {

                    string allch = "select channel.channelid,name,cost from channel , packchannelmapping where channel.channelid=packchannelmapping.channelid and packid='" + reader.GetInt32(0)+"'";
                    SqlCommand cmd1 = new SqlCommand(allch, conn);
                    SqlDataReader reader1 = cmd1.ExecuteReader();
                    List<Channel> channels = new List<Channel>();
                    while (reader1.Read())
                    {
                        Channel c = new Channel()
                        {
                            ChannelId = Convert.ToInt32(reader1["channelid"]),
                            ChannelName = reader1["name"].ToString(),
                            ChannelCost = (float)Convert.ToDouble(reader1["cost"])
                        };
                        channels.Add(c);
                    }

                    pack = new Pack()
                    {
                        PackId = reader.GetInt32(0),
                        Channels = channels,
                        PackName = reader.GetString(1),
                        PackCost = (float)reader.GetDouble(2)
                    };
                    packlist.Add(pack);
                }
            }
            this.conn.Close();
            return packlist;
        }


        public bool AddCh(Channel c)
        {
            string insertuser = "insert into channel ( name , cost) values ('" + c.ChannelName + "',' " + c.ChannelCost + "')";

            this.conn.Open();
            SqlCommand cmd = new SqlCommand(insertuser, this.conn);
            cmd.CommandType = CommandType.Text;
            int res = cmd.ExecuteNonQuery();

            this.conn.Close();
            return res > 0;

        }
        
        public bool AddPack(Pack p)
        {
            string insertpack = "insert into pack ( name , cost) values ('" + p.PackName + "',' " + p.PackCost + "'); SELECT SCOPE_IDENTITY();";

            this.conn.Open();
            SqlCommand cmd = new SqlCommand(insertpack, this.conn);
            cmd.CommandType = CommandType.Text;
//            int res = cmd.ExecuteNonQuery();
            int newid = Convert.ToInt32(cmd.ExecuteScalar());

            List<Channel> channels = p.Channels;
            List<bool> checklist = p.Checked;


            int res1=0;
            for (var i =0;i< checklist.Count;i++)
            {
                if (checklist[i])
                {
                    SqlCommand cmd1 = new SqlCommand("select channelid from channel where name='"+channels[i].ChannelName+"' and cost='"+channels[i].ChannelCost+"'", this.conn);
                    cmd1.CommandType = CommandType.Text;
                    SqlDataReader reader = cmd1.ExecuteReader();
                    reader.Read();
                    int chid=reader.GetInt32(0);
                    string insertpackchmap = "insert into packchannelmapping values ('" + newid + "',' " + chid + "')";
                    SqlCommand cmd2 = new SqlCommand(insertpackchmap, this.conn);
                    cmd2.CommandType = CommandType.Text;
                    res1 = cmd2.ExecuteNonQuery();
                    if (res1 <= 0)
                        break;
                }

            }
            this.conn.Close();
            return res1 >0;
        }

        public List<Subscription> FetchCustomerSubscriptions(string email)
        {
            string checkuser = "select * from subscriptionmaster where email ='" + email + "'";
            this.conn.Open();
            SqlCommand cmd = new SqlCommand(checkuser, conn);
            SqlDataReader reader = cmd.ExecuteReader();

            List<Subscription> sublist = new List<Subscription>();

            if (reader.HasRows)
            {
                Subscription sub = null;
                while (reader.Read())
                {
                    int subid = reader.GetInt32(0);

                    string query1 = "select channelid from subscriptionchannel where sid='" + subid + "'";
                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                    SqlDataReader reader1 = cmd1.ExecuteReader();

                    List<Channel> chlist = new List<Channel>();

                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            int chid=reader1.GetInt32(0);
                            string subquery = "select * from channel where channelid='" + chid + "'";
                            SqlCommand cmd2 = new SqlCommand(subquery, conn);
                            SqlDataReader subreader = cmd2.ExecuteReader();
                            if (subreader.HasRows)
                            {
                                subreader.Read();
                                Channel ch = new Channel()
                                {
                                    ChannelName = subreader.GetString(1),
                                    ChannelCost = (float)subreader.GetDouble(2)
                                };
                                chlist.Add(ch);
                            }
                        }
                    }

                    string query2 = "select packid from subscriptionpack where sid='" + subid + "'";
                    SqlCommand cmd3 = new SqlCommand(query2, conn);
                    SqlDataReader reader2 = cmd3.ExecuteReader();

                    List<Pack> packlist = new List<Pack>();

                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            int packid = reader2.GetInt32(0);
                            string subquery = "select * from pack where packid='" + packid + "'";
                            SqlCommand cmd4 = new SqlCommand(subquery, conn);
                            SqlDataReader subreader = cmd4.ExecuteReader();
                            if (subreader.HasRows)
                            {
                                subreader.Read();
                                Pack p = new Pack()
                                {
                                    PackName = subreader.GetString(1),
                                    PackCost = (float)subreader.GetDouble(2)
                                };
                                packlist.Add(p);
                            }
                        }
                    }


                    sub = new Subscription()
                    {
                        sid = reader.GetInt32(0),
                        email = reader.GetString(1),
                        cost = (float)reader.GetDouble(2),
                        expirydate = reader.GetDateTime(3),
                        channels=chlist,
                        packs=packlist
                    };
                    sublist.Add(sub);
                }
            }
            this.conn.Close();
            return sublist;
        }

    }
}