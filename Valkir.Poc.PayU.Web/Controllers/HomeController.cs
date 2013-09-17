using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using Valkir.Poc.PayU.Web.Models;

namespace Valkir.Poc.PayU.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _newPaymentURI;
        private readonly string _payUUrl;
        private readonly string _encoding;

        private readonly string _key1;
        private readonly string _key2;

        public HomeController()
        {
            _newPaymentURI = ConfigurationManager.AppSettings["NewPaymentURI"];
            _payUUrl = ConfigurationManager.AppSettings["PayU"];
            _encoding = ConfigurationManager.AppSettings["Encoding"];

            _key1 = ConfigurationManager.AppSettings["key1"];
            _key2 = ConfigurationManager.AppSettings["key2"];

        }

        public ActionResult Index()
        {
            return RedirectToAction("Payment");
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var payment = new Payment()
                              {
                                  PosId = Convert.ToInt32(ConfigurationManager.AppSettings["pos_id"]),
                                  PosAuthKey = ConfigurationManager.AppSettings["pos_auth_key"],
                                  FirstName = "Jan",
                                  LastName = "Kowlaski",
                                  Email = "jkowalski@onet.pl",
                                  OrderId = 123,
                                  Description = "Nowy przedmiot",
                                  SessionId = Session.SessionID,
                                  Amount = 999,
                                  ClientIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]
                              };

            return View(payment);
        }

        [HttpPost]
        public void Payment(Payment payment)
        {
            ExecutePayment(payment);
        }

        private void ExecutePayment(Payment payment)
        {
            Response.Clear();

            var url = string.Format("{0}/{1}/{2}", _payUUrl, _encoding, _newPaymentURI);
            var ts = PayUHelper.TS.ToString();
            var sig = PayUHelper.GetSig(payment.PosId.ToString(), // pos_id,
                                        "", // pay_type
                                        payment.SessionId,
                                        payment.PosAuthKey,
                                        payment.Amount.ToString(),
                                        payment.Description,
                                        "", // desc2
                                        "", // trsDesc
                                        payment.OrderId.ToString(),
                                        payment.FirstName,
                                        payment.LastName,
                                        "", // payback_login
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        payment.Email,
                                        "",
                                        "",
                                        payment.ClientIp,
                                        ts,
                                        _key1);


            var result = PayUHelper.PreparePOSTForm(url, new NameValueCollection()
                                                                        {
                                                                            {"pos_id", payment.PosId.ToString()},
                                                                            {"pay_type", ""},
                                                                            {"session_id", payment.SessionId},
                                                                            {"pos_auth_key", payment.PosAuthKey},
                                                                            {"amount", payment.Amount.ToString()},
                                                                            {"desc", payment.Description},
                                                                            {"desc2", ""},
                                                                            {"trsDesc", ""},
                                                                            {"order_id", payment.OrderId.ToString()},
                                                                            {"first_name", payment.FirstName},
                                                                            {"last_name", payment.LastName},
                                                                            {"payback_login", ""},
                                                                            {"street", ""},
                                                                            {"street_hn", ""},
                                                                            {"street_an", ""},
                                                                            {"city", ""},
                                                                            {"post_code", ""},
                                                                            {"country", ""},
                                                                            {"email", payment.Email},
                                                                            {"phone", ""},
                                                                            {"language", ""},
                                                                            {"client_ip", payment.ClientIp},
                                                                            {"ts", ts},
                                                                            {"sig", sig},
                                                                        });
            Response.Write(result);
            Response.End();
        }

        public void Report(PayUReport report)
        {
            // steps
            // 1. Check if report sig is valid
            // 2. If valid ask for status
            // 3. Response: OK

            // sig = md5 ( pos_id + session_id + ts + key2 )
            var sig = PayUHelper.GetSig(report.pos_id.ToString(), report.session_id, report.ts, _key2);
            if (sig != report.sig)
            {
                throw new Exception("Wrong sig!");
            }

            try
            {
                // sig = md5 ( pos_id + session_id + ts + key1 )
                var ts = PayUHelper.TS.ToString();
                sig = PayUHelper.GetSig(report.pos_id.ToString(), report.session_id, ts, _key1);

                using (var client = new WebClient())
                {
                    var url = string.Format("{0}/{1}", _payUUrl, _encoding);
                    byte[] response = client.UploadValues(url + "/Payment/get", new NameValueCollection()
                                                                                    {
                                                                                        {"pos_id",report.pos_id.ToString()},
                                                                                        { "session_id",report.session_id },
                                                                                        {"ts", ts},
                                                                                        {"sig", sig},
                                                                                    });

                    var xmlResult = System.Text.Encoding.Default.GetString(response);

                    // stroe xml in db

                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

            Response.Write("OK");
        }

        [HttpGet]
        public void UrlPositive(string posId, string sessionId, string payType, string transId, string amountPS, string amountCS, string orderId, string error)
        {
            Response.Write("posId: " + posId);
            Response.Write("sessionId: " + sessionId);
            Response.Write("payType: " + payType);
            Response.Write("transId: " + transId);
            Response.Write("amountPS: " + amountPS);
            Response.Write("amountCS: " + amountCS);
            Response.Write("orderId: " + orderId);
            Response.Write("error: " + error);
        }

        [HttpGet]
        public void UrlNegative(string posId, string sessionId, string payType, string transId, string amountPS, string amountCS, string orderId, string error)
        {
            Response.Write("posId: " + posId);
            Response.Write("sessionId: " + sessionId);
            Response.Write("payType: " + payType);
            Response.Write("transId: " + transId);
            Response.Write("amountPS: " + amountPS);
            Response.Write("amountCS: " + amountCS);
            Response.Write("orderId: " + orderId);
            Response.Write("error: " + error);
        }
    }
}
