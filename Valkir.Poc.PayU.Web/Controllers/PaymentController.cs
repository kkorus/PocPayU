using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Valkir.Poc.PayU.Web.Controllers
{
    public class ResourceQuery
    {
        public string Param1 { get; set; }
        public int OptionalParam2 { get; set; }
    }


    public class PaymentController : ApiController
    {
        //
        // GET: /Payment/

        public void Index(int posId)
        {
            
        }

        //[System.Web.Mvc.HttpGet]
        public void Get([FromUri] ResourceQuery query)
        {
            
        }

        //        [System.Web.Mvc.HttpGet]
        //public void Test(int transId, int posId, string payType)
        //{
            
        //}

        //[FromUri] ResourceQuery query
    }
}
