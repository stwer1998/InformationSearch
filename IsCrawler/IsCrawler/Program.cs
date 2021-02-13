using System;
using System.Net.Http;

namespace IsCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var cl = new IsCrawler.Crawler.Crawler();
            cl.Crawl("https://habr.com/ru/");

            Console.WriteLine();



        }
    }
}
