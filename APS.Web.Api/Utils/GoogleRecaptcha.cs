using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APS.Web.Api.Utils
{
    public class GoogleRecaptcha
    {
        public static Models.RecaptchaResponse VerifyRecaptcha(string recaptcha)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var Configs = builder.Build();

            var SiteKey = Configs["GoogleApis:Recaptcha:SiteKey"];
            var SecretKey = Configs["GoogleApis:Recaptcha:SecretKey"];

            var ProxyUrl = Configs["WebProxy:Url"];
            bool ProxyEnabled = Convert.ToBoolean(Configs["WebProxy:Enabled"]);
            var ProxyUser = Configs["WebProxy:User"];
            var ProxyPassword = Configs["WebProxy:Password"];

            if (ProxyEnabled)
            {
                WebProxy proxy = new WebProxy(ProxyUrl, true);
                proxy.Credentials = new NetworkCredential(ProxyUser, ProxyPassword);
                WebRequest.DefaultWebProxy = proxy;
            }

            HttpWebRequest httprequest = WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" + SecretKey + "&response=" + recaptcha) as HttpWebRequest;
            httprequest.Method = "GET";
            HttpWebResponse response = httprequest.GetResponse() as HttpWebResponse;
            
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                return JsonConvert.DeserializeObject<Models.RecaptchaResponse>(reader.ReadToEnd());
            }

            //return null;
        }
    }
}
