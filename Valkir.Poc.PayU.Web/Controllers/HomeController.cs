using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using RestSharp;
using Valkir.Poc.PayU.Web.Models;

namespace Valkir.Poc.PayU.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _payUApiUrl;
        private readonly string _newPaymentUrl;
        private readonly string _payUUrl;

        public HomeController()
        {
            _payUApiUrl = ConfigurationManager.AppSettings["PayUApiUrl"];
            _newPaymentUrl = ConfigurationManager.AppSettings["NewPaymentUrl"];
            _payUUrl = ConfigurationManager.AppSettings["PayU"];
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

            var result = Helper.PreparePOSTForm(_newPaymentUrl, new NameValueCollection()
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
                                                                            {"ts", "20"},
                                                                            {"sig", ComputeSig(payment)},
                                                                        });
            Response.Write(result);
            Response.End();
        }

        private string ComputeSig(Payment payment)
        {
            //sig = md5 ( pos_id + pay_type + session_id + pos_auth_key + amount + desc + desc2 + trsDesc + order_id + first_name + last_name + payback_login + street + street_hn + street_an + city + post_code + country + email + phone + language + client_ip + ts + key1 )
            var ts = 20;
            var key1 = "c298b3de693686bee1318145269a91d7";
            var toHash =
                string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}",
                              payment.PosId, // pos_id,
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
                              key1
                    );

            return Helper.GetMd5Hash(toHash);
        }

        public void Report(PayUReport report)
        {
            Response.Write("OK");

            Response.Write(string.Format("pos_id: {0},session_id: {1},ts: {2} <br />",report.pos_id,report.session_id, report.ts));
            //Response.Write(report.sig);

            // Payment/get

            var result = "";
            using (WebClient client = new WebClient())
            {

                byte[] response = client.UploadValues(_payUUrl + "Payment/get", new NameValueCollection()
                                                                                       {
                                                                                           {"pos_id", report.pos_id.ToString()},
                                                                                           {"session_id", report.session_id},
                                                                                           {"ts", report.ts},
                                                                                           {"sig", report.sig},
                                                                                       });


                result = System.Text.Encoding.Default.GetString(response);
                var start = result.IndexOf("<body>");
                var end = result.IndexOf("</body>");
          

                Response.Write(result.Substring(start, end - start));
            }

        }
    }
}
