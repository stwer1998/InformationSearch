using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace IsCrawler.Crawler
{
    public class Crawler
    {
        HtmlParser.HtmlParser htmlParser;
        HttpClient httpClient;
        public Crawler()
        {
            htmlParser = new HtmlParser.HtmlParser();
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
        }

        public void Crawl(string link) 
        {
            var links = new List<string>() { link };
            var indexed = new List<IndexLink>();
            var newLinks = new List<string>();
            var indexedbad = new List<string>();
            var inQueue = new List<string>();

            var pageWordCount = 1000;

            var pagenum = 100;

            while (indexed.Count < pagenum)
            {
                if (newLinks.Count > 0 || inQueue.Count > 0) 
                {
                    links.AddRange(newLinks.Distinct());
                    links.AddRange(inQueue);
                    links.RemoveAll(x => indexed.Select(x => x.Link).Contains(x));
                    links.RemoveAll(x => indexedbad.Contains(x));
                    inQueue = links.Skip(pagenum - indexed.Count).Distinct().ToList();
                    links = links.Take(pagenum - indexed.Count).ToList();
                    newLinks = new List<string>();
                }

                if(links.Count < 1) 
                {
                    Console.WriteLine("We have problem");
                }

                foreach (var url in links)
                {
                    try 
                    {
                        var html = httpClient.GetStringAsync(url).Result;
                        newLinks.AddRange(htmlParser.GetLinks(html, url));
                        var text = htmlParser.GetText(html);
                        var wordCount = GetWordsCount(text);
                        if (wordCount > pageWordCount) 
                        {
                            indexed.Add(new IndexLink 
                            {
                                Link = url,
                                WordCount = wordCount,
                                Text = text
                            });
                        }
                        else 
                        {
                            indexedbad.Add(url);
                        }
                    }
                    catch (Exception ex)
                    {
                        indexedbad.Add(url);
                        continue;
                    }

                }
            }

            SaveToFile(indexed);

        }

        private void SaveToFile(List<IndexLink> indexLinks) 
        {
            var parentPath = @"d:\InformationSearch\";

            var uri = new Uri(indexLinks[0].Link);

            var pathToDir = $"{parentPath}/{uri.Host}";

            if (!Directory.Exists(pathToDir)) 
            {
                Directory.CreateDirectory(pathToDir);
            }

            for (int i = 0; i < indexLinks.Count; i++)
            {
                var fileName = $"{pathToDir}/{i}.txt";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes(indexLinks[i].Text);
                    fs.Write(title, 0, title.Length);
                }
                indexLinks[i].FilePath = fileName;
                indexLinks[i].NumDoc = i;

            }

            var indexFile = $"{pathToDir}/index.txt";
            if (File.Exists(indexFile))
            {
                File.Delete(indexFile);
            }
            using (FileStream fs = File.Create(indexFile))
            {
                string text = string.Join('\n', indexLinks.Select(x => $"{x.NumDoc} {x.WordCount} {x.FilePath.ToString()} {x.Link}"));
                Byte[] title = new UTF8Encoding(true).GetBytes(text);
                fs.Write(title, 0, title.Length);
            }

        }

        private int GetWordsCount(string str) 
        {
            MatchCollection collection = Regex.Matches(str, @"[\S]{3,}");
            return collection.Count;
        }

        private class IndexLink
        {
            public string Link { get; set; }

            public string Text { get; set; }

            public int WordCount { get; set; }

            public string FilePath { get; set; }

            public int NumDoc { get; set; }

        }
    }
}
