using System;
using System.Collections.Generic;

namespace IsCrawler.HtmlParser
{
    interface IHtmlParser
    {
        string GetText(string html);

        IEnumerable<string> GetLinks(string html, Uri uri);
    }
}
