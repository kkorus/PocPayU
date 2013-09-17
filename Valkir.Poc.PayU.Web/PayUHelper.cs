using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace Valkir.Poc.PayU.Web
{
    public class PayUHelper
    {
        /// <summary>
        /// This method prepares an Html form which holds all data in hidden field in the addetion to form submitting script.
        /// </summary>
        /// <param name="url">The destination Url to which the post and redirection will occur, the Url can be in the same App or ouside the App.</param>
        /// <param name="data">A collection of data that will be posted to the destination Url.</param>
        /// <returns>Returns a string representation of the Posting form.</returns>
        /// <Author>Samer Abu Rabie</Author>
        public static String PreparePOSTForm(string url, NameValueCollection data)
        {
            //Set a name for the form
            string formID = "PostForm";

            //Build the form using the specified data to be posted.
            var strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
            foreach (string key in data)
            {
                strForm.Append("<input id=\"" + key + "\" runat=\"server\" type=\"hidden\" name=\"" + key + "\" value=\"" + data[key] + "\">");
            }
            strForm.Append("</form>");

            //Build the JavaScript which will do the Posting operation.
            var strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            //Return the form and the script concatenated. (The order is important, Form then JavaScript)
            return strForm.ToString() + strScript.ToString();
        }

        public static string GetMd5Hash(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                var sBuilder = new StringBuilder();

                for (var i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("X2"));
                }

                return sBuilder.ToString();
            }
        }

        public static int TS
        {
            get
            {
                var ts = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                return (int)ts.TotalSeconds;
            }
        }

        public static string GetSig(string input, string ts, string key)
        {
            var toHash = string.Format("{0}{1}{2}", input, ts, key);
            return GetMd5Hash(toHash);
        }

        public static string GetSig(params string[] inputs)
        {
            var sb = new StringBuilder();

            foreach (var input in inputs)
            {
                sb.Append(input);
            }

            return GetMd5Hash(sb.ToString());
        }
    }
}