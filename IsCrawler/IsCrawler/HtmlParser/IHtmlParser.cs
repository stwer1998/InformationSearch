using System;
using System.Collections.Generic;
using System.Text;

namespace IsCrawler.HtmlParser
{
    interface IHtmlParser
    {
        string GetText(string html);

        IEnumerable<string> GetLinks(string html, string uri);
    }
}
