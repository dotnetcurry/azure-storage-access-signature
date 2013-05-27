using Microsoft.WindowsAzure.StorageClient;
using SharedClientAcessUrls.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SharedClientAcessUrls.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            IEnumerable<IListBlobItem> items = AzureBlobSA.GetContainerFiles("dnc-demo");
            List<string> urls = new List<string>();
            foreach (var item in items)
            {
                if (item is CloudBlockBlob)
                {
                    var blob = (CloudBlockBlob)item;
                    urls.Add(blob.Name);
                }
            }
            ViewBag.BlobFiles = urls;
            ViewBag.Sas = AzureBlobSA.GetSasUrl("dnc-demo");
            return View();
        }

        [HttpGet]
        public ActionResult GetImage(string id)
        {
            string fileName = AzureBlobSA.DecodeFrom64(id);
            return new RedirectResult(AzureBlobSA.GetSasBlobUrl("dnc-demo", fileName, Request.QueryString["sas"]));
        }

    }
}
