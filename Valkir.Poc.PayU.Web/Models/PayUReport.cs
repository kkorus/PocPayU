using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Valkir.Poc.PayU.Web.Models
{
    public class PayUReport
    {
        public int pos_id { get; set; }
        public string session_id { get; set; }
        public string ts { get; set; }
        public string sig { get; set; }
    }
}