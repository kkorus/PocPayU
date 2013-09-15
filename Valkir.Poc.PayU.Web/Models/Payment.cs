using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Valkir.Poc.PayU.Web.Models
{
    public class Payment
    {
        [Display(Name = "pos_id")]
        public int PosId { get; set; }

        [Display(Name = "Session ID")]
        public string SessionId { get; set; }

        [Display(Name = "pos_auth_key")]
        public string PosAuthKey { get; set; }

        [Display(Name = "Kwota")]
        public decimal Amount { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "ID zamówienia")]
        public int OrderId { get; set; }

        [Display(Name = "Imię")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        public string LastName { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Adres IP")]
        public string ClientIp { get; set; }

    }
}