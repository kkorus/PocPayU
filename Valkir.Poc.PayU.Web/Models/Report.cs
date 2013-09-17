using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Valkir.Poc.PayU.Web.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string XmlReport { get; set; }
    }
}