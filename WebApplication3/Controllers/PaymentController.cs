using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApplication3.Models;



namespace WebApplication3.Controllers
{
    public class PaymentController : Controller
    {
        string ApiLoginID = "58L4dFWkF";
        string ApiTransactionKey = "3Q963f84F82Fgbk2";


        [HttpPost]
        public ActionResult CreateCustProfile(Card c)
        {

            string cardnumber = c.cardnumber;
            string expirydate = c.expirydate;
            string cvv = c.cvv;


            Console.WriteLine("Create Customer Profile Sample");

            // set whether to use the sandbox environment, or production enviornment
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey,
            };

            var creditCard = new creditCardType
            {
                cardNumber = "4111111111111111",
                expirationDate = "0725"
                
            };

            //var bankAccount = new bankAccountType
            //{
            //    accountNumber = "231323342",
            //    routingNumber = "000000224",
            //    accountType = bankAccountTypeEnum.checking,
            //    echeckType = echeckTypeEnum.WEB,
            //    nameOnAccount = "test",
            //    bankName = "Bank Of America"
            //};

            // standard api call to retrieve response
            paymentType cc = new paymentType { Item = creditCard};
            //paymentType echeck = new paymentType { Item = bankAccount };

            List<customerPaymentProfileType> paymentProfileList = new List<customerPaymentProfileType>();
            customerPaymentProfileType ccPaymentProfile = new customerPaymentProfileType();
            ccPaymentProfile.payment = cc;
            //ccPaymentProfile.defaultPaymentProfile = true;

            //customerPaymentProfileType echeckPaymentProfile = new customerPaymentProfileType();
            //echeckPaymentProfile.payment = echeck;

            paymentProfileList.Add(ccPaymentProfile);
            
            //paymentProfileList.Add(echeckPaymentProfile);

            List<customerAddressType> addressInfoList = new List<customerAddressType>();
            customerAddressType homeAddress = new customerAddressType();
            homeAddress.address = "10900 NE 8th St";
            homeAddress.city = "Seattle";
            homeAddress.zip = "98006";


            customerAddressType officeAddress = new customerAddressType();
            officeAddress.address = "1200 148th AVE NE";
            officeAddress.city = "NorthBend";
            officeAddress.zip = "92101";

            addressInfoList.Add(homeAddress);
            addressInfoList.Add(officeAddress);


            customerProfileType customerProfile = new customerProfileType();
            customerProfile.merchantCustomerId = "Test parthlathiya";
            customerProfile.email = Session["Email"].ToString();
            customerProfile.paymentProfiles = paymentProfileList.ToArray();
            customerProfile.shipToList = addressInfoList.ToArray();

            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };

            // instantiate the controller that will call the service
            var controller = new createCustomerProfileController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            createCustomerProfileResponse response = controller.GetApiResponse();

            DAL dal = new DAL();

            // validate response 
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.messages.message != null)
                    {
                        ViewBag.Message = "We have stored your card details. Now you can subscribe to any channel/pack with single click.";
                        ViewBag.Type = "success";

                        dal.UpdateCustProfileId(Session["Email"].ToString(), response.customerProfileId, response.customerPaymentProfileIdList[0]);
                       
                        //Console.WriteLine("Success!");
                        //Console.WriteLine("Customer Profile ID: " + response.customerProfileId);
                        Console.WriteLine("Payment Profile ID: " + response.customerPaymentProfileIdList[0]);
                        Console.WriteLine("Shipping Profile ID: " + response.customerShippingAddressIdList[0]);

                    }
                }
                else
                {
                    ViewBag.Message="Customer Profile Creation Failed.";
                    ViewBag.Type = "danger";
                    Console.WriteLine("Error Code: " + response.messages.message[0].code);
                    Console.WriteLine("Error message: " + response.messages.message[0].text);
                }
            }
            else
            {
                if (controller.GetErrorResponse().messages.message.Length > 0)
                {
                    ViewBag.Message = "Customer Profile Creation Failed.";
                    ViewBag.Type = "danger";
                    Console.WriteLine("Error Code: " + response.messages.message[0].code);
                    Console.WriteLine("Error message: " + response.messages.message[0].text);
                }
                else
                {
                    Console.WriteLine("Null Response.");
                }
            }

            List<Channel> channelsList = dal.AllChannels();
            List<Pack> packsList = dal.AllPacks();
            ViewData["AllChannels"] = channelsList;
            ViewData["AllPacks"] = packsList;
            List<int> chids = new List<int>();
            foreach (var ch1 in channelsList)
                chids.Add(ch1.ChannelId);
            List<bool> subornot = dal.issubornot(chids, Session["Email"].ToString());
            ViewData["SuborNot"] = subornot;
            Customer c1 = dal.FetchCustomer(Session["email"].ToString());

            List<int> packids = new List<int>();
            foreach (var pack in packsList)
                packids.Add(pack.PackId);
            List<bool> subornot1 = dal.issubornot1(packids, Session["Email"].ToString());
            ViewData["SuborNot1"] = subornot1;

            List<bool> subornot2 = dal.issubornotinanypack(chids, packids, Session["Email"].ToString());
            ViewData["SuborNot2"] = subornot2;

            ViewData["ProfileId"] = c1.ProfileId;

            return View("../Home/Welcome");


        }
     

        [HttpPost]
        public ActionResult RefundChannel(Channel ch)
        {
       // LAKSHMI    //            delsubscription from auto rec if its there


            //fetchexpiry and calculate transamount;
            DAL dal = new DAL();
            
            DateTime d = dal.getexpirydate(ch.ChannelId, Session["Email"].ToString());
            decimal TransactionAmount = Convert.ToDecimal((ch.ChannelCost*(d - DateTime.Now).TotalDays)/30.0);

            Console.WriteLine("Refund Transaction");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };

        
            customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
            profileToCharge.customerProfileId = dal.getCustomerProfileId(Session["Email"].ToString());
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = dal.getPaymentProfileId(Session["Email"].ToString()) };

            

            //var creditCard = new creditCardType
            //{
            //    cardNumber = "1111",
            //    expirationDate = "XXXX"
            //};

                ////standard api call to retrieve response
                //var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                //payment = paymentType,
                profile= profileToCharge,
                amount = TransactionAmount
                //refTransId = TransactionID
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {

                        // get sid from subch
                        // del sid wala row from submaster and subch

                        int sid =dal.getSid(ch.ChannelId, Session["Email"].ToString());
                        dal.deleteChannelSubscr(sid);
                        
                        ViewBag.Message = "Successfully refunded amount of " + ch.ChannelCost + ". Transaction ID: " + response.transactionResponse.transId + ". You have successfully unsubscribed to the channel " + ch.ChannelName + ".";
                        ViewBag.Type = "success";

                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        ViewBag.Message = "Transaction Failed.";
                        ViewBag.Type = "danger";
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Transaction Failed.";
                    ViewBag.Type = "danger";
                    Console.WriteLine("Failed Transaction.");

                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
            }

            List<Channel> channelsList = dal.AllChannels();
            List<Pack> packsList = dal.AllPacks();
            ViewData["AllChannels"] = channelsList;
            ViewData["AllPacks"] = packsList;
            List<int> chids = new List<int>();
            foreach (var ch1 in channelsList)
                chids.Add(ch1.ChannelId);
            List<bool> subornot = dal.issubornot(chids, Session["Email"].ToString());
            ViewData["SuborNot"] = subornot;
            Customer c1 = dal.FetchCustomer(Session["email"].ToString());

            List<int> packids = new List<int>();
            foreach (var pack in packsList)
                packids.Add(pack.PackId);
            List<bool> subornot1 = dal.issubornot1(packids, Session["Email"].ToString());
            ViewData["SuborNot1"] = subornot1;

            List<bool> subornot2 = dal.issubornotinanypack(chids, packids, Session["Email"].ToString());
            ViewData["SuborNot2"] = subornot2;

            ViewData["ProfileId"] = c1.ProfileId;

            return View("../Home/Welcome");







        }

        public ActionResult RefundPack(Pack p)
        {
            // LAKSHMI    //            delsubscription from auto rec if its there


            //fetchexpiry and calculate transamount;
            DAL dal = new DAL();

            DateTime d = dal.getexpirydatepack(p.PackId, Session["Email"].ToString());
            decimal TransactionAmount = Convert.ToDecimal((p.PackCost * (d - DateTime.Now).TotalDays) / 30.0);

            Console.WriteLine("Refund Transaction");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };


            customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
            profileToCharge.customerProfileId = dal.getCustomerProfileId(Session["Email"].ToString());
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = dal.getPaymentProfileId(Session["Email"].ToString()) };



            //var creditCard = new creditCardType
            //{
            //    cardNumber = "1111",
            //    expirationDate = "XXXX"
            //};

            ////standard api call to retrieve response
            //var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                //payment = paymentType,
                profile = profileToCharge,
                amount = TransactionAmount
                //refTransId = TransactionID
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {

                        // get sid from subch
                        // del sid wala row from submaster and subch

                        int sid = dal.getSidPack(p.PackId, Session["Email"].ToString());
                        dal.deletePackSubscr(sid);

                        ViewBag.Message = "Successfully refunded amount of " + p.PackCost + ". Transaction ID: " + response.transactionResponse.transId + ". You have successfully unsubscribed to the pack " + p.PackName + ".";
                        ViewBag.Type = "success";

                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        ViewBag.Message = "Transaction Failed.";
                        ViewBag.Type = "danger";
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Transaction Failed.";
                    ViewBag.Type = "danger";
                    Console.WriteLine("Failed Transaction.");

                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
            }

            List<Channel> channelsList = dal.AllChannels();
            List<Pack> packsList = dal.AllPacks();
            ViewData["AllChannels"] = channelsList;
            ViewData["AllPacks"] = packsList;
            List<int> chids = new List<int>();
            foreach (var ch1 in channelsList)
                chids.Add(ch1.ChannelId);
            List<bool> subornot = dal.issubornot(chids, Session["Email"].ToString());
            ViewData["SuborNot"] = subornot;
            Customer c1 = dal.FetchCustomer(Session["email"].ToString());

            List<int> packids = new List<int>();
            foreach (var pack in packsList)
                packids.Add(pack.PackId);
            List<bool> subornot1 = dal.issubornot1(packids, Session["Email"].ToString());
            ViewData["SuborNot1"] = subornot1;

            List<bool> subornot2 = dal.issubornotinanypack(chids, packids, Session["Email"].ToString());
            ViewData["SuborNot2"] = subornot2;

            ViewData["ProfileId"] = c1.ProfileId;

            return View("../Home/Welcome");







        }

        public string RefundChannels(List<Channel> chs)
        {
            string reply = "";
            foreach (Channel ch in chs)
            {
                // LAKSHMI    //            delsubscription from auto rec if its there


                //fetchexpiry and calculate transamount;
                DAL dal = new DAL();

                DateTime d = dal.getexpirydate(ch.ChannelId, Session["Email"].ToString());
                decimal TransactionAmount = Convert.ToDecimal((ch.ChannelCost * (d - DateTime.Now).TotalDays) / 30.0);

                Console.WriteLine("Refund Transaction");

                ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

                // define the merchant information (authentication / transaction id)
                ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
                {
                    name = ApiLoginID,
                    ItemElementName = ItemChoiceType.transactionKey,
                    Item = ApiTransactionKey
                };


                customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
                profileToCharge.customerProfileId = dal.getCustomerProfileId(Session["Email"].ToString());
                profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = dal.getPaymentProfileId(Session["Email"].ToString()) };



                //var creditCard = new creditCardType
                //{
                //    cardNumber = "1111",
                //    expirationDate = "XXXX"
                //};

                ////standard api call to retrieve response
                //var paymentType = new paymentType { Item = creditCard };

                var transactionRequest = new transactionRequestType
                {
                    transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                                                                                           //payment = paymentType,
                    profile = profileToCharge,
                    amount = TransactionAmount
                    //refTransId = TransactionID
                };

                var request = new createTransactionRequest { transactionRequest = transactionRequest };

                // instantiate the controller that will call the service
                var controller = new createTransactionController(request);
                controller.Execute();

                // get the response from the service (errors contained if any)
                var response = controller.GetApiResponse();

                // validate response
                if (response != null)
                {
                    if (response.messages.resultCode == messageTypeEnum.Ok)
                    {
                        if (response.transactionResponse.messages != null)
                        {

                            // get sid from subch
                            // del sid wala row from submaster and subch

                            int sid = dal.getSid(ch.ChannelId, Session["Email"].ToString());
                            dal.deleteChannelSubscr(sid);
                            
                            reply += "\n Successfully refunded amount of " + ch.ChannelCost + " for channel " + ch.ChannelName + ". Transaction ID: " + response.transactionResponse.transId;
                            
                            //ViewBag.Type = "success";

                            Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                            Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                            Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                            Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                            Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                        }
                        else
                        {
                            reply += "\n Refund transaction failed for channel "+ ch.ChannelName;
                            //ViewBag.Type = "danger";
                            Console.WriteLine("Failed Transaction.");
                            if (response.transactionResponse.errors != null)
                            {
                                Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                                Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                            }
                        }
                    }
                    else
                    {
                         reply += "\n Refund transaction failed for channel "+ ch.ChannelName;
                        //ViewBag.Type = "danger";
                        Console.WriteLine("Failed Transaction.");

                        if (response.transactionResponse != null && response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                        else
                        {
                            Console.WriteLine("Error Code: " + response.messages.message[0].code);
                            Console.WriteLine("Error message: " + response.messages.message[0].text);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Null Response.");
                }
                
            }

            return reply;
            

        }

        [HttpPost]
        //public ActionResult ChargePack(Tuple<WebApplication3.Models.Channel, WebApplication3.Models.Pack> t)
        public ActionResult ChargePack(Pack p)
        {
            if (p.month > 1)
            {
                // LAKSHMI //                autorec call 
            }
            Console.WriteLine("Charge Customer Profile");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };

            DAL dal = new DAL();
            //create a customer payment profile
            customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
            profileToCharge.customerProfileId = dal.getCustomerProfileId(Session["Email"].ToString());
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = dal.getPaymentProfileId(Session["Email"].ToString()) };


            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // refund type
                //amount = 12,
                amount = Convert.ToDecimal(p.PackCost),
                profile = profileToCharge
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the collector that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {

                        int sid = dal.AddPackSubscriptionMaster(Session["Email"].ToString(), p, response.transactionResponse.transId); // as of now no validation here

                        //here 1stparametr is sid from above function.. as of now hardcoded.
                        dal.AddSubscriptionPack(sid, p); // as of now no validation here

                        //sid = dal.AddChannelSubscriptionMaster(useremail, InvoiceDetails); // as of now no validation here

                        //here 1stparametr is sid from above function.. as of now hardcoded.
                        //success_insertion_channel = dal.AddSubscriptionChannel(sid, InvoiceDetails); // as of now no validation here

                        //DateTime theDate = DateTime.Now;
                        //DateTime expdate = theDate.AddMonths(1).Date;
                        ViewBag.Type = "success";
                        ViewBag.Message = "Successfully completed payment of " + p.PackCost + ". Transaction ID: " + response.transactionResponse.transId + ". \n Subscribed Pack: " + p.PackName + ". \n We will charge you " + p.PackCost + " on " + DateTime.Now.Day + "th of every month for next " + (p.month - 1) + " months.";

                       
                        List<ChannelsInPack> temp = dal.GetChannelsInPack(p.PackId, Session["Email"].ToString());

                        List<Channel> chlist = new List<Channel>();

                        foreach (var ch1 in temp)
                        {
                            if(ch1.subscribe)
                                chlist.Add(dal.FetchChannel(ch1.ChannelId));
                        }
                        
                        string msg = RefundChannels(chlist);
                        ViewBag.Message += msg;


                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        ViewBag.Message = "Transaction Failed.";
                        ViewBag.Type = "danger";
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Transaction Failed.";
                    ViewBag.Type = "danger";

                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
            }

            List<Channel> channelsList = dal.AllChannels();
            List<Pack> packsList = dal.AllPacks();
            ViewData["AllChannels"] = channelsList;
            ViewData["AllPacks"] = packsList;
            List<int> chids = new List<int>();
            foreach (var ch1 in channelsList)
                chids.Add(ch1.ChannelId);
            List<bool> subornot = dal.issubornot(chids, Session["Email"].ToString());
            ViewData["SuborNot"] = subornot;
            Customer c1 = dal.FetchCustomer(Session["email"].ToString());

            List<int> packids = new List<int>();
            foreach (var pack in packsList)
                packids.Add(pack.PackId);
            List<bool> subornot1 = dal.issubornot1(packids, Session["Email"].ToString());
            ViewData["SuborNot1"] = subornot1;

            List<bool> subornot2 = dal.issubornotinanypack(chids, packids, Session["Email"].ToString());
            ViewData["SuborNot2"] = subornot2;

            ViewData["ProfileId"] = c1.ProfileId;

            return View("../Home/Welcome");

        }

        [HttpPost]
        public ActionResult ChargeChannel(Channel ch)
            {
            if (ch.month > 1)
            {
                // LAKSHMI //                autorec call 
            }
            Console.WriteLine("Charge Customer Profile");

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };

            DAL dal = new DAL();
            //create a customer payment profile
            customerProfilePaymentType profileToCharge = new customerProfilePaymentType();
            profileToCharge.customerProfileId = dal.getCustomerProfileId(Session["Email"].ToString());
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = dal.getPaymentProfileId(Session["Email"].ToString()) };
        

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // refund type
                //amount = 12,
                amount = Convert.ToDecimal(ch.ChannelCost),
                profile = profileToCharge
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the collector that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {

                        int sid = dal.AddChannelSubscriptionMaster(Session["Email"].ToString(), ch, response.transactionResponse.transId); // as of now no validation here

                        //here 1stparametr is sid from above function.. as of now hardcoded.
                        dal.AddSubscriptionChannel(sid, ch); // as of now no validation here

                        //sid = dal.AddChannelSubscriptionMaster(useremail, InvoiceDetails); // as of now no validation here

                        //here 1stparametr is sid from above function.. as of now hardcoded.
                        //success_insertion_channel = dal.AddSubscriptionChannel(sid, InvoiceDetails); // as of now no validation here

                        //DateTime theDate = DateTime.Now;
                        //DateTime expdate = theDate.AddMonths(1).Date;
                        ViewBag.Message = "Successfully completed payment of " + ch.ChannelCost + ". Transaction ID: " + response.transactionResponse.transId + ". \n Subscribed Channel: " + ch.ChannelName + ". \n We will charge you " + ch.ChannelCost + " on " + DateTime.Now.Day + "th of every month for next " + (ch.month - 1) + " months.";
                        ViewBag.Type = "success";


                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        ViewBag.Message = "Transaction Failed.";
                        ViewBag.Type = "danger";
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Transaction Failed.";
                    ViewBag.Type = "danger";

                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
            }

            List<Channel> channelsList = dal.AllChannels();
            List<Pack> packsList = dal.AllPacks();
            ViewData["AllChannels"] = channelsList;
            ViewData["AllPacks"] = packsList;
            List<int> chids = new List<int>();
            foreach (var ch1 in channelsList)
                chids.Add(ch1.ChannelId);
            List<bool> subornot = dal.issubornot(chids, Session["Email"].ToString());
            ViewData["SuborNot"] = subornot;
            Customer c1 = dal.FetchCustomer(Session["email"].ToString());

            List<int> packids = new List<int>();
            foreach (var pack in packsList)
                packids.Add(pack.PackId);
            List<bool> subornot1 = dal.issubornot1(packids, Session["Email"].ToString());
            ViewData["SuborNot1"] = subornot1;

            List<bool> subornot2 = dal.issubornotinanypack(chids, packids, Session["Email"].ToString());
            ViewData["SuborNot2"] = subornot2;

            ViewData["ProfileId"] = c1.ProfileId;

            return View("../Home/Welcome");

        }


     

        }

}

        






