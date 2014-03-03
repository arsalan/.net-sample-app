using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PointIODemo.Models;

namespace PointIODemo.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            String sessionKey = "";
            if(Session["sessionKey"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                sessionKey = Session["sessionKey"].ToString();
            }

            ViewBag.accessRules = await new AccessRule().list(sessionKey);
            return View();
        }


        public ActionResult ErrorTemplate(String errorMessage)
        {
            ViewBag.ErrorText = errorMessage;
            return View();
        }


        public async Task<ActionResult> Browse(String ID,String remotepath,String containerid)
        {
            String sessionKey = "";
            if (Session["sessionKey"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                sessionKey = Session["sessionKey"].ToString();
            }

            var tShare = await new  AccessRule().getshare(sessionKey,ID);
            ViewBag.sharename = tShare.sharename;
            ViewBag.remotepath = remotepath;
            ViewBag.containerid = containerid;
            ViewBag.shareid = ID;
            ViewBag.folderContent = await new FolderContent().list(sessionKey,ID,containerid,remotepath);
            return View();
        }


        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            String sessionKey = "";
            if (Session["sessionKey"] == null)
            {
               //
            }
            else
            {
                sessionKey = Session["sessionKey"].ToString();
            }
            
            var folderid = Request["folderid"];
            var containerid = Request["containerid"];
            var remotepath = Request["remotepath"];

            var result = await new FolderContent().upload(sessionKey, folderid, remotepath, containerid, file);

            return RedirectToAction("Browse", "Home", new { id = folderid, remotepath = remotepath, containerid = containerid });

        }


        public async Task<ActionResult> Download(String folderid,String fileid,String filename,String containerid, String remotepath){
            String sessionKey = "";
            if (Session["sessionKey"] == null)
            {
               //
            }
            else
            {
                sessionKey = Session["sessionKey"].ToString();
            }

           var dlURL = await new FolderContent().download(sessionKey,folderid,containerid,remotepath,filename,fileid);
           return Redirect(dlURL);
        }
       

    }
}