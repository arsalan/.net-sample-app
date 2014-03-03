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

namespace PointIODemo.Models
{
    public class AccessRule
    {
        private string _shareid;
        private string _sharename;
        private string _userid;
        private string _sitetypeid;
        private string _sitetypename;
        private string _name;

        public string shareid
        {
            get { return _shareid; }
            set { _shareid = value; }
        }

        public string sharename
        {
            get { return _sharename; }
            set { _sharename = value; }
        }

        public string userid
        {
            get { return _userid; }
            set { _userid = value; }
        }

        public string sitetypeid
        {
            get { return _sitetypeid; }
            set { _sitetypeid = value; }
        }

        public string sitetypename
        {
            get { return _sitetypename; }
            set { _sitetypename = value; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }


        public async Task<List<AccessRule>> list(String sessionKey)
        {

            HttpClient tClient = new HttpClient();
            tClient.DefaultRequestHeaders.Add("AUTHORIZATION", sessionKey);
            var rTask = await tClient.GetAsync(PointIODemo.MvcApplication.APIUrl + "accessrules/list.json");
            var rContent = rTask.Content.ReadAsStringAsync().Result;
            var oResponse = JsonConvert.DeserializeObject<dynamic>(rContent);

            if (oResponse["ERROR"] == "1")
            {
               HttpContext.Current.Response.Redirect("/Home/ErrorTemplate/?errorMessage=" + oResponse["MESSAGE"]);
            }

            var rawColList = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(oResponse["RESULT"]["COLUMNS"]));
            var rawRuleList = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(oResponse["RESULT"]["DATA"]));
            var aRuleList = new List<AccessRule>();

            foreach (var item in rawRuleList)
            {
                AccessRule tRule = new AccessRule();
                tRule.shareid = item[PointIODemo.MvcApplication.getColNum("SHAREID", rawColList)];
                tRule.userid = item[PointIODemo.MvcApplication.getColNum("USERID", rawColList)];
                tRule.sitetypeid = item[PointIODemo.MvcApplication.getColNum("SITETYPEID", rawColList)];
                tRule.sitetypename = item[PointIODemo.MvcApplication.getColNum("SITETYPENAME", rawColList)];
                tRule.sharename = item[PointIODemo.MvcApplication.getColNum("SHARENAME", rawColList)];
                tRule.name = item[PointIODemo.MvcApplication.getColNum("NAME", rawColList)];
                aRuleList.Add(tRule);
            }

            return aRuleList;
        }

        public async Task<AccessRule> getshare(String sessionKey, String shareid)
        {

            HttpClient tClient = new HttpClient();
            tClient.DefaultRequestHeaders.Add("AUTHORIZATION", sessionKey);
            var rTask = await tClient.GetAsync(PointIODemo.MvcApplication.APIUrl + "accessrules/list.json");
            var rContent = rTask.Content.ReadAsStringAsync().Result;
            var oResponse = JsonConvert.DeserializeObject<dynamic>(rContent);

            if (oResponse["ERROR"] == "1")
            {
               HttpContext.Current.Response.Redirect("/Home/ErrorTemplate/?errorMessage=" + oResponse["MESSAGE"]);
            }
            var rawColList = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(oResponse["RESULT"]["COLUMNS"]));
            var rawRuleList = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(oResponse["RESULT"]["DATA"]));
            foreach (var item in rawRuleList)
            {
                if(item[PointIODemo.MvcApplication.getColNum("SHAREID", rawColList)] == shareid)
                {
               
                    AccessRule tRule = new AccessRule();
                    tRule.shareid = item[PointIODemo.MvcApplication.getColNum("SHAREID", rawColList)];
                    tRule.userid = item[PointIODemo.MvcApplication.getColNum("USERID", rawColList)];
                    tRule.sitetypeid = item[PointIODemo.MvcApplication.getColNum("SITETYPEID", rawColList)];
                    tRule.sitetypename = item[PointIODemo.MvcApplication.getColNum("SITETYPENAME", rawColList)];
                    tRule.sharename = item[PointIODemo.MvcApplication.getColNum("SHARENAME", rawColList)];
                    tRule.name = item[PointIODemo.MvcApplication.getColNum("NAME", rawColList)];
                    return tRule;
                }
            }
            return new AccessRule();
        }

    }

    
}