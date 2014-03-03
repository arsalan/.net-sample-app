using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PointIODemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static string APIKey = "Key Goes Here";
        public static string APIUrl = "https://api.point.io/v2/";
       
        public static int getColNum(String fieldName,List<dynamic> colList)
        {
            for (int i = 0; i < colList.Count; i++)
            {
                if (colList[i] == fieldName)
                {
                    return i;
                }
            }
            return -1;
        }

    }


}
