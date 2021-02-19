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
        private const string ParentForderPath = @"d:\InformationSearch\";
        private readonly int PageCount = 100;
        private readonly int WordCount = 1000;

        public Crawler(int pageCount = 100, int wordCouny = 1000)
        {
            PageCount = pageCount;
            WordCount = wordCouny;
            CreateForder();
            htmlParser = new HtmlParser.HtmlParser();
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
        }

        public void Crawl(string link) 
        {
            var uri = new Uri(link);

            var links = new List<string>() { link };//links step now
            var indexed = new List<IndexLink>();//indexed links
            var newLinks = new List<string>();//new links 
            var indexedbad = new List<string>();//not found or bad links
            var inQueue = new List<string>();//links in queue

            while (indexed.Count < PageCount)
            {
                if (newLinks.Count > 0 || inQueue.Count > 0) 
                {
                    links.AddRange(newLinks.Distinct());
                    links.AddRange(inQueue);
                    links.RemoveAll(x => indexed.Select(x => x.Link).Contains(x));
                    links.RemoveAll(x => indexedbad.Contains(x));
                    inQueue = links.Skip(PageCount - indexed.Count).Distinct().ToList();
                    links = links.Take(PageCount - indexed.Count).ToList();
                    newLinks = new List<string>();
                }

                if(links.Count < 1) 
                {
                    Console.WriteLine("not enough links");
                    break;
                }

                foreach (var url in links)
                {
                    try 
                    {
                        var html = httpClient.GetStringAsync(url).Result;
                        newLinks.AddRange(htmlParser.GetLinks(html, uri));
                        var text = htmlParser.GetText(html);
                        var wordCount = GetWordsCount(text);
                        if (wordCount > WordCount) 
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
                links = new List<string>();
            }

            SaveToFile(indexed);

        }

        private void SaveToFile(List<IndexLink> indexLinks) 
        {

            var uri = new Uri(indexLinks[0].Link);

            var pathToDir = $"{ParentForderPath}/{uri.Host}";

            if (!Directory.Exists(pathToDir)) 
            {//Create forder for link
                Directory.CreateDirectory(pathToDir);
            }

            for (int i = 0; i < indexLinks.Count; i++)
            {//save text in file
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

        private void CreateForder() 
        {
            if (!Directory.Exists(ParentForderPath))
            {//Create forder for link
                Directory.CreateDirectory(ParentForderPath);
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
