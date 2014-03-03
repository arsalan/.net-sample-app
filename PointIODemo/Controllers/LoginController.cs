using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace PointIODemo.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


        public async Task<ActionResult> Process()
        {


            NameValueCollection nvc = Request.Form;
            string userName, password;
            if (!string.IsNullOrEmpty(nvc["emailAddress"]))
            {
                userName = nvc["emailAddress"];
            }
            else
            {
                userName = "";

            }

            if (!string.IsNullOrEmpty(nvc["password"]))
            {
                password = nvc["password"];
            }
            else
            {
                password = "";
            }

            HttpClient tClient = new HttpClient();
            var pData = new List<KeyValuePair<string, string>>();
            pData.Add(new KeyValuePair<string, string>("email",userName));
            pData.Add(new KeyValuePair<string, string>("password", password));
            pData.Add(new KeyValuePair<string, string>("apiKey", PointIODemo.MvcApplication.APIKey));

            HttpContent content = new FormUrlEncodedContent(pData);
            var rTask = await tClient.PostAsync(PointIODemo.MvcApplication.APIUrl + "auth.json", content);
            var rContent = rTask.Content.ReadAsStringAsync().Result;


            var oResponse = JsonConvert.DeserializeObject<dynamic>(rContent);
           
            if(oResponse["ERROR"] == "1")
            {
                ViewBag.ErrorText = oResponse["MESSAGE"];
                return RedirectToAction("ErrorTemplate", "Home", new { errorMessage = oResponse["MESSAGE"] });
            }

            Session.Add("sessionKey", oResponse["RESULT"]["SESSIONKEY"]);
            Session.Add("fname", oResponse["RESULT"]["FNAME"]);
            Session.Add("lname", oResponse["RESULT"]["LNAME"]);
            Session.Add("email", oResponse["RESULT"]["EMAIL"]);

            return RedirectToAction("Index", "Home");
        }



	}
}