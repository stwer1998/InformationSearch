using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsCrawler.HtmlParser
{
    public class HtmlParser : IHtmlParser
    {
        public IEnumerable<string> GetLinks(string html, string uri)
        {
            var uri1 = new Uri(uri);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var hrefList = doc.DocumentNode.SelectNodes("//body//a")
                              .Select(p => p.GetAttributeValue("href",""))
                              .Select(x=>x=CorrectUrl(x,uri1))
                              .Where(x=>x!=null)
                              .Distinct()
                              .ToList();

            return hrefList;
        }

        public string GetText(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return doc.DocumentNode.InnerText.Trim();
        }

        private string CorrectUrl(string url, Uri uri) 
        {
            if (string.IsNullOrEmpty(url))
                return null;

            if (url.StartsWith("//"))
                return $"{uri.Scheme}:{url}";

            if (url.StartsWith("/"))
                return $"{uri.Scheme}://{uri.Host}/{url}";

            if (!url.StartsWith("http"))
                return $"{uri.Scheme}://{uri.Host}/{url}";

            if (!url.Contains(uri.Host))
                return null;

            if (url.Contains('#'))
                return url.Substring(0,url.IndexOf('#'));

            return url;
        }
    }
}
