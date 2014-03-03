using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PointIODemo.Models
{
    public class FolderContent
    {
        private string _fileid;
        private string _filename;
        private string _containerid;
        private string _remotepath;
        private string _type;
        private string _size;
        private string _modified;

        public string fileid
        {
            get { return _fileid; }
            set { _fileid = value; }
        }
        public string filename
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public string containerid
        {
            get { return _containerid; }
            set { _containerid = value; }
        }
        public string remotepath
        {
            get { return _remotepath; }
            set { _remotepath = value; }
        }
        public string type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public async Task<List<FolderContent>> list(String sessionKey, String shareid, String containerid, String path)
        {

            HttpClient tClient = new HttpClient();
            tClient.DefaultRequestHeaders.Add("AUTHORIZATION", sessionKey);


            var query = HttpUtility.ParseQueryString(string.Empty);
            query["folderid"] = shareid;
            query["containerid"] = containerid;
            query["path"] = path;
            string queryString = query.ToString();


            var rTask = await tClient.GetAsync(PointIODemo.MvcApplication.APIUrl + "folders/list.json?" + queryString);
            var rContent = rTask.Content.ReadAsStringAsync().Result;
            var oResponse = JsonConvert.DeserializeObject<dynamic>(rContent);

            if (oResponse["ERROR"] == "1")
            {
                HttpContext.Current.Response.Redirect("/Home/ErrorTemplate/?errorMessage=" + oResponse["MESSAGE"]);
            }

            var rawColList = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(oResponse["RESULT"]["COLUMNS"]));
            var rawContentList = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(oResponse["RESULT"]["DATA"]));
            var fContentList = new List<FolderContent>();

            foreach (var item in rawContentList)
            {
                FolderContent tContent = new FolderContent();


                tContent.fileid = item[MvcApplication.getColNum("FILEID", rawColList)];
                tContent.filename = item[MvcApplication.getColNum("NAME", rawColList)];
                tContent.containerid = item[MvcApplication.getColNum("CONTAINERID", rawColList)];
                tContent.remotepath = item[MvcApplication.getColNum("PATH", rawColList)];
                tContent.type = item[MvcApplication.getColNum("TYPE", rawColList)];
                tContent.size = item[MvcApplication.getColNum("SIZE", rawColList)];
                tContent.modified = item[MvcApplication.getColNum("MODIFIED", rawColList)];
                fContentList.Add(tContent);
            }

            return fContentList;
        }

        public async Task<Boolean> upload(string sessionkey,string folderid,string remotepath,string containerid,HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);

            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes(file.ContentLength);

            var tClient = new HttpClient();

            var multipart = new MultipartFormDataContent();
            var values = new[]
                    {
                        new KeyValuePair<string, string>("folderid", folderid),
                        new KeyValuePair<string, string>("containerid", containerid),
                        new KeyValuePair<string, string>("remotepath", remotepath),
                        new KeyValuePair<string, string>("fileid", fileName),
                        new KeyValuePair<string, string>("filename", fileName),

                    };

            foreach (var keyValuePair in values)
            {
                multipart.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
            }
            var fileContents = new ByteArrayContent(binData);
            fileContents.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {

                Name = "\"filecontents\"",
                FileName = "\"" + fileName + "\""
                
            };
            fileContents.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            multipart.Add(fileContents);
            tClient.DefaultRequestHeaders.Add("AUTHORIZATION", sessionkey);
            var rTask = await tClient.PostAsync(PointIODemo.MvcApplication.APIUrl + "folders/files/upload.json", multipart);
            var rContent = rTask.Content.ReadAsStringAsync().Result;
            var oResponse = JsonConvert.DeserializeObject<dynamic>(rContent);

            if (oResponse["ERROR"] == "1")
            {
                HttpContext.Current.Response.Redirect("/Home/ErrorTemplate/?errorMessage=" + oResponse["MESSAGE"]);
            }
            return true;
           
        }


        public async Task<String> download(string sessionkey,string folderid, string containerid, string remotepath, string filename, string fileid)
        {

            HttpClient tClient = new HttpClient();
            tClient.DefaultRequestHeaders.Add("AUTHORIZATION", sessionkey);
            
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["folderid"] = folderid;
            query["containerid"] = containerid;
            query["remotepath"] = remotepath;
            query["filename"] = filename;
            query["fileid"] = fileid;
            string queryString = query.ToString();

            var rTask = await tClient.GetAsync(PointIODemo.MvcApplication.APIUrl + "folders/files/download.json?" + queryString);
            var rContent = rTask.Content.ReadAsStringAsync().Result;
            var oResponse = JsonConvert.DeserializeObject<dynamic>(rContent);

            if (oResponse["ERROR"] == "1")
            {
                HttpContext.Current.Response.Redirect("/Home/ErrorTemplate/?errorMessage=" + oResponse["MESSAGE"]);
            }

            return oResponse["RESULT"];
        }
        
        

    }
}