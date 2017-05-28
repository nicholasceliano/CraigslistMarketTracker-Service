using HtmlAgilityPack;
using System.IO;
using System.Net;

namespace CraigslistMarketTracker_Service
{
    public class WebRequest
    {
        public HtmlDocument UrlRequest(string url) {

            var webRequest = HttpWebRequest.Create(url);

            var resp = (HttpWebResponse)webRequest.GetResponse();

            var sr = new StreamReader(resp.GetResponseStream()).ReadToEnd();

            var doc = new HtmlDocument();
            doc.LoadHtml(sr);

            return doc;
        }
    }
}
